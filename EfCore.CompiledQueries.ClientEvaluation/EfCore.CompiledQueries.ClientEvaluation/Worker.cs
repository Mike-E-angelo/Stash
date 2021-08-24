using EfCore.CompiledQueries.ClientEvaluation.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation
{
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
}