using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsV2Authentication
{
	public static class Function1
	{
		[FunctionName("Function1")]
		public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
		{
			log.Info("C# HTTP trigger function processed a request.");

			var authentication = req.Authenticate();

			return authentication != null
				? (ActionResult)new OkObjectResult($"Hello, {authentication.UserId}")
				: new BadRequestObjectResult("Authentication not found. :(");
		}
	}
}
