using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public sealed class RemoteLoaderService
{
    private readonly ILogger<RemoteLoaderService> _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly HttpClientHandler? _handler;
    private readonly Uri _remoteUri;
    private SourceInfo? _source;
    private readonly Dictionary<string, string> _scopeMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _tableMap = new(StringComparer.InvariantCultureIgnoreCase);

    internal bool HasHandler => _handler is not null;

    private async Task<SourceInfo> GetSourceAsync(CancellationToken cancellationToken)
    {
        if (_source is null)
        {
            string fqdn = _remoteUri.Host;
            if ((_source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.FQDN == fqdn, cancellationToken)) is null)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null!;
                _source = new()
                {
                    FQDN = fqdn,
                    Label = fqdn,
                    IsPersonalDev = false,
                    LastAccessed = DateTime.Now
                };
                await _dbContext.Sources.AddAsync(_source, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return null!;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        return _source;
    }

    private async Task<SysScope?> DeserializeScopeAsync(JsonObject? sysScopeResult, Uri requestUri, CancellationToken cancellationToken)
    {
        if (sysScopeResult is null || cancellationToken.IsCancellationRequested)
            return null;
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
        {
            _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_ELEMENT, sysScopeResult);
            return null;
        }
        if (!sysScopeResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
        {
            _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_SYS_ID, sysScopeResult);
            return null;
        }
        var scope = await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (scope is not null)
        {
            if (!_scopeMap.ContainsKey(sys_id))
                _scopeMap.Add(sys_id, scope.Value);
            return scope;
        }
        scope = new()
        {
            SysID = sys_id,
            Value = value,
            Name = sysScopeResult.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            ShortDescription = sysScopeResult.GetFieldAsNonEmptyOrNull(JSON_KEY_SHORT_DESCRIPTION),
            LastUpdated = DateTime.Now,
            Source = await GetSourceAsync(cancellationToken)
        };
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.Scopes.AddAsync(scope, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.SaveChangesAsync(cancellationToken);
        if (!_scopeMap.ContainsKey(sys_id))
            _scopeMap.Add(sys_id, scope.Value);
        if (cancellationToken.IsCancellationRequested)
            return null;
        return scope;
    }

    private async Task DeserializeElementAsync(JsonNode? sysDictionaryResult, int index, TableInfo table, Uri requestUri, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested || sysDictionaryResult is null)
            return;
        if (sysDictionaryResult is not JsonObject jsonObject)
        {
            _logger.LogInvalidResultElementType(requestUri, sysDictionaryResult, index);
            return;
        }
        if (!jsonObject.TryGetFieldAsNonEmpty(JSON_KEY_ELEMENT, out string? name))
            _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_ELEMENT, index, jsonObject);
        else if (!jsonObject.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_SYS_ID, index, jsonObject);
        else
        {
            GlideType? type = await GetGlideTypeAsync(jsonObject.GetFieldAsNonEmpty(JSON_KEY_INTERNAL_TYPE, TYPE_NAME_STRING, out string? type_display_value), type_display_value, cancellationToken);
            if (type is null)
                return;
            SysPackage? package;
            if (jsonObject.TryGetPropertyValue(JSON_KEY_SYS_PACKAGE, out JsonNode? node) && node is JsonObject pkg)
            {
                package = await DeserializePackageAsync(pkg, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;
            }
            else
                package = null;
            SysScope? scope;
            if (jsonObject.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? id))
            {
                scope = await GetScopeByIDAsync(id, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;
            }
            else
                scope = null;
            ElementInfo elementInfo = new()
            {
                SysID = sys_id,
                Name = name,
                IsActive = jsonObject.GetFieldAsBoolean(JSON_KEY_ACTIVE),
                IsArray = jsonObject.GetFieldAsBoolean(JSON_KEY_ACTIVE),
                MaxLength = jsonObject.GetFieldAsInt(JSON_KEY_MAX_LENGTH),
                SizeClass = jsonObject.GetFieldAsInt(JSON_KEY_SIZECLASS),
                Comments = jsonObject.GetFieldAsNonEmptyOrNull(JSON_KEY_COMMENTS),
                DefaultValue = jsonObject.GetFieldAsNonEmptyOrNull(JSON_KEY_DEFAULT_VALUE),
                IsDisplay = jsonObject.GetFieldAsBoolean(JSON_KEY_DISPLAY),
                IsMandatory = jsonObject.GetFieldAsBoolean(JSON_KEY_MANDATORY),
                IsPrimary = jsonObject.GetFieldAsBoolean(JSON_KEY_PRIMARY),
                IsReadOnly = jsonObject.GetFieldAsBoolean(JSON_KEY_READ_ONLY),
                IsCalculated = jsonObject.GetFieldAsBoolean(JSON_KEY_VIRTUAL),
                IsUnique = jsonObject.GetFieldAsBoolean(JSON_KEY_UNIQUE),
                Type = type,
                LastUpdated = DateTime.Now,
                Table = table,
                Package = package,
                Scope = scope,
                Source = await GetSourceAsync(cancellationToken)
            };
            if (cancellationToken.IsCancellationRequested)
                return;
            await _dbContext.Elements.AddAsync(elementInfo, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
                table.Elements.Add(elementInfo);
        }
    }

    private async Task<GlideType?> GetGlideTypeAsync(string name, string? label, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;
        GlideType? glideType = await _dbContext.Types.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (glideType is not null)
            return glideType;
        Uri requestUri = _remoteUri.ToTableApiUri(TABLE_NAME_SYS_GLIDE_OBJECT, JSON_KEY_NAME, name);
        string? responseBody = await _handler.GetJsonResponseAsync(requestUri, _logger, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody ?? "");
            return null;
        }
        JsonObject responseObj;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                _logger.LogInvalidResponseType(requestUri, doc.RootElement.ValueKind, responseBody);
                return null;
            }
            responseObj = JsonObject.Create(doc.RootElement) ?? throw new InvalidOperationException("Could not parse as JSON object.");
        }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        if (!responseObj.TryGetPropertyValue(JSON_KEY_RESULT, out JsonNode? jsonNode))
            _logger.LogResponseResultPropertyNotFound(requestUri, responseBody);
        else if (jsonNode is null)
            _logger.LogNoResultsFromQuery(requestUri, responseBody);
        else
        {
            if (jsonNode is not JsonObject resultObj)
            {
                if (jsonNode is not JsonArray arr)
                {
                    _logger.LogInvalidResponseType(requestUri, jsonNode.GetType(), responseBody);
                    return null;
                }
                int length = arr.Count;
                if (length == 0)
                {
                    _logger.LogNoResultsFromQuery(requestUri, responseBody);
                    return null;
                }
                if (length> 1)
                    _logger.LogMultipleResponseItems(requestUri, length - 1, responseBody);
                
                if ((jsonNode = arr[0]) is not JsonObject)
                {
                    _logger.LogInvalidResultElementType(requestUri, jsonNode, 0);
                    return null;
                }
                resultObj = (JsonObject)jsonNode;
            }
            SysPackage? package;
            if (resultObj.TryGetPropertyValue(JSON_KEY_SYS_PACKAGE, out JsonNode? node) && node is JsonObject pkg)
            {
                package = await DeserializePackageAsync(pkg, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return null;
            }
            else
                package = null;
            SysScope? scope;
            if (resultObj.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? id))
            {
                scope = await GetScopeByIDAsync(id, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return null;
            }
            else
                scope = null;
            glideType = new()
            {
                Name = resultObj.GetFieldAsNonEmpty(JSON_KEY_NAME, name),
                SysID = resultObj.GetFieldAsNonEmpty(JSON_KEY_SYS_ID),
                Label = resultObj.GetFieldAsNonEmpty(JSON_KEY_LABEL, string.IsNullOrWhiteSpace(label) ? name : label),
                ScalarType = responseObj.GetFieldAsNonEmptyOrNull(JSON_KEY_SCALAR_TYPE),
                ScalarLength = responseObj.GetFieldAsIntOrNull(JSON_KEY_SCALAR_LENGTH),
                ClassName = responseObj.GetFieldAsNonEmptyOrNull(JSON_KEY_CLASS_NAME),
                UseOriginalValue = responseObj.GetFieldAsBoolean(JSON_KEY_USE_ORIGINAL_VALUE),
                IsVisible = responseObj.GetFieldAsBoolean(JSON_KEY_VISIBLE),
                Scope = scope,
                Package = package,
                Source = await GetSourceAsync(cancellationToken),
                LastUpdated = DateTime.Now
            };
        }
        if (glideType is null)
        {
            if (name == TYPE_NAME_STRING)
                return null;
            glideType = new()
            {
                Name = name,
                Label = string.IsNullOrWhiteSpace(label) ? name : label,
                ScalarType = TYPE_NAME_STRING,
                Source = await GetSourceAsync(cancellationToken),
                LastUpdated = DateTime.Now
            };
        }
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.Types.AddAsync(glideType, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cancellationToken.IsCancellationRequested ? null : glideType;
    }

    private async Task<SysPackage?> DeserializePackageAsync(JsonObject packageFieldElement, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested || !packageFieldElement.TryGetPropertyAsNonEmpty(JSON_KEY_DISPLAY_VALUE, out string? pkgName))
            return null;
        SysPackage? package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Name == pkgName, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (package is not null)
            return package;
        package = new()
        {
            Name = pkgName,
            Value = packageFieldElement.GetPropertyAsNonEmpty(JSON_KEY_VALUE),
            Source = await GetSourceAsync(cancellationToken),
            LastUpdated = DateTime.Now
        };
        await _dbContext.Packages.AddAsync(package, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cancellationToken.IsCancellationRequested ? null : package;
    }

    private async Task<TableInfo?> DeserializeTableAsync(JsonObject? sysDbObjectResult, Uri requestUri, bool checkDb, CancellationToken cancellationToken)
    {
        if (sysDbObjectResult is null || cancellationToken.IsCancellationRequested)
            return null;
        if (!sysDbObjectResult.TryGetFieldAsNonEmpty(JSON_KEY_NAME, out string? name))
        {
            _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_ELEMENT, sysDbObjectResult);
            return null;
        }
        if (!sysDbObjectResult.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
        {
            _logger.LogExpectedPropertyNotFoundError(requestUri, JSON_KEY_SYS_ID, sysDbObjectResult);
            return null;
        }
        TableInfo? tableInfo;
        if (checkDb && (tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken)) is not null)
        {
            if (!_tableMap.ContainsKey(sys_id))
                _tableMap.Add(sys_id, tableInfo.Name);
            return cancellationToken.IsCancellationRequested ? null : tableInfo;
        }
        if (cancellationToken.IsCancellationRequested)
            return null;
        SysPackage? package;
        if (sysDbObjectResult.TryGetPropertyValue(JSON_KEY_SYS_PACKAGE, out JsonNode? node) && node is JsonObject pkg)
        {
            package = await DeserializePackageAsync(pkg, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return null;
        }
        else
            package = null;
        SysScope? scope;
        if (sysDbObjectResult.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? id))
        {
            scope = await GetScopeByIDAsync(id, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return null;
        }
        else
            scope = null;
        tableInfo = new()
        {
            SysID = sys_id,
            Name = name,
            IsExtendable = sysDbObjectResult.GetFieldAsBoolean(JSON_KEY_IS_EXTENDABLE),
            Label = sysDbObjectResult.GetFieldAsNonEmpty(JSON_KEY_LABEL, name),
            NumberPrefix = (sysDbObjectResult.TryGetPropertyValue(JSON_KEY_NUMBER_REF, out node) && node is JsonObject obj) ? obj.CoercePropertyAsNonEmptyOrNull(JSON_KEY_DISPLAY_VALUE) : null,
            Scope = scope,
            Package = package,
            Source = await GetSourceAsync(cancellationToken),
            LastUpdated = DateTime.Now
        };
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.Tables.AddAsync(tableInfo, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        await _dbContext.SaveChangesAsync(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (!_tableMap.ContainsKey(sys_id))
            _tableMap.Add(sys_id, tableInfo.Name);
        TableInfo? superClass;
        if (sysDbObjectResult.TryGetFieldAsNonEmpty(JSON_KEY_SUPER_CLASS, out id) && (superClass = await GetTableByIDAsync(id, cancellationToken)) is not null)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;
            tableInfo.SuperClass = superClass;
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return null;
        }
        else
            superClass = null;
        requestUri = _remoteUri.ToTableApiUri(TABLE_NAME_SYS_DICTIONARY, JSON_KEY_NAME, name);
        using HttpClient httpClient = new(_handler!);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception)
        {
            _logger.LogHttpRequestFailed(response.RequestMessage!.RequestUri!, exception);
            return null;
        }
        string responseBody;
        try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
        catch (Exception exception)
        {
            _logger.LogGetResponseContentFailed(response.RequestMessage!.RequestUri!, exception);
            return null;
        }
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody);
            return null;
        }
        JsonObject responseObj;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                _logger.LogInvalidResponseType(requestUri, doc.RootElement.ValueKind, responseBody);
                return null;
            }
            responseObj = JsonObject.Create(doc.RootElement) ?? throw new InvalidOperationException("Could not parse as JSON object.");
        }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        if (!responseObj.TryGetPropertyValue(JSON_KEY_RESULT, out JsonNode? jsonNode) || jsonNode is null)
            _logger.LogResponseResultPropertyNotFound(requestUri, responseBody);
        else if (jsonNode is not JsonArray arr)
            _logger.LogInvalidResponseType(requestUri, jsonNode.GetType(), responseBody);
        else
        {
            int length = arr.Count;
                if (length < 1)
                    return tableInfo;
            for (int index = 0; index < length; index++)
            {
                await DeserializeElementAsync(arr[index], index, tableInfo, requestUri, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return null;
            }
            if (superClass is not null)
            {
                ElementInfo[] toRemove = tableInfo.Elements.Where(e => superClass.Elements.Any(s => e.OptionsEqualTo(s))).ToArray();
                if (toRemove.Length > 0)
                {
                    foreach (ElementInfo e in toRemove)
                    {
                        tableInfo.Elements.Remove(e);
                        _dbContext.Elements.Remove(e);
                    }
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        return null;
                }
            }
        }
        return tableInfo;
    }
    
    private async Task<TableInfo?> GetTableFromUri(Uri requestUri, bool checkDb, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;
        using HttpClient httpClient = new(_handler!);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception)
        {
            _logger.LogHttpRequestFailed(response.RequestMessage!.RequestUri!, exception);
            return null;
        }
        string responseBody;
        try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
        catch (Exception exception)
        {
            _logger.LogGetResponseContentFailed(response.RequestMessage!.RequestUri!, exception);
            return null;
        }
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody);
            return null;
        }
        JsonObject responseObj;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                _logger.LogInvalidResponseType(requestUri, doc.RootElement.ValueKind, responseBody);
                return null;
            }
            responseObj = JsonObject.Create(doc.RootElement) ?? throw new InvalidOperationException("Could not parse as JSON object.");
        }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        if (!responseObj.TryGetPropertyValue(JSON_KEY_RESULT, out JsonNode? jsonNode))
        {
            _logger.LogResponseResultPropertyNotFound(requestUri, responseBody);
            return null;
        }
        if (jsonNode is null)
        {
            _logger.LogNoResultsFromQuery(requestUri, responseBody);
            return null;
        }
        if (jsonNode is not JsonObject resultObj)
        {
            if (jsonNode is not JsonArray arr)
            {
                _logger.LogInvalidResponseType(requestUri, jsonNode.GetType(), responseBody);
                return null;
            }
            int length = arr.Count;
            if (length == 0)
            {
                _logger.LogNoResultsFromQuery(requestUri, responseBody);
                return null;
            }
            if (length> 1)
                _logger.LogMultipleResponseItems(requestUri, length - 1, responseBody);
            
            if ((jsonNode = arr[0]) is not JsonObject)
            {
                _logger.LogInvalidResultElementType(requestUri, jsonNode, 0);
                return null;
            }
            resultObj = (JsonObject)jsonNode;
        }
        
        TableInfo? tableInfo = await DeserializeTableAsync(resultObj, requestUri, checkDb, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (tableInfo is not null)
            return tableInfo;
        _logger.LogInvalidHttpResponse(requestUri, responseBody ?? "");
        return null;
    }
    
    public async Task<TableInfo?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (_handler is null || cancellationToken.IsCancellationRequested)
            return null;
        name = name.ToLower();
        var tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (tableInfo is not null)
            return tableInfo;
        return await GetTableFromUri(_remoteUri.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name), false, cancellationToken);
    }

    private async Task<SysScope?> GetScopeByIDAsync(string id, CancellationToken cancellationToken)
    {
        if (_handler is null || cancellationToken.IsCancellationRequested)
            return null;
        SysScope? scope;
        if (_scopeMap.TryGetValue(id, out string? name) && (scope = await _dbContext.Scopes.FirstOrDefaultAsync(t => t.Name == name, cancellationToken)) is not null)
            return cancellationToken.IsCancellationRequested ? null : scope;
        id = id.ToLower();
        if ((scope = await _dbContext.Scopes.FirstOrDefaultAsync(t => t.SysID == id, cancellationToken)) is not null)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;
            if (!_scopeMap.ContainsKey(id))
                _scopeMap.Add(id, scope.Name);
            return scope;
        }
        Uri requestUri = _remoteUri.ToTableApiUri(TABLE_NAME_SYS_SCOPE, id);
        string? responseBody = await _handler.GetJsonResponseAsync(requestUri, _logger, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody ?? "");
            return null;
        }
        JsonObject responseObj;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                _logger.LogInvalidResponseType(requestUri, doc.RootElement.ValueKind, responseBody);
                return null;
            }
            responseObj = JsonObject.Create(doc.RootElement) ?? throw new InvalidOperationException("Could not parse as JSON object.");
        }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        if (!responseObj.TryGetPropertyValue(JSON_KEY_RESULT, out JsonNode? jsonNode))
            _logger.LogResponseResultPropertyNotFound(requestUri, responseBody);
        else if (jsonNode is null)
            _logger.LogNoResultsFromQuery(requestUri, responseBody);
        else if (jsonNode is not JsonObject resultObj)
            _logger.LogInvalidResponseType(requestUri, jsonNode.GetType(), responseBody);
        else
        {
            if ((scope = await DeserializeScopeAsync(resultObj, requestUri, cancellationToken)) is not null)
                return cancellationToken.IsCancellationRequested ? null : scope;
            if (cancellationToken.IsCancellationRequested)
                return null;
            _logger.LogInvalidHttpResponse(requestUri, responseBody);
        }
        return null;
    }
    
    private async Task<TableInfo?> GetTableByIDAsync(string id, CancellationToken cancellationToken)
    {
        if (_handler is null || cancellationToken.IsCancellationRequested)
            return null;
        TableInfo? tableInfo;
        if (_tableMap.TryGetValue(id, out string? name) && (tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken)) is not null)
            return cancellationToken.IsCancellationRequested ? null : tableInfo;
        id = id.ToLower();
        if ((tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.SysID == id, cancellationToken)) is not null)
        {
            if (!_tableMap.ContainsKey(id))
                _tableMap.Add(id, tableInfo.Name);
            return cancellationToken.IsCancellationRequested ? null : tableInfo;
        }
        if (cancellationToken.IsCancellationRequested)
            return null;
        return await GetTableFromUri(_remoteUri.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, id), true, cancellationToken);
    }

    public RemoteLoaderService(ILogger<RemoteLoaderService> logger, TypingsDbContext dbContext, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = appSettings;
        var remoteUri = _appSettings.Value.RemoteURL;
        if (string.IsNullOrWhiteSpace(remoteUri))
        {
            if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                _logger.LogCriticalRemoteInstanceUriNotProvided();
            _remoteUri = new Uri(string.Empty);
            return;
        }
        NetworkCredential credentials;
        if (Uri.TryCreate(remoteUri, UriKind.Absolute, out Uri? uri))
        {
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                _remoteUri = uri;
                if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                    _logger.LogCriticalInvalidRemoteInstanceUri();
                return;
            }
            _remoteUri = new UriBuilder(uri) { Fragment = null, Query = null, Path = "/" }.Uri;
            if (_appSettings.Value.UserName is null)
            {
                if (_appSettings.Value.ClientId is null)
                {
                    if (_appSettings.Value.Password is null)
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalUserNameNotProvided();
                        return;
                    }
                    Console.Write("User Name: ");
                    string? userName = Console.ReadLine();
                    if (string.IsNullOrEmpty(userName))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalUserNameNotProvided();
                        return;
                    }
                    string? password = Console.ReadLine();
                    if (string.IsNullOrEmpty(password))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalPasswordNotProvided();
                        return;
                    }
                    credentials = new(userName, password);
                }
                else
                {
                    if (_appSettings.Value.ClientSecret is null)
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalPasswordNotProvided();
                        return;
                    }
                    credentials = new(_appSettings.Value.ClientId, _appSettings.Value.ClientSecret);
                }
            }
            else
            {
                string? password = _appSettings.Value.Password;
                if (password is null && string.IsNullOrEmpty(password = Console.ReadLine()))
                {
                    if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                        _logger.LogCriticalPasswordNotProvided();
                    return;
                }
                credentials = new(_appSettings.Value.UserName, password);
            }
            if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                _handler = new() { Credentials = credentials };
        }
        else
        {
            _remoteUri = Uri.TryCreate(remoteUri, UriKind.Absolute, out uri) ? uri : new Uri(Uri.EscapeDataString(remoteUri), UriKind.Relative);
            if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                _logger.LogCriticalInvalidRemoteInstanceUri();
        }
    }

}
