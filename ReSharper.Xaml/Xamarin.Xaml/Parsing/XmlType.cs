using System.Collections.Generic;

namespace Xamarin.Xaml
{
	public class XmlType
	{
		readonly XmlName name;
		readonly XmlType[] typeArguments;
		/*public string NamespaceUri
		{
			get;
			private set;
		}
		public string Name
		{
			get;
			private set;
		}
		public XmlType[] TypeArguments
		{
			get;
			private set;
		}*/
		public XmlType(XmlName name, params XmlType[] typeArguments)
		{
			this.name = name;
			this.typeArguments = typeArguments;
		}

		public XmlName Name
		{
			get { return name; }
		}

		public XmlType[] TypeArguments
		{
			get { return typeArguments; }
		}
	}
} 