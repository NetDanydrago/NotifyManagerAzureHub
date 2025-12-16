using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Client.Maui.HubService;
using Client.Maui.HubService.Interfaces;
using Firebase;
using Firebase.Messaging;

namespace Client.Maui.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    IPushDemoNotificationActionService _notificationActionService;
    IDeviceInstallationService _deviceInstallationService;
    
    IPushDemoNotificationActionService NotificationActionService =>
        _notificationActionService ?? (_notificationActionService = IPlatformApplication.Current.Services.GetService<IPushDemoNotificationActionService>());

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ?? (_deviceInstallationService = IPlatformApplication.Current.Services.GetService<IDeviceInstallationService>());
    
    
    void ProcessNotificationsAction(Intent intent)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Processing notification action...");
            if (intent.HasExtra("google.message_id"))
            {
                // This is likely from FCM
                foreach (var key in intent.Extras.KeySet())
                {
                    if (key.Contains("action") || key.Contains("Action"))
                    {
                        var action = intent.Extras.GetString(key);
                        System.Diagnostics.Debug.WriteLine($"Found FCM action: {action}");
                        System.Diagnostics.Debug.WriteLine($"Notification Service: {NotificationActionService}");
                        if (!string.IsNullOrEmpty(action))
                        {
                            ShowActionAlert(action == "action_a" ? PushDemoAction.ActionA : PushDemoAction.ActionB);
                            
                            Task.Delay(1500).ContinueWith(_ =>
                            {
                                NotificationActionService.TriggerAction(action);
                            });
                            
                        }
                        

                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
    
    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        ProcessNotificationsAction(intent);
    }
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var firebaseApp = FirebaseApp.InitializeApp(this);
        if (DeviceInstallationService.NotificationsSupported)
            FirebaseMessaging.Instance.GetToken().AddOnSuccessListener(new SuccessListenerImplementation(_deviceInstallationService));

        ProcessNotificationsAction(Intent);
    }
    
    void ShowActionAlert(PushDemoAction action)
    {
        System.Diagnostics.Debug.WriteLine($"Showing Android native alert for action: {action}");
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Push notifications demo");
            builder.SetMessage($"{action} action received.");
            builder.SetPositiveButton("OK", (sender, args) => { });
            var dialog = builder.Create();
            dialog.Show();
        });
    }
}