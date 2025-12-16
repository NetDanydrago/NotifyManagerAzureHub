namespace NotifyManager.Extensions;

public static class DictionaryExtensions
{
    public static string GetRequiredValue(this Dictionary<string, string> source, string key)
    {
        return source.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value)
            ? value
            : throw new ArgumentException($"Required key '{key}' not found or empty", nameof(key));
    }
}
        