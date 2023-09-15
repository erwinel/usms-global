using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SnTsTypeGenerator;

public sealed class RemoteLoaderService
{
    private readonly ILogger<RemoteLoaderService> _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<CommandSettings> _commandLine;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly HttpClientHandler? _handler;
    private readonly string _remoteUri;

    public RemoteLoaderService(ILogger<RemoteLoaderService> logger, TypingsDbContext dbContext, IOptions<CommandSettings> commandLine, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _commandLine = commandLine;
        _appSettings = appSettings;
        var remoteUri = _commandLine.Value.RemoteURI;
        if (string.IsNullOrWhiteSpace(remoteUri) && string.IsNullOrWhiteSpace(remoteUri = _appSettings.Value.RemoteURI))
        {
            _logger.LogCriticalRemoteInstanceUriNotProvidedError();
            _remoteUri = string.Empty;
        }
        if (Uri.TryCreate(remoteUri, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            _remoteUri = new UriBuilder(uri) { Fragment = null, Query = null, Path = "/" }.Uri.AbsoluteUri;
            if (string.IsNullOrWhiteSpace(_commandLine.Value.UserName))
                _logger.LogCriticalUserNameNotProvidedError();
            else if (string.IsNullOrEmpty(_commandLine.Value.Password))
                _logger.LogCriticalPasswordNotProvidedError();
            else
                _handler = new() { Credentials = new NetworkCredential(_commandLine.Value.UserName, _commandLine.Value.Password) };
        }
        else
        {
            _remoteUri = remoteUri!;
            _logger.LogCriticalInvalidRemoteInstanceUriError();
        }
    }

    private readonly Dictionary<string, string> _numberRefMap = new(StringComparer.InvariantCultureIgnoreCase);

    public async Task<TableInfo?> GetTableByName(string name, bool loadChildEntities, CancellationToken cancellationToken)
    {
        if (_handler is null)
            return null;
        var tableInfo = loadChildEntities ? await _dbContext.Tables.Include(t => t.Scope).Include(t => t.SuperClass).Include(t => t.Elements).FirstOrDefaultAsync(t => t.Name == name, cancellationToken) :
            await _dbContext.Tables.FindAsync(name, cancellationToken);
        if (tableInfo is not null)
            return tableInfo;
        using HttpClient httpClient = new(_handler);
        string q = Uri.EscapeDataString($"name={name}");
        Uri requestUri = new UriBuilder(_remoteUri)
        {
            Path = "/api/now/table/sys_db_object",
            Query = $"sysparm_query={q}",
            Fragment = null
        }.Uri;
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add("Accept", "application/json");
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        (JsonDocument? doc, string? responseBody) = await response.GetJsonDocumentAsync(_logger, cancellationToken);
        if (doc is null)
            return null;
        using (doc)
        {
            if (doc.RootElement.TryGetNonEmptyString(TableInfo.COLNAME_SysID, out string? sys_id) && doc.RootElement.TryGetNonEmptyString(TableInfo.COLNAME_Name, out string? value))
            {
                sys_id = sys_id.ToLower();;
                if ((tableInfo = _dbContext.Tables.Include(t => t.Scope).Include(t => t.SuperClass).FirstOrDefault(t => t.SysID == sys_id)) is null)
                {
                    if (value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableInfo = new()
                        {
                            SysID = sys_id,
                            Name = value,
                            IsExtendable = doc.RootElement.GetBoolean(TableInfo.COLNAME_IsExtendable),
                            Label = doc.RootElement.GetNonEmptyString(TableInfo.COLNAME_Label, value)
                        };
                        if (doc.RootElement.TryGetProperty(TableInfo.COLNAME_NumberPrefix, out JsonElement element) && element.ValueKind == JsonValueKind.Object && element.TryGetStringValue("value", out value))
                        {
                            Monitor.Enter(_numberRefMap);
                            try
                            {
                                if (_numberRefMap.TryGetValue(value, out string? link))
                                    tableInfo.NumberPrefix = link;
                                else if (element.TryGetNonEmptyString("link", out link))
                                {
                                    using HttpClient client = new(_handler);
                                    Uri rUri = new UriBuilder(_remoteUri)
                                    {
                                        Path = "/api/now/table/sys_db_object",
                                        Query = $"sysparm_query={q}",
                                        Fragment = null
                                    }.Uri;
                                    HttpRequestMessage msg = new(HttpMethod.Get, rUri);
                                    msg.Headers.Add("Accept", "application/json");
                                    using HttpResponseMessage res = await client.SendAsync(msg, cancellationToken);
                                    (JsonDocument? d, _) = await res.GetJsonDocumentAsync(_logger, cancellationToken);
                                    if (d is not null && d.RootElement.ValueKind == JsonValueKind.Object && d.RootElement.TryGetNonEmptyString("prefix", out link))
                                    {
                                        _numberRefMap.Add(value, link);
                                        tableInfo.NumberPrefix = link;
                                    }
                                }
                            }
                            finally { Monitor.Exit(_numberRefMap); }
                        }
                        if (doc.RootElement.TryGetNonEmptyString(TableInfo.COLNAME_Scope, out value))
                            tableInfo.Scope = await GetScopeByValue(value, cancellationToken);
                        _dbContext.Tables.Add(tableInfo);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        // TODO: Add Elements
                        if (doc.RootElement.TryGetProperty(TableInfo.COLNAME_SuperClass, out element) && element.TryGetNonEmptyString("value", out value) && (tableInfo.SuperClass = await GetTableByName(value, false, cancellationToken)) is not null)
                            await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
                // else
                    // TODO: What do we do now?
                return tableInfo;
            }
            _logger.LogInvalidHttpResponseError(requestUri, responseBody ?? "");
        }
        return null;
    }

    public async Task<SysScope?> GetScopeByValue(string value, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<GlideType?> GetTypeByName(string name, bool includeChildEntities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<CommandSettings> _commandLine;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly RemoteLoaderService _remoteLoader;

    // public MainWorkerService(ILogger<MainWorkerService> logger, IHostApplicationLifetime appLifetime, IOptions<CommandSettings> commandLine, IOptions<AppSettings> appSettings)
    public MainWorkerService(ILogger<MainWorkerService> logger, TypingsDbContext dbContext, IOptions<CommandSettings> commandLine, IOptions<AppSettings> appSettings, RemoteLoaderService remoteLoader)
    {
        _logger = logger;
        _dbContext = dbContext;
        _commandLine = commandLine;
        _appSettings = appSettings;
        _remoteLoader = remoteLoader;
        // appLifetime.ApplicationStarted.Register(OnStarted);
        // appLifetime.ApplicationStopping.Register(OnStopping);
        // appLifetime.ApplicationStopped.Register(OnStopped);

    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tableNames = _commandLine.Value.Table?.Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>();
        if (tableNames.Any() || (tableNames = (_appSettings.Value.DbFile?.Split(',').Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>())).Any())
        {
            Collection<TableInfo> toRender = new();
            foreach (string name in tableNames.Select(n => n.Trim().ToLower()).Distinct())
            {
                if (stoppingToken.IsCancellationRequested)
                    break;
                var tableInfo = await _dbContext.Tables.FindAsync(name, stoppingToken);
                if (tableInfo is null && (tableInfo = await _remoteLoader.GetTableByName(name, true, stoppingToken)) is not null)
                    toRender.Add(tableInfo);
            }
        }
        else
            _logger.LogNoTableNamesSpecifiedWarning();
    }

    // public Task StopAsync(CancellationToken cancellationToken)
    // {
    //     _logger.LogInformation("StopAsync has been called.");

    //     return Task.CompletedTask;
    // }

    // private void OnStarted()
    // {
    //     _logger.LogInformation("2. OnStarted has been called.");
    // }

    // private void OnStopping()
    // {
    //     _logger.LogInformation("3. OnStopping has been called.");
    // }

    // private void OnStopped()
    // {
    //     _logger.LogInformation("5. OnStopped has been called.");
    // }
}