using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
	public class AssemblyProvider : IAssemblyProvider
	{
		public IEnumerable<Assembly> Get()
		{
#if !NETSTANDARD1_6
	return AppDomain.CurrentDomain.GetAssemblies();
#else
			return DependencyContext.Default
			                        .GetRuntimeAssemblyNames(RuntimeEnvironment
				                                                 .GetRuntimeIdentifier())
			                        .Select(x => new AssemblyName(x.Name))
			                        .Select(Assembly.Load);
#endif
		}
	}
}