namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_SCOPE" />) or "Custom Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_APP" />) record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Value">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SCOPE" />.value</c> property.</param>
/// <param name="ID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SOURCE" />.value</c> property.</param>
/// <param name="Version">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VERSION" />.value</c> property.</param>
/// <param name="ShortDescription">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SHORT_DESCRIPTION" />.value</c> property or <see langword="null"/> if the <c>short_description.value</c> is empty.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="Licensable">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_LICENSABLE" />.value</c> property.</param>
/// <param name="Active">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ACTIVE" />.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record ScopeRecord(string Name, string Value, string ID, string Version, string ShortDescription, string SysID, bool Licensable, bool Active, string SourceFqdn) :
    PackageRecord(ID: ID, Name: Name, Version: Version, SysID: SysID, Licensable: Licensable, Active: Active, SourceFqdn: SourceFqdn);
