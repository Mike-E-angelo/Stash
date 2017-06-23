namespace Common
{
	public interface ISource<out T>
	{
		T Get();
	}

	public interface ISource<in TParameter, out TResult>
	{
		TResult Get(TParameter parameter);
	}
}