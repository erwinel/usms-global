using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models.Remote;

/// <summary>
/// Deserialized "Field class" (<see cref="TABLE_NAME_SYS_GLIDE_OBJECT" />) record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c><see cref="JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Label">The value of the <c><see cref="JSON_KEY_LABEL" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="ScalarType">The value of the <c><see cref="JSON_KEY_SCALAR_TYPE" />.value</c> property.</param>
/// <param name="ScalarLength">The numerical value of the <c><see cref="JSON_KEY_SCALAR_LENGTH" />.value</c> property or <see langword="null"/> if the <c>scalar_length.value</c> is empty.</param>
/// <param name="ClassName">The value of the <c><see cref="JSON_KEY_CLASS_NAME" />.value</c> property or <see langword="null"/> if the <c>class_name.value</c> is empty.</param>
/// <param name="UseOriginalValue">The boolean value of the <c><see cref="JSON_KEY_USE_ORIGINAL_VALUE" />.value</c> property.</param>
/// <param name="IsVisible">The boolean value of the <c><see cref="JSON_KEY_VISIBLE" />.value</c> property.</param>
/// <param name="Attributes">The boolean value of the <c><see cref="JSON_KEY_ATTRIBUTES" />.value</c> property.</param>
/// <param name="Package">The deserialized <see cref="JSON_KEY_SYS_PACKAGE" /> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="Scope">The deserialized <see cref="JSON_KEY_SYS_SCOPE" /> property or <see langword="null"/> if the <c>sys_scope.value</c> is empty.</param>
public record FieldClass(string Name, string Label, string SysID, string? ScalarType, int? ScalarLength, string? ClassName, bool UseOriginalValue, bool IsVisible, string? Attributes, Reference? Package,
    Reference? Scope)
{
    internal static FieldClass? FromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysGlideObject)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysGlideObject.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysGlideObject);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysGlideObject);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysGlideObject);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysGlideObject);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysGlideObject, 0);
            sysGlideObject = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysGlideObject = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysGlideObject);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysGlideObject);
        }
        if (!sysGlideObject.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysGlideObject, JSON_KEY_SYS_ID);
        if (!sysGlideObject.TryGetFieldAsNonEmpty(JSON_KEY_NAME, out string? name))
            throw new ExpectedPropertyNotFoundException(requestUri, sysGlideObject, JSON_KEY_NAME);
        return new FieldClass(Name: name,
            Label: sysGlideObject.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            SysID: sys_id,
            ScalarType: sysGlideObject.GetFieldAsNonEmptyOrNull(JSON_KEY_SCALAR_TYPE),
            ScalarLength: sysGlideObject.GetFieldAsIntOrNull(JSON_KEY_SCALAR_LENGTH),
            ClassName: sysGlideObject.GetFieldAsNonEmptyOrNull(JSON_KEY_CLASS_NAME),
            UseOriginalValue: sysGlideObject.GetFieldAsBoolean(JSON_KEY_USE_ORIGINAL_VALUE),
            IsVisible: sysGlideObject.GetFieldAsBoolean(JSON_KEY_VISIBLE),
            Attributes: sysGlideObject.GetFieldAsNonEmptyOrNull(JSON_KEY_ATTRIBUTES),
            Package: Reference.FromProperty(sysGlideObject, JSON_KEY_SYS_PACKAGE),
            Scope: Reference.FromProperty(sysGlideObject, JSON_KEY_SYS_SCOPE));
    }
}
