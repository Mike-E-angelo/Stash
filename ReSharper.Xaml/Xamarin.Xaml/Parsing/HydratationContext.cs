using System;
using System.Collections.Generic;

namespace Xamarin.Xaml
{
	public class HydratationContext
	{
		readonly IDictionary<INode, object> values = new Dictionary<INode, object>();
		readonly Dictionary<IElementNode, Type> types = new Dictionary<IElementNode, Type>();

		public IDictionary<INode, object> Values
		{
			get { return values; }
		}

		public Dictionary<IElementNode, Type> Types
		{
			get { return types; }
		}

		public HydratationContext ParentContext { get; set; }

		public object RootElement { get; set; }
	}
}
