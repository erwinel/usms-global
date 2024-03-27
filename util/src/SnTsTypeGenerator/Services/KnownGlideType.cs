namespace SnTsTypeGenerator.Services;

public class KnownGlideType
{
    /// <summary>
    /// Gets the internal Glide type name.
    /// </summary>
    /// <value>The value of the "Name" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column from the <see cref="Services.SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" /> table.</value>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Gets the default display name.
    /// </summary>
    /// <value>The optional fallback type display name to use when the corresponding <see cref="Models.GlideType"/> does not exist in the database.</value>
    public string? Label { get; set; }
    
    /// <summary>
    /// Gets the default scalar type.
    /// </summary>
    /// <value>The optional fallback scalar type name to use when the corresponding <see cref="Models.GlideType"/> does not exist in the database.</value>
    public string? ScalarType { get; set; }
    
    /// <summary>
    /// Gets the default scalar length.
    /// </summary>
    /// <value>The optional scalar length to use when the corresponding <see cref="Models.GlideType"/> does not exist in the database.</value>
    public int? ScalarLength { get; set; }
    
    /// <summary>
    /// Gets the TypeScript GlideElement type name used in globally-scoped scripts.
    /// </summary>
    /// <value>The TypeScript name to use when referring to the corresponding GlideElement object in globally-scoped scripts or <see langword="null"/> to use the default type name.</value>
    /// <remarks>This is referenced when adding a new corresponding row to the <see cref="Models.GlideType"/> table,
    /// must correspond to the name of a TypeScript definition that is assignable from the 'GlideElement' type,
    /// and is defined in any of the <c>*.d.ts</c> files in the <c>Resources/ts/global</c> folder.</remarks>
    public string? GlobalElementType { get; set; }
    
    /// <summary>
    /// Gets the TypeScript GlideElement type name used in scoped applications.
    /// </summary>
    /// <value>The TypeScript name to use when referring to the corresponding GlideElement object in scoped applications or <see langword="null"/> to use the default type name.</value>
    /// <remarks>This is referenced when adding a new corresponding row to the <see cref="Models.GlideType"/> table,
    /// must correspond to the name of a TypeScript definition that is assignable from the 'GlideElement' type,
    /// and is defined in any of the <c>*.d.ts</c> files in the <c>Resources/ts/scoped</c> folder.</remarks>
    public string? ScopedElementType { get; set; }
 
    public static IEnumerable<KnownGlideType> GetDefaultKnownTypes()
    {
        yield return new KnownGlideType{ Name = "approval_rules" };
        yield return new KnownGlideType{ Name = "audio", GlobalElementType = "GlideElementAudio" };
        yield return new KnownGlideType{ Name = "auto_increment", ScalarType = "longint" };
        yield return new KnownGlideType{ Name = "boolean", ScalarType = "boolean", GlobalElementType = "GlideElementBoolean" };
        yield return new KnownGlideType{ Name = "auto_number" };
        yield return new KnownGlideType{ Name = "bootstrap_color" };
        yield return new KnownGlideType{ Name = "calendar_date_time", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "catalog_preview" };
        yield return new KnownGlideType{ Name = "char", ScalarType = "GUID" };
        yield return new KnownGlideType{ Name = "choice" };
        yield return new KnownGlideType{ Name = "collection" };
        yield return new KnownGlideType{ Name = "color" };
        yield return new KnownGlideType{ Name = "color_display" };
        yield return new KnownGlideType{ Name = "composite_field" };
        yield return new KnownGlideType{ Name = "composite_name" };
        yield return new KnownGlideType{ Name = "compressed", GlobalElementType = "GlideElementCompressed" };
        yield return new KnownGlideType{ Name = "condition_string" };
        yield return new KnownGlideType{ Name = "conditions", GlobalElementType = "GlideElementConditions" };
        yield return new KnownGlideType{ Name = "counter", GlobalElementType = "GlideElementCounter" };
        yield return new KnownGlideType{ Name = "css" };
        yield return new KnownGlideType{ Name = "currency", ScalarType = "decimal", GlobalElementType = "GlideElementCurrency" };
        yield return new KnownGlideType{ Name = "currency2", ScalarType = "GUID" };
        yield return new KnownGlideType{ Name = "data_array", GlobalElementType = "GlideElementDataArray" };
        yield return new KnownGlideType{ Name = "data_object", GlobalElementType = "GlideElementDataObject" };
        yield return new KnownGlideType{ Name = "data_structure", GlobalElementType = "GlideElementDataStructure" };
        yield return new KnownGlideType{ Name = "date", ScalarType = "date", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "datetime", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "day_of_week", ScalarType = "integer", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "days_of_week" };
        yield return new KnownGlideType{ Name = "decimal", ScalarType = "decimal", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "document_id", ScalarType = "GUID", GlobalElementType = "GlideElementDocumentId" };
        yield return new KnownGlideType{ Name = "documentation_field", GlobalElementType = "GlideElementDocumentation" };
        yield return new KnownGlideType{ Name = "domain_id", ScalarType = "GUID", GlobalElementType = "GlideElementDomainId" };
        yield return new KnownGlideType{ Name = "domain_path" };
        yield return new KnownGlideType{ Name = "due_date", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "email" };
        yield return new KnownGlideType{ Name = "email_script" };
        yield return new KnownGlideType{ Name = "expression" };
        yield return new KnownGlideType{ Name = "external_names" };
        yield return new KnownGlideType{ Name = "field_list" };
        yield return new KnownGlideType{ Name = "field_name" };
        yield return new KnownGlideType{ Name = "file_attachment", GlobalElementType = "GlideElementFileAttachment" };
        yield return new KnownGlideType{ Name = "float", ScalarType = "float", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "geo_point", GlobalElementType = "GlideElementGeoPoint" };
        yield return new KnownGlideType{ Name = "glide_action_list", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_date_time", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_date", ScalarType = "date", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_duration", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_list", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_precise_time" };
        yield return new KnownGlideType{ Name = "glide_time", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_utc_time", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "glide_var", GlobalElementType = "GlideElementGlideVar" };
        yield return new KnownGlideType{ Name = "glyphicon" };
        yield return new KnownGlideType{ Name = "graphql_schema", GlobalElementType = "GlideElementGraphQLSchema" };
        yield return new KnownGlideType{ Name = "GUID" };
        yield return new KnownGlideType{ Name = "html", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "html_script" };
        yield return new KnownGlideType{ Name = "html_template" };
        yield return new KnownGlideType{ Name = "icon" };
        yield return new KnownGlideType{ Name = "image" };
        yield return new KnownGlideType{ Name = "index_name" };
        yield return new KnownGlideType{ Name = "insert_timestamp", ScalarType = "datetime" };
        yield return new KnownGlideType{ Name = "int" };
        yield return new KnownGlideType{ Name = "integer_date", ScalarType = "integer", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "integer_time", ScalarType = "integer", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "integer", ScalarType = "integer", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "internal_type", GlobalElementType = "GlideElementInternalType" };
        yield return new KnownGlideType{ Name = "ip_addr", GlobalElementType = "GlideElementIPAddress" };
        yield return new KnownGlideType{ Name = "ip_address" };
        yield return new KnownGlideType{ Name = "journal_input" };
        yield return new KnownGlideType{ Name = "journal_list" };
        yield return new KnownGlideType{ Name = "journal" };
        yield return new KnownGlideType{ Name = "json_translations" };
        yield return new KnownGlideType{ Name = "json" };
        yield return new KnownGlideType{ Name = "language" };
        yield return new KnownGlideType{ Name = "long" };
        yield return new KnownGlideType{ Name = "longint", ScalarType = "longint" };
        yield return new KnownGlideType{ Name = "mask_code" };
        yield return new KnownGlideType{ Name = "metric_absolute", ScalarType = "float", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "metric_counter", ScalarType = "float", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "metric_derive", ScalarType = "float", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "metric_gauge", ScalarType = "float", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "mid_config" };
        yield return new KnownGlideType{ Name = "month_of_year", ScalarType = "integer", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "multi_small" };
        yield return new KnownGlideType{ Name = "multi_two_lines" };
        yield return new KnownGlideType{ Name = "name_values", GlobalElementType = "GlideElementNameValue" };
        yield return new KnownGlideType{ Name = "nds_icon" };
        yield return new KnownGlideType{ Name = "nl_task_int1", ScalarType = "integer", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "order_index", ScalarType = "integer", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "password", GlobalElementType = "GlideElementPassword" };
        yield return new KnownGlideType{ Name = "password2", GlobalElementType = "GlideElementPassword2" };
        yield return new KnownGlideType{ Name = "percent_complete", ScalarType = "decimal", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "ph_number" };
        yield return new KnownGlideType{ Name = "phone_number_e164", GlobalElementType = "GlideElementPhoneNumber" };
        yield return new KnownGlideType{ Name = "phone_number" };
        yield return new KnownGlideType{ Name = "price", ScalarType = "decimal", GlobalElementType = "GlideElementPrice" };
        yield return new KnownGlideType{ Name = "properties" };
        yield return new KnownGlideType{ Name = "radio" };
        yield return new KnownGlideType{ Name = "records" };
        yield return new KnownGlideType{ Name = "reference_name" };
        yield return new KnownGlideType{ Name = "reference", ScalarType = "GUID", GlobalElementType = "GlideElementReference" };
        yield return new KnownGlideType{ Name = "related_tags", GlobalElementType = "GlideElementRelatedTags" };
        yield return new KnownGlideType{ Name = "reminder_field_name" };
        yield return new KnownGlideType{ Name = "repeat_count", ScalarType = "integer", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "repeat_type" };
        yield return new KnownGlideType{ Name = "replication_payload", GlobalElementType = "GlideElementReplicationPayload" };
        yield return new KnownGlideType{ Name = "schedule_date_time", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "schedule_interval_count", ScalarType = "integer", GlobalElementType = "GlideElementNumeric" };
        yield return new KnownGlideType{ Name = "script_client" };
        yield return new KnownGlideType{ Name = "script_plain", GlobalElementType = "GlideElementScript" };
        yield return new KnownGlideType{ Name = "script_server" };
        yield return new KnownGlideType{ Name = "script", GlobalElementType = "GlideElementScript" };
        yield return new KnownGlideType{ Name = "short_field_name", GlobalElementType = "GlideElementShortFieldName" };
        yield return new KnownGlideType{ Name = "short_table_name", GlobalElementType = "GlideElementShortTableName" };
        yield return new KnownGlideType{ Name = "simple_name_values", GlobalElementType = "GlideElementSimpleNameValue" };
        yield return new KnownGlideType{ Name = "script_server" };
        yield return new KnownGlideType{ Name = "slushbucket" };
        yield return new KnownGlideType{ Name = "source_id", ScalarType = "GUID" };
        yield return new KnownGlideType{ Name = "source_id", ScalarType = "GUID", GlobalElementType = "GlideElementSourceId" };
        yield return new KnownGlideType{ Name = "source_name", GlobalElementType = "GlideElementSourceName" };
        yield return new KnownGlideType{ Name = "source_table", GlobalElementType = "GlideElementSourceTable" };
        yield return new KnownGlideType{ Name = "string_boolean", GlobalElementType = "StringBoolean" };
        yield return new KnownGlideType{ Name = "string_full_utf8", GlobalElementType = "GlideElementFullUTF8" };
        yield return new KnownGlideType{ Name = "string" };
        yield return new KnownGlideType{ Name = "structure" };
        yield return new KnownGlideType{ Name = "sys_class_name", GlobalElementType = "GlideElementSysClassName" };
        yield return new KnownGlideType{ Name = "sys_class_path" };
        yield return new KnownGlideType{ Name = "sysevent_name" };
        yield return new KnownGlideType{ Name = "sysrule_field_name" };
        yield return new KnownGlideType{ Name = "table_name" };
        yield return new KnownGlideType{ Name = "template_value", GlobalElementType = "GlideElementWorkflowConditions" };
        yield return new KnownGlideType{ Name = "time", ScalarType = "time", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "timer", ScalarType = "datetime", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "translated_field", GlobalElementType = "GlideElementTranslatedField" };
        yield return new KnownGlideType{ Name = "translated_html", GlobalElementType = "GlideElementTranslatedHTML" };
        yield return new KnownGlideType{ Name = "translated_text", GlobalElementType = "GlideElementTranslatedText" };
        yield return new KnownGlideType{ Name = "translated", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "tree_code" };
        yield return new KnownGlideType{ Name = "tree_path" };
        yield return new KnownGlideType{ Name = "url", GlobalElementType = "GlideElementURL" };
        yield return new KnownGlideType{ Name = "user_image", GlobalElementType = "GlideElementUserImage" };
        yield return new KnownGlideType{ Name = "user_input" };
        yield return new KnownGlideType{ Name = "user_roles" };
        yield return new KnownGlideType{ Name = "variable_conditions", GlobalElementType = "GlideElementVariableConditions" };
        yield return new KnownGlideType{ Name = "variable_template_value", GlobalElementType = "GlideElementVariableTemplateValue" };
        yield return new KnownGlideType{ Name = "variables", GlobalElementType = "GlideElementVariables" };
        yield return new KnownGlideType{ Name = "version" };
        yield return new KnownGlideType{ Name = "video", GlobalElementType = "GlideElementVideo" };
        yield return new KnownGlideType{ Name = "week_of_month", ScalarType = "integer", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "wide_text" };
        yield return new KnownGlideType{ Name = "wiki_text", GlobalElementType = "GlideElementWikiText" };
        yield return new KnownGlideType{ Name = "wms_job", GlobalElementType = "GlideElementGlideObject" };
        yield return new KnownGlideType{ Name = "workflow_conditions", GlobalElementType = "GlideElementWorkflowConditions" };
        yield return new KnownGlideType{ Name = "workflow", GlobalElementType = "GlideElementWorkflow" };
        yield return new KnownGlideType{ Name = "xml",GlobalElementType = "GlideElementScript" };
    }
}