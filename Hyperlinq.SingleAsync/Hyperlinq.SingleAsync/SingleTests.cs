using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NetFabric.Hyperlinq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Hyperlinq.SingleAsync
{
	public sealed class SingleTests
	{
		[Fact]
		public async Task VerifyHyperlinq()
		{
			await using var context = new Context();
			await context.Database.EnsureCreatedAsync();
			var compile = EF.CompileAsyncQuery<DbContext, Subject>(x => x.Set<Subject>().Where(y => y.Name == "Two"));
			var single = await compile(context).AsAsyncValueEnumerable().SingleAsync();
			single.IsSome.Should().BeTrue();
			single.Value.Should().NotBeNull();
		}

		[Fact]
		public async Task VerifyLinq()
		{
			await using var context = new Context();
			await context.Database.EnsureCreatedAsync();
			var compile = EF.CompileAsyncQuery<DbContext, Subject>(x => x.Set<Subject>().Where(y => y.Name == "Two"));
			var single  = await compile(context).SingleAsync();
			single.Should().NotBeNull();
		}

		sealed class Context : DbContext
		{
			public Context()
				: this(new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(new Guid().ToString()).Options) {}

			public Context(DbContextOptions options) : base(options) {}

			public DbSet<Subject> Subjects { get; set; } = default!;

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				modelBuilder.Entity<Subject>()
				            .HasData(new Subject { Id = Guid.NewGuid(), Name = "One" },
				                     new Subject { Id = Guid.NewGuid(), Name = "Two" },
				                     new Subject { Id = Guid.NewGuid(), Name = "Three" });
			}
		}

		public sealed class Subject
		{
			public Guid Id { get; set; }

			public string Name { get; set; } = default!;
		}
	}
}
