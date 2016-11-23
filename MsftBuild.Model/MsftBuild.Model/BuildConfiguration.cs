using System.Collections.Generic;

namespace MsftBuild.Model
{
	public class BuildConfiguration
	{
		public string Name { get; set; }

		public bool Selected { get; set; }

		public LoggingSettings Logging { get; set; }

		public List<Directive> Directives { get; } = new List<Directive>();
	}
}