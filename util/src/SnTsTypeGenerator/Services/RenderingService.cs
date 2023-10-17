using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.SnApiConstants;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public partial class RenderingService
{
    private const string EMPTY_JSDOC_LINE = " *";
    private const string START_JSDOC_LINE = " * ";
    private const string START_INDENTED_JSDOC_LINE = " *    ";
    private const string OPEN_JSDOC = "/**";
    private const string CLOSE_JSDOC = " */";
    private ILogger<RenderingService> _logger;
    private readonly IServiceScope _scope;
    private readonly FileInfo? _outputFile;
    private readonly bool _forceOverwrite;
    private readonly bool _includeReferenced;
    private readonly bool _emitBaseTypes;
    private readonly bool _isScoped;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _outputFile is not null;

    internal async Task RenderAsync(IEnumerable<TableInfo> toRender, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_outputFile is null)
            return;
        using var dbContext = _scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
        if (_includeReferenced)
        {
            List<TableInfo> toValidate = toRender.ToList();
            for (int index = 0; index < toValidate.Count; index++)
            {
                var tbl = toValidate[index];
                var e = dbContext.Tables.Entry(tbl);
                var n = tbl.SuperClassName;
                if (!string.IsNullOrEmpty(n) && !toValidate.Any(t => NameComparer.Equals(t.Name, n)))
                {
                    var sc = await e.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
                    if (sc is not null)
                        toValidate.Add(sc);
                }

                foreach (ElementInfo el in await e.GetRelatedCollectionAsync(t => t.Elements, cancellationToken))
                {
                    n = el.RefTableName;
                    if (!string.IsNullOrEmpty(n) && !toValidate.Any(t => NameComparer.Equals(t.Name, n)))
                    {
                        var rt = await el.GetReferencedEntityAsync(dbContext.Elements, a => a.Reference, cancellationToken);
                        if (rt is not null)
                            toValidate.Add(rt);
                    }
                }
            }
            toRender = toValidate.OrderBy(t => t.Name, NameComparer);
        }
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

                var byNamespace = toRender.GroupBy(t => t.GetNamespace(), NameComparer);
                var gns = byNamespace.FirstOrDefault(n => NameComparer.Equals(n.Key, GLOBAL_NAMESPACE));
                bool appendNewLine = gns is not null;
                if (appendNewLine)
                {
                    var entries = gns!.OrderBy(t => t.Name, NameComparer).Select(dbContext.Tables.Entry);
                    await writer.WriteDeclareNamespace(NS_NAME_GlideRecord);
                    await RenderGlobalGlideRecordAsync(entries.First(), writer, cancellationToken);
                    foreach (var e in entries.Skip(1))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync();
                        await RenderGlobalGlideRecordAsync(e, writer, cancellationToken);
                    }
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                    await writer.WriteLineAsync();

                    await writer.WriteDeclareNamespace(NS_NAME_GlideElement);
                    if (_emitBaseTypes)
                    {
                        await RenderReferenceBaseTypeAsync(writer);
                        foreach (var e in entries)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await writer.WriteLineAsync();
                            await RenderGlobalGlideElementAsync(e.Entity, writer);
                        }
                    }
                    else
                    {
                        await RenderGlobalGlideElementAsync(entries.First().Entity, writer);
                        foreach (var e in entries.Skip(1))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await writer.WriteLineAsync();
                            await RenderGlobalGlideElementAsync(e.Entity, writer);
                        }
                    }
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                    await writer.WriteLineAsync();

                    await writer.WriteDeclareNamespace(NS_NAME_tableFields);
                    if (_emitBaseTypes)
                    {
                        await RenderIBaseRecordAsync(writer, dbContext, cancellationToken);
                        foreach (var e in entries)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await writer.WriteLineAsync();
                            await RenderGlobalTableFieldsAsync(e, writer, dbContext, cancellationToken);
                        }
                    }
                    else
                    {
                        await RenderGlobalTableFieldsAsync(entries.First(), writer, dbContext, cancellationToken);
                        foreach (var e in entries.Skip(1))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await writer.WriteLineAsync();
                            await RenderGlobalTableFieldsAsync(e, writer, dbContext, cancellationToken);
                        }
                    }
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                    byNamespace = byNamespace.Where(g => !NameComparer.Equals(g.Key, GLOBAL_NAMESPACE));
                }
                else if (_emitBaseTypes)
                {
                    await writer.WriteDeclareNamespace(NS_NAME_GlideElement);
                    await RenderReferenceBaseTypeAsync(writer);
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                    await writer.WriteDeclareNamespace(NS_NAME_tableFields);
                    await RenderIBaseRecordAsync(writer, dbContext, cancellationToken);
                    writer.Indent = 0;
                    await writer.WriteLineAsync("}");
                }
                foreach (var scoped in byNamespace.OrderBy(g => g.Key, NameComparer))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (appendNewLine)
                        await writer.WriteLineAsync();
                    else
                        appendNewLine = true;
                    await writer.WriteDeclareNamespace(scoped.Key);
                    await writer.WriteLineAsync(" {");
                    var entries = scoped.OrderBy(t => t.Name, NameComparer).Select(dbContext.Tables.Entry);
                    writer.Indent = 1;
                    await writer.WriteExportNamespace(NS_NAME_record);
                    await RenderScopedGlideRecordAsync(entries.First(), writer, cancellationToken);
                    foreach (var e in entries.Skip(1))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync();
                        await RenderScopedGlideRecordAsync(e, writer, cancellationToken);
                    }
                    writer.Indent = 1;
                    await writer.WriteLineAsync("}");
                    await writer.WriteLineAsync();

                    await writer.WriteExportNamespace(NS_NAME_element);
                    await RenderScopedGlideElementAsync(entries.First().Entity, writer);
                    foreach (var e in entries.Skip(1))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync();
                        await RenderScopedGlideElementAsync(e.Entity, writer);
                    }
                    writer.Indent = 1;
                    await writer.WriteLineAsync("}");
                    await writer.WriteLineAsync();

                    await writer.WriteExportNamespace(NS_NAME_fields);
                    await RenderScopedTableFieldsAsync(entries.First(), writer, dbContext, cancellationToken);
                    foreach (var e in entries.Skip(1))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync();
                        await RenderScopedTableFieldsAsync(e, writer, dbContext, cancellationToken);
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
                }
            }
        }
        catch (Exception error) { _logger.LogUnexpecteException(error); }
    }

    private async Task RenderIBaseRecordAsync(IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        var loaderService = _scope.ServiceProvider.GetRequiredService<DataLoaderService>();
        var baseRecord = await loaderService.GetBaseRecordTypeAsync(cancellationToken);
        await writer.WriteAsync("export interface ");
        await writer.WriteAsync(baseRecord.Name);
        await writer.WriteLineAsync("{");
        var renderingConext = new GlobalElementRenderingContext(baseRecord.PackageName);
        var elements = (await dbContext.Tables.Entry(baseRecord).GetElementsAsync(cancellationToken)).ToArray();
        await RenderElementAsync(elements[0], renderingConext, writer, dbContext, cancellationToken);
        foreach (var e in elements.Skip(1))
        {
            await writer.WriteLineAsync();
            await RenderElementAsync(e, renderingConext, writer, dbContext, cancellationToken);
        }
        writer.Indent = 1;
        await writer.WriteLineAsync("}");
    }

    // /**
    //  * Reference
    //  * Type: reference; Scalar Type: GUID
    //  * @template TFields - The type that defines the fields of the referenced record.
    //  * @template TRecord - The referenced record type.
    //  */
    // export type Reference<TFields = $$tableFields.IBaseRecord, TRecord extends GlideRecord & TFields = GlideRecord & TFields> = TFields & {
    //     getRefRecord(): TRecord;
    // } & GlideElementReference;
    private async Task RenderReferenceBaseTypeAsync(IndentedTextWriter writer)
    {
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteLineAsync(" * Reference Element");
        await writer.WriteLineAsync(" * Type: reference; Scalar Type: GUID");
        await writer.WriteLineAsync(" * @template TFields - The type that defines the fields of the referenced record.");
        await writer.WriteLineAsync(" * @template TRecord - The referenced record type.");
        await writer.WriteLineAsync(CLOSE_JSDOC);
        await writer.WriteLineAsync($"export type Reference<TFields = {NS_NAME_tableFields}.{TS_NAME_BASERECORD}, TRecord extends {TS_NAME_GlideRecord} & TFields = {TS_NAME_GlideRecord} & TFields> = TFields & {{");
        await writer.WriteLineAsync("    getRefRecord(): TRecord;");
        await writer.WriteLineAsync($"}} & {TS_NAME_GlideElementReference};");
    }

    private static async Task RenderGlideRecordJsDocAsync(TableInfo table, SysPackage? package, IndentedTextWriter writer)
    {
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteAsync(START_JSDOC_LINE);
        await writer.WriteAsync(table.Label.SmartQuoteJson());
        if (NameComparer.Equals(table.Name, table.Label))
            await writer.WriteLineAsync(" glide record.");
        else
        {
            await writer.WriteAsync(" (");
            await writer.WriteAsync(table.Name.SmartQuoteJson());
            await writer.WriteLineAsync(") glide record.");
        }
        if (table.IsExtendable)
            await writer.WriteLineAsync(" * Extendable: true");
        if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
        {
            await writer.WriteLineAsync(" * Number Prefix: ");
            await writer.WriteLineAsync(table.NumberPrefix.SmartQuoteJson());
        }
        if (!string.IsNullOrWhiteSpace(table.AccessibleFrom))
        {
            await writer.WriteLineAsync(" * Accessible From: ");
            await writer.WriteLineAsync(table.AccessibleFrom.SmartQuoteJson());
        }
        if (!string.IsNullOrWhiteSpace(table.ExtensionModel))
        {
            await writer.WriteLineAsync(" * Extension Model: ");
            await writer.WriteLineAsync(table.ExtensionModel.SmartQuoteJson());
        }
        if (package is not null)
        {
            await writer.WriteAsync(" * Package: ");
            if (string.IsNullOrWhiteSpace(package.ShortDescription) || NameComparer.Equals(package.Name, package.ShortDescription))
                await writer.WriteLineAsync(package.Name);
            else
            {
                await writer.WriteAsync(package.Name);
                await writer.WriteAsync(" - ");
                await writer.WriteLineAsync(package.ShortDescription.AsWhitespaceNormalized());
            }
        }

        await writer.WriteLineAsync(" */");
    }

    private static async Task RenderGlobalGlideRecordAsync(EntityEntry<TableInfo> entry, IndentedTextWriter writer, CancellationToken cancellationToken)
    {
        writer.Indent = 1;
        TableInfo table = entry.Entity;
        await RenderGlideRecordJsDocAsync(table, await entry.GetReferencedEntityAsync(t => t.Package, cancellationToken), writer);
        await writer.WriteAsync("export type ");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(" = ");
        await writer.WriteAsync(NS_NAME_tableFields);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(" & ");
        TableInfo? superClass = await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
            await writer.WriteAsync(TS_NAME_GlideRecord);
        else if (superClass.IsGlobalScope())
            await writer.WriteAsync(superClass.Name);
        else
        {
            await writer.WriteAsync(superClass.ScopeValue);
            await writer.WriteAsync(".");
            await writer.WriteAsync(NS_NAME_record);
            await writer.WriteAsync(".");
            await writer.WriteAsync(superClass.Name);
        }
        await writer.WriteLineAsync(";");
    }

    private static async Task RenderScopedGlideRecordAsync(EntityEntry<TableInfo> entry, IndentedTextWriter writer, CancellationToken cancellationToken)
    {
        writer.Indent = 2;
        TableInfo table = entry.Entity;
        await RenderGlideRecordJsDocAsync(table, await entry.GetReferencedEntityAsync(t => t.Package, cancellationToken), writer);
        await writer.WriteAsync("export type ");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(" = ");
        await writer.WriteAsync(NS_NAME_fields);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(" & ");
        TableInfo? superClass = await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
            await writer.WriteAsync(TS_NAME_GlideRecord);
        else if (NameComparer.Equals(superClass.ScopeValue, table.ScopeValue))
            await writer.WriteAsync(superClass.Name);
        else if (superClass.IsGlobalScope())
        {
            await writer.WriteAsync(NS_NAME_GlideRecord);
            await writer.WriteAsync(".");
            await writer.WriteAsync(superClass.Name);
        }
        else
        {
            await writer.WriteAsync(superClass.ScopeValue);
            await writer.WriteAsync(".");
            await writer.WriteAsync(NS_NAME_record);
            await writer.WriteAsync(".");
            await writer.WriteAsync(superClass.Name);
        }
        await writer.WriteLineAsync(";");
    }

    private static async Task RenderGlobalGlideElementAsync(TableInfo table, IndentedTextWriter writer)
    {
        writer.Indent = 1;
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteAsync(" * Element that refers to a ");
        await writer.WriteAsync(table.Label.SmartQuoteJson());
        if (NameComparer.Equals(table.Name, table.Label))
            await writer.WriteLineAsync(" glide record.");
        else
        {
            await writer.WriteAsync(" (");
            await writer.WriteAsync(table.Name.SmartQuoteJson());
            await writer.WriteLineAsync(") glide record.");
        }
        await writer.WriteLineAsync(" */");
        await writer.WriteAsync("export type ");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(" = Reference<");
        await writer.WriteAsync(NS_NAME_tableFields);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(", ");
        await writer.WriteAsync(NS_NAME_GlideRecord);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteLineAsync(">;");
    }

    private async Task RenderScopedGlideElementAsync(TableInfo table, IndentedTextWriter writer)
    {
        writer.Indent = 2;
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteAsync(" * Element that refers to a ");
        await writer.WriteAsync(table.Label.SmartQuoteJson());
        if (NameComparer.Equals(table.Name, table.Label))
            await writer.WriteLineAsync(" glide record.");
        else
        {
            await writer.WriteAsync(" (");
            await writer.WriteAsync(table.Name.SmartQuoteJson());
            await writer.WriteLineAsync(") glide record.");
        }
        await writer.WriteLineAsync(" */");
        await writer.WriteAsync("export type ");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(" = ");
        await writer.WriteAsync(NS_NAME_GlideElement);
        await writer.WriteAsync(".Reference<");
        await writer.WriteAsync(NS_NAME_fields);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteAsync(", ");
        await writer.WriteAsync(NS_NAME_record);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteLineAsync(">;");
    }

    private static async Task RenderGlobalTableFieldsAsync(EntityEntry<TableInfo> entry, IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        writer.Indent = 1;
        TableInfo table = entry.Entity;
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteAsync(START_JSDOC_LINE);
        await writer.WriteAsync(table.Label.SmartQuoteJson());
        if (NameComparer.Equals(table.Name, table.Label))
            await writer.WriteLineAsync(" glide record fields.");
        else
        {
            await writer.WriteAsync(" (");
            await writer.WriteAsync(table.Name.SmartQuoteJson());
            await writer.WriteLineAsync(") glide record fields.");
        }
        await writer.WriteAsync(" * @see {@link ");
        await writer.WriteAsync(NS_NAME_GlideRecord);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteLineAsync("}");
        await writer.WriteAsync(" * @see {@link ");
        await writer.WriteAsync(NS_NAME_GlideElement);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteLineAsync("}");
        await writer.WriteLineAsync(" */");

        await writer.WriteAsync("export interface ");
        await writer.WriteAsync(table.Name);

        TableInfo? superClass = await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
        if (superClass is not null)
        {
            await writer.WriteAsync(" extends ");
            if (superClass.IsGlobalScope())
                await writer.WriteAsync(superClass.Name);
            else
            {
                await writer.WriteAsync(superClass.ScopeValue);
                await writer.WriteAsync(".");
                await writer.WriteAsync(NS_NAME_fields);
                await writer.WriteAsync(".");
                await writer.WriteAsync(superClass.Name);
            }
        }
        var elements = (await entry.GetElementsAsync(cancellationToken)).ToArray();
        if (elements.Length == 0)
        {
            await writer.WriteLineAsync(" { }");
            return;
        }
        await writer.WriteLineAsync(" {");
        var renderingConext = new GlobalElementRenderingContext(table.PackageName);
        await RenderElementAsync(elements[0], renderingConext, writer, dbContext, cancellationToken);
        foreach (var e in elements.Skip(1))
        {
            await writer.WriteLineAsync();
            await RenderElementAsync(e, renderingConext, writer, dbContext, cancellationToken);
        }
        writer.Indent = 1;
        await writer.WriteLineAsync("}");
    }

    private static async Task RenderScopedTableFieldsAsync(EntityEntry<TableInfo> entry, IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        writer.Indent = 2;
        await writer.WriteLineAsync(OPEN_JSDOC);
        TableInfo table = entry.Entity;
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteAsync(START_JSDOC_LINE);
        await writer.WriteAsync(table.Label.SmartQuoteJson());
        if (NameComparer.Equals(table.Name, table.Label))
            await writer.WriteLineAsync(" glide record fields.");
        else
        {
            await writer.WriteAsync(" (");
            await writer.WriteAsync(table.Name.SmartQuoteJson());
            await writer.WriteLineAsync(") glide record fields.");
        }
        await writer.WriteAsync(" * @see {@link ");
        await writer.WriteAsync(NS_NAME_record);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteLineAsync("}");
        await writer.WriteAsync(" * @see {@link ");
        await writer.WriteAsync(NS_NAME_element);
        await writer.WriteAsync(".");
        await writer.WriteAsync(table.Name);
        await writer.WriteLineAsync("}");
        await writer.WriteLineAsync(CLOSE_JSDOC);

        await writer.WriteAsync("export interface ");
        await writer.WriteAsync(table.Name);

        TableInfo? superClass = await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
        if (superClass is not null)
        {
            await writer.WriteAsync(" extends ");
            if (NameComparer.Equals(superClass.ScopeValue, table.ScopeValue))
                await writer.WriteAsync(superClass.Name);
            else if (superClass.IsGlobalScope())
            {
                await writer.WriteAsync(NS_NAME_tableFields);
                await writer.WriteAsync(superClass.Name);
            }
            else
            {
                await writer.WriteAsync(superClass.ScopeValue);
                await writer.WriteAsync(".");
                await writer.WriteAsync(NS_NAME_fields);
                await writer.WriteAsync(".");
                await writer.WriteAsync(superClass.Name);
            }
        }
        var elements = (await entry.GetElementsAsync(cancellationToken)).ToArray();
        if (elements.Length == 0)
        {
            await writer.WriteLineAsync(" { }");
            return;
        }
        await writer.WriteLineAsync(" {");
        var renderingConext = new ScopedElementRenderingContext(table.ScopeValue!, table.PackageName);
        await RenderElementAsync(elements[0], renderingConext, writer, dbContext, cancellationToken);
        foreach (var e in elements.Skip(1))
        {
            await writer.WriteLineAsync();
            await RenderElementAsync(e, renderingConext, writer, dbContext, cancellationToken);
        }
        writer.Indent = 2;
        await writer.WriteLineAsync("}");
    }

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

    private static async Task RenderElementAsync(ElementInheritance inheritance, IElementRenderingContext context, IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        writer.Indent = context.IndentLevel;
        ElementInfo element = inheritance.Element;
        EntityEntry<ElementInfo> entry = dbContext.Elements.Entry(element);
        await writer.WriteLineAsync(OPEN_JSDOC);
        await writer.WriteAsync(START_JSDOC_LINE);
        await writer.WriteAsync(element.Label.SmartQuoteJson());
        if (!NameComparer.Equals(element.Name, element.Label))
        {
            await writer.WriteAsync(" (");
            await writer.WriteAsync(element.Name);
            await writer.WriteLineAsync(") element.");
        }
        else
            await writer.WriteLineAsync(" element.");
        bool appendLine;
        if (string.IsNullOrWhiteSpace(element.Comments))
            appendLine = false;
        else
        {
            appendLine = true;
            foreach (string c in element.Comments.Trim().SplitLines().Select(l => l.TrimEnd()))
            {
                if (c.Length > 0)
                {
                    await writer.WriteAsync(START_JSDOC_LINE);
                    await writer.WriteLineAsync(c);
                }
                else
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
            }
        }

        GlideType? type = await entry.GetReferencedEntityAsync(e => e.Type, cancellationToken);
        TableInfo? reference = await entry.GetReferencedEntityAsync(e => e.Reference, cancellationToken);
        if (context.IsExplicitScalarType(element.TypeName))
        {
            if (appendLine)
            {
                await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                appendLine = false;
            }
            await writer.WriteAsync(" * Type: ");
            if (type is null || type.Label == element.TypeName)
            {
                if (element.MaxLength.HasValue || element.SizeClass.HasValue)
                {
                    await writer.WriteAsync(element.TypeName);
                    await writer.WriteAsync("; ");
                }
                else
                    await writer.WriteLineAsync(element.TypeName);
            }
            else
            {
                await writer.WriteAsync(type.Label.SmartQuoteJson());
                await writer.WriteAsync(" (");
                await writer.WriteAsync(type.Name);
                if (reference is null && (element.MaxLength.HasValue || element.SizeClass.HasValue))
                    await writer.WriteAsync("); ");
                else
                    await writer.WriteLineAsync(")");
            }
            if (reference is null)
            {
                if (element.MaxLength.HasValue)
                {
                    if (element.SizeClass.HasValue)
                        await writer.WriteLineAsync($" * Max Length: {element.MaxLength.Value}; Size class: {element.SizeClass.Value}");
                    else
                        await writer.WriteLineAsync($" * Max Length: {element.MaxLength.Value}");
                }
                else if (element.SizeClass.HasValue)
                    await writer.WriteLineAsync($" * Size class: {element.SizeClass.Value}");
            }
        }
        else if (reference is null)
        {
            if (element.MaxLength.HasValue)
            {
                if (appendLine)
                {
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                    appendLine = false;
                }
                if (element.SizeClass.HasValue)
                    await writer.WriteLineAsync($" * Max Length: {element.MaxLength.Value}; Size class: {element.SizeClass.Value}");
                else
                    await writer.WriteLineAsync($" * Max Length: {element.MaxLength.Value}");
            }
            else if (element.SizeClass.HasValue)
            {
                if (appendLine)
                {
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                    appendLine = false;
                }
                await writer.WriteLineAsync($" * Size class: {element.SizeClass.Value}");
            }
        }
        string[] flags = GetFlags(element).ToArray();
        switch (flags.Length)
        {
            case 0:
                break;
            case 1:
                if (appendLine)
                {
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                    appendLine = false;
                }
                await writer.WriteAsync(START_JSDOC_LINE);
                await writer.WriteLineAsync(flags[0]);
                break;
            case 2:
                if (appendLine)
                {
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                    appendLine = false;
                }
                await writer.WriteAsync(START_JSDOC_LINE);
                await writer.WriteAsync(flags[0]);
                await writer.WriteAsync("; ");
                await writer.WriteLineAsync(flags[1]);
                break;
            default:
                if (appendLine)
                {
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                    appendLine = false;
                }
                await writer.WriteAsync(START_JSDOC_LINE);
                await writer.WriteAsync(flags[0]);
                await writer.WriteAsync("; ");
                foreach (var f in flags.Skip(1).SkipLast(1))
                {
                    await writer.WriteAsync(f);
                    await writer.WriteAsync("; ");
                }
                await writer.WriteLineAsync(flags.Last());
                break;
        }
        if (reference is null)
        {
            if (!string.IsNullOrWhiteSpace(element.DefaultValue))
            {
                string[] lines = element.DefaultValue.Trim().SplitLines();
                if (appendLine)
                    await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                await writer.WriteAsync(" * Default Value: ");
                await writer.WriteLineAsync(lines[0]);
                appendLine = lines.Length > 1;
                foreach (string l in lines.Skip(1))
                {
                    await writer.WriteAsync(START_INDENTED_JSDOC_LINE);
                    await writer.WriteLineAsync(l);
                }
            }
        }
        else
        {
            if (appendLine)
            {
                await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                appendLine = false;
            }
            await writer.WriteAsync(" * References: ");
            if (NameComparer.Equals(reference.Name, reference.Label))
                await writer.WriteLineAsync(reference.Label.SmartQuoteJson());
            else
            {
                await writer.WriteAsync(reference.Label.SmartQuoteJson());
                await writer.WriteAsync(" (");
                await writer.WriteAsync(reference.Name);
                await writer.WriteLineAsync(")");
            }
        }
        SysPackage? package = await entry.GetReferencedEntityAsync(e => e.Package, cancellationToken);
        if (package is not null && (context.Package is null || !NameComparer.Equals(package.Name, context.Package)))
        {
            if (appendLine)
            {
                await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                appendLine = false;
            }
            await writer.WriteAsync(" * Package: ");
            await writer.WriteLineAsync(package.Name);
            if (!string.IsNullOrWhiteSpace(package.ShortDescription))
            {
                string[] lines = package.ShortDescription.Trim().SplitLines();
                if (NameComparer.Equals(lines[0], package.Name))
                {
                    if (lines.Length > 1)
                        lines = lines.Skip(1).ToArray();
                    else
                    {
                        await writer.WriteLineAsync(CLOSE_JSDOC);
                        return;
                    }
                }
                foreach (string s in lines.Select(s => s.TrimEnd()))
                {
                    if (s.Length > 0)
                    {
                        await writer.WriteAsync(START_INDENTED_JSDOC_LINE);
                        await writer.WriteLineAsync(s);
                    }
                    else
                        await writer.WriteLineAsync(EMPTY_JSDOC_LINE);
                }
            }
        }
        await writer.WriteLineAsync(CLOSE_JSDOC);
    }

    public RenderingService(ILogger<RenderingService> logger, IServiceProvider services, IOptions<AppSettings> appSettingsOptions)
    {
        _logger = logger;
        _scope = services.CreateScope();
        AppSettings appSettings = appSettingsOptions.Value;
        if (appSettings.ShowHelp())
        {
            _isScoped = _forceOverwrite = false;
            _outputFile = null;
            return;
        }
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
        _includeReferenced = appSettings.IncludeReferenced ?? false;
        _emitBaseTypes = appSettings.EmitBaseTypes ?? false;
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
/*
Are there any plans of what we could do if the next
What would stop places like Fulton County from having "130% voter turnout", and dems win anyway? I think we need to be prepared for that. We can't rely on senators save us from that.
*/