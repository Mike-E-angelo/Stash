using EfCore.CompiledQueries.ClientEvaluation.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation
{
	sealed class Program
	{
		public static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

		static IHostBuilder CreateHostBuilder(string[] args)
			=> Host.CreateDefaultBuilder(args)
			       .ConfigureServices((host, services) =>
			                          {
				                          services.AddDbContextFactory<Context>(x => x.UseSqlServer(host.Configuration.GetConnectionString("ContextConnection")).EnableSensitiveDataLogging());

				                          services.AddHostedService<Worker>();
			                          });

	}



	sealed class Worker : IHostedService
	{
		readonly IDbContextFactory<Context> _contexts;

		public Worker(IDbContextFactory<Context> contexts) => _contexts = contexts;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await using var context = _contexts.CreateDbContext();
			await context.Database.EnsureDeletedAsync();
			if (await context.Database.EnsureCreatedAsync())
			{
				context.Subjects.AddRange(new Subject { Name = "One" }, new Subject { Name = "Two" });
				await context.SaveChangesAsync();
			}

			var evaluate = new SubjectsNotTwo(new DbContexts<Context>(_contexts));
			await evaluate.Get(None.Default);
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}

	sealed class Query : Query<None, Subject>
	{
		public static Query Default { get; } = new Query();

		Query() : this(Where.Default.Get) {}

		Query(Func<IQueryable<Subject>, IQueryable<Subject>> select) : base((q, _) => select(q.Set<Subject>())) {}
	}

	sealed class Where : ISelect<IQueryable<Subject>, IQueryable<Subject>>
	{
		public static Where Default { get; } = new Where();

		Where() {}

		public IQueryable<Subject> Get(IQueryable<Subject> parameter) => parameter.Where(x => x.Name != "Two");
	}

	sealed class SubjectsNotTwo : EvaluateToArray<Context, None, Subject>
	{
		public SubjectsNotTwo(IContexts<Context> contexts) : this(contexts, Query.Default) {}

		public SubjectsNotTwo(IContexts<Context> contexts, IQuery<None, Subject> query) : base(contexts, query) {}
	}
}