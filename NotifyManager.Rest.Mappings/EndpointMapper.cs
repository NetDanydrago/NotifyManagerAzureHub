using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NotifyManager.Dtos;
using NotifyManager.Internals.Controllers;

namespace NotifyManager.Rest.Mappings;

public static class EndpointMapper
{
    public static IEndpointRouteBuilder UseNotifyManagerEndpoints(this IEndpointRouteBuilder builder)
    {
        // Subscribe endpoint - Register a device for push notifications
        builder.MapPost("api/notifications/subscriptions", async ([FromServices] ISubscribeController controller, SubscriptionDto subscription) =>
        {
            return TypedResults.Ok(await controller.SubscribeAsync(subscription));
        });
       
        // Send notification endpoint - Send notifications to recipients
        builder.MapPost("api/notifications/send", async ([FromServices] ISendNotificationController controller, NotificationDto notification) =>
        {
            return TypedResults.Ok(await controller.SendNotificationAsync(notification));
        });
        
        return builder;
    }
}
