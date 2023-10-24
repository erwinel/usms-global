namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>sys_package</c> from the <c>sys_db_object</c>, <c>sys_dictionary</c>, or <c>sys_glide_object</c> table.
/// </summary>
/// <param name="SysID">The value of <c>sys_package.value</c>.</param>
/// <param name="Name">The value of <c>sys_package.display_value</c> or <see null="null"/> if that value was empty.</param>
public record PackageRef(string SysID, string? Name);
