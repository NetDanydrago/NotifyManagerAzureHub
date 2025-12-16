namespace NotifyManager.Controllers;

/// <summary>
/// Controller for sending notifications with templates
/// </summary>
public class SendNotificationController(ISendNotificationInputPort sendNotificationInputPort) : ISendNotificationController
{
    /// <summary>
    /// Sends a notification to Azure Notification Hub
    /// </summary>
    /// <param name="notification">The notification request with title, body, data and tags</param>
    /// <returns>Result of the notification send operation</returns>
    public async Task<HandlerRequestResult> SendNotificationAsync(NotificationDto notification)
    {
        var result = new HandlerRequestResult();
        try
        {
            await sendNotificationInputPort.SendNotificationAsync(notification);
        }
        catch (Exception ex)
        {
            result = new HandlerRequestResult(ex.Message);
        }
        return result;
    }
}
