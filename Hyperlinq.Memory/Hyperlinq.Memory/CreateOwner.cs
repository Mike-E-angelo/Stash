using NetFabric.Hyperlinq;
using System.Collections.Generic;

namespace Hyperlinq.Memory
{
	sealed class CreateOwner<T>
	{
		public static CreateOwner<T> Default { get; } = new CreateOwner<T>();

		CreateOwner() {}

		readonly Owners<T> _pool;

		public CreateOwner(Owners<T> pool) => _pool = pool;

		public Owner<T> Get(IEnumerable<T> parameter, uint size = 4)
		{
			var result = _pool.Get(in size);
			var count  = 0;
			var span   = result.Memory.Span;
			foreach (var i in parameter.AsValueEnumerable())
			{
				span[count++] = i;
			}

			return result;
		}
	}
}