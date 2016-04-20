using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class RegisterXNamesVisitor : XamlNodeVisitorBase
	{
		public RegisterXNamesVisitor(HydratationContext context) : base(false)
		{
			this.Values = context.Values;
		}
		
		IDictionary<INode, object> Values { get; set; }

		private static bool IsXNameProperty(INode node, INode parentNode)
		{
			IElementNode elementNode = parentNode as IElementNode;
			INode node2;
			return elementNode != null && elementNode.Properties.TryGetValue(XmlName.xName, out node2) && node2 == node;
		}

		protected override void Visit(ValueNode node, INode parentNode)
		{
			if ( IsXNameProperty( node, parentNode ) )
			{
				try
				{
					( (INameScope)( (IElementNode)parentNode ).Namescope ).RegisterName( (string)node.Value, this.Values[parentNode] );
				}
				catch ( ArgumentException ex )
				{
					if ( ex.ParamName != "name" )
					{
						throw ex;
					}
					throw new XamlParseException( string.Format( "An element with the name \"{0}\" already exists in this NameScope", new object[]
					{
						(string)node.Value
					} ), node );
				}
			}
		}
	}
}
