using System;
using System.Buffers;

namespace Hyperlinq.Memory
{
	sealed class Owners<T> : IMemoryOwnerProducer<Owner<T>, T>
	{
		public static Owners<T> Default { get; } = new Owners<T>();

		Owners() : this(ArrayPool<T>.Shared) {}

		readonly ArrayPool<T> _pool;

		public Owners(ArrayPool<T> pool) => _pool = pool;

		public Owner<T> Get(in uint size)
		{
			var requested = (int)size;
			var store     = _pool.Rent(requested);
			return new(_pool, store, store.AsMemory()[..requested]);
		}
	}
}