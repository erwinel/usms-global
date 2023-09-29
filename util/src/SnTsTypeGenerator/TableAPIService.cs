using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public sealed class TableAPIService : IDisposable
{
    private readonly ILogger<TableAPIService> _logger;
    private readonly IOptions<AppSettings> _appSettings;
    private HttpClientHandler? _handler;

    public bool HasHandler => _handler is  not null;
    
    public Uri BaseURI { get; }

    internal bool IsInitialized() => BaseURI.IsAbsoluteUri && _handler is not null;

    private SysPackage? GetPackage(JsonObject resultObj) => (resultObj.TryGetPropertyValue(JSON_KEY_SYS_PACKAGE, out JsonNode? jsonNode) && jsonNode is JsonObject packageFieldElement && packageFieldElement.TryGetPropertyAsNonEmpty(JSON_KEY_DISPLAY_VALUE, out string? pkgName)) ? new SysPackage() { Name = pkgName, SysId = packageFieldElement.GetPropertyAsNonEmpty(JSON_KEY_VALUE), SourceFqdn = BaseURI.Host } : null;

    private SysScope? GetScope(JsonObject resultObj) => resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value, out string? display_value) ? new() { SysID = value, Name = display_value ?? value, SourceFqdn = BaseURI.Host } : null;

    private TableInfo? GetTable(JsonObject resultObj, string propertyName) => resultObj.TryGetFieldAsNonEmpty(propertyName, out string? super_class, out string? label) ? new() { SysID = super_class, Label = label ?? super_class, SourceFqdn = BaseURI.Host } : null;

    private async Task<TableInfo?> GetTableFromUri(Uri requestUri, CancellationToken cancellationToken)
    {
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is null)
        {
            _logger.LogNoResultsFromQuery(requestUri, resultObj);
            return null;
        }
        if (jsonNode is JsonObject)
            resultObj = (JsonObject)jsonNode;
        else if (jsonNode is JsonArray arr)
        {
            int length = arr.Count;
            if (length == 0)
            {
                _logger.LogNoResultsFromQuery(requestUri, resultObj);
                return null;
            }
            if (length> 1)
                _logger.LogMultipleResponseItems(requestUri, length - 1, resultObj);
            
            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementType(requestUri, resultObj, 0);
            resultObj = (JsonObject)jsonNode;
        }
        else
            throw new InvalidResponseTypeException(requestUri, resultObj);
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFound(requestUri, resultObj, JSON_KEY_SYS_ID);
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_NAME, out string? name))
            throw new ExpectedPropertyNotFound(requestUri, resultObj, JSON_KEY_NAME);
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

    public async Task<TableInfo?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURI.IsAbsoluteUri)
            throw new InvalidOperationException();
        return await GetTableFromUri(BaseURI.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name), cancellationToken);
    }

    public async Task<TableInfo?> GetTableByIdAsync(string sys_id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURI.IsAbsoluteUri)
            throw new InvalidOperationException();
        ;
        return await GetTableFromUri(BaseURI.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, sys_id), cancellationToken);
    }

    public async Task<ElementInfo[]> GetElementsByTableNameAsync(string tableName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURI.IsAbsoluteUri)
            throw new InvalidOperationException();
        Uri requestUri = BaseURI.ToTableApiUri(TABLE_NAME_SYS_DICTIONARY, JSON_KEY_NAME, tableName);
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is null)
            return Array.Empty<ElementInfo>();
        if (jsonNode is not JsonArray arr)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        if (arr.Count == 0)
            return Array.Empty<ElementInfo>();
        return arr.Select((node, index) =>
        {
            if (node is not JsonObject sysDictionary)
                _logger.LogInvalidResultElementType(requestUri, node, index);
            else if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_ELEMENT, out string? name))
                _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_ELEMENT, index, sysDictionary);
            else if (!sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
                _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_SYS_ID, index, sysDictionary);
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
                    Reference = GetTable(resultObj, JSON_KEY_REFERENCE),
                    Package = GetPackage(resultObj),
                    Scope = GetScope(resultObj),
                    Type = sysDictionary.TryGetFieldAsNonEmpty(JSON_KEY_INTERNAL_TYPE, out string? type, out string? displayValue) ? new GlideType() { Name = type, Label = displayValue ?? type, SourceFqdn = requestUri.Host } : null,
                    SourceFqdn = requestUri.Host
                };
            return null!;
        }).Where(n => n is not null).ToArray();

    }

    public async Task<SysScope?> GetScopeByIDAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURI.IsAbsoluteUri)
            throw new InvalidOperationException();
        Uri requestUri = BaseURI.ToTableApiUri(TABLE_NAME_SYS_SCOPE, id);
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject sysScopeResult)
            throw new InvalidHttpResponseException(requestUri, jsonNode.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is null)
        {
            _logger.LogNoResultsFromQuery(requestUri, resultObj);
            return null;
        }
        if (jsonNode is not JsonObject)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        sysScopeResult = (JsonObject)jsonNode;
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFound(requestUri, resultObj, JSON_KEY_SYS_ID);
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
            throw new ExpectedPropertyNotFound(requestUri, resultObj, JSON_KEY_SCOPE);
        return new()
        {
            SysID = sys_id,
            Value = value,
            Name = sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            ShortDescription = sysScopeResult.GetFieldAsNonEmptyOrNull(JSON_KEY_SHORT_DESCRIPTION),
            SourceFqdn = requestUri.Host
        };
    }

    public async Task<GlideType?> GetGlideTypeByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_handler is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        if (!BaseURI.IsAbsoluteUri)
            throw new InvalidOperationException();
        Uri requestUri = BaseURI.ToTableApiUri(TABLE_NAME_SYS_GLIDE_OBJECT, JSON_KEY_NAME, name);
        JsonNode? jsonNode = await _handler!.GetAPIJsonResult(requestUri, _logger, cancellationToken);
        if (jsonNode is null)
            throw new InvalidHttpResponseException(requestUri, string.Empty);
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidHttpResponseException(requestUri, jsonNode.ToJsonString());
        if (!resultObj.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, resultObj);
        if (jsonNode is null)
        {
            _logger.LogNoResultsFromQuery(requestUri, resultObj);
            return null;
        }
        if (jsonNode is not JsonArray arr)
            throw new InvalidResponseTypeException(requestUri, resultObj);
        int length = arr.Count;
        if (length == 0)
        {
            _logger.LogNoResultsFromQuery(requestUri, resultObj);
            return null;
        }
        if (length> 1)
            _logger.LogMultipleResponseItems(requestUri, length - 1, resultObj);
        
        if ((jsonNode = arr[0]) is not JsonObject)
            throw new InvalidResultElementType(requestUri, resultObj, 0);
        resultObj = (JsonObject)jsonNode;
        if (!resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFound(requestUri, resultObj, JSON_KEY_SYS_ID);
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
                _logger.LogCriticalRemoteInstanceUriNotProvided();
            BaseURI = EmptyURI;
            return;
        }
        if (Uri.TryCreate(remoteUri, UriKind.Absolute, out Uri? uri))
        {
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                BaseURI = EmptyURI;
                if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                    _logger.LogCriticalInvalidRemoteInstanceUri();
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
                            _logger.LogCriticalUserNameNotProvided();
                        BaseURI = EmptyURI;
                        return;
                    }
                    Console.Write("User Name: ");
                    string? userName = Console.ReadLine();
                    if (string.IsNullOrEmpty(userName))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalUserNameNotProvided();
                        BaseURI = EmptyURI;
                        return;
                    }
                    string? password = Console.ReadLine();
                    if (string.IsNullOrEmpty(password))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalPasswordNotProvided();
                        BaseURI = EmptyURI;
                        return;
                    }
                    _handler.Credentials = new NetworkCredential(userName, password);
                }
                else
                {
                    if (_appSettings.Value.ClientSecret is null)
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalPasswordNotProvided();
                        BaseURI = EmptyURI;
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
                        _logger.LogCriticalPasswordNotProvided();
                    BaseURI = EmptyURI;
                    return;
                }
                _handler.Credentials = new NetworkCredential(_appSettings.Value.UserName, password);
            }
            BaseURI = uri;
        }
        else
        {
            BaseURI = EmptyURI;
            if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                _logger.LogCriticalInvalidRemoteInstanceUri();
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

[Serializable]
internal class ExpectedPropertyNotFound : Exception
{
    private Uri requestUri;
    private JsonObject resultObj;
    private string jSON_KEY_SYS_ID;

    public ExpectedPropertyNotFound()
    {
    }

    public ExpectedPropertyNotFound(string? message) : base(message)
    {
    }

    public ExpectedPropertyNotFound(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ExpectedPropertyNotFound(Uri requestUri, JsonObject resultObj, string jSON_KEY_SYS_ID)
    {
        this.requestUri = requestUri;
        this.resultObj = resultObj;
        this.jSON_KEY_SYS_ID = jSON_KEY_SYS_ID;
    }

    protected ExpectedPropertyNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
internal class InvalidResultElementType : Exception
{
    private Uri requestUri;
    private JsonObject resultObj;
    private int v;

    public InvalidResultElementType()
    {
    }

    public InvalidResultElementType(string? message) : base(message)
    {
    }

    public InvalidResultElementType(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public InvalidResultElementType(Uri requestUri, JsonObject resultObj, int v)
    {
        this.requestUri = requestUri;
        this.resultObj = resultObj;
        this.v = v;
    }

    protected InvalidResultElementType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}