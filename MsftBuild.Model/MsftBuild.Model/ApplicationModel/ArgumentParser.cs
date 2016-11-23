using System.Collections.Generic;

namespace MsftBuild.Model.ApplicationModel
{
	public class ArgumentParser : IArgumentParser
	{
		public static ArgumentParser Default { get; } = new ArgumentParser();
		ArgumentParser() {}

		// Very rudimentary (hard-coded) implementation for the files to grab.  Replace to get other file formats (as seen below).
		public IDictionary<string, string> Parse( string[] arguments ) => 
			new Dictionary<string, string>
			{
				{ Arguments.ProcessorFile, "Processor.xaml" }, // Replace with Processor.json or Processor.xml...
				{ Arguments.ProjectFile, "Program.xaml" } // Replace with Project.json or Project.xml...
			};
	}
}