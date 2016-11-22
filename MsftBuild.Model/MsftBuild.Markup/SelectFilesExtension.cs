using System;
using System.IO;
using System.Windows.Markup;

namespace MsftBuild.Markup
{
	public class SelectFilesExtension : MarkupExtension
	{
		readonly string query;

		public SelectFilesExtension( string query )
		{
			this.query = query;
		}

		public override object ProvideValue( IServiceProvider serviceProvider ) => 
			new DirectoryInfo( Directory.GetCurrentDirectory() ).GetFileSystemInfos( query );
	}
}
