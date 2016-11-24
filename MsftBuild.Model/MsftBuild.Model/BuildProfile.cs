namespace MsftBuild.Model
{
	public class BuildProfile
	{
		public IFileList Files { get; set; } = new FileList();

		public Collection<BuildConfiguration> Configurations { get; } = new Collection<BuildConfiguration>();

		public Collection<Dependency> Dependencies { get; } = new Collection<Dependency>();
	}
}