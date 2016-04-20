using System.Xml;

namespace Xamarin.Xaml
{
	public interface INode
	{
		IXmlNamespaceResolver NamespaceResolver
		{
			get;
		}
		INode Parent
		{
			get;
			set;
		}
		void Accept(IXamlNodeVisitor visitor, INode parentNode);
	}
}
