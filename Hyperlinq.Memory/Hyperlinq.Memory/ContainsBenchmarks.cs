using BenchmarkDotNet.Attributes;
using NetFabric.Hyperlinq;
using System.Collections.Generic;
using System.Linq;

namespace Hyperlinq.Memory
{
	public class ContainsBenchmarks
	{
		readonly int[]            numbers  = { 1, 2, 3, 4 };
		readonly IEnumerable<int> sequence = new[] { 1, 2, 3, 4 }.AsEnumerable();

		[Benchmark]
		public bool LinqArray() => numbers.Contains(3);

		[Benchmark]
		public bool HyperlinqArray() => numbers.AsValueEnumerable().Contains(3);

		[Benchmark]
		public bool LinqEnumerable() => sequence.Contains(3);

		[Benchmark]
		public bool HyperlinqEnumerable() => sequence.AsValueEnumerable().Contains(3);
	}
}
