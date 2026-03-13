using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Notifications.Web.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Blazor.Pages;

public partial class Notifications(IJSRuntime jsRuntime, HttpClient client, IOptions<WebPushOptions> options) : IAsyncDisposable
{
    private IJSObjectReference PermissionManager;
    private IJSObjectReference SubscriptionManager;
    private IJSObjectReference ModulePermission;
    private IJSObjectReference ModuleSubscription;


    private string PermissionStatus = "default";
    private bool IsSubscribed = false;
    private string InstallationId = $"1";

    private string VapidKey;

    protected override void OnInitialized()
    {
        VapidKey = options.Value.VapidPublicKey;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Importar módulos JavaScript
                ModulePermission = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/notification-permissions.js");
                ModuleSubscription = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/notification-subscriptions.js");
                // Crear instancias de las clases JavaScript
                PermissionManager = await ModulePermission.InvokeConstructorAsync("NotificationPermissionManager");
                SubscriptionManager = await ModuleSubscription.InvokeConstructorAsync("NotificationSubscriptionManager");
                // Verificar estado inicial
                PermissionStatus = await PermissionManager.InvokeAsync<string>("checkPermission");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task RequestPermission()
    {
        try
        {
            var granted = await PermissionManager!.InvokeAsync<bool>("requestPermission");
            PermissionStatus = granted ? "granted" : "denied";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task Subscribe()
    {
        try
        {
            // Obtener suscripción del navegador
            var subscription = await SubscriptionManager!.InvokeAsync<JsonElement>("getSubscription", VapidKey);

            // Preparar datos para la API
            var subscriptionDto = new
            {
                InstallationId,
                Platform = "browser",
                Tags = new[] { "user:1", "web:users" },
                WebPushSubscription = new
                {
                    Endpoint = subscription.GetProperty("endpoint").GetString(),
                    P256dh = subscription.GetProperty("p256dh").GetString(),
                    Auth = subscription.GetProperty("auth").GetString()
                },
                Payload = new Dictionary<string, string>() {
                { "url", "$(url)" },
                { "user", "$(userid)" },}
            };

            // Enviar a la API
            var response = await client.PostAsJsonAsync($"api/notifications/subscriptions", subscriptionDto);

            if (response.IsSuccessStatusCode)
            {
                IsSubscribed = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    private string GetPermissionBadge() => PermissionStatus switch
    {
        "granted" => "bg-success",
        "denied" => "bg-danger",
        _ => "bg-secondary"
    };

    public async ValueTask DisposeAsync()
    {
        try
        {
            await PermissionManager.DisposeAsync();
            await SubscriptionManager.DisposeAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during disposal: {ex.Message}");
        }
    }
}
