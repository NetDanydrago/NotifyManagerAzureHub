using Client.Maui.HubService;

namespace Client.Maui.HubService.Interfaces;

public interface IPushDemoNotificationActionService
{
    event EventHandler<PushDemoAction> ActionTriggered;
    
    void TriggerAction(string action);
}