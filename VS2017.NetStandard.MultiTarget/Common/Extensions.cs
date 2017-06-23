using System;
using System.Reflection;

namespace Common
{
	public static class Extensions
	{
		public static T Get<T>(this ISource<Type, object> @this, object _) => (T)@this.Get(typeof(T));

		public static T Get<T>(this ISource<Type, object> @this) => (T) @this.Get(typeof(T));

		public static AssemblyName Get<T>(this ISource<AssemblyName, AssemblyName> @this) => @this.Get(typeof(T).GetTypeInfo()
		                                                                                                        .Assembly
		                                                                                                        .GetName());

		public static AssemblyName Get(this ISource<AssemblyName, AssemblyName> @this) =>
			@this.Get<ISource<object, object>>();
	}
}