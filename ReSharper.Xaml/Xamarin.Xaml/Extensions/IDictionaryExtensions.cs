using System.Collections.Generic;

namespace Xamarin.Xaml
{
	internal static class IDictionaryExtensions
	{
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			foreach (KeyValuePair<TKey, TValue> current in collection)
			{
				dictionary.Add(current);
			}
		}
	}
}
