using System.Collections.Generic;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface IEvaluate<in T, TResult> : ISelecting<IAsyncEnumerable<T>, TResult> {}
}