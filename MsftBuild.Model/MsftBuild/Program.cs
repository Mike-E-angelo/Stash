using MsftBuild.Model.ApplicationModel;

namespace MsftBuild
{
	public static class Program
	{
		// Some examples of possible CLI invocations:
		// MsftBuild /p:SampleFormats\Json\Processor.json /i:SampleFormats\Json\Project.json
		//		(JSON-based Processor and Project file.)
		// MsftBuild /p:SampleFormats\Xml\Processor.xml /i:SampleFormats\Xml\Project.xml
		//		(Xml-based Processor and Project file.)
		// MsftBuild /p:SampleFormats\Xaml\Processor.xaml /i:SampleFormats\Xaml\Project.xaml
		//		(Xaml-based Processor and Project file.)
		//
		// Or, if you wanna get cute... :) :) :)
		//
		// MsftBuild /p:SampleFormats\Json\Processor.json /i:SampleFormats\Xaml\Project.xaml
		//		(JSON-based Processor with a Xaml-based Project file.)
		// MsftBuild /p:SampleFormats\Xaml\Processor.xaml /i:SampleFormats\Json\Project.json
		//		(Xaml-based Processor with a JSON-based Project file.)
		// MsftBuild /p:SampleFormats\Xml\Processor.xml /i:SampleFormats\Json\Project.json
		//		(XML-based Processor with a JSON-based Project file.)
		static void Main( string[] args )
		{
			var arguments = ArgumentParser.Default.Parse( args );
			if ( Application.Default.CanExecute( arguments ) )
			{
				Application.Default.Execute( arguments );
			}
		}
	}
}
