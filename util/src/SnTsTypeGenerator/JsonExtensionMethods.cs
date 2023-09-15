using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator;

public static class JsonExtensionMethods
{
    public static async Task<(JsonDocument? Document, string? Content)> GetJsonDocumentAsync(this HttpResponseMessage response, ILogger logger, CancellationToken cancellationToken)
    {
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception)
        {
            logger.LogHttpRequestFailedError(response.RequestMessage!.RequestUri!, exception);
            return (null, null);
        }
        string responseBody;
        try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
        catch (Exception exception)
        {
            logger.LogGetResponseContentFailedError(response.RequestMessage!.RequestUri!, exception);
            return (null, null);
        }
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            logger.LogInvalidHttpResponseError(response.RequestMessage!.RequestUri!, responseBody);
            return (null, null);
        }
        
        try { return (JsonDocument.Parse(responseBody), responseBody); }
        catch (JsonException exception)
        {
            logger.LogJsonCouldNotBeParsedError(response.RequestMessage!.RequestUri!, responseBody, exception);
            return (null, responseBody);
        }
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
