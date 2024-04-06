namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <see cref="Services.SnApiConstants.JSON_KEY_SYS_SCOPE" /> from the "Table" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DB_OBJECT" />)
/// or "Field class" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" />) table.
/// </summary>
/// <param name="ID">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_VALUE" />.value</c>.</param>
/// <param name="Name">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_DISPLAY_VALUE" />.display_value</c> or <see langword="null"/> if that value was empty.</param>
[Obsolete("Use RemoteRef")]
public record ScopeRef(string ID, string? Name);
