using System.Collections.Generic;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface IForm<TIn, out T> : ISelect<In<TIn>, IAsyncEnumerable<T>> where T : class {}
}