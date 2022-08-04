namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface IResult<out T>
	{
		T Get();
	}
}