using System.Xml;

namespace Xamarin.Xaml
{
	public class MarkupNode : BaseNode, IValueNode, INode
	{
		public string MarkupString
		{
			get;
			private set;
		}
		public MarkupNode(string markupString, IXmlNamespaceResolver namespaceResolver, int linenumber = -1, int lineposition = -1) : base(namespaceResolver, linenumber, lineposition)
		{
			this.MarkupString = markupString;
		}
		public override void Accept(IXamlNodeVisitor visitor, INode parentNode)
		{
			visitor.Visit(this, parentNode);
		}
	}
}
