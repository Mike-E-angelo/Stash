using MsftBuild.Model.ApplicationModel;
using System;
using System.IO;

namespace MsftBuild.Model
{
	public class BuildProjectTask : ITask
	{
		readonly TextWriter writer;

		public BuildProjectTask() : this( Console.Out ) {}

		public BuildProjectTask( TextWriter writer )
		{
			this.writer = writer;
		}

		public void Execute( IProcessingContext context )
		{
			var arguments = context.Get<ApplicationArguments>();
			var information = (AssemblyInformation)context.GetService( typeof(AssemblyInformation) );
			var profile = context.GetService( typeof(BuildProfile) ) as BuildProfile ?? new DefaultBuildProfile();

			writer.WriteLine( $"{GetType().Name} has been executed with the following information:" );
			writer.WriteLine( $@"Processor File / Input File: ""{arguments.ProcessorFile}"" / ""{arguments.InputFile}""" );
			writer.WriteLine( $"Application Name: {information.Name}" );
			writer.WriteLine( $"Application Version: {information.Version}" );
			writer.WriteLine( $"Application Author: {information.Author}" );
			writer.WriteLine( string.Empty );
			writer.WriteLine( "Requested Files to Build:" );
			foreach ( var file in profile.Files )
			{
				writer.WriteLine( $"- {file.Path}" );
			}
			writer.WriteLine( string.Empty );
			writer.WriteLine( string.Empty );

			writer.WriteLine( "Thank you for building and exploring this sample.  Further steps could include:" );
			writer.WriteLine( "1. Serialize build profile, application information, and other services as XML." );
			writer.WriteLine( "2. Execute XSLT against XML from above to generate a traditional .csproj file, save result to a temp directory." );
			writer.WriteLine( "3. Launch msbuild.exe and point it to .csproj from above." );
			writer.WriteLine( string.Empty );
		}

		sealed class DefaultBuildProfile : BuildProfile {}
	}
}