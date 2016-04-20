using System.Xml;

namespace Xamarin.Xaml
{
	public abstract class BaseNode : IXmlLineInfo, INode
	{
		public IXmlNamespaceResolver NamespaceResolver
		{
			get;
			private set;
		}
		public INode Parent
		{
			get;
			set;
		}
		public int LineNumber
		{
			get;
			set;
		}
		public int LinePosition
		{
			get;
			set;
		}
		protected BaseNode(IXmlNamespaceResolver namespaceResolver, int linenumber = -1, int lineposition = -1)
		{
			this.NamespaceResolver = namespaceResolver;
			this.LineNumber = linenumber;
			this.LinePosition = lineposition;
		}
		public abstract void Accept(IXamlNodeVisitor visitor, INode parentNode);
		public bool HasLineInfo()
		{
			return this.LineNumber >= 0 && this.LinePosition >= 0;
		}
	}
}
