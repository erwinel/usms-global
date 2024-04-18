using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models.Remote;

/// <summary>
/// Deserialized <c>sys_dictionary</c> record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c>element.value</c> property.</param>
/// <param name="Label">The value of the <c>column_label.value</c> property.</param>
/// <param name="SysID">The value of the <c>sys_id.value</c> property.</param>
/// <param name="Reference">The deserialized <c>reference</c> property or <see langword="null"/> if the <c>reference.value</c> is empty.</param>
/// <param name="IsReadOnly">The boolean value of the <c>read_only.value</c> property.</param>
/// <param name="Type">The deserialized <c>internal_type</c> property or <see langword="null"/> if the <c>internal_type.value</c> is empty.</param>
/// <param name="MaxLength">The numerical value of the <c>max_length.value</c> property or <see langword="null"/> if the <c>max_length.value</c> is empty.</param>
/// <param name="IsActive">The boolean value of the <c>active.value</c> property.</param>
/// <param name="IsUnique">The boolean value of the <c>unique.value</c> property.</param>
/// <param name="IsPrimary">The boolean value of the <c>primary.value</c> property.</param>
/// <param name="IsCalculated">The boolean value of the <c>virtual.value</c> property.</param>
/// <param name="SizeClass">The numerical value of the <c>sizeclass.value</c> property or <see langword="null"/> if the <c>sizeclass.value</c> is empty.</param>
/// <param name="IsMandatory">The boolean value of the <c>mandatory.value</c> property.</param>
/// <param name="IsArray">The boolean value of the <c>array.value</c> property.</param>
/// <param name="Comments">The value of the <c>comments.value</c> property or <see langword="null"/> if the <c>comments.value</c> is empty.</param>
/// <param name="IsDisplay">The boolean value of the <c>is_extendable.value</c> property.</param>
/// <param name="DefaultValue">The value of the <c>default_value.value</c> property or <see langword="null"/> if the <c>default_value.value</c> is empty.</param>
/// <param name="Package">The deserialized <c>sys_package</c> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
public record DictionaryEntry(string Name, string Label, string SysID, Reference? Reference, bool IsReadOnly, Reference? Type, int? MaxLength,
    bool IsActive, bool IsUnique, bool IsPrimary, bool IsCalculated, int? SizeClass, bool IsMandatory, bool IsArray,
    string? Comments, bool IsDisplay, string? DefaultValue, Reference? Scope, Reference? Package)
{
    internal static DictionaryEntry? FromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray = false)
    {
        if (jsonNode is not JsonObject sysDictionary)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysDictionary.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysDictionary);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysDictionary);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysDictionary);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysDictionary);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysDictionary, 0);
            sysDictionary = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysDictionary = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysDictionary);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysDictionary);
        }
        if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysDictionary, JSON_KEY_SYS_ID);
        if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_NAME, out string? name))
            throw new ExpectedPropertyNotFoundException(requestUri, sysDictionary, JSON_KEY_NAME);
        return new DictionaryEntry(
            Name: name,
            Label: sysDictionary.GetFieldAsNonEmpty(JSON_KEY_COLUMN_LABEL, name),
            SysID: sys_id,
            Reference: Reference.FromProperty(sysDictionary, JSON_KEY_REFERENCE),
            IsReadOnly: sysDictionary.GetFieldAsBoolean(JSON_KEY_READ_ONLY),
            Type: Reference.FromProperty(sysDictionary, JSON_KEY_INTERNAL_TYPE),
            MaxLength: sysDictionary.GetFieldAsIntOrNull(JSON_KEY_MAX_LENGTH),
            IsActive: sysDictionary.GetFieldAsBoolean(JSON_KEY_ACTIVE),
            IsUnique: sysDictionary.GetFieldAsBoolean(JSON_KEY_UNIQUE),
            IsPrimary: sysDictionary.GetFieldAsBoolean(JSON_KEY_PRIMARY),
            IsCalculated: sysDictionary.GetFieldAsBoolean(JSON_KEY_VIRTUAL),
            SizeClass: sysDictionary.GetFieldAsIntOrNull(JSON_KEY_SIZECLASS),
            IsMandatory: sysDictionary.GetFieldAsBoolean(JSON_KEY_MANDATORY),
            IsArray: sysDictionary.GetFieldAsBoolean(JSON_KEY_ARRAY),
            Comments: sysDictionary.GetFieldAsNonEmptyOrNull(JSON_KEY_COMMENTS),
            IsDisplay: sysDictionary.GetFieldAsBoolean(JSON_KEY_DISPLAY),
            DefaultValue: sysDictionary.GetFieldAsNonEmptyOrNull(JSON_KEY_DEFAULT_VALUE),
            Scope: Reference.FromProperty(sysDictionary, JSON_KEY_SYS_SCOPE),
            Package: Reference.FromProperty(sysDictionary, JSON_KEY_SYS_PACKAGE)
        );
    }
}
