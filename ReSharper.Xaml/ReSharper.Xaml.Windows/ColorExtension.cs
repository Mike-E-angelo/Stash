using System;
using System.Windows.Data;
using System.Windows.Media;
using Xamarin.Forms.Xaml;

namespace ReSharper.Xaml.Windows
{
	public class ColorExtension : IMarkupExtension
	{
		public static ColorExtension Instance
		{
			get { return InstanceField; }
		}	static readonly ColorExtension InstanceField = new ColorExtension();

		public object ProvideValue( System.IServiceProvider serviceProvider )
		{
			return Brushes.Red;
		}
	}

	public class BindingExtension : Binding, IMarkupExtension
	{
		Object IMarkupExtension.ProvideValue( System.IServiceProvider serviceProvider )
		{
			return this;
		}
	}

	public class MessageExtension : IMarkupExtension
	{
		public static MessageExtension Instance
		{
			get { return InstanceField; }
		}	static readonly MessageExtension InstanceField = new MessageExtension();

		public Object ProvideValue( System.IServiceProvider serviceProvider )
		{
			return "Hello World from a Xamarin Markup Extension!!! WOOOO!!!!";
		}
	}

	public class DataModel
	{

		public string Message { get; set; }
	}
}