namespace Notifications.Web.Options;
public class WebPushOptions
{
    public const string SectionKey = nameof(WebPushOptions);

    public string VapidPublicKey { get; set; } 
}
