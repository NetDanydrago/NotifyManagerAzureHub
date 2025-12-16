namespace NotifyManager.Internals.Controllers;

/// <summary>
/// Controller interface for unsubscription operations
/// </summary>
public interface IUnsubscribeController
{
    /// <summary>
    /// Handles unsubscription requests
    /// </summary>
    /// <param name="installationId">The installation ID to unsubscribe</param>
    Task<HandlerRequestResult> UnsubscribeAsync(string installationId);
}
