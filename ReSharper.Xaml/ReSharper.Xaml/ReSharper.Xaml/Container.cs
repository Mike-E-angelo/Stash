using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReSharper.Xaml
{
	public abstract class Container : Container<object>
	{}

	[ContentProperty( "Item" )]
	public abstract class Container<T> : Page, IContainer
	{
		public T Item
		{
			get { return item; }
			set
			{
				if ( !Equals( item, value ) )
				{
					OnPropertyChanging();

					item = value;

					OnPropertyChanged();
				}
			}
		}	T item;

		public static implicit operator T( Container<T> container )
		{
			return container.Item;
		}

		object IContainer.Item 
		{
			get { return Item; }
		}
	}

	public interface IContainer
	{
		object Item { get; }
	}

	[ContentProperty( "Instance" )]
	public class Item : IMarkupExtension
	{
		public IContainer Instance { get; set; }
		
		public object ProvideValue( IServiceProvider serviceProvider )
		{
			return Instance.Item;
		}
	}
}