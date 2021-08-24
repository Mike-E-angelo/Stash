namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface IResult<out T>
	{
		T Get();
	}
}