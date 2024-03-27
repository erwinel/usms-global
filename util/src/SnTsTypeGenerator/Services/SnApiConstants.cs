namespace SnTsTypeGenerator.Services;

internal static class SnApiConstants
{
    internal static readonly StringComparer NameComparer = StringComparer.InvariantCultureIgnoreCase;

    internal static readonly Uri EmptyURI = new(string.Empty, UriKind.Relative);

    internal const string GLOBAL_NAMESPACE = "global";

    internal const string URI_PARAM_QUERY = "sysparm_query";

    internal const string URI_PARAM_DISPLAY_VALUE = "sysparm_display_value";

    /// <summary>
    /// URI path for the Auth Token API.
    /// </summary>
    internal const string URI_PATH_AUTH_TOKEN = "/oauth_token.do";

    /// <summary>
    /// URI path for the ServiceNow Table API.
    /// </summary>
    internal const string URI_PATH_API = "/api/now/table";

    /// <summary>
    /// Table name for Table records.
    /// </summary>
    internal const string TABLE_NAME_SYS_DB_OBJECT = "sys_db_object";

    /// <summary>
    /// Table name for Application records.
    /// </summary>
    internal const string TABLE_NAME_SYS_SCOPE = "sys_scope";

    /// <summary>
    /// Table name for Store Application records.
    /// </summary>
    internal const string TABLE_NAME_SYS_STORE_APP = "sys_store_app";

    /// <summary>
    /// Table name for Custom Application records.
    /// </summary>
    internal const string TABLE_NAME_SYS_APP = "sys_app";

    /// <summary>
    /// Table name for Sys Plugins records.
    /// </summary>
    internal const string TABLE_NAME_SYS_PLUGINS = "sys_plugins";

    /// <summary>
    /// Table name for Package records.
    /// </summary>
    internal const string TABLE_NAME_SYS_PACKAGE = "sys_package";

    /*
    Package (sys_package)
        Application (sys_scope)
            Store Application (sys_store_app)
            Custom Application (sys_app)
        Sys Plugins (sys_plugins)
    */
    /// <summary>
    /// Table name for ServiceNow column (element) definitions.
    /// </summary>
    internal const string TABLE_NAME_SYS_DICTIONARY = "sys_dictionary";

    /// <summary>
    /// Table name for ServiceNow type definitions.
    /// </summary>
    internal const string TABLE_NAME_SYS_GLIDE_OBJECT = "sys_glide_object";

    /// <summary>
    /// ServiceNow Table API URI path for ServiceNow type definitions.
    /// </summary>
    internal const string URI_PATH_TYPE = $"{URI_PATH_API}/{TABLE_NAME_SYS_GLIDE_OBJECT}";

    /// <summary>
    /// Response header name for accept type.
    /// </summary>
    internal const string HEADER_KEY_ACCEPT = "Accept";

    internal const string HEADER_KEY_ACCESS_TOKEN = "access_token";

    internal const string HEADER_KEY_CLIENT_ID = "client_id";

    internal const string HEADER_KEY_CLIENT_SECRET = "client_secret";

    internal const string HEADER_KEY_GRANT_TYPE = "grant_type";

    internal const string HEADER_KEY_USERNAME = "username";

    internal const string HEADER_KEY_PASSWORD = "password";

    internal const string HEADER_KEY_REFRESH_TOKEN = "refresh_token";

    /// <summary>
    /// "Sys ID" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_RESULT = "result";

    /// <summary>
    /// "Sys ID" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_ID = "sys_id";

    /// <summary>
    /// "Name" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_NAME = "name";

    internal const string JSON_KEY_VERSION = "version";

    internal const string JSON_KEY_SHORT_DESCRIPTION = "short_description";

    /// <summary>
    /// "Label" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_LABEL = "label";

    /// <summary>
    /// "Parent" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_PARENT = "parent";

    /// <summary>
    /// "Comments" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_COMMENTS = "comments";

    /// <summary>
    /// "Column name" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_ELEMENT = "element";

    /// <summary>
    /// "Is Extendable" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_IS_EXTENDABLE = "is_extendable";

    /// <summary>
    /// "Scope" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SCOPE = "scope";

    /// <summary>
    /// "ID" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SOURCE = "source";

    /// <summary>
    /// "Application" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_SCOPE = "sys_scope";

    /// <summary>
    /// "Package" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_PACKAGE = "sys_package";

    /// <summary>
    /// "Dependencies" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_DEPENDENCIES = "dependencies";

    /// <summary>
    /// "Extends table" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SUPER_CLASS = "super_class";

    /// <summary>
    /// "Display" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_DISPLAY = "display";

    /// <summary>
    /// "Sizeclass" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SIZECLASS = "sizeclass";

    /// <summary>
    /// "Column label" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_COLUMN_LABEL = "column_label";

    /// <summary>
    /// "Type" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_INTERNAL_TYPE = "internal_type";

    /// <summary>
    /// "Defaultsort" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_DEFAULTSORT = "defaultsort";

    /// <summary>
    /// "Active" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_ACTIVE = "active";

    /// <summary>
    /// "Licensable" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_LICENSABLE = "licensable";

    /// <summary>
    /// "Array" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_ARRAY = "array";

    /// <summary>
    /// "Max length" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_MAX_LENGTH = "max_length";

    /// <summary>
    /// "Reference" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_REFERENCE = "reference";

    /// <summary>
    /// "Calculated" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_VIRTUAL = "virtual";

    /// <summary>
    /// "Default value" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_DEFAULT_VALUE = "default_value";

    /// <summary>
    /// "Unique" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_UNIQUE = "unique";

    /// <summary>
    /// "Mandatory" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_MANDATORY = "mandatory";

    /// <summary>
    /// "Primary" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_PRIMARY = "primary";

    /// <summary>
    /// "Read only" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_READ_ONLY = "read_only";

    /// <summary>
    /// "Extension model" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_EXTENSION_MODEL = "extension_model";

    /// <summary>
    /// "Accessible from" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_ACCESS = "access";

    /// <summary>
    /// "Auto number" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_NUMBER_REF = "number_ref";

    /// <summary>
    /// "Link" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_LINK = "link";

    /// <summary>
    /// "Value" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_VALUE = "value";

    /// <summary>
    /// "Display Value" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_DISPLAY_VALUE = "display_value";

    /// <summary>
    /// "Prefix" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_PREFIX = "prefix";

    /// <summary>
    /// "Extends" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SCALAR_TYPE = "scalar_type";

    /// <summary>
    /// "Class name" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_CLASS_NAME = "class_name";

    /// <summary>
    /// "Use original value" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_USE_ORIGINAL_VALUE = "use_original_value";

    /// <summary>
    /// "Length" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SCALAR_LENGTH = "scalar_length";

    /// <summary>
    /// "Visible" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_VISIBLE = "visible";

    /// <summary>
    /// "Created By" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_CREATED_BY = "sys_created_by";

    /// <summary>
    /// "Created On" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_CREATED_ON = "sys_created_on";

    /// <summary>
    /// "Updates" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_MOD_COUNT = "sys_mod_count";

    /// <summary>
    /// "Updated By" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_UPDATED_BY = "sys_updated_by";

    /// <summary>
    /// "Updated On" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_UPDATED_ON = "sys_updated_on";

    internal const string JSON_KEY_ACCESS_TOKEN = "access_token";

    internal const string JSON_KEY_REFRESH_TOKEN = "refresh_token";

    /// <summary>
    /// "GUID" type name.
    /// </summary>
    internal const string TYPE_NAME_GUID = "GUID";

    /// <summary>
    /// "String" type name.
    /// </summary>
    internal const string TYPE_NAME_string = "string";

    /// <summary>
    /// "GlideDateTime" type name.
    /// </summary>
    internal const string TYPE_NAME_glide_date_time = "glide_date_time";

    /// <summary>
    /// "Integer" type name.
    /// </summary>
    internal const string TYPE_NAME_integer = "integer";

    internal const string TYPE_NAME_boolean = "boolean";

    internal const string TYPE_NAME_decimal = "decimal";

    internal const string TYPE_NAME_float = "float";

    internal const string TYPE_NAME_percent_complete = "percent_complete";

    internal const string TYPE_NAME_order_index = "order_index";

    internal const string TYPE_NAME_longint = "longint";

    internal const string TYPE_NAME_script_plain = "script_plain";

    internal const string TYPE_NAME_xml = "xml";

    internal const string TYPE_NAME_glide_date = "glide_date";

    internal const string TYPE_NAME_glide_time = "glide_time";

    internal const string TYPE_NAME_timer = "timer";

    internal const string TYPE_NAME_glide_duration = "glide_duration";

    internal const string TYPE_NAME_glide_utc_time = "glide_utc_time";

    internal const string TYPE_NAME_due_date = "due_date";

    internal const string TYPE_NAME_glide_precise_time = "glide_precise_time";

    internal const string TYPE_NAME_calendar_date_time = "calendar_date_time";

    internal const string TYPE_NAME_user_input = "user_input";

    internal const string TYPE_NAME_journal_input = "journal_input";

    internal const string TYPE_NAME_journal_list = "journal_list";

    internal const string TYPE_NAME_html = "html";

    internal const string TYPE_NAME_glide_list = "glide_list";

    internal const string TYPE_NAME_journal = "journal";

    internal const string TYPE_NAME_glide_action_list = "glide_action_list";

    internal const string TYPE_NAME_date = "date";

    internal const string TYPE_NAME_day_of_week = "day_of_week";

    internal const string TYPE_NAME_month_of_year = "month_of_year";

    internal const string TYPE_NAME_week_of_month = "week_of_month";

    internal const string TYPE_NAME_caller_phone_number = "caller_phone_number";

    internal const string TYPE_NAME_phone_number_e164 = "phone_number_e164";

    internal const string TYPE_NAME_reference = "reference";

    internal const string TYPE_NAME_currency2 = "currency2";

    internal const string TYPE_NAME_domain_id = "domain_id";

    internal const string TYPE_NAME_document_id = "document_id";

    internal const string TYPE_NAME_source_id = "source_id";

    /// <summary>
    /// Namespace for global GlideRecord types.
    /// </summary>
    internal const string NS_NAME_GlideRecord = "$$GlideRecord";

    /// <summary>
    /// Namespace for global GlideElement types.
    /// </summary>
    internal const string NS_NAME_GlideElement = "$$GlideElement";

    /// <summary>
    /// Namespace for global table field definition interfaces.
    /// </summary>
    internal const string NS_NAME_tableFields = "$$tableFields";

    /// <summary>
    /// Nested namespace for scoped GlideRecord types.
    /// </summary>
    internal const string NS_NAME_record = "$$record";

    /// <summary>
    /// Nested namespace for scoped GlideElement types.
    /// </summary>
    internal const string NS_NAME_element = "$$element";

    /// <summary>
    /// Nested namespace for scoped table field definition interfaces.
    /// </summary>
    internal const string NS_NAME_fields = "$$fields";

    /// <summary>
    /// IBaseRecord TypeScript class name.
    /// </summary>
    internal const string TS_NAME_BASERECORD = "IBaseRecord";

    /// <summary>
    /// GlideRecord TypeScript class name.
    /// </summary>
    internal const string TS_NAME_GlideRecord = "GlideRecord";

    /// <summary>
    /// ScopedGlideRecord TypeScript class name.
    /// </summary>
    internal const string TS_NAME_ScopedGlideRecord = "ScopedGlideRecord";

    /// <summary>
    /// GlideElement TypeScript class name.
    /// </summary>
    internal const string TS_NAME_GlideElement = "GlideElement";

    internal const string TS_NAME_GlideElementReference = "GlideElementReference";
}
