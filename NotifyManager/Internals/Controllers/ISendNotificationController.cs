namespace NotifyManager.Internals.Controllers;

/// <summary>
/// Interface for the SendNotification controller
/// </summary>
public interface ISendNotificationController
{
    /// <summary>
    /// Sends a notification to Azure Notification Hub
    /// </summary>
    /// <param name="notification">The notification request with title, body, data and tags</param>
    /// <returns>Result of the notification send operation</returns>
    Task<HandlerRequestResult> SendNotificationAsync(NotificationDto notification);
}
