using Newtonsoft.Json;
using Xunit;

namespace MsftBuild.Model.Testing
{
	public class ProjectTests
	{
		[Fact]
		public void Verify()
		{
			var project = new SampleProject.Project();
			var json = JsonConvert.SerializeObject( project );
			var expected = @"[{""Name"":""Console App"",""Version"":null,""Author"":""Mr. Awesome""},{""Configurations"":[{""Name"":""Debug"",""Selected"":true,""Logging"":{""Enabled"":true,""MinimumLevel"":1},""Directives"":[]},{""Name"":""Release"",""Selected"":false,""Logging"":{""Enabled"":false,""MinimumLevel"":2},""Directives"":[]}],""Files"":[],""Dependencies"":[{""Name"":""ExternalProject""}]}]";
			Assert.Equal( expected, json );
		}
	}
}
