using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.SnApiConstants;
using static SnTsTypeGenerator.CmdLineConstants;

namespace SnTsTypeGenerator;

public class RenderingService
{
    private ILogger<RenderingService> _logger;
    private readonly IServiceScope _scope;
    private readonly FileInfo? _outputFile;
    private readonly bool _forceOverwrite;
    private readonly bool _isScoped;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _outputFile is not null;

    private static IEnumerable<string> GetFlags(ElementInfo elementInfo)
    {
        if (elementInfo.IsPrimary)
            yield return "Is Primary: true";
        else if (elementInfo.IsMandatory)
            yield return "Is Mandatory: true";
        if (!elementInfo.IsActive)
            yield return "Is Active: false";
        if (elementInfo.IsArray)
            yield return "Is Array: true";
        if (elementInfo.IsReadOnly)
            yield return "Is Read-only: true";
        if (elementInfo.IsDisplay)
            yield return "Is Display: true";
        if (elementInfo.IsCalculated)
            yield return "Is Calculated: true";
        if (elementInfo.IsUnique)
            yield return "Is Unique: true";
    }

    private async Task RenderJsDocGlobalAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await writer.WriteLineAsync();
        await writer.WriteLinesAsync(cancellationToken,
            $"/**",
            $" * {elementInfo.Label.SmartQuoteJson()} element."
        );
        if (elementInfo.TypeName is not null && IsExplicitScalarTypeGlobal(elementInfo.TypeName))
        {
            GlideType? type = await elementInfo.GetReferencedEntityAsync(dbContext.Elements, e => e.Type, cancellationToken);
            if (type is null)
                await writer.WriteLineAsync($" * Type: {elementInfo.TypeName}");
            else if (type.Name.Equals(type.Label, StringComparison.InvariantCultureIgnoreCase))
                await writer.WriteLineAsync($" * Type: {type.Label.SmartQuoteJson()}");
            else
                await writer.WriteLineAsync($" * Type: {type.Label.SmartQuoteJson()} ({type.Name})");
        }
        string[] values = GetFlags(elementInfo).ToArray();
        switch (values.Length)
        {
            case 0:
                break;
            case 1:
                await writer.WriteLineAsync($" * {values[0]}");
                break;
            default:
                await writer.WriteAsync($" * {values[0]}");
                foreach (string l in values.Skip(1).SkipLast(1))
                    await writer.WriteAsync($"; {l}");
                await writer.WriteLineAsync($"; {values[^1]}");
                break;
        }

        throw new NotImplementedException();
    }

    private async Task RenderPropertyGlobalAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task RenderJsDocScopedAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task RenderPropertyScopedAsync(ElementInfo elementInfo, IndentedTextWriter writer, string @namespace, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
#pragma warning restore CS1998

    private async Task RenderFieldsAsync(TableInfo tableInfo, IndentedTextWriter writer, TypingsDbContext dbContext, Func<ElementInfo, string, Task> renderPropertyAsync,
        Func<ElementInfo, string, Task> renderJsDocAsync, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
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
        await RenderFieldsAsync(tableInfo, writer, dbContext, async (e, ns) => await RenderPropertyGlobalAsync(e, writer, ns, dbContext, cancellationToken),
            async (e, ns) => await RenderJsDocGlobalAsync(e, writer, ns, dbContext, cancellationToken), cancellationToken);

    private async Task RenderFieldsScopedAsync(TableInfo tableInfo, IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken) =>
        await RenderFieldsAsync(tableInfo, writer, dbContext, async (e, ns) => await RenderPropertyScopedAsync(e, writer, ns, dbContext, cancellationToken),
            async (e, ns) => await RenderJsDocScopedAsync(e, writer, ns, dbContext, cancellationToken), cancellationToken);

    private static string GetScopedElementName(string typeName) => typeName switch
    {
        TYPE_NAME_journal or TYPE_NAME_glide_list or TYPE_NAME_glide_action_list or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list => "JournalGlideElement",

        TYPE_NAME_glide_date_time or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or
            TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time => "GlideDateTimeElement",

        TYPE_NAME_reference or TYPE_NAME_currency2 or TYPE_NAME_domain_id or TYPE_NAME_document_id or TYPE_NAME_source_id => TS_NAME_GlideElementReference,
        _ => TS_NAME_GlideElement,
    };

    private static bool IsExplicitScalarTypeScoped(string typeName) => typeName switch
    {
        TYPE_NAME_glide_list or TYPE_NAME_glide_action_list or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or
            TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time or TYPE_NAME_currency2 or TYPE_NAME_domain_id or
            TYPE_NAME_document_id or TYPE_NAME_source_id => true,

        _ => false,
    };

    private static string GetGlobalElementName(string typeName) => typeName switch
    {
        "boolean" => "GlideElementBoolean",

        TYPE_NAME_integer or TYPE_NAME_decimal or TYPE_NAME_float or TYPE_NAME_percent_complete or TYPE_NAME_order_index or TYPE_NAME_longint => "GlideElementNumeric",

        "sys_class_name" => "GlideElementSysClassName",

        TYPE_NAME_document_id => "GlideElementDocumentId",

        TYPE_NAME_domain_id => "GlideElementDomainId",

        "related_tags" => "GlideElementRelatedTags",

        "translated_field" => "GlideElementTranslatedField",

        "documentation_field" => "GlideElementDocumentation",

        "script" or TYPE_NAME_script_plain or TYPE_NAME_xml => "GlideElementScript",

        "conditions" => "GlideElementConditions",

        "variables" => "GlideElementVariables",

        "password" => "GlideElementPassword",

        "user_image" => "GlideElementUserImage",

        "translated_text" => "GlideElementTranslatedText",

        "counter" => "GlideElementCounter",

        "currency" => "GlideElementCurrency",

        "price" => "GlideElementPrice",

        "short_field_name" => "GlideElementShortFieldName",

        "audio" => "GlideElementAudio",

        "replication_payload" => "GlideElementReplicationPayload",

        "breakdown_element" => "GlideElementBreakdownElement",

        "compressed" => "GlideElementCompressed",

        "translated_html" => "GlideElementTranslatedHTML",

        "url" => "GlideElementURL",

        "template_value" => "GlideElementWorkflowConditions",

        "short_table_name" => "GlideElementShortTableName",

        "data_object" => "GlideElementDataObject",

        "string_full_utf8" => "GlideElementFullUTF8",

        "icon" => "GlideElementIcon",

        "glide_var" => "GlideElementGlideVar",

        "internal_type" => "GlideElementInternalType",

        "simple_name_values" => "GlideElementSimpleNameValue",

        "name_values" => "GlideElementNameValue",

        "source_name" => "GlideElementSourceName",

        "source_table" => "GlideElementSourceTable",

        "password2" => "GlideElementPassword2",

        TYPE_NAME_reference => TS_NAME_GlideElementReference,

        "wiki_text" => "GlideElementWikiText",

        "workflow" => "GlideElementWorkflow",

        TYPE_NAME_glide_date_time or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or
            TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list or TYPE_NAME_html or TYPE_NAME_glide_list or
            TYPE_NAME_journal or TYPE_NAME_glide_action_list or TYPE_NAME_date or TYPE_NAME_day_of_week or TYPE_NAME_month_of_year or TYPE_NAME_week_of_month => "GlideElementGlideObject",

        "phone_number" or TYPE_NAME_caller_phone_number or TYPE_NAME_phone_number_e164 => "GlideElementPhoneNumber",

        "ip_addr" => "GlideElementIPAddress",

        _ => TS_NAME_GlideElement,
    };

    private static bool IsExplicitScalarTypeGlobal(string typeName) => typeName switch
    {
        TYPE_NAME_decimal or TYPE_NAME_float or TYPE_NAME_percent_complete or TYPE_NAME_order_index or TYPE_NAME_longint or TYPE_NAME_script_plain or TYPE_NAME_xml or TYPE_NAME_glide_date or
            TYPE_NAME_glide_time or TYPE_NAME_timer or TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time or
            TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list or TYPE_NAME_html or TYPE_NAME_glide_list or TYPE_NAME_journal or TYPE_NAME_glide_action_list or TYPE_NAME_date or
            TYPE_NAME_day_of_week or TYPE_NAME_month_of_year or TYPE_NAME_week_of_month or TYPE_NAME_caller_phone_number or TYPE_NAME_phone_number_e164 => true,

        _ => false,
    };

    internal async Task RenderAsync(IEnumerable<TableInfo> toRender, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_outputFile is null)
            return;
        using var dbContext = _scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
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
                var gns = nsGrouped.FirstOrDefault(n => n.Key == DEFAULT_NAMESPACE);
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
                        await writer.WriteAsync($"export type {table.Name} = {table.GetInterfaceTypeString(DEFAULT_NAMESPACE)}");
                        if (table.SuperClass is null)
                            await writer.WriteLineAsync($" & {TS_NAME_GlideRecord};");
                        else
                            await writer.WriteLineAsync($" & {table.SuperClass.GetGlideRecordTypeString(DEFAULT_NAMESPACE)};");
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
                            $"export type Reference<{table.GetInterfaceTypeString(DEFAULT_NAMESPACE)}, {table.GetGlideRecordTypeString(DEFAULT_NAMESPACE)}>;"
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
                            await RenderFieldsScopedAsync(table, writer, dbContext, cancellationToken);
                        }
                    }
                    else
                    {
                        foreach (TableInfo table in gns.OrderBy(g => g.Name))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;
                            await RenderFieldsGlobalAsync(table, writer, dbContext, cancellationToken);
                        }
                    }
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                    if ((nsGrouped = nsGrouped.Where(n => n.Key == DEFAULT_NAMESPACE).ToArray()).Length > 0)
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
                            await RenderFieldsScopedAsync(table, writer, dbContext, cancellationToken);
                        }
                    }
                    else
                    {
                        foreach (TableInfo table in nsg.OrderBy(g => g.Name))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;
                            await RenderFieldsGlobalAsync(table, writer, dbContext, cancellationToken);
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

    public RenderingService(ILogger<RenderingService> logger, IServiceProvider services, IOptions<AppSettings> appSettingsOptions)
    {
        _logger = logger;
        _scope = services.CreateScope();
        // _dbContext = dbContext;
        AppSettings appSettings = appSettingsOptions.Value;
        if (string.IsNullOrWhiteSpace(appSettings.Mode))
        {
            _isScoped = false;
            _logger.LogDefaultRenderMode(_isScoped);
        }
        else
        {
            switch (appSettings.Mode.Trim().ToLower())
            {
                case MODE_SCOPED:
                case MODE_SCOPED_ABBR:
                    _isScoped = true;
                    break;
                case MODE_GLOBAL:
                case MODE_GLOBAL_ABBR:
                    _isScoped = false;
                    break;
                default:
                    _logger.LogInvalidModeOption(appSettings.Mode);
                    return;
            }
            _logger.LogRenderModeSettingValue(_isScoped);
        }

        _forceOverwrite = appSettings.Force ?? false;
        string outputFileName = appSettings.Output!;
        if (string.IsNullOrEmpty(outputFileName))
            outputFileName = DEFAULT_OUTPUT_FILENAME;
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
            }
            else if (!(outputFile.Directory?.Exists ?? false))
            {
                _logger.LogOutputFileAccessError(outputFile.FullName, "Parent subdirectory does not exist");
                return;
            }
            _logger.LogUsingOutputFile(outputFileName, _forceOverwrite);
            _outputFile = outputFile;
        }
        catch (Exception exception)
        {
            _logger.LogOutputFileAccessError(outputFileName, exception);
            return;
        }
    }
}