using System.Linq;
using System.Xml;

namespace Xamarin.Xaml
{
	public abstract class RootNode : ElementNode
	{
		protected RootNode(XmlType xmlType, IXmlNamespaceResolver resolver) : base(xmlType, resolver, -1, -1)
		{
		}
		/*public override void Accept(IXamlNodeVisitor visitor, INode parentNode)
		{
			if (!visitor.VisitChildrenFirst)
			{
				visitor.Visit(this, parentNode);
			}
			foreach (INode current in base.Properties.Values.ToList())
			{
				current.Accept(visitor, this);
			}
			foreach (INode current2 in base.CollectionItems)
			{
				current2.Accept(visitor, this);
			}
			if (visitor.VisitChildrenFirst)
			{
				visitor.Visit(this, parentNode);
			}
		}*/
	}
}
