using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Table" (<see cref="TABLE_NAME_SYS_DB_OBJECT" />) record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c><see cref="JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Label">The value of the <c><see cref="JSON_KEY_LABEL" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="IsExtendable">The boolean value of the <c><see cref="JSON_KEY_IS_EXTENDABLE" />.value</c> property.</param>
/// <param name="NumberPrefix">The value of the <c><see cref="JSON_KEY_NUMBER_REF" />.display_value</c> property.</param>
/// <param name="Package">The deserialized <see cref="JSON_KEY_SYS_PACKAGE" /> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="Scope">The deserialized <see cref="JSON_KEY_SYS_SCOPE" /> property or <see langword="null"/> if the <c>sys_scope.value</c> is empty.</param>
/// <param name="SuperClass">The deserialized <see cref="JSON_KEY_SUPER_CLASS" /> property or <see langword="null"/> if the <c>super_class.value</c> is empty.</param>
/// <param name="AccessibleFrom">The value of the <c><see cref="JSON_KEY_ACCESS" />.value</c> property.</param>
/// <param name="ExtensionModel">The value of the <c><see cref="JSON_KEY_EXTENSION_MODEL" />.value</c> property.</param>
public record RemoteTable(string Name, string Label, string SysID, bool IsExtendable, string? NumberPrefix, RemoteRef? Package, RemoteRef? Scope, RemoteRef? SuperClass,
    string AccessibleFrom, string? ExtensionModel)
{
    internal static RemoteTable? FromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysDbObject)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysDbObject.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysDbObject);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysDbObject);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysDbObject);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysDbObject);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysDbObject, 0);
            sysDbObject = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysDbObject = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysDbObject);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysDbObject);
        }
        if (!sysDbObject.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysDbObject, JSON_KEY_SYS_ID);
        if (!sysDbObject.TryGetFieldAsNonEmpty(JSON_KEY_NAME, out string? name))
            throw new ExpectedPropertyNotFoundException(requestUri, sysDbObject, JSON_KEY_NAME);
        return new RemoteTable(Name: name,
            Label: sysDbObject.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            SysID: sys_id,
            IsExtendable: sysDbObject.GetFieldAsBoolean(JSON_KEY_IS_EXTENDABLE),
            NumberPrefix: sysDbObject.GetProperty<JsonObject>(JSON_KEY_NUMBER_REF)?.CoercePropertyAsNonEmptyOrNull(JSON_KEY_DISPLAY_VALUE),
            Package: RemoteRef.FromProperty(sysDbObject, JSON_KEY_SYS_PACKAGE),
            Scope: RemoteRef.FromProperty(sysDbObject, JSON_KEY_SYS_SCOPE),
            SuperClass: RemoteRef.FromProperty(sysDbObject, JSON_KEY_SUPER_CLASS),
            AccessibleFrom: sysDbObject.GetFieldAsNonEmpty(JSON_KEY_ACCESS),
            ExtensionModel: sysDbObject.GetFieldAsNonEmptyOrNull(JSON_KEY_EXTENSION_MODEL));
    }
}
