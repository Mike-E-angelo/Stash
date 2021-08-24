namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface IInvoke<in TIn, T> : ISelect<TIn, Invocation<T>> {}
}