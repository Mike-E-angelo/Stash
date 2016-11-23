using Fclp;

namespace MsftBuild.Model.ApplicationModel
{
	public class ArgumentParser : IArgumentParser
	{
		public static ArgumentParser Default { get; } = new ArgumentParser();
		ArgumentParser() {}

		public ApplicationArguments Parse( string[] parameter )
		{
			var parser = new FluentCommandLineParser<ApplicationArguments>();
			parser
				.Setup( arguments => arguments.ProcessorFile )
				.As( 'p', nameof(ApplicationArguments.ProcessorFile) )
				.Required()
				.WithDescription( "Processor file that describes the tasks to execute." );
			parser
				.Setup( arguments => arguments.ProjectFile )
				.As( 'd', nameof(ApplicationArguments.ProjectFile) )
				.Required()
				.WithDescription( "Project (data) file that describes the inputs to send to the processor." );

			var parsed = parser.Parse( parameter );
			var result = parsed.HasErrors ? null : parser.Object;
			return result;
		}
	}
}