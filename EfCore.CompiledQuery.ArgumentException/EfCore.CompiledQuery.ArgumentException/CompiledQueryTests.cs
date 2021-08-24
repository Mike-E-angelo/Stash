using EfCore.CompiledQuery.ArgumentException.Model;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
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

			var append   = new Append<None, Subject>(q => q.Where(x => x.Name != "Two"));
			var evaluate = new SubjectsNotTwo(new DbContexts<Context>(factory), append);
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

			var append   = new Append<None, Subject>((_, q) => q.Where(x => x.Name != "Two"));
			var evaluate = new SubjectsNotTwo(new DbContexts<Context>(factory), append);
			{
				var results = await evaluate.Get(None.Default);
				results.Should().HaveCount(2);
				results.Select(x => x.Name).Should().BeEquivalentTo("One", "Three");
			}
		}

		[Fact]
		public void WorksDirect()
		{
			Func<IQueryable<Subject>, IQueryable<Subject>> select = q => q.Where(x => x.Name != "Two");
			var expression = Expression((context, @in) => select(context.Set<Subject>()));
			var       compiled   = EF.CompileAsyncQuery(expression);
			var       factory    = new InMemoryDbContexts<Context>();
			using var instance   = factory.CreateDbContext();
			compiled(instance, None.Default);
		}

		[Fact]
		public void DoesNotWorkDirect()
		{
			Func<DbContext, IQueryable<Subject>, IQueryable<Subject>> select = (_, q) => q.Where(x => x.Name != "Two");
			Func<DbContext, None, IQueryable<Subject>, IQueryable<Subject>> next =
				(context, _, queryable) => select(context, queryable);

			var       expression = Expression((context, @in) => next(context, @in, context.Set<Subject>()));
			var       compiled   = EF.CompileAsyncQuery(expression);
			var       factory    = new InMemoryDbContexts<Context>();
			using var instance   = factory.CreateDbContext();
			compiled(instance, None.Default);
		}

		Expression<Func<DbContext, None, IQueryable<Subject>>> Expression(
			Expression<Func<DbContext, None, IQueryable<Subject>>> self) => self;
	}
}