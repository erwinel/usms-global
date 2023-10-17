using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

public partial class RenderingService
{
    class GlobalElementRenderingContext : IElementRenderingContext
    {
        public string? Package { get; }

        int IElementRenderingContext.IndentLevel => 3;

        string? IElementRenderingContext.Scope => null;

        public GlobalElementRenderingContext(string? package) => Package = package;
        
        public string GetElementName(string typeName) => typeName switch
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
        
        public bool IsExplicitScalarType(string typeName) => typeName switch
        {
            TYPE_NAME_decimal or TYPE_NAME_float or TYPE_NAME_percent_complete or TYPE_NAME_order_index or TYPE_NAME_longint or TYPE_NAME_script_plain or TYPE_NAME_xml or TYPE_NAME_glide_date or
                TYPE_NAME_glide_time or TYPE_NAME_timer or TYPE_NAME_glide_duration or TYPE_NAME_glide_utc_time or TYPE_NAME_due_date or TYPE_NAME_glide_precise_time or TYPE_NAME_calendar_date_time or
                TYPE_NAME_user_input or TYPE_NAME_journal_input or TYPE_NAME_journal_list or TYPE_NAME_html or TYPE_NAME_glide_list or TYPE_NAME_journal or TYPE_NAME_glide_action_list or TYPE_NAME_date or
                TYPE_NAME_day_of_week or TYPE_NAME_month_of_year or TYPE_NAME_week_of_month or TYPE_NAME_caller_phone_number or TYPE_NAME_phone_number_e164 => true,

            _ => false
        };
    }
}
