using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("Path")]
	internal sealed class BindingExtension : IMarkupExtension
	{
		public string Path
		{
			get;
			set;
		}
		public BindingMode Mode
		{
			get;
			set;
		}
		public IValueConverter Converter
		{
			get;
			set;
		}
		public object ConverterParameter
		{
			get;
			set;
		}
		public string StringFormat
		{
			get;
			set;
		}
		public object Source
		{
			get;
			set;
		}
		public BindingExtension()
		{
			this.Mode = BindingMode.Default;
			this.Path = ".";
		}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return new Binding(this.Path, this.Mode, this.Converter, this.ConverterParameter, this.StringFormat, this.Source);
		}
	}
}
