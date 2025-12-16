namespace NotifyManager.Internals.InputPorts;

/// <summary>
/// Input port for sending notifications with templates
/// </summary>
public interface ISendNotificationInputPort
{
    /// <summary>
    /// Sends a notification to Azure Notification Hub
    /// </summary>
    /// <param name="notification">The notification request with title, body, data and tags</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task SendNotificationAsync(NotificationDto notification);
}
