using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnTsTypeGenerator;

internal static class Constants
{
    internal const string URI_PATH_API = "/api/now/table";
    internal const string URI_PATH_TABLE = $"{URI_PATH_API}/sys_db_object";
    internal const string TABLE_NAME_SYS_DB_OBJECT = "sys_db_object";
    internal const string TABLE_NAME_SYS_DICTIONARY = "sys_dictionary";
    internal const string TABLE_NAME_SYS_GLIDE_OBJECT = "sys_glide_object";
    internal const string URI_PATH_COLUMN = $"{URI_PATH_API}/sys_dictionary";
    internal const string URI_PATH_TYPE = $"{URI_PATH_API}/sys_glide_object";
    internal const string HEADER_KEY_ACCEPT = "Accept";
    internal const string JSON_KEY_SYS_ID = "sys_id";
    internal const string JSON_KEY_NAME = "name";
    internal const string JSON_KEY_LABEL = "label";
    internal const string JSON_KEY_ELEMENT = "element";
    internal const string JSON_KEY_IS_EXTENDABLE = "is_extendable";
    internal const string JSON_KEY_SCOPE = "scope";
    internal const string JSON_KEY_SUPER_CLASS = "super_class";
    internal const string JSON_KEY_NUMBER_REF = "number_ref";
    internal const string JSON_KEY_LINK = "link";
    internal const string JSON_KEY_VALUE = "value";
    internal const string JSON_KEY_PREFIX = "prefix";

}