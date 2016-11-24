namespace MsftBuild.Model
{
	public class Processor : Collection<ITask>, IProcessor
	{
		public void Execute( IProcessingContext context )
		{
			foreach ( var task in this )
			{
				task.Execute( context );
			}
		}
	}
}