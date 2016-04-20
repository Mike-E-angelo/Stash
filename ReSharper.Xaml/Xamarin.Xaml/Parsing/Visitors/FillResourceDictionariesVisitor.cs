using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class FillResourceDictionariesVisitor : XamlNodeVisitorBase
	{
		public FillResourceDictionariesVisitor(HydratationContext context) : base( false )
		{
			this.Context = context;
		}
		
		HydratationContext Context { get; set; }

		IDictionary<INode, object> Values
		{
			get { return Context.Values; }
		}

		private static bool IsCollectionItem(INode node, INode parentNode)
		{
			IListNode listNode = parentNode as IListNode;
			return listNode != null && listNode.CollectionItems.Contains(node);
		}

		protected override void Visit( ElementNode node, INode parentNode )
		{
			var key = parentNode as IElementNode;
			var value = Values[node];
			if ( IsCollectionItem( node, parentNode ) && parentNode is IElementNode && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom( Context.Types[key].GetTypeInfo() ) )
			{
				var serviceProvider = Services.Create( Context, node );
				var obj = serviceProvider.CheckValue( value );
				if ( Context.Types[key] == typeof(ResourceDictionary) && obj is Style && !node.Properties.ContainsKey( XmlName.xKey ) )
				{
					( (ResourceDictionary)Values[parentNode] ).Add( obj as Style );
				}
				else
				{
					if ( Context.Types[key] == typeof(ResourceDictionary) && !node.Properties.ContainsKey( XmlName.xKey ) )
					{
						throw new XamlParseException( "resources in ResourceDictionary require a x:Key attribute", node );
					}
					if ( Context.Types[key] == typeof(ResourceDictionary) && node.Properties.ContainsKey( XmlName.xKey ) )
					{
						( (ResourceDictionary)Values[parentNode] ).Add( (string)( (ValueNode)node.Properties[XmlName.xKey] ).Value, obj );
					}
				}
			}
			XmlName propertyName;
			if ( ApplyPropertiesVisitor.TryGetPropertyName( node, parentNode, out propertyName ) && propertyName.LocalName == "Resources" && value is ResourceDictionary )
			{
				var xamlelement = Values[parentNode];
				ApplyPropertiesVisitor.SetPropertyValue( xamlelement, propertyName, value, Context.RootElement, node, Context, node );
			}
		}
	}
}
