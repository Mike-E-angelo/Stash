using System.Collections.Generic;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface IForm<TIn, out T> : ISelect<In<TIn>, IAsyncEnumerable<T>> where T : class {}
}