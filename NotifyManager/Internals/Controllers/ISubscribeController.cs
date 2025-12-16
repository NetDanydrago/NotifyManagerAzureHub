namespace NotifyManager.Internals.Controllers;

/// <summary>
/// Controller interface for subscription operations
/// </summary>
public interface ISubscribeController
{
    /// <summary>
    /// Handles subscription requests
    /// </summary>
    /// <param name="subscription">The subscription request data</param>
    Task<HandlerRequestResult> SubscribeAsync(SubscriptionDto subscription);
}
