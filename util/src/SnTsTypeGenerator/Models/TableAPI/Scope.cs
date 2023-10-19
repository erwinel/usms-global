namespace SnTsTypeGenerator.Models.TableAPI;

/// <summary>
/// Deserialized <c>sys_scope</c> record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c>name.value</c> property.</param>
/// <param name="Value">The value of the <c>scope.value</c> property.</param>
/// <param name="ShortDescription">The value of the <c>short_description.value</c> property or <see langword="null"/> if the <c>short_description.value</c> is empty.</param>
/// <param name="SysID">The value of the <c>sys_id.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record Scope(string Name, string Value, string? ShortDescription, string SysID, string SourceFqdn);
