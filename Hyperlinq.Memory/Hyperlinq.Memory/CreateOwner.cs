using NetFabric.Hyperlinq;
using System.Collections.Generic;

namespace Hyperlinq.Memory
{
	public sealed class CreateOwner<T>
	{
		public static CreateOwner<T> Default { get; } = new CreateOwner<T>();

		CreateOwner() {}

		public Owner<T> Get(IEnumerable<T> parameter, int size = 4)
		{
			var result = new Owner<T>(size);
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