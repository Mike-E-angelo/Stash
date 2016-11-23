using System;

namespace MsftBuild.Model
{
	public interface IProcessor
	{
		void Execute( IProcessingContext context );
	}

	public interface IProcessingContext : IServiceProvider
	{
		IState State { get; }
	}

	class ProcessingContext : IProcessingContext
	{
		readonly IProject project;

		public ProcessingContext( IProject project ) : this( project, new State() ) {}

		public ProcessingContext( IProject project, IState state )
		{
			this.project = project;
			State = state;
		}

		public object GetService( Type serviceType ) => project.GetService( serviceType );

		public IState State { get; }
	}
}