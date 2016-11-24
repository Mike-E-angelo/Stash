namespace MsftBuild.Model
{
	public interface ITask
	{
		void Execute( IProcessingContext context );
	}
}