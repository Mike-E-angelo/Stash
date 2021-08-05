using System;
using System.Buffers;

namespace Hyperlinq.Memory
{
	public readonly struct Owner<T> : IMemoryOwner<T>
	{
		readonly ArrayPool<T> _pool;
		readonly T[]          _store;

		public Owner(ArrayPool<T> pool, T[] store, Memory<T> memory)
		{
			Memory = memory;
			_pool  = pool;
			_store = store;
		}

		public Memory<T> Memory { get; }

		public void Dispose()
		{
			_pool.Return(_store);
		}
	}
}