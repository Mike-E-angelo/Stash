using System;

namespace Common
{
	public class PlatformActivator : ISource<Type, object>
	{
		public static PlatformActivator Default { get; } = new PlatformActivator();
		PlatformActivator() : this(TypeLocator.Default.Get, Activator.CreateInstance) {}

		readonly Func<Type, Type> _type;
		readonly Func<Type, object> _activate;

		public PlatformActivator(Func<Type, Type> type, Func<Type, object> activate)
		{
			_type = type;
			_activate = activate;
		}

		public object Get(Type parameter) => _activate(_type(parameter));
	}
}