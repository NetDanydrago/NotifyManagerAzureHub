namespace NotifyManager.Internals.InputPorts;

/// <summary>
/// Input port for unsubscribing from notification services
/// </summary>
public interface IUnsubscribeInputPort
{
    /// <summary>
    /// Executes the unsubscription operation
    /// </summary>
    /// <param name="installationId">The installationId to unsubscribe</param>
    Task UnsubscribeAsync(string installationId);
}
