namespace Client.Maui.HubService.Interfaces;

public interface INotificationRegistrationService
{
    Task DeregisterDeviceAsync();
    Task RegisterDeviceAsync(params string[] tags);
    Task RefreshRegistrationAsync();
}