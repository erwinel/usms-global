using System.ComponentModel.DataAnnotations;

namespace SnTsTypeGenerator;

public enum LogActivityType
{
    [Display(Name = "Load Tables")]
    LoadTables,

    [Display(Name = "Render Types")]
    RenderTypes,

    [Display(Name = "Validate Before Save to DB")]
    ValidateBeforeSave,

    [Display(Name = "Save DB Changes")]
    SaveDbChanges,

    [Display(Name = "Get Table from Command Line Arg")]
    GetTableFromArg,

    [Display(Name = "Open Output File")]
    OpenOutputFile,

    [Display(Name = "Render Global Types")]
    RenderGlobalTypes,

    [Display(Name = "Render Namespace Types")]
    RenderNamespaceTypes,

    [Display(Name = "Get Access Token")]
    GetAccessToken,

    [Display(Name = "Refresh Access Token")]
    RefreshAccessToken,
    AddNewTable,
    ModelCreating,
    InitializeDatabase,
    InitializeDbTable
}
