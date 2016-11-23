using System;

namespace MsftBuild.Model
{
	public class TargetFramework
	{
		public string FrameworkMoniker { get; set; }

		public string FrameworkProfile { get; set; }

		public Version Version { get; set; }
	}
}