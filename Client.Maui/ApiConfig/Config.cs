namespace Client.Maui.ApiConfig;

public static partial class Config
{
    public static string ApiKey = DeviceInfo.Platform == DevicePlatform.iOS ? "API_KEY_IOS" : "API_KEY_ANDROID";
    public static string BackendServiceEndpoint = DeviceInfo.Platform == DevicePlatform.Android ? "https://37x2rqm8-7009.usw3.devtunnels.ms/" : "";
}