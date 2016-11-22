using System;
using System.IO;
using System.Windows.Markup;

namespace MsftBuild.Markup
{
	public class VersionExtension : MarkupExtension
	{
		readonly string path;

		public VersionExtension() : this( "Version.txt" ) {}

		public VersionExtension( string path )
		{
			this.path = path;
		}

		public override object ProvideValue( IServiceProvider serviceProvider ) => 
			File.Exists( path ) ? new Version( File.ReadAllText( path ) ) : null;
	}
}