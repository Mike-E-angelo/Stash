namespace MsftBuild.Model
{
	public class BuildConfiguration
	{
		public string Name { get; set; }

		public bool Selected { get; set; }

		public LoggingSettings Logging { get; set; }

		public System.Collections.Generic.List<Directive> Directives { get; } = new System.Collections.Generic.List<Directive>();
	}
}