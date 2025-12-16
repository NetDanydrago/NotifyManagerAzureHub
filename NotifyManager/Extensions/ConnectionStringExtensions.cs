namespace NotifyManager.Extensions;

public static class ConnectionStringExtensions
{
    public static HubConfig ParseNotificationHubConnectionString(this string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        
        HubConfig hubConfig = new HubConfig();

        char[] separator = { ';' };
        string[] parts = connectionString.Split(separator);

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].StartsWith("Endpoint"))
                hubConfig.Endpoint = "https" + parts[i].Substring(11);
            else if (parts[i].StartsWith("SharedAccessKeyName"))
                hubConfig.KeyName = parts[i].Substring(20);
            else if (parts[i].StartsWith("SharedAccessKey") && !parts[i].StartsWith("SharedAccessKeyName"))
                hubConfig.Key = parts[i].Substring(16);
        }
        hubConfig.NamespaceName = new Uri(hubConfig.Endpoint).Host.Split('.')[0];
        return hubConfig;
    }
}
