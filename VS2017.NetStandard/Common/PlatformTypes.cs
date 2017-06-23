using System;
using System.Collections.Immutable;

namespace Common {
	class PlatformTypes : ISource<ImmutableArray<Type>>
	{
		public static PlatformTypes Default { get; } = new PlatformTypes();
		PlatformTypes() : this(PlatformAssembly.Default.Get().GetExportedTypes()) { }

		readonly ImmutableArray<Type> _types;

		public PlatformTypes(params Type[] candidates)
		{
			_types = candidates.ToImmutableArray();
		}

		public ImmutableArray<Type> Get() => _types;
	}
}