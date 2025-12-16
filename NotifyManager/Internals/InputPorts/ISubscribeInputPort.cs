namespace NotifyManager.Internals.InputPorts;

/// <summary>
/// Input port for subscribing to notification services
/// </summary>
public interface ISubscribeInputPort
{
    /// <summary>
    /// Executes the subscription operation
    /// </summary>
    /// <param name="subscription">The subscription request data</param>
    Task SubscribeAsync(SubscriptionDto subscription);
}
