using MsftBuild.Model.Serialization;
using System.IO;

namespace MsftBuild.Model.ApplicationModel
{
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
			var project = Load<IBuildInput>( parameter.InputFile );

			// Run it:
			var context = new ProcessingContext( project );
			context.Set( parameter );
			processor.Execute( context );
		}

		T Load<T>( string fileName )
		{
			var serializer = serializerLocator.Locate( fileName );
			var result = serializer.Load<T>( new StreamReader( fileName ).BaseStream );
			return result;
		}
	}
}