using System.Collections.Immutable;
using System.Reflection;

namespace Common.Testing
{
	class Subject : ISource<ImmutableArray<Assembly>>
	{
		readonly IAssemblyProvider _assemblies;

		public Subject(IAssemblyProvider assemblies)
		{
			_assemblies = assemblies;
		}

		public ImmutableArray<Assembly> Get() => _assemblies.Get()
		                                                    .ToImmutableArray();
	}
}