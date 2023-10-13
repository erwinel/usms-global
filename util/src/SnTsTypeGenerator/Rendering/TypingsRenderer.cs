using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using SnTsTypeGenerator.Models;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Rendering;

public abstract class TypingsRenderer
{
    protected abstract string CurrentScope { get; }
    
    protected IndentedTextWriter Writer { get; }

    protected TypingsDbContext DbContext { get; }

    protected abstract string GlideRecordNamespace { get; }

    protected abstract string GlideElementNamespace { get; }
    
    protected abstract string TableFieldsNamespace { get; }

    protected abstract string GetElementName(string typeName);
 
    protected abstract bool IsExplicitScalarType(string typeName);

    protected abstract Task WriteStartRecordsNamespace();
    
    protected abstract Task WriteStartElementsNamespace();
    
    protected abstract Task WriteStartFieldsNamespace();
    
    protected TypingsRenderer(IndentedTextWriter writer, TypingsDbContext dbContext) => (Writer, DbContext) = (writer, dbContext);

    public async virtual Task WriteAsync(EntityEntry<TableInfo>[] toRender, CancellationToken cancellationToken)
    {
        int indentLevel = Writer.Indent + 1;
        await WriteStartRecordsNamespace();
        await RenderRecordTypeAsync(toRender[0], indentLevel, cancellationToken);
        foreach (EntityEntry<TableInfo> entry in toRender.Skip(1))
        {
            await Writer.WriteLineAsync();
            await RenderRecordTypeAsync(entry, indentLevel, cancellationToken);
        }
        Writer.Indent = indentLevel - 1;
        await Writer.WriteLineAsync("}");
        
        await Writer.WriteLineAsync();
        await WriteStartElementsNamespace();
        await RenderElementTypeAsync(toRender[0].Entity, indentLevel, cancellationToken);
        foreach (EntityEntry<TableInfo> entry in toRender.Skip(1))
        {
            await Writer.WriteLineAsync();
            await RenderElementTypeAsync(entry.Entity, indentLevel, cancellationToken);
        }
        Writer.Indent = indentLevel - 1;
        await Writer.WriteLineAsync("}");
        
        await Writer.WriteLineAsync();
        await WriteStartFieldsNamespace();
        await RenderFieldsTypeAsync(toRender[0], indentLevel, cancellationToken);
        foreach (EntityEntry<TableInfo> entry in toRender.Skip(1))
        {
            await Writer.WriteLineAsync();
            await RenderFieldsTypeAsync(entry, indentLevel, cancellationToken);
        }
        Writer.Indent = indentLevel - 1;
        await Writer.WriteLineAsync("}");
    }

    private static IEnumerable<string> GetGlideRecordJsDocLines(TableInfo table, SysPackage? package)
    {
        // /**
        //  * "Guided Setup Content" glide record.
        //  * Scope: "Guided Setup - Legacy" (sn_guided_setup)
        //  */
        yield return $"{((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.";
        if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
            yield return $"Auto-number Prefix: {table.NumberPrefix}";
        if (table.IsExtendable)
            yield return "IsExtendable: true";
        if (package is null)
        {
            if (!string.IsNullOrWhiteSpace(table.PackageName))
                yield return $"Package: {table.PackageName.SmartQuoteJson()}";
        }
        else if (string.IsNullOrWhiteSpace(package.ShortDescription) || package.ShortDescription == package.Name)
            yield return $"Package: {package.Name.SmartQuoteJson()}";
        else
            yield return $"Package: {package.ShortDescription.SmartQuoteJson()} ({package.Name.SmartQuoteJson()})";
    }

    private async Task RenderRecordTypeAsync(EntityEntry<TableInfo> entry, int indentLevel, CancellationToken cancellationToken)
    {
        TableInfo table = entry.Entity;
        Writer.Indent = indentLevel;
        if (string.IsNullOrWhiteSpace(table.PackageName))
            await Writer.WriteJsDocAsync(GetGlideRecordJsDocLines(table, null), cancellationToken);
        else
            await Writer.WriteJsDocAsync(GetGlideRecordJsDocLines(table, await entry.GetReferencedEntityAsync(t => t.Package, cancellationToken)), cancellationToken);
        TableInfo? superClass = string.IsNullOrEmpty(table.SuperClassName) ? null : await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
        await Writer.WriteAsync("export type ");
        await Writer.WriteAsync(table.Name);
        await Writer.WriteAsync(" = ");
        await Writer.WriteAsync(TableFieldsNamespace);
        await Writer.WriteAsync(".");
        await Writer.WriteAsync(table.Name);
        await Writer.WriteAsync(" & ");
        await Writer.WriteAsync(table.Name);
        if (superClass is null)
            await Writer.WriteAsync(TS_NAME_GlideRecord);
        else
            await Writer.WriteAsync(superClass.GetInterfaceTypeString(CurrentScope));
        await Writer.WriteLineAsync(";");
    }

    private async Task RenderElementTypeAsync(TableInfo table, int indentLevel, CancellationToken cancellationToken)
    {
        Writer.Indent = indentLevel;
        await Writer.WriteJsDocAsync(cancellationToken, $"Element that refers to a {table.Label.SmartQuoteJson()} glide record.");
        await Writer.WriteAsync("export type ");
        await Writer.WriteAsync(table.Name);
        await Writer.WriteAsync(" = Reference<");
        await Writer.WriteAsync(TableFieldsNamespace);
        await Writer.WriteAsync(".");
        await Writer.WriteAsync(table.Name);
        await Writer.WriteAsync(", ");
        await Writer.WriteAsync(GlideRecordNamespace);
        await Writer.WriteAsync(".");
        await Writer.WriteAsync(table.Name);
        await Writer.WriteLineAsync(">;");
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

    private IEnumerable<string> GetElementJsDoc(ElementInfo elementInfo, GlideType? type, TableInfo? reference, SysPackage? package)
    {
        if (reference is null)
            yield return $"{elementInfo.Label.SmartQuoteJson()} element.";
        else
            yield return $"Element that refers to a {reference.Label.SmartQuoteJson()} glide record.";
        bool appendBlank = !string.IsNullOrWhiteSpace(elementInfo.Comments);
        if (appendBlank)
            foreach (string c in elementInfo.Comments!.Trim().SplitLines())
                yield return c;
        
        if (elementInfo.MaxLength.HasValue && reference is not null)
        {
            if (appendBlank)
            {
                yield return "";
                appendBlank = false;
            }
            if (IsExplicitScalarType(elementInfo.TypeName))
            {
                if (elementInfo.SizeClass.HasValue)
                {
                    if (type is null || type.Name == elementInfo.TypeName)
                        yield return $"Type: {elementInfo.TypeName}; Max Length: {elementInfo.MaxLength.Value}; Size class: {elementInfo.SizeClass.Value}";
                    else
                        yield return $"Type: {type.Name.SmartQuoteJson()} ({elementInfo.TypeName}); Max Length: {elementInfo.MaxLength.Value}; Size class: {elementInfo.SizeClass.Value}";
                }
                else if (type is null || type.Name == elementInfo.TypeName)
                    yield return $"Type: {elementInfo.TypeName}; Max Length: {elementInfo.MaxLength.Value}";
                else
                    yield return $"Type: {type.Name.SmartQuoteJson()} ({elementInfo.TypeName}); Max Length: {elementInfo.MaxLength.Value}";
            }
            else if (elementInfo.SizeClass.HasValue)
                yield return $"Max Length: {elementInfo.MaxLength.Value}; Size class: {elementInfo.SizeClass.Value}";
            else
                yield return $"Max Length: {elementInfo.MaxLength.Value}";
        }
        else if (IsExplicitScalarType(elementInfo.TypeName))
        {
            if (appendBlank)
            {
                yield return "";
                appendBlank = false;
            }
            if (elementInfo.SizeClass.HasValue && reference is not null)
            {
                if (type is null || type.Name == elementInfo.TypeName)
                    yield return $"Type: {elementInfo.TypeName}; Size class: {elementInfo.SizeClass.Value}";
                else
                    yield return $"Type: {type.Name.SmartQuoteJson()} ({elementInfo.TypeName}); Size class: {elementInfo.SizeClass.Value}";
            }
            else if (type is null || type.Name == elementInfo.TypeName)
                yield return $"Type: {elementInfo.TypeName}";
            else
                yield return $"Type: {type.Name.SmartQuoteJson()} ({elementInfo.TypeName})";
        }
        else if (elementInfo.SizeClass.HasValue && reference is not null)
        {
            if (appendBlank)
            {
                yield return "";
                appendBlank = false;
            }
            yield return $"Size class: {elementInfo.SizeClass.Value}";
        }
        string[] flags = GetFlags(elementInfo).ToArray();
        switch (flags.Length)
        {
            case 0:
                break;
            case 1:
                if (appendBlank)
                {
                    yield return "";
                    appendBlank = false;
                }
                yield return flags[0];
                break;
            default:
                if (appendBlank)
                {
                    yield return "";
                    appendBlank = false;
                }
                yield return string.Join("; ", flags);
                break;
        }
        if (!string.IsNullOrWhiteSpace(elementInfo.DefaultValue))
        {
            if (appendBlank)
            {
                yield return "";
                appendBlank = false;
            }
            yield return $"Default: {elementInfo.DefaultValue.SmartQuoteJson()}";
        }
        if (package is null)
        {
            if (!string.IsNullOrWhiteSpace(elementInfo.PackageName))
            {
                if (appendBlank)
                    yield return "";
                yield return $"Package: {elementInfo.PackageName.SmartQuoteJson()}";
            }
        }
        else
        {
            if (appendBlank)
                yield return "";
            if (string.IsNullOrWhiteSpace(package.ShortDescription) || package.ShortDescription == package.Name)
                yield return $"Package: {package.Name.SmartQuoteJson()}";
            else
                yield return $"Package: {package.ShortDescription.SmartQuoteJson()} ({package.Name.SmartQuoteJson()})";
        }
    }

    private async Task RenderFieldsTypeAsync(EntityEntry<TableInfo> entry, int indentLevel, CancellationToken cancellationToken)
    {
        Writer.Indent = indentLevel++;
        TableInfo table = entry.Entity;
        await Writer.WriteJsDocAsync(cancellationToken,
            $"{table.Label.SmartQuoteJson()} glide record fields.",
            $"@see {{@link {GlideRecordNamespace}.{table.Name}}}",
            $"@see {{@link {GlideElementNamespace}.{table.Name}}}");
        ElementInfo[] elements = (await entry.GetRelatedCollectionAsync(t => t.Elements, cancellationToken)).ToArray();
        (IEnumerable<ElementInheritance> inheritances, bool extendsBaseRecord) = await entry.GetElementInheritancesAsync(cancellationToken);
        Collection<ElementInfo> commentOnlyElements = new();
        Collection<(ElementInfo Element, bool IsNew)> toRender = new();
        foreach (ElementInheritance i in inheritances)
        {
            if (i.Super is null)
                toRender.Add((i.Element, false));
            else if (i.Element.Overrides(i.Super, out bool isNew))
            {
                if (isNew)
                    toRender.Add((i.Element, true));
                else
                    commentOnlyElements.Add(i.Element);
            }
        }
        TableInfo? superClass = table.SuperClass;
        await Writer.WriteAsync("export interface ");
        await Writer.WriteAsync(table.Name);
        if (extendsBaseRecord)
        {
            await Writer.WriteAsync(" extends ");
            await Writer.WriteAsync(TS_NAME_BASERECORD);
        }
        else if (superClass is not null)
        {
            await Writer.WriteAsync(" extends ");
            await Writer.WriteAsync(superClass.GetInterfaceTypeString(CurrentScope));

        }
        if (toRender.Count == 0 && commentOnlyElements.Count == 0)
        {
            await Writer.WriteLineAsync(" { }");
            return;
        }
        await Writer.WriteLineAsync(" {");
        if (commentOnlyElements.Count > 0)
        {
            await RenderJsDocOnlyElementAsync(commentOnlyElements[0], indentLevel, cancellationToken);
            foreach (ElementInfo element in commentOnlyElements.Skip(1))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                await Writer.WriteLineAsync();
                await RenderJsDocOnlyElementAsync(element, indentLevel, cancellationToken);
            }
            foreach ((ElementInfo element, bool isNew) in toRender)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                await Writer.WriteLineAsync();
                await RenderElementAsync(element, isNew, indentLevel, cancellationToken);
            }
        }
        else if (toRender.Count > 0)
        {
            var ie = toRender[0];
            await RenderElementAsync(ie.Element, ie.IsNew, indentLevel, cancellationToken);
            foreach ((ElementInfo element, bool isNew) in toRender.Skip(1))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                await Writer.WriteLineAsync();
                await RenderElementAsync(element, isNew, indentLevel, cancellationToken);
            }
        }
        foreach (ElementInfo element in elements)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            EntityEntry<ElementInfo> ee = DbContext.Elements.Entry(element);
            TableInfo? reference = string.IsNullOrEmpty(element.RefTableName) ? null : await ee.GetReferencedEntityAsync(t => t.Reference, cancellationToken);
            await Writer.WriteJsDocAsync(GetElementJsDoc(element, await ee.GetReferencedEntityAsync(t => t.Type, cancellationToken), reference,
                string.IsNullOrEmpty(table.PackageName) ? null : await ee.GetReferencedEntityAsync(t => t.Package, cancellationToken)), cancellationToken);
            await Writer.WriteAsync(element.Name);
            await Writer.WriteAsync(": ");
            if (reference is null)
                await Writer.WriteAsync(GetElementName(element.TypeName));
            else
                await Writer.WriteAsync(reference.GetGlideElementTypeString(CurrentScope));
            await Writer.WriteLineAsync(";");
        }
        Writer.Indent = indentLevel - 1;
        await Writer.WriteLineAsync("}");
    }

    private IEnumerable<string> GetFullElementJsDoc(ElementInfo element, GlideType? type, TableInfo? reference, SysPackage? package)
    {
        string[] description = GetElementJsDoc(element, type, reference, package).ToArray();
        string code = $"@property {{{GetElementName(element.TypeName)}}} {element.Name}";
        if (description.Length == 0)
            yield return code;
        else
        {
            yield return $"{code} - {description[0]}";
            if (description.Length > 1)
            {
                string typeString = GetElementName(element.TypeName);
                string indent = new(' ', typeString.Length + element.Name.Length + 16);
                foreach (string d in description.Skip(1))
                    yield return indent + d;
            }
        }
    }

    private async Task RenderJsDocOnlyElementAsync(ElementInfo element, int indentLevel, CancellationToken cancellationToken)
    {
        Writer.Indent = indentLevel;
        if (cancellationToken.IsCancellationRequested)
            return;
        EntityEntry<ElementInfo> ee = DbContext.Elements.Entry(element);
        TableInfo? reference = string.IsNullOrEmpty(element.RefTableName) ? null : await ee.GetReferencedEntityAsync(t => t.Reference, cancellationToken);
        await Writer.WriteJsDocAsync(GetFullElementJsDoc(element, await ee.GetReferencedEntityAsync(t => t.Type, cancellationToken), reference,
            string.IsNullOrEmpty(element.PackageName) ? null : await ee.GetReferencedEntityAsync(t => t.Package, cancellationToken)), cancellationToken);
    }

    private async Task RenderElementAsync(ElementInfo element, bool isNew, int indentLevel, CancellationToken cancellationToken)
    {
        Writer.Indent = indentLevel;
        if (cancellationToken.IsCancellationRequested)
            return;
        EntityEntry<ElementInfo> ee = DbContext.Elements.Entry(element);
        TableInfo? reference = string.IsNullOrEmpty(element.RefTableName) ? null : await ee.GetReferencedEntityAsync(t => t.Reference, cancellationToken);
        await Writer.WriteJsDocAsync(GetElementJsDoc(element, await ee.GetReferencedEntityAsync(t => t.Type, cancellationToken), reference,
            string.IsNullOrEmpty(element.PackageName) ? null : await ee.GetReferencedEntityAsync(t => t.Package, cancellationToken)), cancellationToken);
        if (isNew)
            Writer.Write("new ");
        await Writer.WriteAsync(element.Name);
        await Writer.WriteAsync(": ");
        if (reference is null)
            await Writer.WriteAsync(GetElementName(element.TypeName));
        else
            await Writer.WriteAsync(reference.GetGlideElementTypeString(CurrentScope));
        await Writer.WriteAsync(";");
    }
}
