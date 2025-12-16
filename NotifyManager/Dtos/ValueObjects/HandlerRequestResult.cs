namespace NotifyManager.Dtos.ValueObjects;

public class HandlerRequestResult
{
    public bool Success { get; init; }
    public string ErrorMessage { get; init; }

    public HandlerRequestResult()
    {
        Success = true;
        ErrorMessage = string.Empty;
    }

    public HandlerRequestResult(string message)
    {
        Success = false;
        ErrorMessage = message;
    }
}