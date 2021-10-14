using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Refit;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Http.Policy
{
	public sealed class Program
	{
		static Task Main()
		{
			var collection = new ServiceCollection();
			var @base      = new Uri("https://localhost:44399");
			var services = collection.AddRefitClient<IPreview>()
			                         .ConfigureHttpClient(client => client.BaseAddress = @base);

			var first = HttpPolicyExtensions.HandleTransientHttpError()
			                                .OrResult(y => y.StatusCode == System.Net.HttpStatusCode.NotFound)
			                                .WaitAndRetryAsync(5, count => TimeSpan.FromSeconds(Math.Pow(2, count)));
			var second = HttpPolicyExtensions.HandleTransientHttpError()
			                                 .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
			services.AddPolicyHandler(first).AddPolicyHandler(second);
			return Task.CompletedTask;
		}
	}

	interface IPreview
	{
		[Post("/preview/generate")]
		Task Generate([Body] PreviewDefinition definition);
	}

	sealed class PreviewDefinition {}
}