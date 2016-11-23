using MsftBuild.Model.Serialization;

namespace MsftBuild.Model.ApplicationModel
{
	public class ApplicationArguments
	{
		public string ProcessorFile { get; set; }
		public string ProjectFile { get; set; }
	}
	public class Application : CommandBase<ApplicationArguments>
	{
		public static Application Default { get; } = new Application();
		Application() : this( SerializerLocator.Default ) {}

		readonly ISerializerLocator serializerLocator;

		public Application( ISerializerLocator serializerLocator )
		{
			this.serializerLocator = serializerLocator;
		}

		public override void Execute( ApplicationArguments parameter )
		{
			// Get the processor:
			var processor = Load<IProcessor>( parameter.ProcessorFile );

			// Get the data file (inputs to process):
			var project = Load<IProject>( parameter.ProjectFile );

			// Run it:
			var context = new ProcessingContext( project );
			processor.Execute( context );
		}

		T Load<T>( string fileName )
		{
			var serializer = serializerLocator.Locate( fileName );
			var result = serializer.Load<T>( fileName );
			return result;
		}
	}
}