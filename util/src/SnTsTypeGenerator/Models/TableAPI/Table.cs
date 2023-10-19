namespace SnTsTypeGenerator.Models.TableAPI;

/// <summary>
/// Deserialized <c>sys_db_object</c> record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c>name.value</c> property.</param>
/// <param name="Label">The value of the <c>label.value</c> property.</param>
/// <param name="SysID">The value of the <c>sys_id.value</c> property.</param>
/// <param name="IsExtendable">The boolean value of the <c>is_extendable.value</c> property.</param>
/// <param name="NumberPrefix">The value of the <c>number_ref.display_value</c> property.</param>
/// <param name="Package">The deserialized <c>sys_package</c> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="Scope">The deserialized <c>sys_scope</c> property or <see langword="null"/> if the <c>sys_scope.value</c> is empty.</param>
/// <param name="SuperClass">The deserialized <c>super_class</c> property or <see langword="null"/> if the <c>super_class.value</c> is empty.</param>
/// <param name="AccessibleFrom">The value of the <c>access.value</c> property.</param>
/// <param name="ExtensionModel">The value of the <c>extension_model.value</c> property.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record Table(string Name, string Label, string SysID, bool IsExtendable, string? NumberPrefix, RecordRef? Package, RecordRef? Scope, RecordRef? SuperClass, string AccessibleFrom, string? ExtensionModel, string SourceFqdn);
