using MsftBuild.Model;
using System;
using System.IO;

namespace Contoso.Tasks
{
	public class CustomTask : ITask
	{
		readonly TextWriter writer;

		public CustomTask() : this( Console.Out ) {}

		public CustomTask( TextWriter writer )
		{
			this.writer = writer;
		}

		public string SomeProperty { get; set; }

		public DateTime SomeDate { get; set; }

		public int SomeNumber { get; set; }

		public void Execute( IProcessingContext context )
		{
			writer.WriteLine( "This task simply writes out to the console the specified values:" );
			writer.WriteLine( "SomeProperty: {0}", SomeProperty );
			writer.WriteLine( "SomeDate: {0}", SomeDate );
			writer.WriteLine( "SomeNumber: {0}", SomeNumber );
			writer.WriteLine( string.Empty );
		}
	}
}
