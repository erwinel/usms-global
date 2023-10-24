namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>internal_type</c> from the <c>sys_dictionary</c> table.
/// </summary>
/// <param name="Name">The value of <c>internal_type.value</c>.</param>
/// <param name="Label">The value of <c>internal_type.display_value</c>.</param>
public record TypeRef(string Name, string Label);
