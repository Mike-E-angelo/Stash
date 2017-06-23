using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Platform
{
	public class AssemblyProvider : IAssemblyProvider
	{
		public IEnumerable<Assembly> Get() => DependencyContext.Default
		                                                       .GetRuntimeAssemblyNames(RuntimeEnvironment
			                                                                                .GetRuntimeIdentifier())
		                                                       .Select(x => new AssemblyName(x.Name))
		                                                       .Select(Assembly.Load);
	}
}