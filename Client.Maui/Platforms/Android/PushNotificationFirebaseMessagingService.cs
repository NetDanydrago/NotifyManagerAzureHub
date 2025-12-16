using Android.App;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Client.Maui.HubService.Interfaces;
using Firebase.Messaging;
using System.Net.Http;

namespace Client.Maui;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
    IPushDemoNotificationActionService _notificationActionService;
    INotificationRegistrationService _notificationRegistrationService;
    IDeviceInstallationService _deviceInstallationService;

    // id autoincremental para las notis
    int _messageId;
    const string ChannelId = "push_demo_channel";

    IPushDemoNotificationActionService NotificationActionService =>
        _notificationActionService ??= IPlatformApplication.Current.Services.GetService<IPushDemoNotificationActionService>();

    INotificationRegistrationService NotificationRegistrationService =>
        _notificationRegistrationService ??= IPlatformApplication.Current.Services.GetService<INotificationRegistrationService>();

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ??= IPlatformApplication.Current.Services.GetService<IDeviceInstallationService>();


    public override void OnNewToken(string token)
    {
        // guardar token y re-registrar en el hub
        DeviceInstallationService.Token = token;

        NotificationRegistrationService.RefreshRegistrationAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                    throw task.Exception;
            });
    }

    public override async void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);

        // 1. lo que ya tenías: disparar acción si viene
        if (message.Data.TryGetValue("action", out var messageAction) &&
            !string.IsNullOrWhiteSpace(messageAction))
        {
            NotificationActionService.TriggerAction(messageAction);
        }

        // 2. ahora: mostrar notificación solo si viene info visual
        //    (si mañana vuelves a querer silent, con no mandar notification ni imageUrl basta)
        var title = message.GetNotification()?.Title ?? "PushDemo";
        var body = message.GetNotification()?.Body  ?? "PushMessage";

        // si no hay título/cuerpo y tampoco hay imagen -> no mostrar nada
        var hasImage = message.Data.TryGetValue("imageUrl", out var imageUrl) && !string.IsNullOrWhiteSpace(imageUrl);
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body) && !hasImage)
            return;

        // crear canal
        EnsureChannel();

        // builder base
        var builder = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetSmallIcon(Resource.Drawable.ic_m3_chip_check)
            .SetAutoCancel(true)
            .SetPriority((int)NotificationPriority.High);

        // 3. si viene imagen -> big picture
        if (hasImage)
        {
            var bitmap = await TryDownloadBitmapAsync(imageUrl);
            if (bitmap != null)
            {
                var style = new NotificationCompat.BigPictureStyle()
                    .BigPicture(bitmap);
                builder.SetStyle(style);
            }
            else
            {
                // si falla la descarga, al menos lo mostramos expandido
                builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(body));
            }
        }
        else
        {
            // sin imagen, pero con texto -> expandido
            builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(body));
        }

        // 4. mostrar
        _messageId++;
        NotificationManagerCompat.From(this).Notify(_messageId, builder.Build());
    }

    void EnsureChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                ChannelId,
                "Push demo",
                NotificationImportance.High);
            var manager = (NotificationManager)GetSystemService(NotificationService);
            manager.CreateNotificationChannel(channel);
        }
    }

    static async Task<Bitmap> TryDownloadBitmapAsync(string url)
    {
        try
        {
            using var http = new HttpClient();
            var bytes = await http.GetByteArrayAsync(url);
            return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
        }
        catch
        {
            return null;
        }
    }
}