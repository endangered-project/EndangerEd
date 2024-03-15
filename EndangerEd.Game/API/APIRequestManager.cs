using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using osu.Framework.Logging;

namespace EndangerEd.Game.API;

/// <summary>
/// Class store API request for the game.
/// </summary>
public class APIRequestManager
{
    private readonly APIEndpointConfig _config;

    private readonly HttpClient _client;

    public APIRequestManager(APIEndpointConfig config)
    {
        _config = config;
        _client = new HttpClient();
    }

    /// <summary>
    /// Get the full endpoint URL.
    /// </summary>
    /// <param name="endpoint">Endpoint URL.</param>
    /// <returns>A string of the full endpoint URL.</returns>
    public string GetEndpoint(string endpoint)
    {
        return _config.APIBaseUrl + endpoint;
    }

    /// <summary>
    /// Add header to the request.
    /// </summary>
    /// <param name="key">Dictionary key.</param>
    /// <param name="value">Dictionary value.</param>
    public void AddHeader(string key, string value)
    {
        if (_client.DefaultRequestHeaders.Contains(key))
        {
            _client.DefaultRequestHeaders.Remove(key);
        }

        _client.DefaultRequestHeaders.Add(key, value);
    }

    /// <summary>
    /// Send POST JSON request to the API and return the response as Dictionary.
    /// </summary>
    public async Task<Dictionary<string, object>> PostJsonAsync(string endpoint, Dictionary<string, object> data)
    {
        Logger.Log($"Sending POST request to {GetEndpoint(endpoint)} with data: {JsonSerializer.Serialize(data)}");
        var responseTask = _client.PostAsync(GetEndpoint(endpoint), new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        var request = responseTask.GetAwaiter().GetResult();
        var response = await request.Content.ReadAsStringAsync();
        Logger.Log($"Response from {GetEndpoint(endpoint)}: {response}");

        if (request.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(response);
        }

        throw new HttpRequestException($"Request to {GetEndpoint(endpoint)} failed with status code {request.StatusCode} and response: {response}");
    }

    public Dictionary<string, object> PostJson(string endpoint, Dictionary<string, object> data)
    {
        try
        {
            return PostJsonAsync(endpoint, data).GetAwaiter().GetResult();
        }
        catch (HttpRequestException e)
        {
            Logger.Log($"Request to {GetEndpoint(endpoint)} failed with error: {e.Message}");
            throw new HttpRequestException($"Request to {GetEndpoint(endpoint)} failed with status code {e.Message}");
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
