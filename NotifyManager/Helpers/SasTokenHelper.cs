using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NotifyManager.Helpers;

public static class SasTokenHelper
{
    public static string GenerateSasToken(string resourceUri, string keyName, string key)
    {
        var expiry = DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds();
        string stringToSign = WebUtility.UrlEncode(resourceUri) + "\n" + expiry;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        var token = $"SharedAccessSignature sr={WebUtility.UrlEncode(resourceUri)}&sig={WebUtility.UrlEncode(signature)}&se={expiry}&skn={keyName}";
        return token;
    }
}
