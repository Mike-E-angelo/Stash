using System.Collections.Generic;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface IEvaluate<in T, TResult> : ISelecting<IAsyncEnumerable<T>, TResult> {}
}