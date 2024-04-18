namespace SnTsTypeGenerator.Models.Remote;

/// <summary>
/// Deserialized "Sys Plugins" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_PLUGINS" />) record from a ServiceNow instance.
/// </summary>
/// <param name="ID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SOURCE" />.value</c> property.</param>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Version">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VERSION" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="Parent">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_PARENT" />.value</c> property.</param>
/// <param name="Licensable">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_LICENSABLE" />.value</c> property.</param>
/// <param name="SubscriptionRequirement">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ENFORCE_LICENSE" />.value</c> property.</param>
/// <param name="Vendor">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VENDOR" />.value</c> property.</param>
/// <param name="VendorPrefix">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VENDOR_PREFIX" />.value</c> property.</param>
/// <param name="InstallDate">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_INSTALL_DATE" />.value</c> property.</param>
/// <param name="Private">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_PRIVATE" />.value</c> property.</param>
/// <param name="Active">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ACTIVE" />.value</c> property.</param>
public record SysPlugin(string ID, string Name, string Version, string SysID, string? Parent, bool Licensable, string SubscriptionRequirement,
    string Vendor, string VendorPrefix, DateTime? InstallDate, bool Private, bool Active) :
    Package(ID, Name, Version, SysID, Licensable, SubscriptionRequirement, Active);
