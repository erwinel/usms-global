using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SnTsTypeGenerator;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScope _scope;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ImmutableArray<string> _tableNames;

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (_tableNames.Length == 0 || stoppingToken.IsCancellationRequested)
                return;

            DataLoaderService _dataLoader = _scope.ServiceProvider.GetRequiredService<DataLoaderService>();
            RenderingService _renderer = _scope.ServiceProvider.GetRequiredService<RenderingService>();

            if (!(_dataLoader.InitSuccessful && _renderer.InitSuccessful))
                return;

            Collection<TableInfo> toRender = new();
            foreach (string name in _tableNames)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                try
                {
                    var tableInfo = await _dataLoader.GetTableByNameAsync(name, stoppingToken);
                    if (tableInfo is not null)
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
        catch (OperationCanceledException) { throw; }
        catch (Exception error)
        {
            _logger.LogUnexpectedServiceException<MainWorkerService>(error);
        }
        finally
        {
            if (!stoppingToken.IsCancellationRequested)
                _applicationLifetime.StopApplication();
        }
    }

    public MainWorkerService(ILogger<MainWorkerService> logger, IServiceProvider services, IOptions<AppSettings> appSettings, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _scope = services.CreateScope();
        _applicationLifetime = applicationLifetime;
        AppSettings _appSettings = appSettings.Value;
        var tableNames = _appSettings.Table?.Split(',').Where(t => !string.IsNullOrEmpty(t));
        if ((tableNames is not null && tableNames.Any()) || ((tableNames = _appSettings.Tables?.Where(t => !string.IsNullOrEmpty(t))) is not null))
        {
            _tableNames = tableNames.Select(n => n.Trim().ToLower()).Distinct().ToImmutableArray();
            return;
        }
        _logger.LogNoTableNamesSpecifiedWarning();
        _tableNames = ImmutableArray.Create<string>();
    }
}