using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	public class ElementNode : BaseNode, IValueNode, IElementNode
	{
		readonly XmlType xmlType;
		readonly ICollection<INode> items = new List<INode>(); 
		readonly IDictionary<XmlName, INode> properties = new Dictionary<XmlName, INode>();

		public ElementNode(XmlType xmlType, IXmlNamespaceResolver namespaceResolver, int linenumber = -1, int lineposition = -1) : base(namespaceResolver, linenumber, lineposition)
		{
			this.xmlType = xmlType;
		}

		public ICollection<INode> CollectionItems
		{
			get { return items; }
		}

		public IDictionary<XmlName, INode> Properties
		{
			get { return properties; }
		}

		public XmlType XmlType
		{
			get { return xmlType; }
		}

		string IElementNode.NamespaceUri
		{
			get { return xmlType.Name.NamespaceUri; }
		}

		/*public bool Xaml2009LanguagePrimitive
		{
			get { return xmlType.Name.NamespaceUri == KnownSchemas.Xaml2009; }
		}*/

		public object Namescope { get; set; }

		public override void Accept(IXamlNodeVisitor visitor, INode parentNode)
		{
			if (!visitor.VisitChildrenFirst)
			{
				visitor.Visit(this, parentNode);
			}
			foreach (INode current in this.Properties.Values.ToList())
			{
				current.Accept(visitor, this);
			}
			foreach (INode current2 in this.CollectionItems)
			{
				current2.Accept(visitor, this);
			}
			if (visitor.VisitChildrenFirst)
			{
				visitor.Visit(this, parentNode);
			}
		}
	}
}
