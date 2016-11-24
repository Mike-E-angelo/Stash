using MsftBuild.Model.ApplicationModel;
using System;
using System.IO;

namespace MsftBuild.Model
{
	public class AssignCurrentDirectoryTask : ITask
	{
		readonly TextWriter writer;

		public AssignCurrentDirectoryTask() : this( Console.Out ) {}

		public AssignCurrentDirectoryTask( TextWriter writer )
		{
			this.writer = writer;
		}

		public void Execute( IProcessingContext context )
		{
			var arguments = context.Get<ApplicationArguments>();

			// Set the current directory to the processor file (temporary adjustment to get files to only load in specified folder and ignore others):
			var fullName = new FileInfo( arguments.ProcessorFile ).Directory.FullName;
			Directory.SetCurrentDirectory( fullName );
			writer.WriteLine( $"Assigned Current Directory to: {fullName}" );
		}
	}
}