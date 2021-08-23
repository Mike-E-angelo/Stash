using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EfCore.ScopedVsTransaction
{
	[MemoryDiagnoser]
	public class ProvidedBenchmarks
	{
		private BlogContext _reusableContext { get; set; }
		private PooledDbContextFactory<BlogContext> _factory { get; set; }

		[GlobalSetup]
		public async Task Setup()
		{
			var options = new DbContextOptionsBuilder<BlogContext>().UseInMemoryDatabase("foo").Options;
			_reusableContext = new BlogContext(options);
			await _reusableContext.Database.EnsureDeletedAsync();
			_factory = new PooledDbContextFactory<BlogContext>(options);
		}

		[Benchmark]
		public Blog[] Same()
		{
			return _reusableContext.Blogs.AsNoTracking().ToArray();
		}

		[Benchmark]
		public Blog[] Pooled()
		{
			using var context = _factory.CreateDbContext();
			return context.Blogs.AsNoTracking().ToArray();
		}

		public class BlogContext : DbContext
		{
			public DbSet<Blog> Blogs { get; set; }
			public BlogContext(DbContextOptions options) : base(options) {}
		}

		public class Blog
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		public enum Providers { SqlServer, InMemory }
	}
}
