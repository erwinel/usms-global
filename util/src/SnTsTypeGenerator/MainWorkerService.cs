using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SnTsTypeGenerator;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly DataLoaderService _dataLoader;
    private readonly RenderingService _renderer;

    // public MainWorkerService(ILogger<MainWorkerService> logger, IHostApplicationLifetime appLifetime, IOptions<AppSettings> appSettings)
    public MainWorkerService(ILogger<MainWorkerService> logger, TypingsDbContext dbContext, IOptions<AppSettings> appSettings, DataLoaderService dataLoader, RenderingService renderer)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = appSettings;
        _dataLoader = dataLoader;
        _renderer = renderer;
        // appLifetime.ApplicationStarted.Register(OnStarted);
        // appLifetime.ApplicationStopping.Register(OnStopping);
        // appLifetime.ApplicationStopped.Register(OnStopped);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        AppSettings settings = _appSettings.Value;
        if (settings.Help.HasValue && settings.Help.Value)
        {
            AppSettings.WriteHelpToConsole();
            return;
        }

        if (!(_dataLoader.InitSuccessful && _renderer.InitSuccessful))
            return;

        var tableNames = settings.Table?.Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>();
        if (tableNames.Any() || (tableNames = settings.DbFile?.Split(',').Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>()).Any())
        {
            Collection<TableInfo> toRender = new();
            foreach (string name in tableNames.Select(n => n.Trim().ToLower()).Distinct())
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                try
                {
                    var tableInfo = await _dbContext.Tables.Include(t => t.SuperClass).Include(t => t.Scope).FirstOrDefaultAsync(t => t.Name == name, stoppingToken);
                    if (tableInfo is not null || (tableInfo = await _dataLoader.GetTableByNameAsync(name, stoppingToken)) is not null)
                        toRender.Add(tableInfo);
                }
                catch (Exception exception)
                {
                    if (stoppingToken.IsCancellationRequested)
                        return;
                    if (exception is ILogTrackable logTrackable)
                    {
                        if (!logTrackable.IsLogged)
                            logTrackable.Log(_logger);
                    }
                    else
                        _logger.LogUnexpecteException(exception);
                    return;
                }
            }
            if (!stoppingToken.IsCancellationRequested)
                await _renderer.RenderAsync(toRender, stoppingToken);
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