using Client.Maui.ApiConfig;
using Client.Maui.HubService;
using Client.Maui.HubService.Interfaces;
using Client.Maui.Platforms.Android;
using Microsoft.Extensions.Logging;

namespace Client.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        builder.Services.AddHttpClient<INotificationRegistrationService, NotificationRegistrationService>(c =>
    c.BaseAddress = new Uri(Config.BackendServiceEndpoint));
        builder.Services.AddSingleton<IDeviceInstallationService, DeviceInstallationService>();
        builder.Services.AddSingleton<IPushDemoNotificationActionService, PushDemoNotificationActionService>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
