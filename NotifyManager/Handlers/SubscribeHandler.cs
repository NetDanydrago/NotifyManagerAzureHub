namespace NotifyManager.Handlers;

public class SubscribeHandler(IOptions<AzureNotificationHubOptions> azureOptions, IHttpClientFactory clientFactory, ILogger<SubscribeHandler> logger) : ISubscribeInputPort
{

    public async Task SubscribeAsync(SubscriptionDto subscription)
    {
        var pushChannel = subscription.ExtractPushChannel();
        string bodyJson = JsonSerializer.Serialize(subscription.CreateBodyPayload());
        var installation = new
        {
            installationId = subscription.InstallationId,
            platform = subscription.Platform,
            pushChannel,
            tags = subscription.Tags,
            templates = new
            {
                DefaultTemplate = new
                {
                    body = bodyJson
                }
            }
        };

        string json = JsonSerializer.Serialize(installation);

        HubConfig hubConfig = azureOptions.Value.ConnectionString.ParseNotificationHubConnectionString();
        string resourceUri = $"https://{hubConfig.NamespaceName}.servicebus.windows.net/{azureOptions.Value.HubName}";
        string path = $"/installations/{subscription.InstallationId}";
        string sasToken = SasTokenHelper.GenerateSasToken(resourceUri, hubConfig.KeyName, hubConfig.Key);

        var client = clientFactory.CreateClient();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("x-ms-version", "2020-06");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", sasToken.Replace("SharedAccessSignature ", ""));

        string deleteuri = $"{resourceUri}/installations/{subscription.InstallationId}?api-version=2020-06";
        var responsdelete = await client.DeleteAsync(deleteuri);
        var cpmtemtdelete = await responsdelete.Content.ReadAsStringAsync();

        string getUri = $"{resourceUri}/installations/{subscription.InstallationId}?api-version=2020-06";
        var responseget = await client.GetAsync(getUri);
        var cpmtemtd = await responseget.Content.ReadAsStringAsync();

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{resourceUri}{path}?api-version=2020-06", content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to register/update installation. StatusCode: {StatusCode}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Error registering installation: {response.StatusCode} {error}");
        }
    }

}
