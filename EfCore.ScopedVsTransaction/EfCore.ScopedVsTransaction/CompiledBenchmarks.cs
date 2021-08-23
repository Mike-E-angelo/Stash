using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCore.ScopedVsTransaction
{
	public class CompiledBenchmarks
	{
		readonly IDbContextFactory<Context>               _factory;
		readonly Func<Context, IAsyncEnumerable<Subject>> _compile;
		readonly IQueryable<Subject>                      _subjects;

		public CompiledBenchmarks()
			: this(new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(new Guid().ToString()).Options) {}

		public CompiledBenchmarks(DbContextOptions<Context> options)
			: this(new PooledDbContextFactory<Context>(options),
			       Select.Default.Get(new Context(options).Set<Subject>().AsNoTracking())) {}

		public CompiledBenchmarks(IDbContextFactory<Context> factory, IQueryable<Subject> subjects)
			: this(factory, EF.CompileAsyncQuery<Context, Subject>(x => x.Subjects.AsNoTracking()
			                                                             .Where(y => y.Name != "Two")
			                                                             .Where(y => y.Name != "Two")
			                                                             .Where(y => y.Name != "Two")), subjects) {}

		public CompiledBenchmarks(IDbContextFactory<Context> factory, Func<Context, IAsyncEnumerable<Subject>> compile,
		                          IQueryable<Subject> subjects)
		{
			_factory  = factory;
			_compile  = compile;
			_subjects = subjects;
		}

		[Benchmark(Baseline = true)]
		public Task<Subject[]> Scoped() => _subjects.ToArrayAsync();

		[Benchmark]
		public async Task<Subject[]> Pooled()
		{
			await using var context = _factory.CreateDbContext();
			var             compile = await _compile(context).ToArrayAsync();
			return compile;
		}
	}
}