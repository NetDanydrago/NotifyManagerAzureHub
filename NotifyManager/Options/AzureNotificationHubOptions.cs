namespace NotifyManager.Options;

/// <summary>
/// Configuration options for Azure Notification Hub
/// </summary>
public class AzureNotificationHubOptions
{

    public const string SectionKey = nameof(AzureNotificationHubOptions);

    /// <summary>
    /// Azure Notification Hub connection string
    /// </summary>
    public string ConnectionString { get; set; } 

    /// <summary>
    /// Azure Notification Hub name
    /// </summary>
    public string HubName { get; set; } 
}
