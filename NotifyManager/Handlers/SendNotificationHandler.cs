namespace NotifyManager.Handlers;

public class SendNotificationHandler(IHttpClientFactory httpClientFactory, IOptions<AzureNotificationHubOptions> azureOptions, ILogger<SendNotificationHandler> logger) : ISendNotificationInputPort
{
    public async Task SendNotificationAsync(NotificationDto notification)
    {
        try
        {
            HubConfig hubConfig = azureOptions.Value.ConnectionString.ParseNotificationHubConnectionString();

            // Build the resource URI and path
            string resourceUri = $"https://{hubConfig.NamespaceName}.servicebus.windows.net/{azureOptions.Value.HubName}";
            string path = $"/messages/?api-version=2020-06";

            // Prepare notification payload with title, body and data
            var notificationPayload = new Dictionary<string, string>
            {
                { "title", notification.Title },
                { "body", notification.Body }
            };

            if (notification.Data != null)
            {
                foreach (var kvp in notification.Data)
                {
                    notificationPayload[kvp.Key] = kvp.Value;
                }
            }

            string json = JsonSerializer.Serialize(notificationPayload);

            // Generate SAS token for authentication
            string sasToken = SasTokenHelper.GenerateSasToken(resourceUri, hubConfig.KeyName, hubConfig.Key);

            // Create HTTP client with necessary headers
            HttpClient client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-ms-version", "2020-06");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", sasToken.Replace("SharedAccessSignature ", ""));

            // Add ServiceBusNotification-Tags header if tags are provided
            if (notification.Tags != null && notification.Tags.Count > 0)
            {
                string tagsString = string.Join(" || ", notification.Tags);
                client.DefaultRequestHeaders.Add("ServiceBusNotification-Tags", tagsString);
            }

            // Create content and send POST request
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(resourceUri + path, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to send notification. StatusCode: {StatusCode}, Error: {Error}", response.StatusCode, error);
                throw new Exception($"Error sending notification: {response.StatusCode} - {error}");
            }
            logger.LogInformation("Notification sent successfully. Title: {Title}, Tags: {Tags}", notification.Title, string.Join(",", notification.Tags ?? new List<string>()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while sending notification");
            throw;
        }
    }
}
