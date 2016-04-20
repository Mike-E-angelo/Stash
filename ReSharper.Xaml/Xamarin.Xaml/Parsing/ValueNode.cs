using System.Xml;

namespace Xamarin.Xaml
{
	public class ValueNode : BaseNode, IValueNode, INode
	{
		public object Value
		{
			get;
			set;
		}
		public ValueNode(object value, IXmlNamespaceResolver namespaceResolver, int linenumber = -1, int lineposition = -1) : base(namespaceResolver, linenumber, lineposition)
		{
			this.Value = value;
		}
		public override void Accept(IXamlNodeVisitor visitor, INode parentNode)
		{
			visitor.Visit(this, parentNode);
		}
	}
}
