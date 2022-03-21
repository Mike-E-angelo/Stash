namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface ISelect<in TIn, out TOut>
	{
		TOut Get(TIn parameter);
	}
}