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
    private readonly SnClientHandlerService _handler;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _handler.InitSuccessful;

    delegate TResult DeserializeRef<out TResult>(string value, string? display_value);

    private static T? DeserializeProperty<T>(JsonObject obj, string propertyName, DeserializeRef<T> onDeserialize) where T : class => (obj.TryGetProperty(propertyName, out JsonObject? p) && p.TryGetPropertyAsNonEmpty(JSON_KEY_VALUE, out string? value)) ?
        onDeserialize(value, p.GetPropertyNullIfWhitespace(JSON_KEY_DISPLAY_VALUE)) : null;

    private async Task<(Uri RequestUri, JsonObject? Item, JsonObject ResponseObject)> GetTableApiJsonResponseAsync(string tableName, string sysId, CancellationToken cancellationToken)
    {
        (Uri requestUri, JsonNode? jsonNode) = await _handler!.GetTableApiJsonResponseAsync(tableName, sysId, cancellationToken);
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
    /// <returns>The <see cref="RemoteTable"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<RemoteTable?> GetTableRecordByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingTableByNameFromRemote(name);
        (Uri requestUri, JsonNode? response) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name, cancellationToken);
        return RemoteTable.FromJson(requestUri, response, _logger, true);
    }

    /// <summary>
    /// Gets the table from the remote ServiceNow instance that matches the specified Sys ID.
    /// </summary>
    /// <param name="sys_id">The Sys ID of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="RemoteTable"/> record that matches the specified <paramref name="sys_id"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<RemoteTable?> GetTableRecordBySysIdAsync(string sys_id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingTableBySysIdFromRemote(sys_id);
        (Uri requestUri, JsonNode? response) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_DB_OBJECT, sys_id, cancellationToken);
        return RemoteTable.FromJson(requestUri, response, _logger, true);
    }

    /// /// <summary>
    /// Gets the elements (columns) from the remote ServiceNow instance that matches the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="RemoteDictionaryEntry"/> records that match the specified <paramref name="tableName"/>.</returns>
    internal async Task<RemoteDictionaryEntry[]> GetDictionaryEntryRecordsByTableNameAsync(string tableName, CancellationToken cancellationToken)
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
            return Array.Empty<RemoteDictionaryEntry>();
        return arr.Select((node, index) => RemoteDictionaryEntry.FromJson(requestUri, node, _logger)!).Where(n => n is not null).ToArray();
    }

    /// 
    /// <summary>
    /// Gets the Application from the remote ServiceNow instance that matches the specified unique identifier.
    /// </summary>
    /// <param name="sys_id">The unique identifier of the <see cref="RemoteApplication" />.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="RemoteApplication"/> record that matches the specified <paramref name="sys_id"/> or <see langword="null" /> if no scope was found in the remote ServiceNow instance.</returns>
    internal async Task<RemoteApplication?> GetApplicationRecordBySysIdAsync(string sys_id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingScopeByIdentifierFromRemote(sys_id);
        // (Uri requestUri, JsonObject? sysScopeResult, JsonObject responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_SCOPE, id, cancellationToken);
        (Uri requestUri, JsonObject? sysScopeResult, JsonObject responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_STORE_APP, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.StoreAppFromJson(requestUri, sysScopeResult, _logger);
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_APP, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.CustomApplicationFromJson(requestUri, sysScopeResult, _logger);
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_SCOPE, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.ApplicationFromJson(requestUri, sysScopeResult, _logger);
        _logger.LogNoResultsFromQuery(requestUri, responseObj);
        return null;
    }

    /// <summary>
    /// Gets the scope from the remote ServiceNow instance that matches the specified unique identifier.
    /// </summary>
    /// <param name="sys_id">The unique identifier of the scope record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="RemotePackage"/> record that matches the specified <paramref name="sys_id"/> or <see langword="null" /> if no scope was found in the remote ServiceNow instance.</returns>
    internal async Task<RemotePackage?> GetPackageRecordBySysIdAsync(string sys_id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingScopeByIdentifierFromRemote(sys_id);

        (Uri? requestUri, JsonObject? sysScopeResult, JsonObject responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_PLUGINS, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.PluginFromJson(requestUri, sysScopeResult, _logger);
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_STORE_APP, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.StoreAppFromJson(requestUri, sysScopeResult, _logger);
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_APP, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.CustomApplicationFromJson(requestUri, sysScopeResult, _logger);
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_SCOPE, sys_id, cancellationToken);
        if (sysScopeResult is not null)
            return RemotePackage.ApplicationFromJson(requestUri, sysScopeResult, _logger);
        (requestUri, sysScopeResult, responseObj) = await GetTableApiJsonResponseAsync(TABLE_NAME_SYS_PACKAGE, sys_id, cancellationToken);
        if (sysScopeResult is null)
        {
            _logger.LogNoResultsFromQuery(requestUri, responseObj);
            return null;
        }
        return RemotePackage.PackageFromJson(requestUri, sysScopeResult, _logger);
    }

    /// <summary>
    /// Gets the type information from the remote ServiceNow instance that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the type record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="GlideTypeRecord"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no type record was found in the remote ServiceNow instance.</returns>
    internal async Task<RemoteFieldClass?> GetFieldClassRecordByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!_handler.InitSuccessful)
            throw new InvalidOperationException();
        _logger.LogGettingTypeByNameFromRemoteTrace(name);
        (Uri requestUri, JsonNode? jsonNode) = await _handler.GetTableApiJsonResponseAsync(TABLE_NAME_SYS_GLIDE_OBJECT, JSON_KEY_NAME, name, cancellationToken);
        return RemoteFieldClass.FromJson(requestUri, jsonNode, _logger, true);
    }

    public TableAPIService(SnClientHandlerService handler, ILogger<TableAPIService> logger)
    {
        _logger = logger;
        _handler = handler;
    }
}
