using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace MsftBuild.Model
{
	[ContentProperty( nameof(Tasks) )]
	public class Processor : IProcessor
	{
		public void Execute( IProcessingContext context )
		{
			foreach ( var task in Tasks )
			{
				task.Execute( context, context.State );
			}
		}

		public Collection<ITask> Tasks { get; } = new Collection<ITask>();
	}
}