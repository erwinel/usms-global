using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public static class JsonExtensionMethods
{
    public static Uri ToApiUri(this Uri baseUri, string tableName, string element, string value)
    {
        value = Uri.EscapeDataString($"{element}={value}");
        return new UriBuilder(baseUri)
        {
            Path = $"{URI_PATH_API}/{tableName}",
            Query = $"sysparm_query={value}",
            Fragment = null
        }.Uri;
    }

    public static async Task<T?> GetLinkedObjectAsync<T>(this HttpClientHandler clientHandler, JsonElement element, string name, Func<string, Task<T?>> lookupFunc, Func<JsonElement, Task<T?>> createFunc, ILogger logger, CancellationToken cancellationToken) where T : class
    {
        if (!element.TryGetProperty(name, out JsonElement linkElement) || linkElement.ValueKind != JsonValueKind.Object)
            return null;
        string? link;
        T? result;
        Uri? uri;
        string responseBody;
        if (linkElement.TryGetNonEmptyString(JSON_KEY_VALUE, out string? value))
        {
            if ((result = await lookupFunc(value)) is not null)
                return result;
            if (!(linkElement.TryGetNonEmptyString(JSON_KEY_LINK, out link) && Uri.TryCreate(link, UriKind.Absolute, out uri)))
                return null;
            using HttpClient client = new(clientHandler);
            HttpRequestMessage msg = new(HttpMethod.Get, uri);
            msg.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await client.SendAsync(msg, cancellationToken);
            try { response.EnsureSuccessStatusCode(); }
            catch (HttpRequestException exception)
            {
                logger.LogHttpRequestFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
            try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception exception)
            {
                logger.LogGetResponseContentFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
        }
        else if (linkElement.TryGetNonEmptyString(JSON_KEY_LINK, out link) && Uri.TryCreate(link, UriKind.Absolute, out uri))
        {
            using HttpClient client = new(clientHandler);
            HttpRequestMessage msg = new(HttpMethod.Get, uri);
            msg.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await client.SendAsync(msg, cancellationToken);
            try { response.EnsureSuccessStatusCode(); }
            catch (HttpRequestException exception)
            {
                logger.LogHttpRequestFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
            try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception exception)
            {
                logger.LogGetResponseContentFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
        }
        else
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            logger.LogInvalidHttpResponseError(uri, responseBody);
            return null;
        }
        
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            logger.LogJsonCouldNotBeParsedError(uri, responseBody, exception);
            return null;
        }
        using (doc)
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
                return await createFunc(doc.RootElement);
        }
        return null;
    }

    public static bool TryGetStringValue(this JsonElement element, string name, [NotNullWhen(true)] out string? result)
    {
        if (element.TryGetProperty(name, out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String)
            return (result = nameElement.GetString()) is not null;
        result = null;
        return false;
    }

    public static bool GetBoolean(this JsonElement source, string name, bool defaultValue = false) => source.TryGetProperty(name, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0,
        _ => defaultValue,
    } : defaultValue;

    public static string GetString(this JsonElement source, string name, string defaultValue = "") => source.TryGetProperty(name, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.Null or JsonValueKind.Undefined => defaultValue,
        _ => source.GetString() ?? defaultValue,
    } : defaultValue;

    public static bool TryGetNonEmptyString(this JsonElement source, string name, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetProperty(name, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    break;
                default:
                    var s = element.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        result = s;
                        return true;
                    }
                    break;
            }
            result = null;
            return false;
    }

    public static string GetNonEmptyString(this JsonElement source, string name, string defaultValue = "")
    {
        if (source.TryGetProperty(name, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return defaultValue;
                default:
                    var s = element.GetString();
                    return string.IsNullOrWhiteSpace(s) ? defaultValue : s;
            }
        return defaultValue;
    }
}
