using System;
using System.Collections.Generic;
using System.Linq;

namespace MsftBuild.Model
{
	public interface IProject : IServiceProvider {}
	public class Project : List<object>, IServiceProvider
	{
		public object GetService( Type serviceType ) => this.First( serviceType.IsInstanceOfType );
	}
	
	public class TargetFramework
	{
		public string FrameworkMoniker { get; set; }

		public string FrameworkProfile { get; set; }

		public Version Version { get; set; }
	}

	public class AssemblyInformation
	{
		public string Name { get; set; }
		public Version Version { get; set; }
		public string Author { get; set; }
	}

	public class BuildProfile
	{
		public List<BuildConfiguration> Configurations { get; } = new List<BuildConfiguration>();

		public List<IFile> Files { get; set; } = new List<IFile>();

		public List<Dependency> Dependencies { get; set; } = new List<Dependency>();
	}

	public class Dependency
	{
		public string Name { get; set; }
	}

	public interface IFile {}

	public class BuildConfiguration
	{
		public string Name { get; set; }

		public bool Selected { get; set; }

		public LoggingSettings Logging { get; set; }

		public List<Directive> Directives { get; } = new List<Directive>();
	}

	public class Directive
	{
		public string Symbol { get; set; }

		public bool Enabled { get; set; }
	}

	public enum LogEventLevel { Verbose, Debug, Information, Warning, Error, Fatal, }

	public class LoggingSettings
	{
		public bool Enabled { get; set; } = true;

		public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Information;
	}
}