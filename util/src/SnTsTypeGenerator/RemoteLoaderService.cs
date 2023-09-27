using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    private Dictionary<string, string> _scopeMap = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, string> _tableMap = new(StringComparer.InvariantCultureIgnoreCase);

    internal bool HasHandler => _handler is not null;

    private async Task<SourceInfo> GetSourceAsync(CancellationToken cancellationToken)
    {
        if (_source is null)
        {
            string fqdn = _remoteUri.Host;
            if ((_source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.FQDN == fqdn, cancellationToken)) is null)
            {
                _source = new()
                {
                    FQDN = fqdn,
                    Label = fqdn,
                    IsPersonalDev = false,
                    LastAccessed = DateTime.Now
                };
                await _dbContext.Sources.AddAsync(_source, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        return _source;
    }


    private async Task<SysScope?> ScopeFromElementAsync(JsonElement element, CancellationToken cancellationToken)
    {
        if (!(element.TryGetPropertyAsNonEmptyString(JSON_KEY_SYS_ID, JSON_KEY_VALUE, out string? sys_id) && element.TryGetPropertyAsNonEmptyString(JSON_KEY_SCOPE, JSON_KEY_VALUE, out string? value)))
            return null;
        value = value.ToLower();
        var scope = await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken);
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
            Name = element.GetPropertyAsNonEmptyString(JSON_KEY_NAME, JSON_KEY_VALUE, value),
            ShortDescription = element.GetPropertyAsString(JSON_KEY_SHORT_DESCRIPTION),
            LastUpdated = DateTime.Now,
            Source = await GetSourceAsync(cancellationToken)
        };
        await _dbContext.Scopes.AddAsync(scope, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        if (!_scopeMap.ContainsKey(sys_id))
            _scopeMap.Add(sys_id, scope.Value);
        return scope;
    }

    private async Task<TableInfo?> TableFromElementAsync(JsonElement element, bool checkDb, CancellationToken cancellationToken)
    {
        if (!(element.TryGetPropertyAsNonEmptyString(JSON_KEY_SYS_ID, JSON_KEY_VALUE, out string? sys_id) && element.TryGetPropertyAsNonEmptyString(JSON_KEY_NAME, JSON_KEY_VALUE, out string? name)))
            return null;
        name = name.ToLower();
        TableInfo? tableInfo;
        if (checkDb && (tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken)) is not null)
        {
            if (!_tableMap.ContainsKey(sys_id))
                _tableMap.Add(sys_id, tableInfo.Name);
            return tableInfo;
        }
        SysPackage? package;
        if (element.TryGetPropertyAsNonEmptyString(JSON_KEY_SYS_PACKAGE, JSON_KEY_DISPLAY_VALUE, out string? pkgName) &&
            (package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Name == pkgName, cancellationToken)) is null)
        {
            package = new()
            {
                Name = pkgName,
                Value = element.GetPropertyAsString(JSON_KEY_VALUE),
                Source = await GetSourceAsync(cancellationToken),
                LastUpdated = DateTime.Now
            };
            await _dbContext.Packages.AddAsync(package, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
            package = null;
        tableInfo = new()
        {
            SysID = sys_id,
            Name = name,
            IsExtendable = element.GetPropertyAsBoolean(JSON_KEY_IS_EXTENDABLE, JSON_KEY_VALUE),
            Label = element.GetPropertyAsString(JSON_KEY_LABEL, JSON_KEY_VALUE, name),
            NumberPrefix = element.TryGetPropertyAsNonEmptyString(JSON_KEY_NUMBER_REF, JSON_KEY_DISPLAY_VALUE, out string? f) ? f : null,
            Scope = await _handler!.GetLinkedObjectAsync(element, JSON_KEY_SCOPE,
                async n => await _dbContext.Scopes.FindAsync(n), e => Task.FromResult(SysScope.FromElement(e)), _logger, cancellationToken),
            Package = package,
            Source = _source,
            LastUpdated = DateTime.Now
        };
        _dbContext.Tables.Add(tableInfo);
        await _dbContext.SaveChangesAsync(cancellationToken);
        if (!_tableMap.ContainsKey(sys_id))
            _tableMap.Add(sys_id, tableInfo.Name);
        if (element.TryGetPropertyAsNonEmptyString(JSON_KEY_SUPER_CLASS, JSON_KEY_VALUE, out sys_id) && (tableInfo.SuperClass = await GetTableByID(sys_id, cancellationToken)) is not null)
            await _dbContext.SaveChangesAsync(cancellationToken);
        var requestUri = _remoteUri.ToTableApiUri(TABLE_NAME_SYS_DICTIONARY, JSON_KEY_NAME, name);
        using HttpClient httpClient = new(_handler!);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
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
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody);
            return null;
        }
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        
        using (doc)
        {
            // if (doc.RootElement.ValueKind == JsonValueKind.Object)
            // {
            //     TableInfo? tableInfo = await TableFromElementAsync(doc.RootElement, checkDb, cancellationToken);
            //     if (tableInfo is not null)
            //         return tableInfo;
            // }
            // _logger.LogInvalidHttpResponse(requestUri, responseBody ?? "");
        }
        return null;
        return tableInfo;
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

    private async Task<TableInfo?> GetTableFromUri(Uri requestUri, bool checkDb, CancellationToken cancellationToken)
    {
        using HttpClient httpClient = new(_handler!);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
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
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody);
            return null;
        }
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        
        using (doc)
        {
            // BUG: root element is "result", and may either be an array or object
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                TableInfo? tableInfo = await TableFromElementAsync(doc.RootElement, checkDb, cancellationToken);
                if (tableInfo is not null)
                    return tableInfo;
            }
            _logger.LogInvalidHttpResponse(requestUri, responseBody ?? "");
        }
        return null;
    }
    
    public async Task<TableInfo?> GetTableByName(string name, CancellationToken cancellationToken)
    {
        if (_handler is null || cancellationToken.IsCancellationRequested)
            return null;
        name = name.ToLower();
        var tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (tableInfo is not null)
            return tableInfo;
        return await GetTableFromUri(_remoteUri.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name), false, cancellationToken);
    }
    
    private async Task<SysScope?> GetScopeByID(string id, CancellationToken cancellationToken)
    {
        if (_handler is null || cancellationToken.IsCancellationRequested)
            return null;
        SysScope? scope;
        if (_scopeMap.TryGetValue(id, out string? name) && (scope = await _dbContext.Scopes.FirstOrDefaultAsync(t => t.Name == name, cancellationToken)) is not null)
            return scope;
        id = id.ToLower();
        if ((scope = await _dbContext.Scopes.FirstOrDefaultAsync(t => t.SysID == id, cancellationToken)) is not null)
        {
            if (!_scopeMap.ContainsKey(id))
                _scopeMap.Add(id, scope.Name);
            return scope;
        }
        using HttpClient httpClient = new(_handler!);
        Uri requestUri = _remoteUri.ToTableApiUri(TABLE_NAME_SYS_SCOPE, id);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
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
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponse(requestUri, responseBody);
            return null;
        }
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsed(requestUri, responseBody, exception);
            return null;
        }
        
        using (doc)
        {
            // BUG: root element is "result"
            if (doc.RootElement.ValueKind == JsonValueKind.Object || (scope = await ScopeFromElementAsync(doc.RootElement, cancellationToken)) is null)
                _logger.LogInvalidHttpResponse(requestUri, responseBody ?? "");
        }
        return scope;
    }
    
    private async Task<TableInfo?> GetTableByID(string id, CancellationToken cancellationToken)
    {
        if (_handler is null || cancellationToken.IsCancellationRequested)
            return null;
        TableInfo? tableInfo;
        if (_tableMap.TryGetValue(id, out string? name) && (tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken)) is not null)
            return tableInfo;
        id = id.ToLower();
        if ((tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.SysID == id, cancellationToken)) is not null)
        {
            if (!_tableMap.ContainsKey(id))
                _tableMap.Add(id, tableInfo.Name);
            return tableInfo;
        }
        return await GetTableFromUri(_remoteUri.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, id), true, cancellationToken);
    }
}
