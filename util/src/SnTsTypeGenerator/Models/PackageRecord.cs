namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Sys Plugins" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_PLUGINS" />), "Custom Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_APP" />),
/// "Store Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_STORE_APP" />), or "Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_SCOPE" />) record from a ServiceNow instance.
/// </summary>
/// <param name="ID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SOURCE" />.value</c> property.</param>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Version">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VERSION" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record PackageRecord(string ID, string Name, string Version, string SysID, string SourceFqdn);
