namespace MsftBuild.Model
{
	public class LoggingSettings
	{
		public bool Enabled { get; set; } = true;

		public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Information;
	}
}