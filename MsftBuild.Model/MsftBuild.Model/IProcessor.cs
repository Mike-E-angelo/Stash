using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace MsftBuild.Model
{
	public interface IProcessor
	{
		void Execute( IServiceProvider provider );
	}

	[ContentProperty( nameof(Tasks) )]
	public class Processor : IProcessor
	{
		public void Execute( IServiceProvider provider )
		{
			foreach ( var task in Tasks )
			{
				task.Execute( provider );
			}
		}

		public Collection<ITask> Tasks { get; } = new Collection<ITask>();
	}

	public interface ITask
	{
		void Execute( IServiceProvider provider );
	}

	public class BuildProcessor : ITask
	{
		public void Execute( IServiceProvider provider )
		{
			var profile = provider.GetService( typeof(BuildProfile) ) ?? new DefaultBuildProfile();
			// ...
		}

		public class DefaultBuildProfile
		{
			// ...
		}
	}
}