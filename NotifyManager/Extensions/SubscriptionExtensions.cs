using System;
using System.Collections.Generic;
using System.Text;

namespace NotifyManager.Extensions;

public static class SubscriptionExtensions
{
    public static object ExtractPushChannel(this SubscriptionDto subscription)
    {
        return new
        {
            endpoint = subscription.WebPushSubscription.GetRequiredValue("endpoint"),
            p256dh = subscription.WebPushSubscription.GetRequiredValue("p256dh"),
            auth = subscription.WebPushSubscription.GetRequiredValue("auth")
        };
    }

    public static BodyPayload CreateBodyPayload(this SubscriptionDto subscription)
    {
        return new BodyPayload
        {
            Title = "$(title)",
            Message = "$(body)",
            Data = subscription.PayLoad
        };
    }
}
