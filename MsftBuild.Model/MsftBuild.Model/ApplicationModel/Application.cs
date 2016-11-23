using MsftBuild.Model.Serialization;
using System;
using System.Linq;
using System.Windows.Input;

namespace MsftBuild.Model.ApplicationModel
{
	public class Application : ICommand
	{
		readonly static string[] RequiredParameters = { "ProcessorFile", "ProjectFile" };

		public static Application Default { get; } = new Application();
		Application() : this( ArgumentParser.Default, SerializerLocator.Default ) {}

		public event EventHandler CanExecuteChanged = delegate {};

		readonly IArgumentParser parser;
		
		readonly ISerializerLocator serializerLocator;

		public Application( IArgumentParser parser, ISerializerLocator serializerLocator )
		{
			this.parser = parser;
			this.serializerLocator = serializerLocator;
		}

		public bool CanExecute( object parameter )
		{
			var input = parameter as string[];
			if ( input != null )
			{
				var dictionary = parser.Parse( input );
				var result = RequiredParameters.All( dictionary.ContainsKey );
				return result;
			}
			return false;
		}

		public void Execute( object parameter )
		{
			var input = parameter as string[];
			if ( input != null )
			{
				var arguments = parser.Parse( input );

				// Get the processor:
				var processorFile = arguments[Arguments.ProcessorFile];
				var processor = Load<IProcessor>( processorFile );

				// Get the data file (inputs to process):
				var projectFile = arguments[Arguments.ProjectFile];
				var project = Load<IProject>( projectFile );

				// Run it:
				var context = new ProcessingContext( project );
				processor.Execute( context );
			}
		}

		T Load<T>( string fileName )
		{
			var serializer = serializerLocator.Locate( fileName );
			var result = serializer.Load<T>( fileName );
			return result;
		}
	}
}