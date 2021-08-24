using System.Collections.Generic;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface IForm<TIn, out T> : ISelect<In<TIn>, IAsyncEnumerable<T>> where T : class {}
}