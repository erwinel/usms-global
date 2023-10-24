namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" /> from the "Table" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DB_OBJECT" />),
/// "Dictionary Entry" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DICTIONARY" />), or "Field class" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" />) table.
/// </summary>
/// <param name="SysID">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" />.value</c>.</param>
/// <param name="Name">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" />.display_value</c> or <see langword="null"/> if that value was empty.</param>
public record PackageRef(string SysID, string? Name);
