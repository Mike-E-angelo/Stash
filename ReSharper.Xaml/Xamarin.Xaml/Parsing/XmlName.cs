using System.Diagnostics;

namespace Xamarin.Xaml
{
	[DebuggerDisplay("{NamespaceUri}:{LocalName}")]
	public struct XmlName
	{
		public static readonly XmlName _CreateContent = new XmlName("_", "CreateContent");
		public static readonly XmlName xKey = new XmlName("x", "Key");
		public static readonly XmlName xName = new XmlName("x", "Name");
		public static readonly XmlName xTypeArguments = new XmlName("x", "TypeArguments");
		public static readonly XmlName xArguments = new XmlName("x", "Arguments");
		public static readonly XmlName xFactoryMethod = new XmlName("x", "xFactoryMethod");
		readonly string namespaceUri;
		readonly string localName;

		public XmlName(string namespaceUri, string localName)
		{
			this.namespaceUri = namespaceUri;
			this.localName = localName;
		}

		public string NamespaceUri
		{
			get { return namespaceUri; }
		}

		public string LocalName
		{
			get { return localName; }
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(XmlName))
			{
				return false;
			}
			XmlName xmlName = (XmlName)obj;
			return this.NamespaceUri == xmlName.NamespaceUri && this.LocalName == xmlName.LocalName;
		}
		public override int GetHashCode()
		{
			int num = 0;
			if (this.namespaceUri != null)
			{
				num = this.namespaceUri.GetHashCode();
			}
			if (this.localName != null)
			{
				num = (num * 397 ^ this.localName.GetHashCode());
			}
			return num;
		}
		public static bool operator ==(XmlName x1, XmlName x2)
		{
			return x1.NamespaceUri == x2.NamespaceUri && x1.LocalName == x2.LocalName;
		}
		public static bool operator !=(XmlName x1, XmlName x2)
		{
			return !(x1 == x2);
		}
	}
}