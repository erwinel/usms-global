using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

public static class JsonExtensions
{
    /// <summary>
    /// Surrounds a string with quotes if it contains spaces or specific symbols.
    /// </summary>
    /// <param name="value">The source string value.</param>
    /// <returns>A JSON-quoted string if the value contains spaces or special characters.</returns>
    public static string SmartQuoteJson(this string? value)
    {
        if (value is null)
            return "null";
        if (value.Length == 0)
            return "\"\"";
        string result = JsonValue.Create(value)!.ToJsonString();
        return !value.Any(c => char.IsWhiteSpace(c)) && value.Length == result.Length - 2 ? value : result;
    }

    public static async Task<(Uri requestUri, JsonNode? Result)> GetTableApiJsonResponseAsync(this SnClientHandlerService handler, string tableName, string id, CancellationToken cancellationToken) =>
        await handler.GetJsonAsync($"{URI_PATH_API}/{tableName}/{Uri.EscapeDataString(id)}", $"{URI_PARAM_DISPLAY_VALUE}=all", cancellationToken);

    public static async Task<(Uri requestUri, JsonNode? Result)> GetTableApiJsonResponseAsync(this SnClientHandlerService handler, string tableName, string element, string value, CancellationToken cancellationToken)
    {
        value = Uri.EscapeDataString($"{element}={value}");
        return await handler.GetJsonAsync($"{URI_PATH_API}/{tableName}", $"{URI_PARAM_QUERY}={value}&{URI_PARAM_DISPLAY_VALUE}=all", cancellationToken);
    }

    public static T? GetProperty<T>(this JsonObject source, string propertyName) where T : JsonNode => (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is T result) ? result : null;

    public static bool TryGetProperty<T>(this JsonObject source, string propertyName, [NotNullWhen(true)] out T? result) where T : JsonNode
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is T t)
        {
            result = t;
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryGetPropertyValue(this JsonObject source, string propertyName, string innerPropertyName, out JsonNode? jsonNode) =>
        source.TryGetPropertyValue(propertyName, out jsonNode) && jsonNode is JsonObject obj && obj.TryGetPropertyValue(innerPropertyName, out jsonNode);

    public static bool TryGetPropertyAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
            return jsonValue.TryGetValue(out result);
        result = null;
        return false;
    }

    public static bool TryGetPropertyAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
            return jsonValue.TryGetValue(out result) && !string.IsNullOrWhiteSpace(result);
        result = null;
        return false;
    }

    public static string? GetPropertyAsString(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue && jsonValue.TryGetValue(out string? result)) ? result : null;

    public static string GetPropertyAsNonEmpty(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue && jsonValue.TryGetValue(out string? result) && !string.IsNullOrWhiteSpace(result)) ? result : string.Empty;

    public static bool TryCoercePropertyAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out result))
                result = jsonValue.ToString();
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryCoercePropertyAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out result))
                result = jsonValue.ToString();
            return !string.IsNullOrWhiteSpace(result);
        }
        result = null;
        return false;
    }

    public static string? CoercePropertyAsStringOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? jsonValue.TryGetValue(out string? result) ? result : jsonValue.ToString() : null;

    public static string? CoercePropertyAsNonEmptyOrNull(this JsonObject source, string propertyName)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out string? result))
                result = jsonValue.ToString();
            if (!string.IsNullOrWhiteSpace(result))
                return result;
        }
        return null;
    }

    public static string CoercePropertyAsString(this JsonObject source, string propertyName, string defaultValue = "") =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? jsonValue.TryGetValue(out string? result) ? result : jsonValue.ToString() : defaultValue;

    public static string CoercePropertyAsNonEmpty(this JsonObject source, string propertyName, string defaultValue = "")
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out string? result))
                result = jsonValue.ToJsonString();
            if (!string.IsNullOrWhiteSpace(result))
                return result;
        }
        return defaultValue;
    }

    public static bool TryCoercePropertyAsInt(this JsonObject source, string propertyName, out int result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue(out result) || jsonValue.TryGetValue(out string? s) && int.TryParse(s, out result))
                return true;
        }
        else
            result = default;
        return false;
    }

    public static int? CoercePropertyAsIntOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? jsonValue.TryGetValue(out int? result) ? result :
            jsonValue.TryGetValue(out string? s) && int.TryParse(s, out int i) ? i : null : null;

    public static int CoercePropertyAsInt(this JsonObject source, string propertyName, int defaultValue = 0) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? jsonValue.TryGetValue(out int? result) ? result.Value :
            jsonValue.TryGetValue(out string? s) && int.TryParse(s, out int i) ? i : defaultValue : defaultValue;

    public static bool TryCoercePropertyAsBoolean(this JsonObject source, string propertyName, out bool result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue(out result))
                return true;
            if (jsonValue.TryGetValue(out string? s) && bool.TryParse(s, out bool b))
            {
                result = b;
                return true;
            }
        }
        result = false;
        return false;
    }

    public static bool? CoercePropertyAsBooleanOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? jsonValue.TryGetValue(out bool? result) ? result :
            jsonValue.TryGetValue(out string? s) && bool.TryParse(s, out bool b) ? b : null : null;

    public static bool CoercePropertyAsBoolean(this JsonObject source, string propertyName, bool defaultValue = false) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? jsonValue.TryGetValue(out bool? result) ? result.Value :
            jsonValue.TryGetValue(out string? s) && bool.TryParse(s, out bool b) ? b : defaultValue : defaultValue;

    public static bool TryGetFieldAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
            display_value = null;
        }
        else
            value = display_value = null;
        return false;
    }

    public static bool TryGetFieldAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsNonEmpty(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
            display_value = null;
        }
        else
            value = display_value = null;
        return false;
    }

    public static bool TryGetFieldAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsString(JSON_KEY_VALUE, out value);
        value = null;
        return false;
    }

    public static bool TryGetFieldAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsNonEmpty(JSON_KEY_VALUE, out value);
        value = null;
        return false;
    }

    public static string? GetFieldAsStringOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static string? GetFieldAsNonEmptyOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsNonEmpty(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static string? GetFieldAsStringOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsStringOrNull(JSON_KEY_VALUE) : null;

    public static string? GetFieldAsNonEmptyOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsNonEmptyOrNull(JSON_KEY_VALUE) : null;

    public static string GetFieldAsString(this JsonObject source, string propertyName, string defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static string GetFieldAsNonEmpty(this JsonObject source, string propertyName, string defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsNonEmpty(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static string GetFieldAsString(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return string.Empty;
    }

    public static string GetFieldAsNonEmpty(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsNonEmpty(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return string.Empty;
    }

    public static string GetFieldAsString(this JsonObject source, string propertyName, string defaultValue = "") =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field && field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value)) ? value : defaultValue;

    public static string GetFieldAsNonEmpty(this JsonObject source, string propertyName, string defaultValue = "") =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field && field.TryCoercePropertyAsNonEmpty(JSON_KEY_VALUE, out string? value)) ? value : defaultValue;

    public static bool TryGetFieldAsInt(this JsonObject source, string propertyName, [NotNullWhen(true)] out int value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
        }
        else
            value = default;
        display_value = null;
        return false;
    }

    public static bool TryGetFieldAsInt(this JsonObject source, string propertyName, [NotNullWhen(true)] out int value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out value);
        value = default;
        return false;
    }

    public static int? GetFieldAsIntOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out int value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static int? GetFieldAsIntOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsIntOrNull(JSON_KEY_VALUE) : null;

    public static int GetFieldAsInt(this JsonObject source, string propertyName, int defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out int value))
                return value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static int GetFieldAsInt(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out int value))
                return value;
        }
        else
            display_value = null;
        return 0;
    }

    public static int GetFieldAsInt(this JsonObject source, string propertyName, int defaultValue = 0) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.GetFieldAsInt(JSON_KEY_VALUE, defaultValue) : defaultValue;

    public static bool TryGetFieldAsBoolean(this JsonObject source, string propertyName, [NotNullWhen(true)] out bool value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
        }
        else
            value = false;
        display_value = null;
        return false;
    }

    public static bool TryGetFieldAsBoolean(this JsonObject source, string propertyName, [NotNullWhen(true)] out bool value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out value);
        value = false;
        return false;
    }

    public static bool? GetFieldAsBooleanOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static bool? GetFieldAsBooleanOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsBooleanOrNull(JSON_KEY_VALUE) : null;

    public static bool GetFieldAsBoolean(this JsonObject source, string propertyName, bool defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool value))
                return value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static bool GetFieldAsBoolean(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool value))
                return value;
        }
        else
            display_value = null;
        return false;
    }

    public static bool GetFieldAsBoolean(this JsonObject source, string propertyName, bool defaultValue = false) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field && field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool value)) ? value : defaultValue;
}
