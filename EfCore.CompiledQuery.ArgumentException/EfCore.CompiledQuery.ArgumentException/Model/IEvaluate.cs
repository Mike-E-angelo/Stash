using System.Collections.Generic;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface IEvaluate<in T, TResult> : ISelecting<IAsyncEnumerable<T>, TResult> {}
}