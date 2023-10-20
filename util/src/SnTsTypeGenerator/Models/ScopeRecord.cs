namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>sys_scope</c> record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c>name.value</c> property.</param>
/// <param name="Value">The value of the <c>scope.value</c> property.</param>
/// <param name="ID">The value of the <c>scope.source</c> property.</param>
/// <param name="Version">The value of the <c>scope.version</c> property.</param>
/// <param name="ShortDescription">The value of the <c>short_description.value</c> property or <see langword="null"/> if the <c>short_description.value</c> is empty.</param>
/// <param name="SysID">The value of the <c>sys_id.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record ScopeRecord(string Name, string Value, string ID, string Version, string ShortDescription, string SysID, string SourceFqdn) :
    PackageRecord(ID: ID, Name: Name, Version: Version, SysID: SysID, SourceFqdn: SourceFqdn);
