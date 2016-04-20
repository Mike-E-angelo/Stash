using System;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("Key")]
	internal sealed class StaticResourceExtension : IMarkupExtension
	{
		public string Key
		{
			get;
			set;
		}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (this.Key == null)
			{
				throw new XamlParseException("you must specify a key in {StaticResource}");
			}
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			IProvideParentValues provideParentValues = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideParentValues;
			if (provideParentValues == null)
			{
				throw new ArgumentException();
			}
			IXmlLineInfoProvider xmlLineInfoProvider = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider;
			IXmlLineInfo xmlInfo = (xmlLineInfoProvider != null) ? xmlLineInfoProvider.XmlLineInfo : null;
			foreach (object current in provideParentValues.ParentObjects)
			{
				VisualElement visualElement = current as VisualElement;
				object obj;
				if (visualElement != null && visualElement.Resources != null && visualElement.Resources.TryGetValue(this.Key, out obj))
				{
					object result = obj;
					return result;
				}
			}
			if (Application.Current != null && Application.Current.Resources != null && Application.Current.Resources.ContainsKey(this.Key))
			{
				return Application.Current.Resources[this.Key];
			}
			throw new XamlParseException(string.Format("StaticResource not found for key {0}", new object[]
			{
				this.Key
			}), xmlInfo);
		}
	}
}
