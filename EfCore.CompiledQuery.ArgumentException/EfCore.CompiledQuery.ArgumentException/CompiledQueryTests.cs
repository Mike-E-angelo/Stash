using EfCore.CompiledQuery.ArgumentException.Model;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EfCore.CompiledQuery.ArgumentException
{
	public sealed class CompiledQueryTests
	{

		[Fact]
		public async Task Works()
		{
			var factory = new InMemoryDbContexts<Context>();
			{
				await using var context = factory.CreateDbContext();
				context.Subjects.AddRange(new Subject { Name = "One" }, new Subject { Name = "Two" },
				                          new Subject { Name = "Three" });
				await context.SaveChangesAsync();
			}

			var evaluate = new SubjectsNotTwo(new DbContexts<Context>(factory));
			{
				var results = await evaluate.Get(None.Default);
				results.Should().HaveCount(2);
				results.Select(x => x.Name).Should().BeEquivalentTo("One", "Three");
			}


		}


		[Fact]
		public async Task DoesNotWork()
		{
			var factory = new InMemoryDbContexts<Context>();
			{
				await using var context = factory.CreateDbContext();
				context.Subjects.AddRange(new Subject { Name = "One" }, new Subject { Name = "Two" },
				                          new Subject { Name = "Three" });
				await context.SaveChangesAsync();
			}

			var evaluate = new SubjectsNotTwo(new DbContexts<Context>(factory), Complex.Default);
			{
				var results = await evaluate.Get(None.Default);
				results.Should().HaveCount(2);
				results.Select(x => x.Name).Should().BeEquivalentTo("One", "Three");
			}


		}
	}
}