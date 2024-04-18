namespace SnTsTypeGenerator.Services;

public class KnownGlideType
{
    /// <summary>
    /// Gets the internal Glide type name.
    /// </summary>
    /// <value>The value of the "Name" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column from the <see cref="SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" /> table.</value>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets the name of the JavaScript class or interface representing the element type.
    /// </summary>
    public string JsClass { get; set; } = null!;

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
    /// Gets or sets the value representing the type of object returned by the getGlideObject() method, which may be derived from of the "Class name" (<see cref="SnApiConstants.JSON_KEY_CLASS_NAME" />) column.
    /// </summary>
    public string? UnderlyingType { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the "Visible" (<see cref="Services.SnApiConstants.JSON_KEY_VISIBLE" />) column.
    /// </summary>
    public bool? Visible { get; set; }

    /// <summary>
    /// Gets or sets the inverse value corresponding to the "Use original value" (<see cref="SnApiConstants.JSON_KEY_USE_ORIGINAL_VALUE" />) column.
    /// </summary>
    public bool? DoNotUseOriginalValue { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_CASE_SENSITIVE" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? CaseSensitive { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_ENCODE_UTF8" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? EncodeUtf8 { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_OMIT_SYS_ORIGINAL" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? OmitSysOriginal { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_EDGE_ENCRYPTION_ENABLED" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? EdgeEncryptionEnabled { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_SERIALIZER" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public string? Serializer { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_IS_MULTI_TEXT" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? IsMultiText { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_PDF_CELL_TYPE" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public string? PdfCellType { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_NO_SORT" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? NoSort { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_NO_DATA_REPLICATE" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? NoDataReplicate { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_NO_AUDIT" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool? NoAudit { get; set; }

    /// <summary>
    /// Gets the un-parsed attributes for the current Field class.
    /// </summary>
    /// <value>The un-parsed segments from the the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column of the <see cref="SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" /> table.</value>
    public List<string>? Attributes { get; set; }

    public static IEnumerable<KnownGlideType> GetDefaultKnownTypes()
    {
        yield return new KnownGlideType { Name = "action_conditions", Label = "Action Conditions", JsClass = "GlideElementActionConditions", ScalarType = "string" };
        yield return new KnownGlideType { Name = "audio", Label = "Audio", JsClass = "GlideElementAudio", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "auto_increment", Label = "Auto Increment", JsClass = "GlideElement", ScalarType = "longint" };
        yield return new KnownGlideType { Name = "boolean", Label = "True/False", JsClass = "GlideElementBoolean", ScalarType = "boolean", Visible = true };
        yield return new KnownGlideType { Name = "bootstrap_color", Label = "Bootstrap color", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "breakdown_element", Label = "Breakdown Element", JsClass = "GlideElementBreakdownElement", ScalarType = "GUID", ScalarLength = 32, Serializer = "com.snc.pa.dc.GlideBreakdownElementXMLSerializer" };
        yield return new KnownGlideType { Name = "calendar_date_time", Label = "Calendar Date/Time", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "CalendarDateTime", Visible = true };
        yield return new KnownGlideType { Name = "catalog_preview", Label = "Catalog Preview", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "char", Label = "Char", JsClass = "GlideElement", ScalarType = "GUID", ScalarLength = 32 };
        yield return new KnownGlideType { Name = "choice", Label = "Choice", JsClass = "GlideElement", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "color", Label = "Color", JsClass = "GlideElement", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "color_display", Label = "Color Display", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "composite_field", Label = "Composite Field", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 300 };
        yield return new KnownGlideType { Name = "composite_name", Label = "Composite Name", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "compressed", Label = "Compressed", JsClass = "GlideElementCompressed", ScalarType = "string" };
        yield return new KnownGlideType { Name = "conditions", Label = "Conditions", JsClass = "GlideElementConditions", ScalarType = "string", ScalarLength = 4000, Visible = true, DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "condition_string", Label = "Condition String", JsClass = "GlideElement", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "counter", Label = "Counter", JsClass = "GlideElementCounter", ScalarType = "string" };
        yield return new KnownGlideType { Name = "css", Label = "CSS", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "currency", Label = "Currency", JsClass = "GlideElementCurrency", ScalarType = "decimal", ScalarLength = 20, UnderlyingType = "Currency", Visible = true, EdgeEncryptionEnabled = true };
        yield return new KnownGlideType { Name = "currency2", Label = "FX Currency", JsClass = "GlideElementCurrency2", ScalarType = "GUID", ScalarLength = 32, Visible = true, EdgeEncryptionEnabled = true, Serializer = "com.glide.currency2.Currency2XMLSerializer" };
        yield return new KnownGlideType { Name = "data_array", Label = "Data Array", JsClass = "GlideElementDataArray", ScalarType = "string", ScalarLength = 65535, Attributes = new() { "slushbucket_ref_no_expand", "list_layout_ignore" } };
        yield return new KnownGlideType { Name = "data_object", Label = "Data Object", JsClass = "GlideElementDataObject", ScalarType = "string", ScalarLength = 65535, Attributes = new() { "slushbucket_ref_no_expand", "list_layout_ignore" } };
        yield return new KnownGlideType { Name = "date", Label = "Other Date", JsClass = "GlideElementGlideObject", ScalarType = "date", UnderlyingType = "GlideDate", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "datetime", Label = "Basic Date/Time", JsClass = "GlideElement", ScalarType = "datetime" };
        yield return new KnownGlideType { Name = "days_of_week", Label = "Days of Week", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "day_of_week", Label = "Day of Week", JsClass = "GlideElementGlideObject", ScalarType = "integer", UnderlyingType = "DayOfWeek" };
        yield return new KnownGlideType { Name = "decimal", Label = "Decimal", JsClass = "GlideElementNumeric", ScalarType = "decimal", ScalarLength = 15, Visible = true };
        yield return new KnownGlideType { Name = "decoration", Label = "Decoration", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 255 };
        yield return new KnownGlideType { Name = "documentation_field", Label = "Documentation Field", JsClass = "GlideElementDocumentation", ScalarType = "string", ScalarLength = 80, UnderlyingType = "GlideElementTranslatedField" };
        yield return new KnownGlideType { Name = "document_id", Label = "Document ID", JsClass = "GlideElementDocumentId", ScalarType = "GUID", ScalarLength = 32, Visible = true };
        yield return new KnownGlideType { Name = "domain_id", Label = "Domain ID", JsClass = "GlideElementDomainId", ScalarType = "GUID", ScalarLength = 32, Visible = true };
        yield return new KnownGlideType { Name = "domain_path", Label = "Domain Path", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 255, CaseSensitive = true };
        yield return new KnownGlideType { Name = "due_date", Label = "Due Date", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "GlideDueDate", Visible = true, DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "email", Label = "Email", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "email_script", Label = "Email Script", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "expression", Label = "Expression", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "external_names", Label = "External Names", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "field_list", Label = "Field List", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000, IsMultiText = false };
        yield return new KnownGlideType { Name = "field_name", Label = "Field Name", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 80, Visible = true };
        yield return new KnownGlideType { Name = "file_attachment", Label = "File Attachment", JsClass = "GlideElementFileAttachment", ScalarType = "string", UnderlyingType = "FileAttachment", Visible = true };
        yield return new KnownGlideType { Name = "float", Label = "Floating Point Number", JsClass = "GlideElementNumeric", ScalarType = "float", Visible = true };
        yield return new KnownGlideType { Name = "formula", Label = "Formula", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "geo_point", Label = "Geo Point", JsClass = "GlideElementGeoPoint", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "glide_action_list", Label = "UI Action List", JsClass = "GlideElementGlideObject", ScalarType = "string", ScalarLength = 1024, UnderlyingType = "GlideActionList", DoNotUseOriginalValue = true, NoSort = true, Attributes = new() { "slushbucket_ref_no_expand" } };
        yield return new KnownGlideType { Name = "glide_date", Label = "Date", JsClass = "GlideElementGlideObject", ScalarType = "date", UnderlyingType = "GlideDate", Visible = true };
        yield return new KnownGlideType { Name = "glide_date_time", Label = "Date/Time", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "GlideDateTime", Visible = true };
        yield return new KnownGlideType { Name = "glide_duration", Label = "Duration", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "GlideDuration", Visible = true };
        yield return new KnownGlideType { Name = "glide_list", Label = "List", JsClass = "GlideElementGlideObject", ScalarType = "string", ScalarLength = 1024, UnderlyingType = "GlideList", Visible = true, DoNotUseOriginalValue = true, NoSort = true, Attributes = new() { "slushbucket_ref_no_expand" } };
        yield return new KnownGlideType { Name = "glide_precise_time", Label = "Precise Time", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "GlidePreciseTime" };
        yield return new KnownGlideType { Name = "glide_time", Label = "Time", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "GlideTime", Visible = true };
        yield return new KnownGlideType { Name = "glide_utc_time", Label = "UTC Time", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "GlideUTCTime", Visible = true };
        yield return new KnownGlideType { Name = "glide_var", Label = "Glide Var", JsClass = "GlideElementGlideVar", ScalarType = "string", UnderlyingType = "com.glide.vars.GlideElementGlideVar", Serializer = "com.glide.vars.VariableValueXMLSerializer", Attributes = new() { "slushbucket_ref_no_expand", "list_layout_ignore", "record_watcher_blacklist" } };
        yield return new KnownGlideType { Name = "glyphicon", Label = "Glyph Icon (Bootstrap)", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "graphql_schema", Label = "GraphQL Schema", JsClass = "GlideElementGraphQLSchema", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "GUID", Label = "Sys ID (GUID)", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 32 };
        yield return new KnownGlideType { Name = "html", Label = "HTML", JsClass = "GlideElementGlideObject", ScalarType = "string", ScalarLength = 65536, UnderlyingType = "GlideHTML", Visible = true };
        yield return new KnownGlideType { Name = "html_script", Label = "HTML Script", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "html_template", Label = "HTML Template", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 65000 };
        yield return new KnownGlideType { Name = "icon", Label = "Icon", JsClass = "GlideElementIcon", ScalarType = "string", ScalarLength = 100, Visible = true };
        yield return new KnownGlideType { Name = "image", Label = "Basic Image", JsClass = "GlideElement", ScalarType = "string", PdfCellType = "pdf_cell_type" };
        yield return new KnownGlideType { Name = "int", Label = "Integer String", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "integer", Label = "Integer", JsClass = "GlideElementNumeric", ScalarType = "integer", Visible = true };
        yield return new KnownGlideType { Name = "integer_date", Label = "Integer Date", JsClass = "GlideElementGlideObject", ScalarType = "integer", UnderlyingType = "IntegerDate", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "integer_time", Label = "Integer Time", JsClass = "GlideElementGlideObject", ScalarType = "integer", UnderlyingType = "IntegerTime", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "internal_type", Label = "Internal Type", JsClass = "GlideElementInternalType", ScalarType = "string" };
        yield return new KnownGlideType { Name = "ip_addr", Label = "IP Address (Validated IPV4, IPV6)", JsClass = "GlideElementIPAddress", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "ip_address", Label = "IP Address", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "journal", Label = "Journal", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "Journal", Visible = true };
        yield return new KnownGlideType { Name = "journal_input", Label = "Journal Input", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "Journal", Visible = true, DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "journal_list", Label = "Journal List", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "Journal", Visible = true, DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "json", Label = "JSON", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "json_translations", Label = "JSON Translations", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "long", Label = "Long Integer String", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 19 };
        yield return new KnownGlideType { Name = "longint", Label = "Long", JsClass = "GlideElement", ScalarType = "longint", ScalarLength = 19, Visible = true };
        yield return new KnownGlideType { Name = "month_of_year", Label = "Month of Year", JsClass = "GlideElementGlideObject", ScalarType = "integer", UnderlyingType = "MonthOfYear" };
        yield return new KnownGlideType { Name = "multi_small", Label = "Multiple Line Small Text Area", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "multi_two_lines", Label = "Two Line Text Area", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "name_values", Label = "Name/Values", JsClass = "GlideElementNameValue", ScalarType = "string" };
        yield return new KnownGlideType { Name = "nds_icon", Label = "NDS Icon", JsClass = "GlideElement", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "order_index", Label = "Order Index", JsClass = "GlideElementNumeric", ScalarType = "integer", DoNotUseOriginalValue = true, Attributes = new() { "list_layout_ignore=true", "form_layout_ignore=true" } };
        yield return new KnownGlideType { Name = "password", Label = "Password (1 Way Encrypted)", JsClass = "GlideElementPassword", ScalarType = "string", UnderlyingType = "Password", Visible = true };
        yield return new KnownGlideType { Name = "password2", Label = "Password (2 Way Encrypted)", JsClass = "GlideElementPassword2", ScalarType = "string", ScalarLength = 255, UnderlyingType = "Password2", Visible = true, NoDataReplicate = true };
        yield return new KnownGlideType { Name = "percent_complete", Label = "Percent Complete", JsClass = "GlideElementNumeric", ScalarType = "decimal", ScalarLength = 15, Visible = true };
        yield return new KnownGlideType { Name = "phone_number_e164", Label = "Phone Number (E164)", JsClass = "GlideElementPhoneNumber", ScalarType = "string", UnderlyingType = "com.glide.script.glide_elements.GlideElementPhoneNumber", Visible = true, DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "ph_number", Label = "Phone Number", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "price", Label = "Price", JsClass = "GlideElementPrice", ScalarType = "decimal", ScalarLength = 20, UnderlyingType = "Price", Visible = true, EdgeEncryptionEnabled = true, Serializer = "com.glide.script.PriceXMLSerializer" };
        yield return new KnownGlideType { Name = "properties", Label = "Properties", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "radio", Label = "Radio Button Choice", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "reference", Label = "Reference", JsClass = "GlideElementReference", ScalarType = "GUID", Visible = true };
        yield return new KnownGlideType { Name = "reference_name", Label = "Reference Name", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "related_tags", Label = "Related Tags", JsClass = "GlideElementRelatedTags", ScalarType = "string", NoSort = true, NoAudit = true, Attributes = new() { "can_group=false" } };
        yield return new KnownGlideType { Name = "repeat_count", Label = "Repeat Count", JsClass = "GlideElementNumeric", ScalarType = "integer", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "repeat_type", Label = "Repeat Type", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "replication_payload", Label = "Replication Payload", JsClass = "GlideElementReplicationPayload", ScalarType = "string" };
        yield return new KnownGlideType { Name = "schedule_date_time", Label = "Schedule Date/Time", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "ScheduleDateTime", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "script", Label = "Script", JsClass = "GlideElementScript", ScalarType = "string", ScalarLength = 4000, Visible = true };
        yield return new KnownGlideType { Name = "script_client", Label = "Script (Client)", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "script_plain", Label = "Script (Plain)", JsClass = "GlideElementScript", ScalarType = "string", ScalarLength = 4000, Visible = true };
        yield return new KnownGlideType { Name = "script_server", Label = "Script (server side)", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "short_field_name", Label = "Short Field Name", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 40 };
        yield return new KnownGlideType { Name = "short_table_name", Label = "Short Table Name", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 40 };
        yield return new KnownGlideType { Name = "simple_name_values", Label = "Name-Value Pairs", JsClass = "GlideElementSimpleNameValue", ScalarType = "string", ScalarLength = 4000, Visible = true };
        yield return new KnownGlideType { Name = "slushbucket", Label = "Slush Bucket", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 4000 };
        yield return new KnownGlideType { Name = "source_id", Label = "Source ID", JsClass = "GlideElementSourceId", ScalarType = "GUID", UnderlyingType = "com.snc.apps.glide_elements.GlideElementSourceId" };
        yield return new KnownGlideType { Name = "source_name", Label = "Source Name", JsClass = "GlideElementSourceName", ScalarType = "string", UnderlyingType = "com.snc.apps.glide_elements.GlideElementSourceName" };
        yield return new KnownGlideType { Name = "source_table", Label = "Source Table", JsClass = "GlideElementSourceTable", ScalarType = "string", ScalarLength = 80, UnderlyingType = "com.snc.apps.glide_elements.GlideElementSourceTable" };
        yield return new KnownGlideType { Name = "string", Label = "String", JsClass = "GlideElement", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "string_full_utf8", Label = "String (Full UTF-8)", JsClass = "GlideElementFullUTF8", ScalarType = "string", ScalarLength = 255, Visible = true };
        yield return new KnownGlideType { Name = "sysevent_name", Label = "System Event Name", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "sys_class_name", Label = "System Class Name", JsClass = "GlideElementSysClassName", ScalarType = "string", UnderlyingType = "com.glide.glideobject.SysClassName" };
        yield return new KnownGlideType { Name = "sys_class_path", Label = "System Class path", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "table_name", Label = "Table Name", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 80, Visible = true, Attributes = new() { "base_start=true" } };
        yield return new KnownGlideType { Name = "template_value", Label = "Template Value", JsClass = "GlideElementWorkflowConditions", ScalarType = "string", ScalarLength = 65000, UnderlyingType = "Conditions", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "timer", Label = "Timer", JsClass = "GlideElementGlideObject", ScalarType = "datetime", UnderlyingType = "com.glide.glideobject.GlideDuration" };
        yield return new KnownGlideType { Name = "translated", Label = "Translated", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "Translated" };
        yield return new KnownGlideType { Name = "translated_field", Label = "Translated Field", JsClass = "GlideElementTranslatedField", ScalarType = "string" };
        yield return new KnownGlideType { Name = "translated_html", Label = "Translated HTML", JsClass = "GlideElementTranslatedHTML", ScalarType = "string", ScalarLength = 65536, Visible = true };
        yield return new KnownGlideType { Name = "translated_text", Label = "Translated Text", JsClass = "GlideElementTranslatedText", ScalarType = "string", Visible = true };
        yield return new KnownGlideType { Name = "url", Label = "URL", JsClass = "GlideElementURL", ScalarType = "string", ScalarLength = 1024, UnderlyingType = "URL", Visible = true, DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "user_image", Label = "Image", JsClass = "GlideElementUserImage", ScalarType = "string", UnderlyingType = "UserImage", Visible = true, DoNotUseOriginalValue = true, PdfCellType = "pdf_cell_type" };
        yield return new KnownGlideType { Name = "user_input", Label = "User Input", JsClass = "GlideElementGlideObject", ScalarType = "string", UnderlyingType = "GlideUserInput" };
        yield return new KnownGlideType { Name = "user_roles", Label = "User Roles", JsClass = "GlideElement", ScalarType = "string", ScalarLength = 255, Attributes = new() { "record_watcher_blacklist" } };
        yield return new KnownGlideType { Name = "variables", Label = "Variables", JsClass = "GlideElementVariables", ScalarType = "string" };
        yield return new KnownGlideType { Name = "variable_conditions", Label = "Variable Conditions", JsClass = "GlideElementVariableConditions", ScalarType = "string", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "variable_template_value", Label = "Variable template value", JsClass = "GlideElementVariableTemplateValue", ScalarType = "string" };
        yield return new KnownGlideType { Name = "version", Label = "Version", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "week_of_month", Label = "Week of Month", JsClass = "GlideElementGlideObject", ScalarType = "integer", UnderlyingType = "WeekOfMonth" };
        yield return new KnownGlideType { Name = "wide_text", Label = "Wide Text", JsClass = "GlideElement", ScalarType = "string" };
        yield return new KnownGlideType { Name = "wiki_text", Label = "Wiki", JsClass = "GlideElementWikiText", ScalarType = "string", ScalarLength = 65536, Visible = true, IsMultiText = true, Attributes = new() { "preview_selector=true", "preview_first=true" } };
        yield return new KnownGlideType { Name = "workflow", Label = "Workflow", JsClass = "GlideElementWorkflow", ScalarType = "string", ScalarLength = 80, Visible = true };
        yield return new KnownGlideType { Name = "workflow_conditions", Label = "Workflow Conditions", JsClass = "GlideElementWorkflowConditions", ScalarType = "string", DoNotUseOriginalValue = true };
        yield return new KnownGlideType { Name = "xml", Label = "XML", JsClass = "GlideElementScript", ScalarType = "string" };
    }
}