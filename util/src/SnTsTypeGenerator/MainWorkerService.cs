using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    internal async Task RenderJsDocGlobalAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal async Task RenderPropertyGlobalAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal async Task RenderJsDocScopedAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal async Task RenderPropertyScopedAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task RenderFieldsAsync(TableInfo tableInfo, IndentedTextWriter writer, TypingsDbContext dbContext, Func<ElementInfo, string, Task> renderPropertyAsync, Func<ElementInfo, string, Task> renderJsDocAsync,
        CancellationToken cancellationToken)
    {
        string @namespace = tableInfo.GetNamespace();
        var tableName = tableInfo.Name;
        EntityEntry<TableInfo> entry = dbContext.Tables.Entry(tableInfo);
        var elements = (await entry.GetRelatedCollectionAsync(e => e.Elements, cancellationToken)).ToArray();
        TableInfo? superClass = await entry.GetReferencedEntityAsync(e => e.SuperClass, cancellationToken);
        string extends;
        ElementInfo[] jsDocElements;
        if (superClass is not null)
        {
            extends = $"extends {superClass.GetInterfaceTypeString(@namespace)}{{";
            if (elements.Length > 0)
            {
                var baseElements = await superClass.GetRelatedCollectionAsync(dbContext.Tables, t => t.Elements, cancellationToken);
                if (baseElements.Any())
                {
                    var withBase = elements.GetBaseElements(baseElements);
                    jsDocElements = withBase.Where(a => a.Base is not null && !a.IsTypeOverride).Select(a => a.Inherited).ToArray();
                    elements = withBase.Where(a => a.Base is null || a.IsTypeOverride).Select(a => a.Inherited).ToArray();
                }
                else
                    jsDocElements = Array.Empty<ElementInfo>();
            }
            else
                jsDocElements = Array.Empty<ElementInfo>();
        }
        else if (elements.ExtendsBaseRecord())
        {
            jsDocElements = Array.Empty<ElementInfo>();
            elements = elements.GetNonBaseRecordElements().ToArray();
            extends = "extends IBaseRecord {";
        }
        else
        {
            jsDocElements = Array.Empty<ElementInfo>();
            extends = "{";
        }
        tableName = tableInfo.GetShortName();
        await writer.WriteLineAsync();
        await writer.WriteLinesAsync(cancellationToken,
            "/**",
            (tableName == tableInfo.Name) ?
                $" * {((string.IsNullOrWhiteSpace(tableInfo.Label) || tableInfo.Label == tableName) ? tableName : tableInfo.Label.SmartQuoteJson())} glide record fields." :
                $" * {((string.IsNullOrWhiteSpace(tableInfo.Label) || tableInfo.Label == tableName) ? tableName : tableInfo.Label.SmartQuoteJson())} ({tableInfo.Name}) glide record fields.",
            $" * @see {{@link {tableInfo.GetGlideRecordTypeString(@namespace)}}}",
            $" * @see {{@link {tableInfo.GetGlideElementTypeString(@namespace)}}}",
            " */"
        );
        
        if (elements.Length > 0 || jsDocElements.Length > 0)
        {
            await writer.WriteAsync($"export interface {tableName} {extends}");
            var indent = writer.Indent + 1;
            foreach (ElementInfo e in jsDocElements)
            {
                writer.Indent = indent;
                await renderJsDocAsync(e, @namespace);
            }
            foreach (ElementInfo e in elements)
            {
                writer.Indent = indent;
                await renderPropertyAsync(e, @namespace);
            }
            writer.Indent = indent - 1;
            await writer.WriteLineAsync("}");
        }
        else
            await writer.WriteLineAsync($"export interface {tableName} {extends} }}");
    }

    private async Task RenderFieldsGlobalAsync(TableInfo tableInfo, IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken) =>
        await RenderFieldsAsync(tableInfo, writer, dbContext, async (e, ns) => await RenderPropertyGlobalAsync(e, writer, ns, cancellationToken),
            async (e, ns) => await RenderJsDocGlobalAsync(e, writer, ns, cancellationToken), cancellationToken);

    private async Task RenderFieldsScopedAsync(TableInfo tableInfo, IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken) =>
        await RenderFieldsAsync(tableInfo, writer, dbContext, async (e, ns) => await RenderPropertyScopedAsync(e, writer, ns, cancellationToken),
            async (e, ns) => await RenderJsDocScopedAsync(e, writer, ns, cancellationToken), cancellationToken);

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
                            if (stoppingToken.IsCancellationRequested)
                                return;
                            await writer.WriteLineAsync();
                            await writer.WriteLinesAsync(stoppingToken,
                                "/**",
                                $" * {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record."
                            );
                            if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
                                await writer.WriteLineAsync($" * Auto-number Prefix: {table.NumberPrefix}");
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
                            if (stoppingToken.IsCancellationRequested)
                                return;
                            await writer.WriteLineAsync();
                            await writer.WriteLinesAsync(stoppingToken,
                                "/**",
                                $" * Element that refers to a {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.",
                                " */",
                                $"export type Reference<{table.GetInterfaceTypeString(AppSettings.DEFAULT_NAMESPACE)}, {table.GetGlideRecordTypeString(AppSettings.DEFAULT_NAMESPACE)}>;"
                            );
                        }
                        writer.Indent = 0;
                        await writer.WriteLineAsync("}");
                        await writer.WriteLineAsync();
                        await writer.WriteAsync($"declare namespace {NS_NAME_tableFields} {{");
                        writer.Indent = 1;
                        if (isScoped)
                        {
                            foreach (TableInfo table in gns.OrderBy(g => g.Name))
                            {
                                if (stoppingToken.IsCancellationRequested)
                                    return;
                                await RenderFieldsScopedAsync(table, writer, _dbContext, stoppingToken);
                            }
                        }
                        else
                        {
                            foreach (TableInfo table in gns.OrderBy(g => g.Name))
                            {
                                if (stoppingToken.IsCancellationRequested)
                                    return;
                                await RenderFieldsGlobalAsync(table, writer, _dbContext, stoppingToken);
                            }
                        }
                        writer.Indent = 0;
                        await writer.WriteLineAsync("}");
                        if ((nsGrouped = nsGrouped.Where(n => n.Key == AppSettings.DEFAULT_NAMESPACE).ToArray()).Length > 0)
                            await writer.WriteLineAsync();
                    }

                    foreach (var nsg in nsGrouped.OrderBy(n => n.Key))
                    {
                        if (stoppingToken.IsCancellationRequested)
                            return;
                        var scope = nsg.First().Scope!;
                        await writer.WriteLinesAsync(stoppingToken,
                            "/**",
                            $" * {((string.IsNullOrWhiteSpace(scope.Name) || scope.Name == scope.Value) ? scope.Value : scope.Name.SmartQuoteJson())} scope.",
                            " */"
                        );
                        await writer.WriteAsync($"declare namespace {nsg.Key} {{");
                        writer.Indent = 1;
                        await writer.WriteAsync($"export namespace {NS_NAME_record} {{");
                        writer.Indent = 2;
                        foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                        {
                            if (stoppingToken.IsCancellationRequested)
                                return;
                            await writer.WriteLineAsync();
                            await writer.WriteLineAsync("/**");
                            await writer.WriteLineAsync($" * {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.");
                            if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
                                await writer.WriteLineAsync($" * Auto-number Prefix: {table.NumberPrefix}");
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
                            if (stoppingToken.IsCancellationRequested)
                                return;
                            await writer.WriteLineAsync();
                            await writer.WriteLinesAsync(stoppingToken,
                                "/**",
                                $" * Element that refers to a {((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.",
                                " */",
                                $"export type Reference<{table.GetInterfaceTypeString(nsg.Key)}, {table.GetGlideRecordTypeString(nsg.Key)}>;"
                            );
                        }
                        writer.Indent = 1;
                        await writer.WriteLineAsync("}");
                        await writer.WriteLineAsync();
                        await writer.WriteAsync($"export namespace {NS_NAME_fields} {{");
                        writer.Indent = 2;
                        if (isScoped)
                        {
                            foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                            {
                                if (stoppingToken.IsCancellationRequested)
                                    return;
                                await RenderFieldsScopedAsync(table, writer, _dbContext, stoppingToken);
                            }
                        }
                        else
                        {
                            foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                            {
                                if (stoppingToken.IsCancellationRequested)
                                    return;
                                await RenderFieldsGlobalAsync(table, writer, _dbContext, stoppingToken);
                            }
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