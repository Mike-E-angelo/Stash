namespace MsftBuild.Model
{
	public interface IProcessor
	{
		void Execute( IProcessingContext context );
	}
}