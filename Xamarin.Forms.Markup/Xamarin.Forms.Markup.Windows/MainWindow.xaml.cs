using System;
using System.Diagnostics;
using System.Windows.Markup;

namespace Xamarin.Forms.Markup.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			Debug.Assert( ContentControl.Content is string );
		}
	}

	public class StringExtension : MarkupExtension
	{
		public override object ProvideValue( IServiceProvider serviceProvider )
		{
			return "Hello World... MarkupExtensions own your bones.";
		}
	}

	[ContentProperty( "SomeObject" )]
	public class ContainerExtension : MarkupExtension
	{
		public object SomeObject { get; set; }
		
		public override object ProvideValue( IServiceProvider serviceProvider )
		{
			return SomeObject;
		}
	}
}
