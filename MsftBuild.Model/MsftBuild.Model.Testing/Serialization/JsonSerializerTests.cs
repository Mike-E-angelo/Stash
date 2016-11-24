using Contoso.Tasks;
using MsftBuild.Model.Serialization;
using System.IO;
using System.Linq;
using Xunit;
using JsonSerializer = MsftBuild.Model.Serialization.JsonSerializer;

namespace MsftBuild.Model.Testing.Serialization
{
	public class JsonSerializerTests
	{
		[Fact]
		public void VerifyJsonInput()
		{
			var project = XamlSerializer.Default.Load<IBuildInput>( new StreamReader( @"Xaml\Project.xaml" ).BaseStream );

			Directory.SetCurrentDirectory( Directory.CreateDirectory( @".\Other" ).FullName );

			var json = JsonSerializer.Default.Save( project );
			var expected = @"{""$type"":""MsftBuild.Model.Project, MsftBuild.Model"",""$values"":[{""$type"":""MsftBuild.Model.AssemblyInformation, MsftBuild.Model"",""Name"":""Console App"",""Version"":{""Major"":1,""Minor"":0,""Build"":0,""Revision"":0,""MajorRevision"":0,""MinorRevision"":0},""Author"":""Mr. Awesome""},{""$type"":""MsftBuild.Model.BuildProfile, MsftBuild.Model"",""Files"":{""$type"":""MsftBuild.Model.QueryableFileCollection, MsftBuild.Model"",""Query"":""**\\*.cs""},""Configurations"":[{""Name"":""Debug"",""Selected"":true,""Logging"":{""Enabled"":true,""MinimumLevel"":1},""Directives"":[]},{""Name"":""Release"",""Selected"":false,""Logging"":{""Enabled"":false,""MinimumLevel"":2},""Directives"":[]}],""Dependencies"":[{""Name"":""ExternalProject""}]}]}";
			Assert.Equal( expected, json );

			var deserialized = JsonSerializer.Default.Load<Project>( json );
			var information = Assert.IsType<AssemblyInformation>( deserialized.GetService( typeof(AssemblyInformation) ) );
			Assert.Equal( "Console App", information.Name );
			Assert.Equal( "Mr. Awesome", information.Author );
			var profile = (BuildProfile)deserialized.GetService( typeof(BuildProfile) );
			var files = Assert.IsType<QueryableFileCollection>( profile.Files );
			Assert.Equal( @"**\*.cs", files.Query );
		}

		[Fact]
		public void VerifyJsonProcessor()
		{
			var processor = XamlSerializer.Default.Load<IProcessor>( new StreamReader( @"Xaml\Processor.xaml" ).BaseStream );
			var json = JsonSerializer.Default.Save( processor );

			var expected = @"{""$type"":""MsftBuild.Model.Processor, MsftBuild.Model"",""$values"":[{""$type"":""MsftBuild.Model.MessageTask, MsftBuild.Model"",""Message"":""Welcome to the MsftBuild POCO-based Proof of Concept Application.  Building your provided input...""},{""$type"":""MsftBuild.Model.AssignCurrentDirectoryTask, MsftBuild.Model""},{""$type"":""Contoso.Tasks.CustomTask, Contoso.Tasks"",""SomeProperty"":""Hello World!"",""SomeDate"":""2016-11-23T00:00:00"",""SomeNumber"":123456789},{""$type"":""MsftBuild.Model.BuildProjectTask, MsftBuild.Model""},{""$type"":""MsftBuild.Model.MessageTask, MsftBuild.Model"",""Message"":""Build completed.""},{""$type"":""MsftBuild.Model.ReadKeyTask, MsftBuild.Model"",""Message"":""Press Enter to Continue..."",""Exiting"":""Now Exiting... Have a Nice Day. :)""}]}";
			Assert.Equal( expected, json );

			var deserialized = JsonSerializer.Default.Load<Processor>( json );
			Assert.Equal( 6, deserialized.Count );
			var custom = Assert.Single( deserialized.OfType<CustomTask>() );
			Assert.Equal( 123456789, custom.SomeNumber );
			var first = Assert.IsType<MessageTask>( deserialized.First() );
			Assert.Equal( "Welcome to the MsftBuild POCO-based Proof of Concept Application.  Building your provided input...", first.Message );
			var last = Assert.IsType<ReadKeyTask>( deserialized.Last() );
			Assert.Equal( "Now Exiting... Have a Nice Day. :)", last.Exiting );
		}
	}
}
