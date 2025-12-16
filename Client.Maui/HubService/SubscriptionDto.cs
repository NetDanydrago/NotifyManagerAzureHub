namespace Client.Maui.HubService;

public class SubscriptionDto
{
    /// <summary>
    /// User Id associated with the subscription
    /// </summary> 
    public string InstallationId { get; set; }

    /// <summary>
    /// Platform type (iOS, Android, Web, Windows, etc.)
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// Updated tags for targeting/segmentation
    /// </summary>
    public List<string> Tags { get; set; }

    /// <summary>
    /// Service-specific identifier for push notifications (token, endpoint, etc.)
    /// </summary>
    public string NotificationHandle { get; set; }

}
