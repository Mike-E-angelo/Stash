namespace Common
{
	public interface ICommand<in T>
	{
		void Execute(T parameter);
	}
}