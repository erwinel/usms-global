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

    private readonly Dictionary<string, string> _numberRefMap = new(StringComparer.InvariantCultureIgnoreCase);

    internal bool HasHandler => _handler is not null;

    private async Task<TableInfo?> TableFromElementAsync(JsonElement element, bool checkDb, CancellationToken cancellationToken)
    {
        // BUG: Elements have schema { display_value: string, value: string, link?: string; } when sysparm_display_value=all
        if (!(element.TryGetNonEmptyString(JSON_KEY_SYS_ID, out string? sys_id) && element.TryGetNonEmptyString(JSON_KEY_NAME, out string? value)))
            return null;
        value = value.ToLower();
        TableInfo? tableInfo;
        if (checkDb && (tableInfo = _dbContext.Tables.Include(t => t.Scope).Include(t => t.SuperClass).FirstOrDefault(t => t.Name == value)) is not null)
            return tableInfo;
        tableInfo = new()
        {
            SysID = sys_id,
            Name = value,
            IsExtendable = element.GetBoolean(JSON_KEY_IS_EXTENDABLE),
            Label = element.GetNonEmptyString(JSON_KEY_LABEL, value),
            NumberPrefix = await _handler!.GetLinkedObjectAsync(element, JSON_KEY_NUMBER_REF,
                n => Task.FromResult(_numberRefMap.TryGetValue(n, out string? v) ? v : null), e => Task.FromResult(e.TryGetNonEmptyString(JSON_KEY_PREFIX, out string? p) ? p : null), _logger, cancellationToken),
            Scope = await _handler!.GetLinkedObjectAsync(element, JSON_KEY_SCOPE,
                async n => await _dbContext.Scopes.FindAsync(n), e => Task.FromResult(SysScope.FromElement(e)), _logger, cancellationToken)
        };
        _dbContext.Tables.Add(tableInfo);
        await _dbContext.SaveChangesAsync(cancellationToken);
        if ((tableInfo.SuperClass = await _handler!.GetLinkedObjectAsync(element, JSON_KEY_SUPER_CLASS, async n => await _dbContext.Tables.FindAsync(n),
                async e => await TableFromElementAsync(e, true, cancellationToken), _logger, cancellationToken)) is not null)
            await _dbContext.SaveChangesAsync(cancellationToken);
        
        // TODO: Get Elements
        
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
                _logger.LogCriticalRemoteInstanceUriNotProvidedError();
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
                    _logger.LogCriticalInvalidRemoteInstanceUriError();
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
                            _logger.LogCriticalUserNameNotProvidedError();
                        return;
                    }
                    Console.Write("User Name: ");
                    string? userName = Console.ReadLine();
                    if (string.IsNullOrEmpty(userName))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalUserNameNotProvidedError();
                        return;
                    }
                    string? password = Console.ReadLine();
                    if (string.IsNullOrEmpty(password))
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalPasswordNotProvidedError();
                        return;
                    }
                    credentials = new(userName, password);
                }
                else
                {
                    if (_appSettings.Value.ClientSecret is null)
                    {
                        if (!(appSettings.Value.Help.HasValue && appSettings.Value.Help.Value))
                            _logger.LogCriticalPasswordNotProvidedError();
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
                        _logger.LogCriticalPasswordNotProvidedError();
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
                _logger.LogCriticalInvalidRemoteInstanceUriError();
        }
    }

    public async Task<TableInfo?> GetTableByName(string name, CancellationToken cancellationToken)
    {
        if (_handler is null)
            return null;
        name = name.ToLower();
        var tableInfo = await _dbContext.Tables.Include(t => t.Scope).Include(t => t.SuperClass).Include(t => t.Elements).FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (tableInfo is not null)
            return tableInfo;
        using HttpClient httpClient = new(_handler);
        Uri requestUri = _remoteUri.ToTableApiUri(TABLE_NAME_SYS_DB_OBJECT, JSON_KEY_NAME, name);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception)
        {
            _logger.LogHttpRequestFailedError(response.RequestMessage!.RequestUri!, exception);
            return null;
        }
        string responseBody;
        try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
        catch (Exception exception)
        {
            _logger.LogGetResponseContentFailedError(response.RequestMessage!.RequestUri!, exception);
            return null;
        }
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            _logger.LogInvalidHttpResponseError(requestUri, responseBody);
            return null;
        }
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            _logger.LogJsonCouldNotBeParsedError(requestUri, responseBody, exception);
            return null;
        }
        using (doc)
        {
            if (doc.RootElement.ValueKind != JsonValueKind.Object || (tableInfo = await TableFromElementAsync(doc.RootElement, false, cancellationToken)) is null)
                _logger.LogInvalidHttpResponseError(requestUri, responseBody ?? "");
        }
        return tableInfo;
    }
}
