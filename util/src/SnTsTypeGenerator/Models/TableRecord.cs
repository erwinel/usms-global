namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Table" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DB_OBJECT" />) record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Label">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_LABEL" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="IsExtendable">The boolean value of the <c><see cref="Services.SnApiConstants.JSON_KEY_IS_EXTENDABLE" />.value</c> property.</param>
/// <param name="NumberPrefix">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NUMBER_REF" />.display_value</c> property.</param>
/// <param name="Package">The deserialized <see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" /> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="Scope">The deserialized <see cref="Services.SnApiConstants.JSON_KEY_SYS_SCOPE" /> property or <see langword="null"/> if the <c>sys_scope.value</c> is empty.</param>
/// <param name="SuperClass">The deserialized <see cref="Services.SnApiConstants.JSON_KEY_SUPER_CLASS" /> property or <see langword="null"/> if the <c>super_class.value</c> is empty.</param>
/// <param name="AccessibleFrom">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_ACCESS" />.value</c> property.</param>
/// <param name="ExtensionModel">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_EXTENSION_MODEL" />.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
[Obsolete("Use RemoteTable")]
public record TableRecord(string Name, string Label, string SysID, bool IsExtendable, string? NumberPrefix, PackageRef? Package, ScopeRef? Scope, SuperClassRef? SuperClass,
    string AccessibleFrom, string? ExtensionModel, string SourceFqdn);


