using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("TypeName")]
	public class TypeExtension : IMarkupExtension
	{
		public string TypeName
		{
			get;
			set;
		}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
			if (xamlTypeResolver == null)
			{
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			}
			return xamlTypeResolver.Resolve(this.TypeName, serviceProvider);
		}
	}
}
