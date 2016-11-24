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
			new Version( File.Exists( path ) ? File.ReadAllText( path ) : "1.0.0.0" );
	}
}