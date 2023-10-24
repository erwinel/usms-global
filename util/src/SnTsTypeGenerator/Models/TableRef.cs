namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <see cref="Services.SnApiConstants.JSON_KEY_REFERENCE" /> from the "Dictionary Entry" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DICTIONARY" />) table.
/// </summary>
/// <param name="Name">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_REFERENCE" />.value</c>.</param>
/// <param name="Label">The value of <c><see cref="Services.SnApiConstants.JSON_KEY_REFERENCE" />.display_value</c>.</param>
public record TableRef(string Name, string Label);
