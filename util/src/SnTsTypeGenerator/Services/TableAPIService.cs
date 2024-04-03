using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

/// <summary>
/// Gets table information from a remote ServiceNow instance.
/// </summary>
public sealed class TableAPIService
{
    private readonly ILogger<TableAPIService> _logger;
    private readonly SnClientHandlerService? _handler;

    /// <summary>
    /// Gets the base URL of the remote ServiceNow instance.
    /// </summary>
    internal string SourceFqdn => (_handler ?? throw new InvalidOperationException()).BaseURL.Host;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _handler?.InitSuccessful ?? false;

    delegate TResult DeserializeRef<out TResult>(string value, string? display_value);

    private static T? DeserializeProperty<T>(JsonObject obj, string propertyName, DeserializeRef<T> onDeserialize) where T : class => (obj.TryGetProperty(propertyName, out JsonObject? p) && p.TryGetPropertyAsNonEmpty(JSON_KEY_VALUE, out string? value)) ?
        onDeserialize(value, p.GetPropertyNullIfWhitespace(JSON_KEY_DISPLAY_VALUE)) : null;

    private TableRecord? GetTableFromResponse(Uri requestUri, JsonNode? jsonNode, bool expectArray)
    {
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, resultObj);
            int length = arr.Count;
            if (length == 0)
            {
                _logger.LogNoResultsFromQuery(requestUri, resultObj);
                return null;
            }
            if (length > 1)
                _logger.LogMultipleResponseItems(requestUri, length - 1, resultObj);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, resultObj, 0);
            resultObj = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            resultObj = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                _logger.LogNoResultsFromQuery(requestUri, resultObj);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, resultObj);
        }
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, resultObj, JSON_KEY_SYS_ID);
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_NAME, out string? name))
            throw new ExpectedPropertyNotFoundException(requestUri, resultObj, JSON_KEY_NAME);
        TableRecord tableRecord = new(Name: name,
                   Label: resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
                   SysID: sys_id,
                   IsExtendable: resultObj.GetFieldAsBoolean(JSON_KEY_IS_EXTENDABLE),
                   NumberPrefix: resultObj.GetProperty<JsonObject>(JSON_KEY_NUMBER_REF)?.CoercePropertyAsNonEmptyOrNull(JSON_KEY_DISPLAY_VALUE),
                   Package: DeserializeProperty(resultObj, JSON_KEY_SYS_PACKAGE, (sys_id, name) => new PackageRef(SysID: sys_id, Name: name.NullIfWhiteSpace())),
                   Scope: DeserializeProperty(resultObj, JSON_KEY_SYS_SCOPE, (id, name) => new ScopeRef(ID: id, Name: name.NullIfWhiteSpace())),
                   SuperClass: DeserializeProperty(resultObj, JSON_KEY_SUPER_CLASS, (sys_id, label) => new SuperClassRef(SysID: sys_id, Label: label.NullIfWhiteSpace())),
                   AccessibleFrom: resultObj.GetFieldAsNonEmpty(JSON_KEY_ACCESS),
                   ExtensionModel: resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_EXTENSION_MODEL),
                   SourceFqdn: requestUri.Host);
        return tableRecord;
    }

    private async Task<(Uri RequestUri, JsonObject? Item, JsonObject ResponseObject)> GetTableApiJsonResponseAsync(string tableName, string id, CancellationToken cancellationToken)
    {
        (Uri requestUri, JsonNode? jsonNode) = await _handler!.GetTableApiJsonResponseAsync(tableName, id, cancellationToken);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is JsonObject response)
            return (requestUri, response, resultObj);
        if (jsonNode is JsonArray arr && arr.Count == 0)
            return (requestUri, null, resultObj);
        throw new InvalidHttpResponseException(requestUri, string.Empty);
    }

    /// <summary>
    /// Gets the table from the remote ServiceNow instance that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="TableRecord"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<TableRecord?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingTableByNameFromRemote(name);
        (Uri requestUri, JsonNode? response) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name, cancellationToken);
        return GetTableFromResponse(requestUri, response, true);
    }

    /// <summary>
    /// Gets the table from the remote ServiceNow instance that matches the specified Sys ID.
    /// </summary>
    /// <param name="sys_id">The Sys ID of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="TableRecord"/> record that matches the specified <paramref name="sys_id"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<TableRecord?> GetTableByIdAsync(string sys_id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingTableBySysIdFromRemote(sys_id);
        (Uri requestUri, JsonNode? response) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_DB_OBJECT, sys_id, cancellationToken);
        return GetTableFromResponse(requestUri, response, false);
    }

    /// <summary>
    /// Gets the elements (columns) from the remote ServiceNow instance that matches the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="ElementRecord"/> records that match the specified <paramref name="tableName"/>.</returns>
    internal async Task<ElementRecord[]> GetElementsByTableNameAsync(string tableName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingElementsByTableNameFromRemote(tableName);
        (Uri requestUri, JsonNode? jsonNode) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_DICTIONARY, JSON_KEY_NAME, tableName, cancellationToken);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is not JsonArray arr)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        if (arr.Count == 0)
            return Array.Empty<ElementRecord>();
        return arr.Select((node, index) =>
        {
            if (node is not JsonObject sysDictionary)
                _logger.LogInvalidResultElementType(requestUri, node, index);
            else if (sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_ELEMENT, out string? name))
            {
                if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
                    _logger.LogExpectedPropertyNotFound(requestUri, JSON_KEY_SYS_ID, index, sysDictionary);
                else
                {
                    ElementRecord elementRecord = new(
                        Name: name,
                        Label: sysDictionary.GetFieldAsNonEmpty(JSON_KEY_COLUMN_LABEL, name),
                        SysID: sys_id,
                        Reference: DeserializeProperty(sysDictionary, JSON_KEY_REFERENCE, (name, label) => new TableRef(Name: name, Label: label.AsNonEmpty(name))),
                        IsReadOnly: sysDictionary.GetFieldAsBoolean(JSON_KEY_READ_ONLY),
                        Type: DeserializeProperty(sysDictionary, JSON_KEY_INTERNAL_TYPE, (name, label) => new TypeRef(Name: name, Label: label.AsNonEmpty(name))),
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
                        Scope: DeserializeProperty(sysDictionary, JSON_KEY_SYS_SCOPE, (id, name) => new ScopeRef(ID: id, Name: name.NullIfWhiteSpace())),
                        Package: DeserializeProperty(sysDictionary, JSON_KEY_SYS_PACKAGE, (sys_id, name) => new PackageRef(SysID: sys_id, Name: name.NullIfWhiteSpace())),
                        SourceFqdn: requestUri.Host);
                    return elementRecord;
                }
            }
            return null!;
        }).Where(n => n is not null).ToArray();
    }

    /// <summary>
    /// Gets the scope from the remote ServiceNow instance that matches the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the scope record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="ScopeRecord"/> record that matches the specified <paramref name="id"/> or <see langword="null" /> if no scope was found in the remote ServiceNow instance.</returns>
    internal async Task<ScopeRecord?> GetScopeByIDAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingScopeByIdentifierFromRemote(id);
        (Uri requestUri, JsonObject? sysScopeResult, JsonObject responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_SCOPE, id, cancellationToken);
        if (sysScopeResult is null)
        {
            (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_STORE_APP, id, cancellationToken);
            if (sysScopeResult is null)
            {
                (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_APP, id, cancellationToken);
                if (sysScopeResult is null)
                {
                    _logger.LogNoResultsFromQuery(requestUri, responseObj);
                    return null;
                }
            }
        }
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SYS_ID);
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
            throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SCOPE);
        return new(Name: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            Value: value,
            ID: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_SOURCE),
            Version: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            ShortDescription: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_SHORT_DESCRIPTION),
            SysID: sys_id,
            Licensable: sysScopeResult.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            Active: sysScopeResult.GetFieldAsBoolean(JSON_KEY_ACTIVE),
            SourceFqdn: requestUri.Host);
    }

    /// <summary>
    /// Gets the scope from the remote ServiceNow instance that matches the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the scope record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="ScopeRecord"/> record that matches the specified <paramref name="id"/> or <see langword="null" /> if no scope was found in the remote ServiceNow instance.</returns>
    internal async Task<PackageRecord?> GetPackageByIDAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingScopeByIdentifierFromRemote(id);

        (Uri? requestUri, JsonObject? sysScopeResult, JsonObject responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_PLUGINS, id, cancellationToken);
        string? sys_id, source;
        if (sysScopeResult is not null)
        {
            if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out sys_id))
                throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SYS_ID);
            if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SOURCE, out source))
                throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SOURCE);
            return new PluginRecord(ID: source,
                Name: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, source),
                Version: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_VERSION),
                SysID: sys_id,
                ParentID: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_PARENT),
                Licensable: sysScopeResult.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
                Active: sysScopeResult.GetFieldAsBoolean(JSON_KEY_ACTIVE),
                SourceFqdn: requestUri.Host);
        }
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_STORE_APP, id, cancellationToken);
        if (sysScopeResult is not null)
        {
            if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out sys_id))
                throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SYS_ID);
            if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
                throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SCOPE);
            return new StoreAppRecord(Name: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
                Value: value,
                ID: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_SOURCE),
                Version: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_VERSION),
                ShortDescription: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_SHORT_DESCRIPTION),
                SysID: sys_id,
                Licensable: sysScopeResult.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
                Active: sysScopeResult.GetFieldAsBoolean(JSON_KEY_ACTIVE),
                Dependencies: sysScopeResult.GetFieldAsStringArray(JSON_KEY_DEPENDENCIES),
                SourceFqdn: requestUri.Host);
        }
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_SCOPE, id, cancellationToken);
        if (sysScopeResult is not null)
        {
            if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out sys_id))
                throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SYS_ID);
            if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
                throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SCOPE);
            return new ScopeRecord(Name: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
                Value: value,
                ID: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_SOURCE),
                Version: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_VERSION),
                ShortDescription: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_SHORT_DESCRIPTION),
                SysID: sys_id,
                Licensable: sysScopeResult.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
                Active: sysScopeResult.GetFieldAsBoolean(JSON_KEY_ACTIVE),
                SourceFqdn: requestUri.Host);
        }
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_PACKAGE, id, cancellationToken);
        if (sysScopeResult is null)
        {
            _logger.LogNoResultsFromQuery(requestUri, responseObj);
            return null;
        }
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SYS_ID);
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SOURCE, out source))
            throw new ExpectedPropertyNotFoundException(requestUri, responseObj, JSON_KEY_SOURCE);
        return new PackageRecord(ID: source,
            Name: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, source),
            Version: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            SysID: sys_id,
            Licensable: sysScopeResult.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            Active: sysScopeResult.GetFieldAsBoolean(JSON_KEY_ACTIVE),
            SourceFqdn: requestUri.Host);
    }

    /// <summary>
    /// Gets the type information from the remote ServiceNow instance that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the type record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="GlideTypeRecord"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no type record was found in the remote ServiceNow instance.</returns>
    internal async Task<GlideTypeRecord?> GetGlideTypeByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingTypeByNameFromRemoteTrace(name);
        (Uri requestUri, JsonNode? jsonNode) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_GLIDE_OBJECT, JSON_KEY_NAME, name, cancellationToken);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is not JsonArray arr)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        int length = arr.Count;
        if (length == 0)
        {
            _logger.LogNoResultsFromQuery(requestUri, resultObj);
            return null;
        }
        if (length > 1)
            _logger.LogMultipleResponseItems(requestUri, length - 1, resultObj);

        if ((jsonNode = arr[0]) is not JsonObject)
            throw new InvalidResultElementTypeException(requestUri, resultObj, 0);
        resultObj = (JsonObject)jsonNode;
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, resultObj, JSON_KEY_SYS_ID);
        GlideTypeRecord glideTypeRecord = new(Name: name,
            Label: resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            SysID: sys_id,
            ScalarType: resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_SCALAR_TYPE),
            ScalarLength: resultObj.GetFieldAsIntOrNull(JSON_KEY_SCALAR_LENGTH),
            ClassName: resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_CLASS_NAME),
            UseOriginalValue: resultObj.GetFieldAsBoolean(JSON_KEY_USE_ORIGINAL_VALUE),
            IsVisible: resultObj.GetFieldAsBoolean(JSON_KEY_VISIBLE),
            Package: DeserializeProperty(resultObj, JSON_KEY_SYS_PACKAGE, (sys_id, name) => new PackageRef(SysID: sys_id, Name: name.NullIfWhiteSpace())),
            Scope: DeserializeProperty(resultObj, JSON_KEY_SYS_SCOPE, (id, name) => new ScopeRef(ID: id, Name: name.NullIfWhiteSpace())),
            SourceFqdn: requestUri.Host);
        return glideTypeRecord;
    }

    public TableAPIService(SnClientHandlerService handler, ILogger<TableAPIService> logger)
    {
        _logger = logger;
        _handler = handler;
    }
}
