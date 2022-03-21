using EfCore.CompiledQueries.BasicExpression.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.BasicExpression
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
				var query = EF.CompileAsyncQuery<Context, Statistic>(x => new Statistic
				{
					Day = x.Subjects.Count()
				});

				await query(context); // System.ArgumentException: 'Argument types do not match'
			}

			Debugger.Break();
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}


// TODO:
public class Statistic
{
	public int Day { get; set; }
	public int Week { get; set; }
	public long All { get; set; }
}
