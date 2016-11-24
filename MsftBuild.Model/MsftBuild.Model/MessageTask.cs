using System;
using System.IO;

namespace MsftBuild.Model
{
	public class MessageTask : ITask
	{
		readonly TextWriter writer;

		public MessageTask() : this( Console.Out ) {}

		public MessageTask( TextWriter writer )
		{
			this.writer = writer;
		}

		public string Message { get; set; }

		public void Execute( IProcessingContext context ) => writer.WriteLine( Message );
	}
}