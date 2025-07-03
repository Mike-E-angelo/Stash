using Aspects.Model;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace MauiApp7
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

	        var services = builder.Services.AddScoped<MainModelHierarchy>().AddScoped<MainModel>();
	        Registrations.Default.Execute(services);

            return builder.Build();
        }
    }
}
