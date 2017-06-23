using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Platform
{
	public class AssemblyProvider : IAssemblyProvider
	{
		public IEnumerable<Assembly> Get() => AppDomain.CurrentDomain.GetAssemblies();
	}
}