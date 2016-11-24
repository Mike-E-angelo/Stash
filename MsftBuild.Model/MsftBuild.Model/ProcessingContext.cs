using System;

namespace MsftBuild.Model
{
	public sealed class ProcessingContext : IProcessingContext
	{
		readonly IBuildInput project;
		readonly IState state;

		public ProcessingContext( IBuildInput project ) : this( project, new State() ) {}

		public ProcessingContext( IBuildInput project, IState state )
		{
			this.project = project;
			this.state = state;
		}

		public object GetService( Type serviceType ) => project.GetService( serviceType );
		public T Get<T>() where T : class => state.Get<T>();

		public void Set<T>( T value ) where T : class => state.Set( value );
	}
}