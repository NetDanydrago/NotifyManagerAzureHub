namespace NotifyManager.Extensions;

public static class SubscriptionExtensions
{
    public static object ExtractPushChannel(this SubscriptionDto subscription) => subscription.Platform switch
    {
        "fcmv1" => new { subscription.NotificationHandle },
        "browser" => new
        {
            Endpoint = subscription.WebPushSubscription.GetRequiredValue("endpoint"),
            P256DH = subscription.WebPushSubscription.GetRequiredValue("p256dh"),
            Auth = subscription.WebPushSubscription.GetRequiredValue("auth")
        },
        _ => throw new NotSupportedException($"Platform '{subscription.Platform}' is not supported")
    };

    public static object CreateBodyPayload(this SubscriptionDto subscription) => subscription.Platform switch
    {
        "fcmv1" => new { message = new { notification = new { title = "$(title)", body = "$(body)" }, data = subscription.PayLoad } },
        "browser" => new
        {
            title = "$(title)",
            message  = "$(body)",
            data = subscription.PayLoad
        },
        _ => throw new NotSupportedException($"Platform '{subscription.Platform}' is not supported")
    };

}
