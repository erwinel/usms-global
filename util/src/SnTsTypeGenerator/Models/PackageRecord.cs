namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Package" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_PACKAGE" />) record from a ServiceNow instance.
/// </summary>
/// <param name="ID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SOURCE" />.value</c> property.</param>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Version">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VERSION" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="Licensable">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_LICENSABLE" />.value</c> property.</param>
/// <param name="Active">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ACTIVE" />.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
[Obsolete("Use RemotePackage")]
public record PackageRecord(string ID, string Name, string Version, string SysID, bool Licensable, bool Active, string SourceFqdn);
