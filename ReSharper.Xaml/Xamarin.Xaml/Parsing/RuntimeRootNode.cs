using System.Xml;

namespace Xamarin.Xaml
{
	class RuntimeRootNode : RootNode
	{
		public object Root { get; private set; }

		public RuntimeRootNode( XmlType xmlType, IXmlNamespaceResolver resolver, object root ) : base( xmlType, resolver )
		{
			Root = root;
		}
	}
}