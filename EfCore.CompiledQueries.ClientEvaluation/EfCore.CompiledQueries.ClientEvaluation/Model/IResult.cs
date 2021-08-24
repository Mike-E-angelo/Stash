namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface IResult<out T>
	{
		T Get();
	}
}