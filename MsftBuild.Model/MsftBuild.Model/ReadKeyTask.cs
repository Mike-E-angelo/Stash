using System;
using System.IO;

namespace MsftBuild.Model
{
	public class ReadKeyTask : ITask
	{
		readonly TextWriter writer;
		readonly TextReader reader;
		public ReadKeyTask() : this( Console.Out, Console.In ) {}
		public ReadKeyTask( TextWriter writer, TextReader reader )
		{
			this.writer = writer;
			this.reader = reader;
		}

		public string Message { get; set; }

		public string Exiting { get; set; }

		public void Execute( IProcessingContext parameter )
		{
			writer.WriteLine();
			writer.Write( Message );
			reader.ReadLine();

			writer.Write( Exiting );
		}
	}
}