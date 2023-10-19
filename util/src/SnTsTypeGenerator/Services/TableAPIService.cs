using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Models.TableAPI;
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

    private Table? GetTableFromResponse(Uri requestUri, JsonNode? jsonNode, bool expectArray)
    {
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode);
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
        return new(Name: name,
                   Label: resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
                   SysID: sys_id,
                   IsExtendable: resultObj.GetFieldAsBoolean(JSON_KEY_IS_EXTENDABLE),
                   NumberPrefix: resultObj.GetProperty<JsonObject>(JSON_KEY_NUMBER_REF)?.CoercePropertyAsNonEmptyOrNull(JSON_KEY_DISPLAY_VALUE),
                   Package: RecordRef.DeserializeProperty(resultObj, JSON_KEY_SYS_PACKAGE),
                   Scope: RecordRef.DeserializeProperty(resultObj, JSON_KEY_SYS_SCOPE),
                   SuperClass: RecordRef.DeserializeProperty(resultObj, JSON_KEY_SUPER_CLASS),
                   AccessibleFrom: resultObj.GetFieldAsNonEmpty(JSON_KEY_ACCESS),
                   ExtensionModel: resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_EXTENSION_MODEL),
                   SourceFqdn: requestUri.Host);
    }

    /// <summary>
    /// Gets the table from the remote ServiceNow instance that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="Table"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<Table?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
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
    /// <returns>The <see cref="Table"/> record that matches the specified <paramref name="sys_id"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<Table?> GetTableByIdAsync(string sys_id, CancellationToken cancellationToken)
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
    /// <returns>The <see cref="Element"/> records that match the specified <paramref name="tableName"/>.</returns>
    public async Task<Element[]> GetElementsByTableNameAsync(string tableName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingElementsByTableNameFromRemote(tableName);
        (Uri requestUri, JsonNode? jsonNode) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_DICTIONARY, JSON_KEY_NAME, tableName, cancellationToken);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode);
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is not JsonArray arr)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        if (arr.Count == 0)
            return Array.Empty<Element>();
        return arr.Select((node, index) =>
        {
            if (node is not JsonObject sysDictionary)
                _logger.LogInvalidResultElementType(requestUri, node, index);
            else if (sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_ELEMENT, out string? name))
            {
                if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
                    _logger.LogExpectedPropertyNotFound(requestUri, JSON_KEY_SYS_ID, index, sysDictionary);
                else
                    return new Element(name, sysDictionary.GetFieldAsNonEmpty(JSON_KEY_COLUMN_LABEL, name), sys_id, RecordRef.DeserializeProperty(sysDictionary, JSON_KEY_REFERENCE), sysDictionary.GetFieldAsBoolean(JSON_KEY_READ_ONLY),
                        sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_INTERNAL_TYPE, out string? type, out string? displayValue) ? new RecordRef(type, displayValue ?? type) : null, sysDictionary.GetFieldAsIntOrNull(JSON_KEY_MAX_LENGTH),
                        sysDictionary.GetFieldAsBoolean(JSON_KEY_ACTIVE), sysDictionary.GetFieldAsBoolean(JSON_KEY_UNIQUE), sysDictionary.GetFieldAsBoolean(JSON_KEY_PRIMARY), sysDictionary.GetFieldAsBoolean(JSON_KEY_VIRTUAL),
                        sysDictionary.GetFieldAsIntOrNull(JSON_KEY_SIZECLASS), sysDictionary.GetFieldAsBoolean(JSON_KEY_MANDATORY), sysDictionary.GetFieldAsBoolean(JSON_KEY_ARRAY), sysDictionary.GetFieldAsNonEmptyOrNull(JSON_KEY_COMMENTS),
                        sysDictionary.GetFieldAsBoolean(JSON_KEY_DISPLAY), sysDictionary.GetFieldAsNonEmptyOrNull(JSON_KEY_DEFAULT_VALUE), RecordRef.DeserializeProperty(sysDictionary, JSON_KEY_SYS_PACKAGE), requestUri.Host);
            }
            return null!;
        }).Where(n => n is not null).ToArray();
    }

    /// <summary>
    /// Gets the scope from the remote ServiceNow instance that matches the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the scope record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="Scope"/> record that matches the specified <paramref name="id"/> or <see langword="null" /> if no scope was found in the remote ServiceNow instance.</returns>
    internal async Task<Scope?> GetScopeByIDAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingScopeByIdentifierFromRemote(id);
        (Uri requestUri, JsonNode? jsonNode) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_SCOPE, id, cancellationToken);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode);
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is not JsonObject sysScopeResult)
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                _logger.LogNoResultsFromQuery(requestUri, resultObj);
                return null;
            }
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        }
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, resultObj, JSON_KEY_SYS_ID);
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
            throw new ExpectedPropertyNotFoundException(requestUri, resultObj, JSON_KEY_SCOPE);
        return new(
            Name: sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            Value: value,
            ShortDescription: sysScopeResult.GetFieldAsNonEmptyOrNull(JSON_KEY_SHORT_DESCRIPTION),
            SysID: sys_id,
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
            throw new InvalidHttpResponseException(requestUri, jsonNode);
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
        return new(Name: name,
            Label: resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            SysID: sys_id,
            ScalarType: resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_SCALAR_TYPE),
            ScalarLength: resultObj.GetFieldAsIntOrNull(JSON_KEY_SCALAR_LENGTH),
            ClassName: resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_CLASS_NAME),
            UseOriginalValue: resultObj.GetFieldAsBoolean(JSON_KEY_USE_ORIGINAL_VALUE),
            IsVisible: resultObj.GetFieldAsBoolean(JSON_KEY_VISIBLE),
            Package: RecordRef.DeserializeProperty(resultObj, JSON_KEY_SYS_PACKAGE),
            Scope: RecordRef.DeserializeProperty(resultObj, JSON_KEY_SYS_SCOPE),
            SourceFqdn: requestUri.Host);
    }

    public TableAPIService(SnClientHandlerService handler, ILogger<TableAPIService> logger)
    {
        _logger = logger;
        _handler = handler;
    }
}
