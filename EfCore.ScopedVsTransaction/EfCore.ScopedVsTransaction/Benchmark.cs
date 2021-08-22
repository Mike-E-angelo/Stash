using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;

namespace EfCore.ScopedVsTransaction
{
	public class Benchmark
	{
		readonly DbContextOptions           _options;
		readonly IDbContextFactory<Context> _factory;
		readonly IQueryable<Subject>        _subjects;

		public Benchmark()
			: this(new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(new Guid().ToString()).Options) {}

		public Benchmark(DbContextOptions<Context> options)
			: this(options, new PooledDbContextFactory<Context>(options), new Context(options).Subjects) {}

		public Benchmark(DbContextOptions options, IDbContextFactory<Context> factory, IQueryable<Subject> subjects)
		{
			_options  = options;
			_factory  = factory;
			_subjects = subjects;
		}

		[Benchmark(Baseline = true)]
		public Subject[] Scoped() => _subjects.ToArray();

		[Benchmark]
		public Subject[] Pooled()
		{
			using var context = _factory.CreateDbContext();
			return context.Set<Subject>().ToArray();
		}

		[Benchmark]
		public Subject[] Transactional()
		{
			using var context = new Context(_options);
			return context.Set<Subject>().ToArray();
		}
	}
}