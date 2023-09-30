using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly DataLoaderService _dataLoader;

    // public MainWorkerService(ILogger<MainWorkerService> logger, IHostApplicationLifetime appLifetime, IOptions<AppSettings> appSettings)
    public MainWorkerService(ILogger<MainWorkerService> logger, TypingsDbContext dbContext, IOptions<AppSettings> appSettings, DataLoaderService dataLoader)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = appSettings;
        _dataLoader = dataLoader;
        // appLifetime.ApplicationStarted.Register(OnStarted);
        // appLifetime.ApplicationStopping.Register(OnStopping);
        // appLifetime.ApplicationStopped.Register(OnStopped);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        if (_appSettings.Value.Help.HasValue && _appSettings.Value.Help.Value)
        {
            AppSettings.WriteHelpToConsole();
            return;
        }

        if (!_dataLoader.InitSuccessful)
            return;

        bool isScoped = _appSettings.Value.Scoped ?? false;
        if (isScoped && (_appSettings.Value.Global ?? false))
        {
            _logger.LogGlobalAndScopedSwitchesBothSet();
            return;
        }

        string outputFileName = _appSettings.Value.Output!;
        if (string.IsNullOrEmpty(outputFileName))
            outputFileName = AppSettings.DEFAULT_OUTPUT_FILENAME;
        bool forceOverwrite = _appSettings.Value.Force ?? false;
        try
        {
            FileInfo fileInfo = new(outputFileName);
            outputFileName = fileInfo.FullName;
            if (fileInfo.Exists)
            {
                if (!forceOverwrite)
                {
                    _logger.LogOutputFileAlreadyExists(fileInfo.FullName);
                    return;
                }
            }
            else if (!(fileInfo.Directory?.Exists ?? false))
            {
                _logger.LogOutputFileAccessError(fileInfo.FullName, "Parent subdirectory does not exist");
                return;
            }
        }
        catch (Exception exception)
        {
            if (stoppingToken.IsCancellationRequested)
                return;
            _logger.LogOutputFileAccessError(outputFileName, exception);
            return;
        }
        var tableNames = _appSettings.Value.Table?.Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>();
        if (tableNames.Any() || (tableNames = _appSettings.Value.DbFile?.Split(',').Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>()).Any())
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
            if (stoppingToken.IsCancellationRequested)
                return;
            StreamWriter streamWriter;
            try
            {
                streamWriter = new(outputFileName, new FileStreamOptions()
                {
                    Access = FileAccess.Write,
                    Mode = forceOverwrite ? FileMode.Create : FileMode.CreateNew
                });
            }
            catch (Exception error)
            {
                _logger.LogOutputFileAccessError(outputFileName, error);
                return;
            }
            try
            {
                using (streamWriter)
                {
                    using IndentedTextWriter writer = new(streamWriter, "    ");

                    var nsGrouped = toRender.GroupBy(t => t.GetNamespace()).OrderBy(g => g.Key).ToArray();
                    var gns = nsGrouped.FirstOrDefault(n => n.Key == AppSettings.DEFAULT_NAMESPACE);
                    if (gns is not null)
                    {
                        await writer.WriteAsync($"declare namespace {NS_NAME_GlideRecord} {{");
                        writer.Indent = 1;
                        foreach (TableInfo table in gns.OrderBy(g => g.Name))
                        {
                            await writer.WriteLineAsync();
                            await writer.WriteLineAsync("/**");
                            await writer.WriteLineAsync($" * {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.");
                            if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
                            {
                                await writer.WriteAsync(" * Auto-number Prefix: ");
                                await writer.WriteLineAsync(table.NumberPrefix);
                            }
                            if (table.IsExtendable)
                                await writer.WriteLineAsync(" * IsExtendable: true");
                            await writer.WriteLineAsync(" */");
                            await writer.WriteAsync($"export type {table.Name} = {table.GetInterfaceTypeString(AppSettings.DEFAULT_NAMESPACE)}");
                            if (table.SuperClass is null)
                                await writer.WriteLineAsync($" & {TS_NAME_GlideRecord};");
                            else
                                await writer.WriteLineAsync($" & {table.SuperClass.GetGlideRecordTypeString(AppSettings.DEFAULT_NAMESPACE)};");
                        }
                        writer.Indent = 0;
                        await writer.WriteLineAsync("}");
                        await writer.WriteLineAsync();
                        await writer.WriteAsync($"declare namespace {NS_NAME_GlideElement} {{");
                        writer.Indent = 1;
                        foreach (TableInfo table in gns.OrderBy(g => g.Name))
                        {
                            await writer.WriteLineAsync();
                            await writer.WriteLineAsync("/**");
                            await writer.WriteLineAsync($" * Element that refers to a {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name :
                                table.Label.SmartQuoteJson())} glide record.");
                            await writer.WriteLineAsync(" */");
                            await writer.WriteLineAsync($"export type Reference<{table.GetInterfaceTypeString(AppSettings.DEFAULT_NAMESPACE)}, {table.GetGlideRecordTypeString(AppSettings.DEFAULT_NAMESPACE)}>;");
                        }
                        writer.Indent = 0;
                        await writer.WriteLineAsync("}");
                        await writer.WriteLineAsync();
                        await writer.WriteAsync($"declare namespace {NS_NAME_tableFields} {{");
                        writer.Indent = 1;
                        if (isScoped)
                        {
                            foreach (TableInfo table in gns.OrderBy(g => g.Name))
                                await table.RenderFieldsScopedAsync(writer, _dbContext, stoppingToken);
                        }
                        else
                        {
                            foreach (TableInfo table in gns.OrderBy(g => g.Name))
                                await table.RenderFieldsGlobalAsync(writer, _dbContext, stoppingToken);
                        }
                        writer.Indent = 0;
                        await writer.WriteLineAsync("}");
                        if ((nsGrouped = nsGrouped.Where(n => n.Key == AppSettings.DEFAULT_NAMESPACE).ToArray()).Length > 0)
                            await writer.WriteLineAsync();
                    }

                    foreach (var nsg in nsGrouped.OrderBy(n => n.Key))
                    {
                        var scope = nsg.First().Scope!;
                        await writer.WriteAsync("/**");
                        await writer.WriteAsync($" * {((string.IsNullOrWhiteSpace(scope.Name) || scope.Name == scope.Value) ? scope.Value : scope.Name.SmartQuoteJson())} scope.");
                        await writer.WriteAsync(" */");
                        await writer.WriteAsync($"declare namespace {nsg.Key} {{");
                        writer.Indent = 1;
                        await writer.WriteAsync($"export namespace {NS_NAME_record} {{");
                        writer.Indent = 2;
                        foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                        {
                            await writer.WriteLineAsync();
                            await writer.WriteLineAsync("/**");
                            await writer.WriteLineAsync($" * {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.");
                            if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
                            {
                                await writer.WriteAsync(" * Auto-number Prefix: ");
                                await writer.WriteLineAsync(table.NumberPrefix);
                            }
                            if (table.IsExtendable)
                                await writer.WriteLineAsync(" * IsExtendable: true");
                            await writer.WriteLineAsync(" */");
                            await writer.WriteAsync($"export type {table.Name} = {table.GetInterfaceTypeString(nsg.Key)}");
                            if (table.SuperClass is null)
                                await writer.WriteLineAsync($" & {TS_NAME_GlideRecord};");
                            else
                                await writer.WriteLineAsync($" & {table.SuperClass.GetGlideRecordTypeString(nsg.Key)};");
                        }
                        writer.Indent = 1;
                        await writer.WriteLineAsync("}");
                        await writer.WriteLineAsync();
                        await writer.WriteAsync($"export namespace {NS_NAME_element} {{");
                        writer.Indent = 2;
                        foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                        {
                            await writer.WriteLineAsync();
                            await writer.WriteLineAsync("/**");
                            await writer.WriteLineAsync($" * Element that refers to a {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name :
                                table.Label.SmartQuoteJson())} glide record.");
                            await writer.WriteLineAsync(" */");
                            await writer.WriteLineAsync($"export type Reference<{table.GetInterfaceTypeString(nsg.Key)}, {table.GetGlideRecordTypeString(nsg.Key)}>;");
                        }
                        writer.Indent = 1;
                        await writer.WriteLineAsync("}");
                        await writer.WriteLineAsync();
                        await writer.WriteAsync($"export namespace {NS_NAME_fields} {{");
                        writer.Indent = 2;
                        if (isScoped)
                        {
                            foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                                await table.RenderFieldsScopedAsync(writer, _dbContext, stoppingToken);
                        }
                        else
                        {
                            foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                                await table.RenderFieldsGlobalAsync(writer, _dbContext, stoppingToken);
                        }
                        writer.Indent = 1;
                        await writer.WriteLineAsync("}");
                        writer.Indent = 0;
                        await writer.WriteLineAsync("}");
                    }
                    try
                    {
                        await writer.FlushAsync();
                        await streamWriter.FlushAsync();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogOutputFileAccessError(outputFileName, exception);
                        return;
                    }
                }
            }
            catch (Exception error) { _logger.LogUnexpecteException(error); }
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