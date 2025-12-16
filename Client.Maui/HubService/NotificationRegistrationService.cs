using Client.Maui.HubService.Interfaces;
using System.Text;
using System.Text.Json;

namespace Client.Maui.HubService;

public class NotificationRegistrationService : INotificationRegistrationService
{
    const string CachedDeviceTokenKey = "cached_device_token";
    const string CachedTagsKey = "cached_tags";
    HttpClient _client;
    IDeviceInstallationService _deviceInstallationService;
    private string UserId = $"user-11";

    public NotificationRegistrationService(HttpClient httpClient,IDeviceInstallationService deviceInstallationService)
    {
        _deviceInstallationService = deviceInstallationService;
        _client = httpClient;
    }
    

    public async Task DeregisterDeviceAsync()
    {
        var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
            .ConfigureAwait(false);

        if (cachedToken == null)
            return;

        var deviceId = _deviceInstallationService?.GetDeviceId();

        if (string.IsNullOrWhiteSpace(deviceId))
            throw new Exception("Unable to resolve an ID for the device.");

        await SendAsync(HttpMethod.Delete, $"api/notifications/subscriptions/{deviceId}")
            .ConfigureAwait(false);

        SecureStorage.Remove(CachedDeviceTokenKey);
        SecureStorage.Remove(CachedTagsKey);
    }

    public async Task RegisterDeviceAsync(params string[] tags)
    {


        var deviceInstallation = _deviceInstallationService?.GetDeviceInstallation(tags);

        var subscriptionDto = new SubscriptionDto
        {
            InstallationId = UserId,
            Platform = "fcmv1",
            Tags =  new List<string> { UserId, "mobile-users" ,"android" },
            NotificationHandle = deviceInstallation.PushChannel
        };

        await SendAsync<SubscriptionDto>(HttpMethod.Post, "api/notifications/subscriptions", subscriptionDto)
            .ConfigureAwait(false);

        await SecureStorage.SetAsync(CachedDeviceTokenKey, deviceInstallation.PushChannel)
            .ConfigureAwait(false);

        await SecureStorage.SetAsync(CachedTagsKey, JsonSerializer.Serialize(tags));
    }

    public async Task RefreshRegistrationAsync()
    {
        var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
            .ConfigureAwait(false);

        var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(cachedToken) ||
            string.IsNullOrWhiteSpace(serializedTags) ||
            string.IsNullOrWhiteSpace(_deviceInstallationService?.Token) ||
            cachedToken == _deviceInstallationService?.Token)
            return;

        var tags = JsonSerializer.Deserialize<string[]>(serializedTags);

        await RegisterDeviceAsync(tags);
    }

    async Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
    {
        string serializedContent = null;

        await Task.Run(() => serializedContent = JsonSerializer.Serialize(obj))
            .ConfigureAwait(false);

        await SendAsync(requestType, requestUri, serializedContent);
    }

    async Task SendAsync(HttpMethod requestType, string requestUri, string jsonRequest = null)
    {
        var request = new HttpRequestMessage(requestType, requestUri);

        if (jsonRequest != null)
            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}