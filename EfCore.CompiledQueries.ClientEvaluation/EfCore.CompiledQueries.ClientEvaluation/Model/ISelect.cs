namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface ISelect<in TIn, out TOut>
	{
		TOut Get(TIn parameter);
	}
}