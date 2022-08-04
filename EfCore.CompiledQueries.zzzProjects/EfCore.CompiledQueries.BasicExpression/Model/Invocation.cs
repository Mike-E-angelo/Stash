using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public readonly struct Invocation<T> : IAsyncDisposable, IDisposable
	{
		readonly IAsyncDisposable _disposable;

		public Invocation(IAsyncDisposable disposable, IAsyncEnumerable<T> elements)
		{
			_disposable = disposable;
			Elements    = elements;
		}

		public IAsyncEnumerable<T> Elements { get; }

		public ValueTask DisposeAsync() => _disposable.DisposeAsync();

		public void Dispose() {}
	}
}