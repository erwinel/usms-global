namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>sys_scope</c> from the <c>sys_db_object</c>, or <c>sys_glide_object</c> table.
/// </summary>
/// <param name="ID">The value of <c>sys_scope.value</c>.</param>
/// <param name="Name">The value of <c>sys_scope.display_value</c> or <see null="null"/> if that value was empty.</param>
public record ScopeRef(string ID, string? Name);
