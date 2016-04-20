using System;
using System.Xml;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class ExpandMarkupsVisitor : XamlNodeVisitorBase
	{
		readonly HydratationContext context;

		public ExpandMarkupsVisitor(HydratationContext context) : base( true )
		{
			this.context = context;
		}

		public class MarkupExpansionParser : MarkupExpressionParser, IExpressionParser
		{
			private IElementNode node;
			object IExpressionParser.Parse(string match, ref string remaining, IServiceProvider serviceProvider)
			{
				IXmlNamespaceResolver xmlNamespaceResolver = serviceProvider.GetService<IXmlNamespaceResolver>();
				if (xmlNamespaceResolver == null)
				{
					throw new ArgumentException();
				}
				IXmlLineInfo xmlLineInfo = null;
				IXmlLineInfoProvider xmlLineInfoProvider = serviceProvider.GetService<IXmlLineInfoProvider>();
				if (xmlLineInfoProvider != null)
				{
					xmlLineInfo = xmlLineInfoProvider.XmlLineInfo;
				}
				string[] array = match.Split( ':' );
				if (array.Length > 2)
				{
					throw new ArgumentException();
				}
				var prefix = array.Length == 2 ? array[0] : string.Empty;
				IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService<IXamlTypeResolver>();
				Type type;
				if (xamlTypeResolver == null)
				{
					type = null;
				}
				else if (!xamlTypeResolver.TryResolve(match + "Extension", out type) && !xamlTypeResolver.TryResolve(match, out type))
				{
					throw new XamlParseException(string.Format("MarkupExtension not found for {0}", match ));
				}

				if (type == null)
				{
					throw new NotSupportedException();
				}
				string namespaceUri = xmlNamespaceResolver.LookupNamespace(prefix) ?? string.Empty;
				XmlType type2 = new XmlType(new XmlName(namespaceUri, type.Name));
				
				this.node = new ElementNode(type2, xmlNamespaceResolver,  xmlLineInfo != null ? xmlLineInfo.LineNumber : -1,  xmlLineInfo == null ? xmlLineInfo.LinePosition : -1);
				if (remaining == "}")
				{
					return this.node;
				}
				char c;
				string nextPiece;
				while ((nextPiece = base.GetNextPiece(ref remaining, out c)) != null)
				{
					base.HandleProperty(nextPiece, serviceProvider, ref remaining, c != '=');
				}
				return this.node;
			}
			protected override void SetPropertyValue(string prop, string strValue, object value, IServiceProvider serviceProvider)
			{
				IXmlNamespaceResolver namespaceResolver = serviceProvider.GetService<IXmlNamespaceResolver>();
				INode node = (value as INode) ?? new ValueNode(strValue, namespaceResolver);
				node.Parent = this.node;
				if (prop != null)
				{
					XmlName key = new XmlName(this.node.NamespaceUri, prop);
					this.node.Properties[key] = node;
					return;
				}
				this.node.CollectionItems.Add(node);
			}
		}

		protected override void Visit(MarkupNode markupnode, INode parentNode)
		{
			XmlName xmlName;
			if ( ApplyPropertiesVisitor.TryGetPropertyName( markupnode, parentNode, out xmlName ) && !ApplyPropertiesVisitor.Skips.Contains( xmlName ) )
			{
				string markupString = markupnode.MarkupString;
				IElementNode elementNode = this.ParseExpression( ref markupString, markupnode.NamespaceResolver, markupnode ) as IElementNode;
				if ( elementNode != null )
				{
					( (IElementNode)parentNode ).Properties[xmlName] = elementNode;
					elementNode.Parent = parentNode;
				}
			}
		}

		private INode ParseExpression(ref string expression, IXmlNamespaceResolver nsResolver, INode node)
		{
			if (expression.StartsWith("{}", StringComparison.Ordinal))
			{
				return new ValueNode(expression.Substring(2), null, -1, -1);
			}
			if (expression[expression.Length - 1] != '}')
			{
				throw new Exception("Expression must end with '}'");
			}
			string match;
			int startIndex;
			if (!MarkupExpressionParser.MatchMarkup(out match, expression, out startIndex))
			{
				throw new Exception();
			}
			expression = expression.Substring(startIndex).TrimStart();
			if (expression.Length == 0)
			{
				throw new Exception("Expression did not end in '}'");
			}
			var provider = Services.Create( context, node, registry => registry.Add( nsResolver ) );
			var result = new MarkupExpansionParser().Parse<INode>(match, ref expression, provider);
			return result;
		}
	}
}