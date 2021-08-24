using EfCore.CompiledQueries.ClientEvaluation.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
}