using EfCore.CompiledQueries.ClientEvaluation.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation
{
	sealed class Worker : IHostedService
	{
		readonly IDbContextFactory<Context> _contexts;
		readonly ILogger<Worker>            _logger;

		public Worker(IDbContextFactory<Context> contexts, ILogger<Worker> logger)
		{
			_contexts = contexts;
			_logger   = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await using var context = _contexts.CreateDbContext();
			await context.Database.EnsureDeletedAsync();
			if (await context.Database.EnsureCreatedAsync())
			{
				context.Subjects.AddRange(new Subject { Name = "One" }, new Subject { Name = "Two" },
				                          new Subject { Name = "Three" });
				await context.SaveChangesAsync();
			}

			var evaluate = new SubjectsNotTwo(new DbContexts<Context>(_contexts));
			var items    = await evaluate.Get(None.Default);
			_logger.LogInformation("There are {Count} Subjects.", items.Length);

			foreach (var subject in items)
			{
				_logger.LogInformation("Subject Name: {Name}", subject.Name);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}