using MsftBuild.Model.ApplicationModel;
using System;

namespace MsftBuild
{
	public class Program
	{
		// Some examples of possible CLI invocations:
		// MsftBuild /ProccessorFile:Processors\Processor.json /ProjectFile:Projects\Project.xaml
		//		(JSON-based Processor with a Xaml-based Project file.)
		// MsftBuild /ProccessorFile:Processors\Processor.xaml /ProjectFile:Projects\Project.json
		//		(Xaml-based Processor with a JSON-based Project file.)
		// MsftBuild /ProccessorFile:Processors\Processor.xml /ProjectFile:Projects\Project.json
		//		(XML-based Processor with a JSON-based Project file.)
		static void Main( string[] args )
		{
			throw new InvalidOperationException( 
				@"No, this sample doesn't actually work at present, but is meant for envisioning, design consideration, and direction purposes. However, feel free to modify it so that it does work. :)" 
			);

			if ( Application.Default.CanExecute( args ) )
			{
				Application.Default.Execute( args );
			}
		}
	}
}
