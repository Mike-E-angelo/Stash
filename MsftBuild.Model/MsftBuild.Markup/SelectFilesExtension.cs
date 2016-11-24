using MsftBuild.Model;
using System;
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
			new QueryableFileCollection { Query = query };
	}
}
