using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwaitPerformance
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
			[Benchmark]
			public async Task<uint> AwaitList()
			{
				var list = new List<Task>();

				for (var i = 0u; i < 10; i++)
				{
					list.Add(Task.Delay(1));
				}

				await Task.WhenAll(list).ConfigureAwait(false);

				return 123;
			}

			[Benchmark]
			public async Task<uint> AwaitEach()
			{
				for (var i = 0u; i < 10; i++)
				{
					await Task.Delay(1).ConfigureAwait(false);
				}

				return 123;
			}
		}
	}
}