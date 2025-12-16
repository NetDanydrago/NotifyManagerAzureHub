using Client.Maui.HubService.Interfaces;
using Object = Java.Lang.Object;

namespace Client.Maui;

public class SuccessListenerImplementation : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
{
    IDeviceInstallationService _installationService;
    
    public SuccessListenerImplementation(IDeviceInstallationService installationService)
    {
        _installationService = installationService;
    }
    
    public void OnSuccess(Java.Lang.Object result)
    {
        _installationService.Token = result.ToString();
    }
}