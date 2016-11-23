using System;

namespace MsftBuild.Model
{
	public class BuildTask : ITask
	{
		public void Execute( IServiceProvider provider, IState state )
		{
			// Get the BuildProfile that is defined in the project file, if it exists: 
			var profile = provider.GetService( typeof(BuildProfile) ) as BuildProfile ?? new DefaultBuildProfile();

			// From here, we could:
			// 1. Use XSLT to generate a traditional .csproj file, save it to a temp directory.
			// 2. Launch msbuild.exe and point it to .csproj from above.
			// 3. Profit. https://www.youtube.com/watch?v=tO5sxLapAts
		}

		class DefaultBuildProfile : BuildProfile {
			// ...
		}
	}
}