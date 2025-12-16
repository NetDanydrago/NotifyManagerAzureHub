using NotifyManager.Controllers;
using NotifyManager.Handlers;

namespace NotifyManager;

/// <summary>
/// Extension methods for registering NotifyManager services
/// </summary>
public static class DependencyContainer
{
    public static IServiceCollection AddNotifyManagerCore(this IServiceCollection services, Action<AzureNotificationHubOptions> configureAzureOptions)
    {
        services.AddHttpClient();

        // Configure options if provided
        services.Configure(configureAzureOptions);

        // Register subscription handlers
        services.AddScoped<ISubscribeInputPort, SubscribeHandler>();
        services.AddScoped<ISubscribeController, SubscribeController>();

        // Register notification sending handlers
        services.AddScoped<ISendNotificationInputPort, SendNotificationHandler>();
        services.AddScoped<ISendNotificationController, SendNotificationController>();

        return services;
    }
}
