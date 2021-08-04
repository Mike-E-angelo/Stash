using System;
using System.Buffers;

namespace Hyperlinq.Memory
{
	public readonly struct Owner<T> : IMemoryOwner<T>
	{
		readonly ArrayPool<T> _pool;

		public Owner(int size) : this(ArrayPool<T>.Shared, size) {}

		public Owner(ArrayPool<T> pool, int size) : this(pool, size, pool.Rent(size)) {}

		public Owner(ArrayPool<T> pool, int size, T[] store) : this(pool, store, store.AsMemory()[..size]) {}

		public Owner(ArrayPool<T> pool, T[] store, Memory<T> memory)
		{
			Store  = store;
			Memory = memory;
			_pool  = pool;
		}

		public T[] Store { get; }

		public Memory<T> Memory { get; }



		public void Dispose()
		{
			_pool.Return(Store);
		}
	}
}