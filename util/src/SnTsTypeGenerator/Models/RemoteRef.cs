using System.Text.Json.Nodes;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized reference from a remote API call.
/// </summary>
/// <param name="Value">The value of the <see cref="JSON_KEY_VALUE" /> property.</param>
/// <param name="Display">The value of the <see cref="JSON_KEY_DISPLAY_VALUE" /> property or <see langword="null"/> if that value was empty.</param>
public record RemoteRef(string Value, string? Display)
{
    internal static RemoteRef? FromProperty(JsonObject obj, string propertyName) => (obj.TryGetProperty(propertyName, out JsonObject? p) && p.TryGetPropertyAsNonEmpty(JSON_KEY_VALUE, out string? value)) ?
        new(Value: value, Display: p.GetPropertyNullIfWhitespace(JSON_KEY_DISPLAY_VALUE)) : null;
}
