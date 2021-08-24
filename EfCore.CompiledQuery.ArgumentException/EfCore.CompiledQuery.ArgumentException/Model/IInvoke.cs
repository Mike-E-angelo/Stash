namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface IInvoke<in TIn, T> : ISelect<TIn, Invocation<T>> {}
}