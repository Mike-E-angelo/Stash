using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCore.ScopedVsTransaction
{
	public class CompiledQueryBenchmarks
	{
		readonly IDbContextFactory<Context>               _factory;
		readonly Func<Context, IAsyncEnumerable<Subject>> _direct;
		readonly Func<Context, IAsyncEnumerable<Subject>> _delegated;

		public CompiledQueryBenchmarks()
			: this(new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(new Guid().ToString()).Options) {}

		public CompiledQueryBenchmarks(DbContextOptions<Context> options)
			: this(new PooledDbContextFactory<Context>(options), x => x.Subjects.AsNoTracking()
			                                                           .Where(y => y.Name != "Two")
			                                                           .Where(y => y.Name != "Two")
			                                                           .Where(y => y.Name != "Two")) {}

		public CompiledQueryBenchmarks(IDbContextFactory<Context> factory, Func<Context, IQueryable<Subject>> select)
			: this(factory, EF.CompileAsyncQuery<Context, Subject>(x => x.Subjects.AsNoTracking()
			                                                             .Where(y => y.Name != "Two")
			                                                             .Where(y => y.Name != "Two")
			                                                             .Where(y => y.Name != "Two")),
			       EF.CompileAsyncQuery<Context, Subject>(x => select(x))) {}

		public CompiledQueryBenchmarks(IDbContextFactory<Context> factory,
		                               Func<Context, IAsyncEnumerable<Subject>> direct,
		                               Func<Context, IAsyncEnumerable<Subject>> delegated)
		{
			_factory   = factory;
			_direct    = direct;
			_delegated = delegated;
		}

		[Benchmark]
		public async Task<Subject[]> Direct()
		{
			await using var context = _factory.CreateDbContext();
			var             compile = await _direct(context).ToArrayAsync();
			return compile;
		}

		[Benchmark]
		public async Task<Subject[]> Delegated()
		{
			await using var context = _factory.CreateDbContext();
			var             compile = await _delegated(context).ToArrayAsync();
			return compile;
		}

	}
}