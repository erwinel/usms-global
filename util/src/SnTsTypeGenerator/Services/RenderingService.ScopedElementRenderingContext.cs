using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

public partial class RenderingService
{
    class ScopedElementRenderingContext : IElementRenderingContext
    {
        public string Scope { get; }

        public string? Package { get; }

        int IElementRenderingContext.IndentLevel => 3;

        public string GetElementName(string typeName) => typeName switch
        {
            TYPE_NAME_journal or TYPE_NAME_glide_list or TYPE_NAME_glide_action_list or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list => "JournalGlideElement",

            TYPE_NAME_glide_date_time or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or
                TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time => "GlideDateTimeElement",

            TYPE_NAME_reference or TYPE_NAME_currency2 or TYPE_NAME_domain_id or TYPE_NAME_document_id or TYPE_NAME_source_id => TS_NAME_GlideElementReference,
            _ => TS_NAME_GlideElement
        };

        public bool IsExplicitScalarType(string typeName) => typeName switch
        {
            TYPE_NAME_glide_list or TYPE_NAME_glide_action_list or TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list or TYPE_NAME_glide_date or TYPE_NAME_glide_time or TYPE_NAME_timer or
                TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time or TYPE_NAME_currency2 or TYPE_NAME_domain_id or
                TYPE_NAME_document_id or TYPE_NAME_source_id => true,

            _ => false
        };

        internal ScopedElementRenderingContext(string scope, string? package) => (Scope, Package) = (scope, package);
    }
}
/*
Are there any plans of what we could do if the next
What would stop places like Fulton County from having "130% voter turnout", and dems win anyway? I think we need to be prepared for that. We can't rely on senators save us from that.
*/