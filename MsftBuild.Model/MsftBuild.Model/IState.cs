namespace MsftBuild.Model
{
	public interface IState
	{
		T Get<T>() where T : class;

		void Set<T>( T value ) where T : class;
	}
}