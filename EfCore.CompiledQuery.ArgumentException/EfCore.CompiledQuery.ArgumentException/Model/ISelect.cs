namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface ISelect<in TIn, out TOut>
	{
		TOut Get(TIn parameter);
	}
}