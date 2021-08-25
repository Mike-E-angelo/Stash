using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConsoleApp2
{
	class Program
	{
		static async Task Main()
		{
			var builder = new HostBuilder();
			builder.UseEnvironment(EnvironmentName.Development);
			builder.ConfigureWebJobs(b =>
			                         {
				                         b.AddAzureStorageCoreServices();
			                         });
			builder.ConfigureLogging((context, b) =>
			                         {
				                         b.AddConsole();
			                         });
			builder.ConfigureWebJobs(b =>
			                         {
				                         b.AddAzureStorageCoreServices();
				                         b.AddAzureStorageQueues();
			                         });
			var host = builder.Build();
			using (host)
			{
				await host.RunAsync();
			}
		}

	}

	public class Functions
	{
		public static void ProcessQueueMessage([QueueTrigger("queue-name")] string message, ILogger logger)
		{
			logger.LogInformation(message);
		}
	}
}
