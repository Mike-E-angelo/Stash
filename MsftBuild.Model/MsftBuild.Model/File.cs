namespace MsftBuild.Model
{
	public class File : IFile
	{
		public File() : this( string.Empty ) {}

		public File( string path )
		{
			Path = path;
		}

		public string Path { get; set; }
	}
}