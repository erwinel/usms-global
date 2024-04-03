namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized <c>sys_dictionary</c> record from ServiceNow instance.
/// </summary>
/// <param name="Name">The value of the <c>element.value</c> property.</param>
/// <param name="Label">The value of the <c>column_label.value</c> property.</param>
/// <param name="SysID">The value of the <c>sys_id.value</c> property.</param>
/// <param name="Reference">The deserialized <c>reference</c> property or <see langword="null"/> if the <c>reference.value</c> is empty.</param>
/// <param name="IsReadOnly">The boolean value of the <c>read_only.value</c> property.</param>
/// <param name="Type">The deserialized <c>internal_type</c> property or <see langword="null"/> if the <c>internal_type.value</c> is empty.</param>
/// <param name="MaxLength">The numerical value of the <c>max_length.value</c> property or <see langword="null"/> if the <c>max_length.value</c> is empty.</param>
/// <param name="IsActive">The boolean value of the <c>active.value</c> property.</param>
/// <param name="IsUnique">The boolean value of the <c>unique.value</c> property.</param>
/// <param name="IsPrimary">The boolean value of the <c>primary.value</c> property.</param>
/// <param name="IsCalculated">The boolean value of the <c>virtual.value</c> property.</param>
/// <param name="SizeClass">The numerical value of the <c>sizeclass.value</c> property or <see langword="null"/> if the <c>sizeclass.value</c> is empty.</param>
/// <param name="IsMandatory">The boolean value of the <c>mandatory.value</c> property.</param>
/// <param name="IsArray">The boolean value of the <c>array.value</c> property.</param>
/// <param name="Comments">The value of the <c>comments.value</c> property or <see langword="null"/> if the <c>comments.value</c> is empty.</param>
/// <param name="IsDisplay">The boolean value of the <c>is_extendable.value</c> property.</param>
/// <param name="DefaultValue">The value of the <c>default_value.value</c> property or <see langword="null"/> if the <c>default_value.value</c> is empty.</param>
/// <param name="Package">The deserialized <c>sys_package</c> property or <see langword="null"/> if the <c>sys_package.value</c> is empty.</param>
/// <param name="SourceFqdn">The FQDN of the source ServiceNow instance.</param>
public record ElementRecord(string Name, string Label, string SysID, TableRef? Reference, bool IsReadOnly, TypeRef? Type, int? MaxLength,
    bool IsActive, bool IsUnique, bool IsPrimary, bool IsCalculated, int? SizeClass, bool IsMandatory, bool IsArray,
    string? Comments, bool IsDisplay, string? DefaultValue, ScopeRef? Scope, PackageRef? Package, string SourceFqdn);
