namespace SnTsTypeGenerator.Models.TableAPI;

/// <summary>
/// Deserialized <c>sys_glide_object</c> record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c>name.value</c> property.</param>
/// <param name="Label">The value of the <c>label.value</c> property.</param>
/// <param name="SysID">The value of the <c>sys_id.value</c> property.</param>
/// <param name="ScalarType">The value of the <c>scalar_type.value</c> property.</param>
/// <param name="ScalarLength">The numerical value of the <c>scalar_length.value</c> property or <see langword="null"/> if the <c>scalar_length.value</c> is empty.</param>
/// <param name="ClassName">The value of the <c>class_name.value</c> property or <see langword="null"/> if the <c>class_name.value</c> is empty.</param>
/// <param name="UseOriginalValue">The boolean value of the <c>use_original_value.value</c> property.</param>
/// <param name="IsVisible">The boolean value of the <c>visible.value</c> property.</param>
/// <param name="Package">The deserialized <c>sys_package</c> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="Scope">The deserialized <c>sys_scope</c> property or <see langword="null"/> if the <c>sys_scope.value</c> is empty.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record GlideTypeRecord(string Name, string Label, string SysID, string? ScalarType, int? ScalarLength, string? ClassName, bool UseOriginalValue, bool IsVisible, RecordRef? Package, RecordRef? Scope, string SourceFqdn);
