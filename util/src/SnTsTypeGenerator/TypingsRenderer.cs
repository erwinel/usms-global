using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SnTsTypeGenerator;

public abstract class TypingsRenderer
{
    protected abstract string CurrentScope { get; }
    
    protected IndentedTextWriter Writer { get; }

    protected TypingsDbContext DbContext { get; }

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
        await RenderElementTypeAsync(toRender[0], indentLevel, cancellationToken);
        foreach (EntityEntry<TableInfo> entry in toRender.Skip(1))
        {
            await Writer.WriteLineAsync();
            await RenderElementTypeAsync(entry, indentLevel, cancellationToken);
        }
        Writer.Indent = indentLevel - 1;
        await Writer.WriteLineAsync("}");
        
        await Writer.WriteLineAsync();
        await WriteStartFieldsNamespace();
        await RenderFieldTypeAsync(toRender[0], indentLevel, cancellationToken);
        foreach (EntityEntry<TableInfo> entry in toRender.Skip(1))
        {
            await Writer.WriteLineAsync();
            await RenderFieldTypeAsync(entry, indentLevel, cancellationToken);
        }
        Writer.Indent = indentLevel - 1;
        await Writer.WriteLineAsync("}");
    }

    private static IEnumerable<string> GetGlideRecordJsDocLines(TableInfo table, SysPackage? package)
    {
        yield return $"{((string.IsNullOrWhiteSpace(table.Label) || table.Label == table.Name) ? table.Name : table.Label.SmartQuoteJson())} glide record.";
        if (!string.IsNullOrWhiteSpace(table.NumberPrefix))
            yield return $"Auto-number Prefix: {table.NumberPrefix}";
        if (table.IsExtendable)
            yield return "IsExtendable: true";
        if (package is not null)
        {
            if (string.IsNullOrWhiteSpace(package.ShortDescription) || package.ShortDescription == package.Name)
                yield return $"Package: {package.Name.SmartQuoteJson()}";
            else
                yield return $"Package: {package.ShortDescription.SmartQuoteJson()} ({package.Name.SmartQuoteJson()})";
        }
        else if (!string.IsNullOrWhiteSpace(table.PackageName))
            yield return $"Package: {table.PackageName.SmartQuoteJson()}";
    }

    private async Task RenderRecordTypeAsync(EntityEntry<TableInfo> entry, int indentLevel, CancellationToken cancellationToken)
    {
        TableInfo table = entry.Entity;
        Writer.Indent = indentLevel;
        if (!string.IsNullOrWhiteSpace(table.PackageName))
            await Writer.WriteJsDocAsync(GetGlideRecordJsDocLines(table, await entry.GetReferencedEntityAsync(t => t.Package, cancellationToken)), cancellationToken);
        else
            await Writer.WriteJsDocAsync(GetGlideRecordJsDocLines(table, null), cancellationToken);
        TableInfo? superClass = string.IsNullOrEmpty(table.SuperClassName) ? null : await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
        throw new NotImplementedException();
    }

    private async Task RenderElementTypeAsync(EntityEntry<TableInfo> entry, int indentLevel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task RenderFieldTypeAsync(EntityEntry<TableInfo> entry, int indentLevel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected abstract Task WriteStartRecordsNamespace();
    protected abstract Task WriteStartElementsNamespace();
    protected abstract Task WriteStartFieldsNamespace();
}
