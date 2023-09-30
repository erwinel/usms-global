using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public class RenderingService
{
    private ILogger<MainWorkerService> _logger;
    private TypingsDbContext _dbContext;
    private readonly FileInfo? _outputFile;
    private readonly bool _forceOverwrite;
    private readonly bool _isScoped;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _outputFile is not null;

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

    internal async Task RenderAsync(IEnumerable<TableInfo> toRender, CancellationToken cancellationToken)
    {
        if (_outputFile is null)
            return;
        StreamWriter streamWriter;
        try
        {
            streamWriter = new(_outputFile.FullName, new FileStreamOptions()
            {
                Access = FileAccess.Write,
                Mode = _forceOverwrite ? FileMode.Create : FileMode.CreateNew
            });
        }
        catch (Exception error)
        {
            _logger.LogOutputFileAccessError(_outputFile.FullName, error);
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
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        await writer.WriteLineAsync();
                        await writer.WriteLinesAsync(cancellationToken,
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
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        await writer.WriteLineAsync();
                        await writer.WriteLinesAsync(cancellationToken,
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
                    if (_isScoped)
                    {
                        foreach (TableInfo table in gns.OrderBy(g => g.Name))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;
                            await RenderFieldsScopedAsync(table, writer, _dbContext, cancellationToken);
                        }
                    }
                    else
                    {
                        foreach (TableInfo table in gns.OrderBy(g => g.Name))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;
                            await RenderFieldsGlobalAsync(table, writer, _dbContext, cancellationToken);
                        }
                    }
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                    if ((nsGrouped = nsGrouped.Where(n => n.Key == AppSettings.DEFAULT_NAMESPACE).ToArray()).Length > 0)
                        await writer.WriteLineAsync();
                }

                foreach (var nsg in nsGrouped.OrderBy(n => n.Key))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    var scope = nsg.First().Scope!;
                    await writer.WriteLinesAsync(cancellationToken,
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
                        if (cancellationToken.IsCancellationRequested)
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
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        await writer.WriteLineAsync();
                        await writer.WriteLinesAsync(cancellationToken,
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
                    if (_isScoped)
                    {
                        foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;
                            await RenderFieldsScopedAsync(table, writer, _dbContext, cancellationToken);
                        }
                    }
                    else
                    {
                        foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;
                            await RenderFieldsGlobalAsync(table, writer, _dbContext, cancellationToken);
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
                    _logger.LogOutputFileAccessError(_outputFile.FullName, exception);
                    return;
                }
            }
        }
        catch (Exception error) { _logger.LogUnexpecteException(error); }
    }

    public RenderingService(ILogger<MainWorkerService> logger, TypingsDbContext dbContext, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _isScoped = appSettings.Value.Scoped ?? false;
        if (_isScoped && (appSettings.Value.Global ?? false))
        {
            _logger.LogGlobalAndScopedSwitchesBothSet();
            return;
        }
        _forceOverwrite = appSettings.Value.Force ?? false;
        string outputFileName = appSettings.Value.Output!;
        if (string.IsNullOrEmpty(outputFileName))
            outputFileName = AppSettings.DEFAULT_OUTPUT_FILENAME;
        try
        {
            FileInfo outputFile = new(outputFileName);
            outputFileName = outputFile.FullName;
            if (outputFile.Exists)
            {
                if (!_forceOverwrite)
                {
                    _logger.LogOutputFileAlreadyExists(outputFile.FullName);
                    return;
                }
                _outputFile = outputFile;
            }
            else if (!(outputFile.Directory?.Exists ?? false))
            {
                _logger.LogOutputFileAccessError(outputFile.FullName, "Parent subdirectory does not exist");
                return;
            }
        }
        catch (Exception exception)
        {
            _logger.LogOutputFileAccessError(outputFileName, exception);
            return;
        }
    }
}