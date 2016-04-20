using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("Key")]
	public sealed class DynamicResourceExtension : IMarkupExtension
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
				throw new XamlParseException("DynamicResource markup require a Key");
			}
			return new DynamicResource(this.Key);
		}
	}
}
