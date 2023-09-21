using System.Collections.ObjectModel;
using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SnTsTypeGenerator;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly RemoteLoaderService _remoteLoader;

    // public MainWorkerService(ILogger<MainWorkerService> logger, IHostApplicationLifetime appLifetime, IOptions<AppSettings> appSettings)
    public MainWorkerService(ILogger<MainWorkerService> logger, TypingsDbContext dbContext, IOptions<AppSettings> appSettings, RemoteLoaderService remoteLoader)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = appSettings;
        _remoteLoader = remoteLoader;
        // appLifetime.ApplicationStarted.Register(OnStarted);
        // appLifetime.ApplicationStopping.Register(OnStopping);
        // appLifetime.ApplicationStopped.Register(OnStopped);

    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        if (_appSettings.Value.Help.HasValue && _appSettings.Value.Help.Value)
        {
            AppSettings.WriteHelpToConsole();
            return;
        }
        
        if (_appSettings.Value.Scoped.HasValue && _appSettings.Value.Scoped.Value && _appSettings.Value.Global.HasValue && _appSettings.Value.Global.Value)
        {
            _logger.LogCriticalGlobalAndScopedSwitchesBothSetError();
            return;
        }

        var tableNames = _appSettings.Value.Table?.Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>();
        if (tableNames.Any() || (tableNames = _appSettings.Value.DbFile?.Split(',').Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>()).Any())
        {
            Collection<TableInfo> toRender = new();
            foreach (string name in tableNames.Select(n => n.Trim().ToLower()).Distinct())
            {
                if (stoppingToken.IsCancellationRequested)
                    break;
                var tableInfo = await _dbContext.Tables.FindAsync(name, stoppingToken);
                if (tableInfo is null && (tableInfo = await _remoteLoader.GetTableByName(name, stoppingToken)) is not null)
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