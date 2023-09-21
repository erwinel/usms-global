using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnTsTypeGenerator;

internal static class Constants
{
    /// <summary>
    /// URI path for the ServiceNow Table API.
    /// </summary>
    internal const string URI_PATH_API = "/api/now/table";

    /// <summary>
    /// Table name for ServiceNow table definitions.
    /// </summary>
    internal const string TABLE_NAME_SYS_DB_OBJECT = "sys_db_object";

    /// <summary>
    /// ServiceNow Table API URI path for ServiceNow table definitions.
    /// </summary>
    internal const string URI_PATH_TABLE = $"{URI_PATH_API}/{TABLE_NAME_SYS_DB_OBJECT}";
    
    /// <summary>
    /// Table name for ServiceNow column (element) definitions.
    /// </summary>
    internal const string TABLE_NAME_SYS_DICTIONARY = "sys_dictionary";
    
    /// <summary>
    /// ServiceNow Table API URI path for ServiceNow column (element) definitions.
    /// </summary>
    internal const string URI_PATH_COLUMN = $"{URI_PATH_API}/{TABLE_NAME_SYS_DICTIONARY}";
    
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
    
    /// <summary>
    /// "Label" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_LABEL = "label";
    
    /// <summary>
    /// "Element" Column (element) name.
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
    /// "Parent Class" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_SUPER_CLASS = "super_class";
    
    /// <summary>
    /// "Number Reference" Column (element) name.
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
    /// "Prefix" Column (element) name.
    /// </summary>
    internal const string JSON_KEY_PREFIX = "prefix";
    
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