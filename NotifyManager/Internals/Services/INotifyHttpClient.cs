namespace NotifyManager.Internals.Services;

/// <summary>
/// Interface for typed HTTP client operations
/// </summary>
public interface INotifyHttpClient
{
    /// <summary>
    /// Sends a GET request to the specified path
    /// </summary>
    /// <typeparam name="TResponse">Type of the expected response</typeparam>
    /// <param name="path">Relative path for the request</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response of type TResponse</returns>
    Task<TResponse> GetAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request with a body to the specified path
    /// </summary>
    /// <typeparam name="TRequest">Type of the request body</typeparam>
    /// <typeparam name="TResponse">Type of the expected response</typeparam>
    /// <param name="path">Relative path for the request</param>
    /// <param name="body">Request body to serialize</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response of type TResponse</returns>
    Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request without expecting a response body
    /// </summary>
    /// <typeparam name="TRequest">Type of the request body</typeparam>
    /// <param name="path">Relative path for the request</param>
    /// <param name="body">Request body to serialize</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PostAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a PUT request with a body to the specified path
    /// </summary>
    /// <typeparam name="TRequest">Type of the request body</typeparam>
    /// <typeparam name="TResponse">Type of the expected response</typeparam>
    /// <param name="path">Relative path for the request</param>
    /// <param name="body">Request body to serialize</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response of type TResponse</returns>
    Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a PUT request without expecting a response body
    /// </summary>
    /// <typeparam name="TRequest">Type of the request body</typeparam>
    /// <param name="path">Relative path for the request</param>
    /// <param name="body">Request body to serialize</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PutAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request to the specified path
    /// </summary>
    /// <param name="path">Relative path for the request</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request with a response
    /// </summary>
    /// <typeparam name="TResponse">Type of the expected response</typeparam>
    /// <param name="path">Relative path for the request</param>
    /// <param name="headers">Optional custom headers to add to the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response of type TResponse</returns>
    Task<TResponse> DeleteAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
}
