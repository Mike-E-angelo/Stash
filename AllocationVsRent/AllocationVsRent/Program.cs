using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Buffers;

namespace AllocationVsRent
{
	public sealed class Program
	{
		static void Main()
		{
			BenchmarkRunner.Run<Benchmarks>();
		}

		[MemoryDiagnoser]
		public class Benchmarks
		{
			readonly ArrayPool<int> _pool;

			public Benchmarks() : this(ArrayPool<int>.Shared) {}

			public Benchmarks(ArrayPool<int> pool) => _pool = pool;

			[Benchmark]
			public int Rented()
			{
				var array = _pool.Rent(2);
				array[0] = 1;
				array[1] = 2;
				var result = array[0] + array[1];
				_pool.Return(array);
				return result;
			}

			[Benchmark(Baseline = true)]
			public int Allocated()
			{
				var array = new int[2];
				array[0] = 1;
				array[1] = 2;

				var result = array[0] + array[1];
				return result;
			}
		}
	}
}