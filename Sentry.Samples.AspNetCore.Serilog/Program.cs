using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sentry;
using Sentry.Extensibility;
using Serilog;
using System;
using System.Diagnostics;

namespace Samples.AspNetCore.Serilog;

public class Program
{
	public static void Main(string[] args) => BuildWebApp(args).Run();

	public static WebApplication BuildWebApp(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Host.UseSerilog((_, c) => c.ReadFrom.Configuration(builder.Configuration), writeToProviders: true);
		builder.WebHost.UseSentry();

		var application = builder.Build();
		Configure(application);

		return application;
	}

	private static void Configure(IApplicationBuilder app)
	{
		// An example ASP.NET Core middleware that throws an
		// exception when serving a request to path: /throw
		app.Use(async (context, next) =>
		        {
			        // See MinimumBreadcrumbLevel set at the Serilog configuration above
			        Log.Logger.Debug("Static Serilog logger debug log stored as breadcrumbs.");

			        var log = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

			        log.LogInformation("Handling some request...");

			        // Sends an event which includes the info and debug messages above
			        Log.Logger.Error("Logging using static Serilog directly also goes to Sentry."); // <-- This fires!
			        log.LogError(new InvalidOperationException("hello?"),
			                     "This is a problem"); // <-- This does NOT fire!
			        if (context.Request.Path == "/greetings")
			        {
				        context.Response.ContentType = "text/plain";
				        await context.Response.WriteAsync("Hello, World!");
			        }
			        else if (context.Request.Path == "/throw")
			        {
				        var hub = context.RequestServices.GetRequiredService<IHub>();
				        hub.ConfigureScope(s =>
				                           {
					                           // More data can be added to the scope like this:
					                           s.SetTag("Sample", "ASP.NET Core"); // indexed by Sentry
					                           s.SetExtra("Extra!", "Some extra information");
				                           });

				        // Logging through the ASP.NET Core `ILogger` while using Serilog
				        log.LogInformation("Logging info...");
				        log.LogWarning("Logging some warning!");

				        // The following exception will be captured by the SDK and the event
				        // will include the Log messages and any custom scope modifications
				        // as exemplified above.
				        throw new Exception("An exception thrown from the ASP.NET Core pipeline");
			        }
			        else
			        {
				        await next();
			        }
		        });
	}
}

sealed class ExampleEventProcessor : ISentryEventProcessor
{
	public SentryEvent Process(SentryEvent @event)
	{
		Debugger.Break();
		return @event;
	}
}