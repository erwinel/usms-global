namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <see cref="Services.SnApiConstants.JSON_KEY_SUPER_CLASS" /> from the "Table" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DB_OBJECT" />) table.
/// </summary>
/// <param name="SysID">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_SUPER_CLASS" />.value</c>.</param>
/// <param name="Label">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_SUPER_CLASS" />.display_value</c> or <see langword="null"/> if that value was empty.</param>
public record SuperClassRef(string SysID, string? Label);
