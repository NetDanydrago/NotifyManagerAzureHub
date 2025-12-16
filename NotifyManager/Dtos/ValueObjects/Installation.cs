namespace NotifyManager.Dtos.ValueObjects;

public class Installation
{
    public string InstallationId { get; set; }
    public string Platform { get; set; }
    public PushChannel PushChannel { get; set; }
    public List<string> Tags { get; set; }
    public Templates Templates { get; set; }
}
