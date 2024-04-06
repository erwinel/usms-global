namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Custom Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_APP" />) record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Value">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SCOPE" />.value</c> property.</param>
/// <param name="ID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SOURCE" />.value</c> property.</param>
/// <param name="Version">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VERSION" />.value</c> property.</param>
/// <param name="ShortDescription">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SHORT_DESCRIPTION" />.value</c> property or <see langword="null"/> if the <c>short_description.value</c> is empty.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="Licensable">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_LICENSABLE" />.value</c> property.</param>
/// <param name="SubscriptionRequirement">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ENFORCE_LICENSE" />.value</c> property.</param>
/// <param name="Vendor">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VENDOR" />.value</c> property.</param>
/// <param name="VendorPrefix">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VENDOR_PREFIX" />.value</c> property.</param>
/// <param name="StoreURL">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_STORE_URL" />.value</c> property.</param>
/// <param name="StoreCorrelationID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_STORE_CORRELATION_ID" />.value</c> property.</param>
/// <param name="Code">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_CODE" />.value</c> property.</param>
/// <param name="Private">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_PRIVATE" />.value</c> property.</param>
/// <param name="InstalledViaDependency">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_INSTALLED_AS_DEPENDENCY" />.value</c> property.</param>
/// <param name="Active">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ACTIVE" />.value</c> property.</param>
/// <param name="Dependencies">The values of the <c><see cref="Services.SnApiConstants.JSON_KEY_DEPENDENCIES" />.value</c> property.</param>
public record RemoteCustomApplication(string Name, string Value, string ID, string Version, string ShortDescription, string SysID, bool Licensable, string SubscriptionRequirement,
    string Vendor, string VendorPrefix, string StoreURL, string StoreCorrelationID, string Code, bool Private, bool InstalledViaDependency, bool Active, string[] Dependencies) :
    RemoteApplication(Name, Value, ID, Version, ShortDescription, SysID, Licensable, SubscriptionRequirement, Vendor, VendorPrefix, Private, Active);
