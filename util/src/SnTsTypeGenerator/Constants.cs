using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnTsTypeGenerator;

internal static class Constants
{
    /// <summary>
    /// The Sqlite collation for case-insensitive matching.
    /// </summary>
    public const string COLLATION_NOCASE = "NOCASE";

    /// <summary>
    /// The Sqlite code for the current date and time.
    /// </summary>
    public const string DEFAULT_SQL_NOW = "(datetime('now','localtime'))";

    internal const string URI_PARAM_QUERY = "sysparm_query";

    internal const string URI_PARAM_DISPLAY_VALUE = "sysparm_display_value";

    /// <summary>
    /// URI path for the ServiceNow Table API.
    /// </summary>
    internal const string URI_PATH_API = "/api/now/table";

    /// <summary>
    /// Table name for ServiceNow table definitions.
    /// </summary>
    internal const string TABLE_NAME_SYS_DB_OBJECT = "sys_db_object";

    /// <summary>
    /// Table name for ServiceNow table definitions.
    /// </summary>
    internal const string TABLE_NAME_SYS_SCOPE = "sys_scope";

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
    
    /// <summary>
    /// "Sys ID" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_ID = "sys_id";
    
    /// <summary>
    /// "Name" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_NAME = "name";
    
    internal const string JSON_KEY_SHORT_DESCRIPTION = "short_description";

    /// <summary>
    /// "Label" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_LABEL = "label";
    
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
    /// "Application" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_SCOPE = "sys_scope";
    
    /// <summary>
    /// "Package" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SYS_PACKAGE = "sys_package";
    
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
    
    /// <summary>
    /// "GUID" type name.
    /// </summary>
    internal const string TYPE_NAME_GUID = "GUID";
    
    /// <summary>
    /// "String" type name.
    /// </summary>
    internal const string TYPE_NAME_STRING = "string";
    
    /// <summary>
    /// "GlideDateTime" type name.
    /// </summary>
    internal const string TYPE_NAME_GLIDE_DATE_TIME = "glide_date_time";
    
    /// <summary>
    /// "Integer" type name.
    /// </summary>
    internal const string TYPE_NAME_INTEGER = "integer";
    
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
    /// GlideRecord TypeScript class name.
    /// </summary>
    internal const string TS_NAME_GlideRecord = "GlideRecord";
    
    /// <summary>
    /// GlideElement TypeScript class name.
    /// </summary>
    internal const string TS_NAME_GlideElement = "GlideElement";
}