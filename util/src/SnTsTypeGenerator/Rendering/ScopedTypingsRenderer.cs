using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Rendering;

public sealed class ScopedTypingsRenderer : TypingsRenderer
{
    private string _currentScope;
    protected override string CurrentScope => _currentScope;

    protected override string GlideRecordNamespace => NS_NAME_record;

    protected override string GlideElementNamespace => NS_NAME_element;

    protected override string TableFieldsNamespace => NS_NAME_fields;

    public ScopedTypingsRenderer(string currentScope, IndentedTextWriter writer, Services.TypingsDbContext dbContext) : base(writer, dbContext) => _currentScope = currentScope;

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

    protected override string GetElementName(string typeName) => typeName switch
    {
        TYPE_NAME_journal or TYPE_NAME_glide_list or TYPE_NAME_glide_action_list or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list => "JournalGlideElement",

        TYPE_NAME_glide_date_time or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or
            TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time => "GlideDateTimeElement",

        TYPE_NAME_reference or TYPE_NAME_currency2 or TYPE_NAME_domain_id or TYPE_NAME_document_id or TYPE_NAME_source_id => TS_NAME_GlideElementReference,
        _ => TS_NAME_GlideElement,
    };

    protected override bool IsExplicitScalarType(string typeName) => typeName switch
    {
        TYPE_NAME_glide_list or TYPE_NAME_glide_action_list or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or
            TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time or TYPE_NAME_currency2 or TYPE_NAME_domain_id or
            TYPE_NAME_document_id or TYPE_NAME_source_id => true,

        _ => false,
    };
}
