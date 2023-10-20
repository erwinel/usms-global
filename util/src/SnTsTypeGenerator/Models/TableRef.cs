namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>reference</c> from the <c>sys_dictionary</c> table.
/// </summary>
/// <param name="Name">The value of <c>reference.value</c>.</param>
/// <param name="Label">The value of <c>reference.display_value</c>.</param>
public record TableRef(string Name, string Label);