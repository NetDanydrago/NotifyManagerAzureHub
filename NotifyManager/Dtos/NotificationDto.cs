namespace NotifyManager.Dtos;

/// <summary>
/// DTO for sending notifications to Azure Notification Hub
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Constructor for NotificationDto with required fields
    /// </summary>
    /// <param name="title">The title/subject of the notification</param>
    /// <param name="body">The main content/body of the notification</param>
    /// <param name="data">Additional data/payload for the notification</param>
    public NotificationDto(string title, string body, Dictionary<string, string> data)
    {
        Title = title;
        Body = body;
        Data = data;
    }

    /// <summary>
    /// The title/subject of the notification
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The main content/body of the notification
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Additional data/payload for the notification
    /// </summary>
    public Dictionary<string, string> Data { get; set; }

    /// <summary>
    /// Platform-specific tags for targeting (optional)
    /// </summary>
    public List<string> Tags { get; set; } = new();
}
