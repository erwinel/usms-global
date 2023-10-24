namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Field class" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" />) record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Label">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_LABEL" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="ScalarType">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SCALAR_TYPE" />.value</c> property.</param>
/// <param name="ScalarLength">The numerical value of the <c><see cref="Services.SnApiConstants.JSON_KEY_SCALAR_LENGTH" />.value</c> property or <see langword="null"/> if the <c>scalar_length.value</c> is empty.</param>
/// <param name="ClassName">The value of the <c><see cref="Services.SnApiConstants.JSON_KEY_CLASS_NAME" />.value</c> property or <see langword="null"/> if the <c>class_name.value</c> is empty.</param>
/// <param name="UseOriginalValue">The boolean value of the <c><see cref="Services.SnApiConstants.JSON_KEY_USE_ORIGINAL_VALUE" />.value</c> property.</param>
/// <param name="IsVisible">The boolean value of the <c><see cref="Services.SnApiConstants.JSON_KEY_VISIBLE" />.value</c> property.</param>
/// <param name="Package">The deserialized <see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" /> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="Scope">The deserialized <see cref="Services.SnApiConstants.JSON_KEY_SYS_SCOPE" /> property or <see langword="null"/> if the <c>sys_scope.value</c> is empty.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record GlideTypeRecord(string Name, string Label, string SysID, string? ScalarType, int? ScalarLength, string? ClassName, bool UseOriginalValue, bool IsVisible, PackageRef? Package,
    ScopeRef? Scope, string SourceFqdn);
