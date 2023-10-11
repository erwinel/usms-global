using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static SnTsTypeGenerator.SnApiConstants;

namespace SnTsTypeGenerator;

public sealed class ScopedTypingsRenderer : TypingsRenderer
{
    private string _currentScope;
    protected override string CurrentScope => _currentScope;

    public ScopedTypingsRenderer(string currentScope, IndentedTextWriter writer, TypingsDbContext dbContext) : base(writer, dbContext) => _currentScope = currentScope;

    public async override Task WriteAsync(EntityEntry<TableInfo>[] toRender, CancellationToken cancellationToken)
    {
        Writer.Indent = 0;
        foreach (EntityEntry<TableInfo> entry in toRender.Where(e => !string.IsNullOrEmpty(e.Entity.ScopeValue)))
        {
            SysScope? scope = await entry.GetReferencedEntityAsync(t => t.Scope, cancellationToken);
            if (scope is not null)
            {
                string shortDescription = scope.ShortDescription.AsWhitespaceNormalized();
                if (shortDescription.Length > 0 && _currentScope != shortDescription)
                {
                    if (scope.Name != _currentScope && shortDescription != scope.Name)
                        await Writer.WriteJsDocAsync(new string[] { $"Namspace for the {scope.Name} scope."}.Concat(scope.ShortDescription.SplitLines()), cancellationToken);
                    else
                        await Writer.WriteJsDocAsync(cancellationToken, $"Namspace for the {shortDescription} scope.");

                }
                else if (scope.Name != _currentScope)
                    await Writer.WriteJsDocAsync(cancellationToken, $"Namspace for the {scope.Name} scope.");
            }
        }
        await Writer.WriteLineAsync($"declare namespace {CurrentScope} {{");
        Writer.Indent = 1;
        await base.WriteAsync(toRender, cancellationToken); 
        Writer.Indent = 0;
        await Writer.WriteLineAsync("}");
    }

    protected override Task WriteStartRecordsNamespace() => Writer.WriteLineAsync($"export namespace {NS_NAME_record} {{");

    protected override Task WriteStartElementsNamespace() => Writer.WriteLineAsync($"export namespace {NS_NAME_element} {{");

    protected override Task WriteStartFieldsNamespace() => Writer.WriteLineAsync($"export namespace {NS_NAME_fields} {{");
}
