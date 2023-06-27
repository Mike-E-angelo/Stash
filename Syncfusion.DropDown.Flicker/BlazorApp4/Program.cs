using BlazorApp4.Data;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

/*
var host = await Start.A.Host()
           .WithDefaultComposition()
           .RegisterModularity()
           .WithDeferredRegistrations()
           .WithAmbientConfiguration()
           .Configure(x => x.AddSingleton<WeatherForecastService>())
           .WithBlazorServerApplication()
           .NamedFromPrimaryAssembly()
           /*.WithConnectionConfigurations()
           .WithClientConnectionConfigurations()#1#
           .Configure(x => x.WithFrameworkConfigurations()
                            /*.WithDataSecurity()#1#
                            .WithPresentationConfigurations()
                            /*.WithCircuitDiagnostics()#1#)
           .As.Application()
           .Await(new HostBuilder());
await host.WaitForShutdownAsync();
*/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().Services.AddServerSideBlazor().Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

SyncfusionLicenseProvider.RegisterLicense(""); // TODO Add license

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();