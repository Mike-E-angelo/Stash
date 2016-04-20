using System;
using System.Windows.Markup;
using Application = System.Windows.Application;

namespace ReSharper.Xaml.Windows
{
	[ContentProperty( "Instance" )]
	public class ItemExtension : MarkupExtension
	{
		public IContainer Instance { get; set; }
		
		public override object ProvideValue( IServiceProvider serviceProvider )
		{
			return Instance.Item;
		}
	}
}
