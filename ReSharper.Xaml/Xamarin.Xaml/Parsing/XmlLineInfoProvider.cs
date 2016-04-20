using System.Xml;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	internal class XmlLineInfoProvider : IXmlLineInfoProvider
	{
		public IXmlLineInfo XmlLineInfo
		{
			get;
			private set;
		}
		public XmlLineInfoProvider(IXmlLineInfo xmlLineInfo)
		{
			this.XmlLineInfo = xmlLineInfo;
		}
	}
}
