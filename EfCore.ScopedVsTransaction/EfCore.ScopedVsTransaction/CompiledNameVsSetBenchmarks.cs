using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCore.ScopedVsTransaction
{
	public class CompiledNameVsSetBenchmarks
	{
		readonly IDbContextFactory<Context>               _factory;
		readonly Func<Context, IAsyncEnumerable<Subject>> _named;
		readonly Func<Context, IAsyncEnumerable<Subject>> _set;

		public CompiledNameVsSetBenchmarks()
			: this(new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(new Guid().ToString()).Options) {}

		public CompiledNameVsSetBenchmarks(DbContextOptions<Context> options)
			: this(new PooledDbContextFactory<Context>(options),
			       EF.CompileAsyncQuery<Context, Subject>(x => x.Set<Subject>()
			                                                    .AsNoTracking()
			                                                    .Where(y => y.Name != "Two")
			                                                    .Where(y => y.Name != "Two")
			                                                    .Where(y => y.Name != "Two")),
			       EF.CompileAsyncQuery<Context, Subject>(x => x.Subjects.AsNoTracking()
			                                                    .Where(y => y.Name != "Two")
			                                                    .Where(y => y.Name != "Two")
			                                                    .Where(y => y.Name != "Two"))) {}

		public CompiledNameVsSetBenchmarks(IDbContextFactory<Context> factory,
		                                   Func<Context, IAsyncEnumerable<Subject>> named,
		                                   Func<Context, IAsyncEnumerable<Subject>> set)
		{
			_factory = factory;
			_named   = named;
			_set     = set;
		}

		[Benchmark]
		public async Task<Subject[]> Named()
		{
			await using var context = _factory.CreateDbContext();
			var             compile = await _named(context).ToArrayAsync();
			return compile;
		}

		[Benchmark]
		public async Task<Subject[]> Set()
		{
			await using var context = _factory.CreateDbContext();
			var             compile = await _set(context).ToArrayAsync();
			return compile;
		}
	}
}