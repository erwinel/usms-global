namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>super_class</c> from the <c>sys_db_object</c> table.
/// </summary>
/// <param name="SysID">The value of <c>super_class.value</c>.</param>
/// <param name="Label">The value of <c>super_class.display_value</c> or <see null="null"/> if that value was empty.</param>
public record SuperClassRef(string SysID, string? Label);
