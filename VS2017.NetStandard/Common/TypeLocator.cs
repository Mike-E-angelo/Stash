using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Common
{
	class TypeLocator : ISource<Type, Type>
	{
		public static TypeLocator Default { get; } = new TypeLocator();
		TypeLocator() : this(PlatformTypes.Default.Get()) {}


		readonly ImmutableArray<Type> _candidates;

		public TypeLocator(ImmutableArray<Type> candidates)
		{
			_candidates = candidates;
		}

		public Type Get(Type parameter) => _candidates.FirstOrDefault(parameter.IsAssignableFrom);
	}
}