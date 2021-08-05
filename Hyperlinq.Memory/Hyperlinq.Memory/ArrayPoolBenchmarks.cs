using BenchmarkDotNet.Attributes;
using NetFabric.Hyperlinq;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Hyperlinq.Memory
{
	public class ArrayPoolBenchmarks
	{
		readonly int[]            numbers  = { 1, 2, 3, 4 };
		readonly IEnumerable<int> sequence = new[] { 1, 2, 3, 4 }.AsEnumerable();

		[Benchmark]
		public int LinqEnumerable() => sequence.Sum();

		[Benchmark]
		public int HyperlinqEnumerable()
		{
			using var values = sequence.AsValueEnumerable().ToArray(ArrayPool<int>.Shared);
			var       result = 0;
			var       memory = values.Memory;
			var       span   = memory.Span;
			for (int i = 0; i < memory.Length; i++)
			{
				result += span[i];
			}

			return result;
		}

		[Benchmark]
		public int BasicEnumerable()
		{
			using var owner  = CreateOwner<int>.Default.Get(sequence);
			var       result = 0;
			var       memory = owner.Memory;
			var       span   = memory.Span;
			for (var i = 0; i < span.Length; i++)
			{
				result += span[i];
			}

			return result;
		}

		[Benchmark]
		public int LinqArray() => numbers.Sum();

		[Benchmark]
		public int HyperlinqArray()
		{
			using var values = numbers.AsValueEnumerable().ToArray(ArrayPool<int>.Shared);
			var       result = 0;
			var       memory = values.Memory;
			var       span   = memory.Span;
			for (int i = 0; i < memory.Length; i++)
			{
				result += span[i];
			}

			return result;
		}

		[Benchmark]
		public int BasicArray()
		{
			using var owner  = CreateOwner<int>.Default.Get(numbers);
			var       result = 0;
			var       memory = owner.Memory;
			var       span   = memory.Span;
			for (var i = 0; i < span.Length; i++)
			{
				result += span[i];
			}

			return result;
		}
	}
}
