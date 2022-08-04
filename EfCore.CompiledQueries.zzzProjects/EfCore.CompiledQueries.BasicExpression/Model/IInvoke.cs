namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface IInvoke<in TIn, T> : ISelect<TIn, Invocation<T>> {}
}