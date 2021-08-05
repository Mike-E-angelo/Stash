using System.Buffers;

namespace Hyperlinq.Memory
{
	public interface IMemoryOwnerProducer<out T, V> where T : IMemoryOwner<V>
	{
		T Get(in uint size);
	}
}