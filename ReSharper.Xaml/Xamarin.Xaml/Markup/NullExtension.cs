using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class NullExtension : IMarkupExtension
	{
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return null;
		}
	}
}
