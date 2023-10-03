using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

/// <summary>
/// Gets table information from a remote ServiceNow instance.
/// </summary>
public sealed class TableAPIService : IDisposable
{
    private readonly ILogger<TableAPIService> _logger;
    private readonly IOptions<AppSettings> _appSettings;
    private HttpClientHandler? _handler;

    /// <summary>
    /// Gets the base URL of the remote ServiceNow instance.
    /// </summary>
    internal Uri BaseURL { get; }

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => BaseURL.IsAbsoluteUri && _handler is not null;

    private SysPackage? GetPackage(JsonObject resultObj) => (resultObj.TryGetPropertyValue(JSON_KEY_SYS_PACKAGE, out JsonNode? jsonNode) && jsonNode is JsonObject packageFieldElement &&
        packageFieldElement.TryGetPropertyAsNonEmpty(JSON_KEY_DISPLAY_VALUE, out string? pkgName)) ? new()
        {
            Name = pkgName,
            SysId = packageFieldElement.GetPropertyAsNonEmpty(JSON_KEY_VALUE),
            SourceFqdn = BaseURL.Host
        } : null;

    private SysScope? GetScope(JsonObject resultObj) => resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value, out string? display_value) ? new()
        {
            SysID = value,
            Name = display_value ?? value,
            SourceFqdn = BaseURL.Host
        } : null;

    private TableInfo? GetTable(JsonObject resultObj, string propertyName) => resultObj.TryGetFieldAsNonEmpty(propertyName, out string? super_class, out string? label) ? new()
        {
            SysID = super_class,
            Label = label ?? super_class,
            SourceFqdn = BaseURL.Host
        } : null;

    private async Task<TableInfo?> GetTableFromUri(Uri requestUri, bool expectArray, CancellationToken cancellationToken)
    {
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken) ?? throw new InvalidHttpResponseException(requestUri, null);
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
                throw new InvalidResultElementType(requestUri, resultObj, 0);
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
        return new()
        {
            SysID = sys_id,
            Name = name,
            IsExtendable = resultObj.GetFieldAsBoolean(JSON_KEY_IS_EXTENDABLE),
            Label = resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            NumberPrefix = (resultObj.TryGetPropertyValue(JSON_KEY_NUMBER_REF, out jsonNode) && jsonNode is JsonObject obj) ? obj.CoercePropertyAsNonEmptyOrNull(JSON_KEY_DISPLAY_VALUE) : null,
            Package = GetPackage(resultObj),
            Scope = GetScope(resultObj),
            SuperClass = GetTable(resultObj, JSON_KEY_SUPER_CLASS),
            AccessibleFrom = resultObj.GetFieldAsNonEmpty(JSON_KEY_ACCESS),
            ExtensionModel = resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_EXTENSION_MODEL),
            SourceFqdn = requestUri.Host
        };
    }

    /// <summary>
    /// Gets the table from the remote ServiceNow instance that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="TableInfo"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<TableInfo?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURL.IsAbsoluteUri)
            throw new InvalidOperationException();
        _logger.LogGettingTableByNameFromRemote(name);
        return await GetTableFromUri(BaseURL.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name), true, cancellationToken);
    }

    /// <summary>
    /// Gets the table from the remote ServiceNow instance that matches the specified Sys ID.
    /// </summary>
    /// <param name="sys_id">The Sys ID of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="TableInfo"/> record that matches the specified <paramref name="sys_id"/> or <see langword="null" /> if no table was found in the remote ServiceNow instance.</returns>
    internal async Task<TableInfo?> GetTableByIdAsync(string sys_id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURL.IsAbsoluteUri)
            throw new InvalidOperationException();
        _logger.LogGettingTableBySysIdFromRemote(sys_id);
        return await GetTableFromUri(BaseURL.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, sys_id), false, cancellationToken);
    }

    /// <summary>
    /// Gets the elements (columns) from the remote ServiceNow instance that matches the specified table name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="ElementInfo"/> records that match the specified <paramref name="tableName"/>.</returns>
    public async Task<ElementInfo[]> GetElementsByTableNameAsync(string tableName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURL.IsAbsoluteUri)
            throw new InvalidOperationException();
        _logger.LogGettingElementsByTableNameFromRemote(tableName);
        Uri requestUri = BaseURL.ToTableApiUri(TABLE_NAME_SYS_DICTIONARY, JSON_KEY_NAME, tableName);
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode);
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is not JsonArray arr)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        if (arr.Count == 0)
            return Array.Empty<ElementInfo>();
        return arr.Select((node, index) =>
        {
            if (node is not JsonObject sysDictionary)
                _logger.LogInvalidResultElementType(requestUri, node, index);
            else if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_ELEMENT, out string? name))
                _logger.LogExpectedPropertyNotFound(requestUri, JSON_KEY_ELEMENT, index, sysDictionary);
            else if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
                _logger.LogExpectedPropertyNotFound(requestUri, JSON_KEY_SYS_ID, index, sysDictionary);
            else
                return new ElementInfo()
                {
                    SysID = sys_id,
                    Name = name,
                    Label = sysDictionary.GetFieldAsNonEmpty(JSON_KEY_COLUMN_LABEL, name),
                    TableName = tableName,
                    Comments = sysDictionary.GetFieldAsNonEmptyOrNull(JSON_KEY_COMMENTS),
                    DefaultValue = sysDictionary.GetFieldAsNonEmptyOrNull(JSON_KEY_DEFAULT_VALUE),
                    IsActive = sysDictionary.GetFieldAsBoolean(JSON_KEY_ACTIVE),
                    IsArray = sysDictionary.GetFieldAsBoolean(JSON_KEY_ARRAY),
                    IsCalculated = sysDictionary.GetFieldAsBoolean(JSON_KEY_VIRTUAL),
                    IsDisplay = sysDictionary.GetFieldAsBoolean(JSON_KEY_DISPLAY),
                    IsMandatory = sysDictionary.GetFieldAsBoolean(JSON_KEY_MANDATORY),
                    IsPrimary = sysDictionary.GetFieldAsBoolean(JSON_KEY_PRIMARY),
                    IsReadOnly = sysDictionary.GetFieldAsBoolean(JSON_KEY_READ_ONLY),
                    IsUnique = sysDictionary.GetFieldAsBoolean(JSON_KEY_UNIQUE),
                    MaxLength = sysDictionary.GetFieldAsIntOrNull(JSON_KEY_MAX_LENGTH),
                    SizeClass = sysDictionary.GetFieldAsIntOrNull(JSON_KEY_SIZECLASS),
                    Reference = GetTable(sysDictionary, JSON_KEY_REFERENCE),
                    Package = GetPackage(sysDictionary),
                    Scope = GetScope(sysDictionary),
                    Type = sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_INTERNAL_TYPE, out string? type, out string? displayValue) ? new GlideType()
                        {
                            Name = type,
                            Label = displayValue ?? type,
                            SourceFqdn = requestUri.Host
                        } : null,
                    SourceFqdn = requestUri.Host
                };
            return null!;
        }).Where(n => n is not null).ToArray();

    }

    /// <summary>
    /// Gets the scope from the remote ServiceNow instance that matches the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the scope record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="SysScope"/> record that matches the specified <paramref name="id"/> or <see langword="null" /> if no scope was found in the remote ServiceNow instance.</returns>
    internal async Task<SysScope?> GetScopeByIDAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURL.IsAbsoluteUri)
            throw new InvalidOperationException();
        _logger.LogGettingScopeByIdentifierFromRemote(id);
        Uri requestUri = BaseURL.ToTableApiUri(TABLE_NAME_SYS_SCOPE, id);
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
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
        return new()
        {
            SysID = sys_id,
            Value = value,
            Name = sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            ShortDescription = sysScopeResult.GetFieldAsNonEmptyOrNull(JSON_KEY_SHORT_DESCRIPTION),
            SourceFqdn = requestUri.Host
        };
    }

    /// <summary>
    /// Gets the type information from the remote ServiceNow instance that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the type record.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="GlideType"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no type record was found in the remote ServiceNow instance.</returns>
    internal async Task<GlideType?> GetGlideTypeByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURL.IsAbsoluteUri)
            throw new InvalidOperationException();
        _logger.LogGettingTypeByNameFromRemoteTrace(name);
        Uri requestUri = BaseURL.ToTableApiUri(TABLE_NAME_SYS_GLIDE_OBJECT, JSON_KEY_NAME, name);
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
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
            throw new InvalidResultElementType(requestUri, resultObj, 0);
        resultObj = (JsonObject)jsonNode;
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, resultObj, JSON_KEY_SYS_ID);
        return new()
        {
            SysID = sys_id,
            Name = name,
            Label = resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            ScalarType = resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_SCALAR_TYPE),
            ScalarLength = resultObj.GetFieldAsIntOrNull(JSON_KEY_SCALAR_LENGTH),
            ClassName = resultObj.GetFieldAsNonEmptyOrNull(JSON_KEY_CLASS_NAME),
            UseOriginalValue = resultObj.GetFieldAsBoolean(JSON_KEY_USE_ORIGINAL_VALUE),
            IsVisible = resultObj.GetFieldAsBoolean(JSON_KEY_VISIBLE),
            Package = GetPackage(resultObj),
            Scope = GetScope(resultObj),
            SourceFqdn = requestUri.Host
        };
    }

    public TableAPIService(IOptions<AppSettings> appSettings, ILogger<TableAPIService> logger)
    {
        _logger = logger;
        _appSettings = appSettings;
        _handler = new();
        var remoteUri = _appSettings.Value.RemoteURL;
        if (string.IsNullOrWhiteSpace(remoteUri))
        {
            if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                _logger.LogRemoteInstanceUriNotProvided();
            BaseURL = EmptyURI;
            return;
        }
        if (Uri.TryCreate(remoteUri, UriKind.Absolute, out Uri? uri))
        {
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                BaseURL = EmptyURI;
                if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                    _logger.LogInvalidRemoteInstanceUri();
                return;
            }
            uri = new UriBuilder(uri) { Fragment = null, Query = null, Path = "/" }.Uri;
            if (_appSettings.Value.UserName is null)
            {
                if (_appSettings.Value.ClientId is null)
                {
                    if (_appSettings.Value.Password is null)
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogUserNameNotProvided();
                        BaseURL = EmptyURI;
                        return;
                    }
                    Console.Write("User Name: ");
                    string? userName = Console.ReadLine();
                    if (string.IsNullOrEmpty(userName))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogUserNameNotProvided();
                        BaseURL = EmptyURI;
                        return;
                    }
                    string? password = Console.ReadLine();
                    if (string.IsNullOrEmpty(password))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogPasswordNotProvided();
                        BaseURL = EmptyURI;
                        return;
                    }
                    _handler.Credentials = new NetworkCredential(userName, password);
                }
                else
                {
                    if (_appSettings.Value.ClientSecret is null)
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogPasswordNotProvided();
                        BaseURL = EmptyURI;
                        return;
                    }
                    _handler.Credentials = new NetworkCredential(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret);
                }
            }
            else
            {
                string? password = _appSettings.Value.Password;
                if (password is null && string.IsNullOrEmpty(password = Console.ReadLine()))
                {
                    if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                        _logger.LogPasswordNotProvided();
                    BaseURL = EmptyURI;
                    return;
                }
                _handler.Credentials = new NetworkCredential(_appSettings.Value.UserName, password);
            }
            BaseURL = uri;
        }
        else
        {
            BaseURL = EmptyURI;
            if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                _logger.LogInvalidRemoteInstanceUri();
        }
    }

    private void Dispose(bool disposing)
    {
        HttpClientHandler? handler = _handler;
        _handler = null;
        if (handler is not null && disposing)
            handler.Dispose();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
