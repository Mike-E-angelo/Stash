using System.Collections.Generic;

namespace MsftBuild.Model
{
	public class BuildProfile
	{
		public List<BuildConfiguration> Configurations { get; } = new List<BuildConfiguration>();

		public List<IFile> Files { get; set; } = new List<IFile>();

		public List<Dependency> Dependencies { get; set; } = new List<Dependency>();
	}
}