using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Xamarin.Xaml
{
	public class ListNode : BaseNode, IListNode, IValueNode, INode
	{
		public ListNode(IEnumerable<INode> nodes, IXmlNamespaceResolver namespaceResolver, int linenumber = -1, int lineposition = -1) : base(namespaceResolver, linenumber, lineposition)
		{
			this.CollectionItems = nodes.ToList();
		}

		public ICollection<INode> CollectionItems
		{
			get;
			set;
		}

		public XmlName XmlName
		{
			get;
			set;
		}

		public override void Accept(IXamlNodeVisitor visitor, INode parentNode)
		{
			if (!visitor.VisitChildrenFirst)
			{
				visitor.Visit(this, parentNode);
			}
			foreach (INode current in this.CollectionItems)
			{
				current.Accept(visitor, this);
			}
			if (visitor.VisitChildrenFirst)
			{
				visitor.Visit(this, parentNode);
			}
		}
	}
}
