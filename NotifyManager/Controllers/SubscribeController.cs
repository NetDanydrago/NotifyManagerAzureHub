namespace NotifyManager.Controllers;

public class SubscribeController(ISubscribeInputPort subscribeInputPort) : ISubscribeController
{
    public async Task<HandlerRequestResult> SubscribeAsync(SubscriptionDto subscription)
    {
        HandlerRequestResult Result = new();
        try
        {
            await subscribeInputPort.SubscribeAsync(subscription);
        }
        catch (Exception e)
        {
            Result = new HandlerRequestResult(e.Message);
        }
        return Result;     
    }
}
