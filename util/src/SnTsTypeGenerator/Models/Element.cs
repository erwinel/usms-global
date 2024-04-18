using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Dictionary Entry" (<see cref="SnApiConstants.TABLE_NAME_SYS_DICTIONARY" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Elements))]
public sealed class Element : IEquatable<Element>, IValidatableObject
{
    private readonly object _syncRoot = new();

    #region Name Property

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Column name" (<see cref="SnApiConstants.JSON_KEY_ELEMENT" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value.EmptyIfWhiteSpace();
    }

    #endregion

    #region Label Property

    private string _label = string.Empty;

    /// <summary>
    /// Value of the "Column label" (<see cref="SnApiConstants.JSON_KEY_COLUMN_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value.EmptyIfWhiteSpace();
    }

    #endregion

    /// <summary>
    /// Value of the "Active" (<see cref="SnApiConstants.JSON_KEY_ACTIVE" />) column.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Value of the "Array" (<see cref="SnApiConstants.JSON_KEY_ARRAY" />) column.
    /// </summary>
    public bool IsArray { get; set; }

    /// <summary>
    /// Value of the "Max length" (<see cref="SnApiConstants.JSON_KEY_MAX_LENGTH" />) column.
    /// </summary>
    public int? MaxLength { get; set; }

    #region Comments Property

    private string? _comments;
    /// <summary>
    /// Value of the "Comments" (<see cref="SnApiConstants.JSON_KEY_COMMENTS" />) column.
    /// </summary>
    [BackingField(nameof(_comments))]
    public string? Comments { get => _comments; set => _comments = value.NullIfWhiteSpace(); }

    #endregion

    #region DefaultValue Property

    private string? _defaultValue;
    /// <summary>
    /// Value of the "Default value" (<see cref="SnApiConstants.JSON_KEY_DEFAULT_VALUE" />) column.
    /// </summary>
    [BackingField(nameof(_defaultValue))]
    public string? DefaultValue { get => _defaultValue; set => _defaultValue = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Value of the "Display" (<see cref="SnApiConstants.JSON_KEY_DISPLAY" />) column.
    /// </summary>
    public bool IsDisplay { get; set; }

    /// <summary>
    /// Value of the "Sizeclass" (<see cref="SnApiConstants.JSON_KEY_SIZECLASS" />) column.
    /// </summary>
    public int? SizeClass { get; set; }

    /// <summary>
    /// Value of the "Mandatory" (<see cref="SnApiConstants.JSON_KEY_MANDATORY" />) column.
    /// </summary>
    public bool IsMandatory { get; set; }

    /// <summary>
    /// Value of the "Primary" (<see cref="SnApiConstants.JSON_KEY_PRIMARY" />) column.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Value of the "Read only" (<see cref="SnApiConstants.JSON_KEY_READ_ONLY" />) column.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Value of the "Calculated" (<see cref="SnApiConstants.JSON_KEY_VIRTUAL" />) column.
    /// </summary>
    public bool IsCalculated { get; set; }

    /// <summary>
    /// Value of the "Unique" (<see cref="SnApiConstants.JSON_KEY_UNIQUE" />) column.
    /// </summary>
    public bool IsUnique { get; set; }

    #region SysID Property

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value.EmptyIfWhiteSpace();
    }

    #endregion

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    #region Package Navigation Property

    private string? _packageID;

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="SnApiConstants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    [BackingField(nameof(_packageID))]
    public string? PackageID
    {
        get { lock (_syncRoot) { return _package?.ID ?? _packageID; } }
        set => SetOptionalNavForeignKey(_syncRoot, value, ref _packageID, ref _package, p => p.ID);
    }

    private Package? _package;

    /// <summary>
    /// The source package for the element.
    /// </summary>
    public Package? Package
    {
        get { lock (_syncRoot) { return _package; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _packageID, ref _package);
    }

    #endregion

    #region Table Navigation Property

    private string _tableName = string.Empty;

    /// <summary>
    /// Value of the "Table" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    [BackingField(nameof(_tableName))]
    [Key]
    public string TableName
    {
        get { lock (_syncRoot) { return _table?.Name ?? _tableName; } }
        set => SetRequiredNonEmptyNavForeignKey(_syncRoot, value, ref _tableName, ref _table, t => t.Name);
    }

    private Table? _table;

    /// <summary>
    /// The table that the current element belongs to.
    /// </summary>
    public Table? Table
    {
        get { lock (_syncRoot) { return _table; } }
        set => SetRequiredNavProperty(_syncRoot, value, ref _tableName, ref _table, s => s.Name);
    }

    #endregion

    #region Type Navigation Property

    private string? _typeName = null;

    /// <summary>
    /// Name of the related value for the "Type" (<see cref="SnApiConstants.JSON_KEY_INTERNAL_TYPE" />) column.
    /// </summary>
    [BackingField(nameof(_typeName))]
    public string? TypeName
    {
        get { lock (_syncRoot) { return _type?.Name ?? _typeName; } }
        set => SetOptionalNonEmptyNavForeignKey(_syncRoot, value, ref _typeName, ref _type, t => t.Name);
    }

    private GlideType? _type;

    /// <summary>
    /// The record representing the column type.
    /// </summary>
    public GlideType? Type
    {
        get { lock (_syncRoot) { return _type; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _typeName, ref _type);
    }

    #endregion

    #region Reference Navigation Property

    private string? _refTableName;

    /// <summary>
    /// The value of the associated name for the "Reference" (<see cref="SnApiConstants.JSON_KEY_REFERENCE" />) column.
    /// </summary>
    [BackingField(nameof(_refTableName))]
    public string? RefTableName
    {
        get { lock (_syncRoot) { return _reference?.Name ?? _refTableName; } }
        set => SetOptionalNonEmptyNavForeignKey(_syncRoot, value, ref _refTableName, ref _reference, t => t.Name);
    }

    private Table? _reference;

    /// <summary>
    /// The table the current column refers to.
    /// </summary>
    public Table? Reference
    {
        get { lock (_syncRoot) { return _reference; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _refTableName, ref _reference);
    }

    #endregion

    #region Source Navigation Property

    private string _sourceFqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [BackingField(nameof(_sourceFqdn))]
    public string SourceFqdn
    {
        get { lock (_syncRoot) { return _source?.FQDN ?? _sourceFqdn; } }
        set => SetRequiredNonEmptyNavForeignKey(_syncRoot, value, ref _sourceFqdn, ref _source, s => s.FQDN);
    }

    private SourceInstance? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SourceInstance? Source
    {
        get { lock (_syncRoot) { return _source; } }
        set => SetRequiredNavProperty(_syncRoot, value, ref _sourceFqdn, ref _source, s => s.FQDN);
    }

    #endregion

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null)
        {
            if (string.IsNullOrWhiteSpace(_name))
                results.Add(new ValidationResult($"{nameof(Name)} cannot be empty.", new[] { nameof(Name) }));
            if (string.IsNullOrWhiteSpace(_label))
                results.Add(new ValidationResult($"{nameof(Label)} cannot be empty.", new[] { nameof(Label) }));
            if (string.IsNullOrWhiteSpace(TableName))
                results.Add(new ValidationResult($"{nameof(TableName)} cannot be empty.", new[] { nameof(TableName) }));
            if (string.IsNullOrWhiteSpace(SourceFqdn))
                results.Add(new ValidationResult($"{nameof(SourceFqdn)} cannot be empty.", new[] { nameof(SourceFqdn) }));
        }
        return results;
    }

    public bool Equals(Element? other) => other is not null && (ReferenceEquals(this, other) ||
        (Services.SnApiConstants.NameComparer.Equals(_name, other._name) && Services.SnApiConstants.NameComparer.Equals(_tableName, other._tableName)));

    public override bool Equals(object? obj) => Equals(obj as Element);

    public override int GetHashCode()
    {
        unchecked
        {
            return (21 + Services.SnApiConstants.NameComparer.GetHashCode(_name)) * 7 + (_table?.GetHashCode() ?? Services.SnApiConstants.NameComparer.GetHashCode(_tableName));
        }
    }

    public override string ToString() => nameof(Element) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(IsActive), JsonValue.Create(IsActive) },
        { nameof(IsArray), JsonValue.Create(IsArray) },
        { nameof(MaxLength), JsonValue.Create(MaxLength) },
        { nameof(Comments), JsonValue.Create(Comments) },
        { nameof(DefaultValue), JsonValue.Create(DefaultValue) },
        { nameof(IsDisplay), JsonValue.Create(IsDisplay) },
        { nameof(SizeClass), JsonValue.Create(SizeClass) },
        { nameof(IsMandatory), JsonValue.Create(IsMandatory) },
        { nameof(IsPrimary), JsonValue.Create(IsPrimary) },
        { nameof(IsReadOnly), JsonValue.Create(IsReadOnly) },
        { nameof(IsCalculated), JsonValue.Create(IsCalculated) },
        { nameof(IsUnique), JsonValue.Create(IsUnique) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Package), JsonValue.Create(_packageID) },
        { nameof(Table), JsonValue.Create(_tableName) },
        { nameof(Type), JsonValue.Create(_typeName) },
        { nameof(Reference), JsonValue.Create(_refTableName) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
