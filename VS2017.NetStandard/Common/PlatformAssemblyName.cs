using System.Reflection;

namespace Common
{
	class PlatformAssemblyName : ISource<AssemblyName, AssemblyName>
	{
		public static PlatformAssemblyName Default { get; } = new PlatformAssemblyName();
		PlatformAssemblyName() : this("{0}.Platform") {}

		readonly string _format;

		public PlatformAssemblyName(string format)
		{
			_format = format;
		}

		public AssemblyName Get(AssemblyName parameter) => new AssemblyName(string.Format(_format, parameter.Name));
	}
}