using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("Items")]
	public class ArrayExtension : IMarkupExtension
	{
		public IList Items
		{
			get;
			private set;
		}
		public Type Type
		{
			get;
			set;
		}
		public ArrayExtension()
		{
			this.Items = new List<object>();
		}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (this.Type == null)
			{
				throw new InvalidOperationException("Type argument mandatory for x:Array extension");
			}
			if (this.Items == null)
			{
				return null;
			}
			Array array = Array.CreateInstance(this.Type, new int[]
			{
				this.Items.Count
			});
			for (int i = 0; i < this.Items.Count; i++)
			{
				((IList)array)[i] = this.Items[i];
			}
			return array;
		}
	}
}
