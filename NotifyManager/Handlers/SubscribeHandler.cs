using Microsoft.Azure.NotificationHubs;

namespace NotifyManager.Handlers;

public class SubscribeHandler(IOptions<AzureNotificationHubOptions> azureOptions, ILogger<SubscribeHandler> logger) : ISubscribeInputPort
{

    public async Task SubscribeAsync(SubscriptionDto subscription)
    {
        // Create NotificationHubClient using the connection string
        NotificationHubClient hubClient = NotificationHubClient.CreateClientFromConnectionString(
            azureOptions.Value.ConnectionString,
            azureOptions.Value.HubName);

        // Create the installation object
        Microsoft.Azure.NotificationHubs.Installation installation = new Microsoft.Azure.NotificationHubs.Installation
        {
            InstallationId = subscription.InstallationId,
            Platform = ParseNotificationPlatform(subscription.Platform),
            Tags = subscription.Tags?.ToList()
        };

        // Set platform-specific push channel
        if (subscription.Platform == "fcmv1")
        {
            installation.PushChannel = subscription.NotificationHandle;
        }
        else
        {
            throw new NotSupportedException($"Platform '{subscription.Platform}' is not supported");
        }

        // Add template
        installation.Templates = new Dictionary<string, InstallationTemplate>
        {
            ["defaultTemplate"] = new InstallationTemplate
            {
                Body = PrepareTemplatePlatformSpecific(subscription).ToString()
            }
        };

        try
        {
            await hubClient.CreateOrUpdateInstallationAsync(installation);
            logger.LogInformation("Successfully registered/updated installation {InstallationId} for platform {Platform}",
                subscription.InstallationId, subscription.Platform);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register/update installation {InstallationId}", subscription.InstallationId);
            throw;
        }
    }

    private object PrepareTemplatePlatformSpecific(SubscriptionDto subscription)
    {
        string bodyPayloadJson;
        if (subscription.Platform == "fcmv1")
        {
            // Define FCM V1 notification template payload structure
            var bodyPayload = new { message = new { notification = new { title = "$(title)", body = "$(body)" }, data = subscription.PayLoad } };
            bodyPayloadJson = JsonSerializer.Serialize(bodyPayload);
        }
        else
        {
            throw new NotSupportedException($"Platform '{subscription.Platform}' is not supported");
        }
        return bodyPayloadJson;
    }



    private NotificationPlatform ParseNotificationPlatform(string platform)
    {
        return platform switch
        {
            "fcmv1" => NotificationPlatform.FcmV1,
            _ => throw new NotSupportedException($"Platform '{platform}' is not supported")
        };
    }

}
