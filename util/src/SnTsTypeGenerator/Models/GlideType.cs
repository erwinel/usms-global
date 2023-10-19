using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Field class" (<see cref="SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Types))]
public sealed class GlideType : IEquatable<GlideType>, IValidatableObject
{
    private readonly object _syncRoot = new();

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Name" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    private string _label = string.Empty;

    /// <summary>
    /// Value of the "Label" (<see cref="SnApiConstants.JSON_KEY_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    private string? _scalarType;
    /// <summary>
    /// Value of the "Extends" (<see cref="SnApiConstants.JSON_KEY_SCALAR_TYPE" />) column.
    /// </summary>
    [BackingField(nameof(_scalarType))]
    public string? ScalarType { get => _scalarType; set => _scalarType = value.NullIfWhiteSpace(); }

    /// <summary>
    /// Value of the "Length" (<see cref="SnApiConstants.JSON_KEY_SCALAR_LENGTH" />) column.
    /// </summary>
    public int? ScalarLength { get; set; }

    private string? _className;
    /// <summary>
    /// Value of the "Class name" (<see cref="SnApiConstants.JSON_KEY_CLASS_NAME" />) column.
    /// </summary>
    [BackingField(nameof(_className))]
    public string? ClassName { get => _className; set => _className = value.NullIfWhiteSpace(); }

    /// <summary>
    /// Value of the "Use original value" (<see cref="SnApiConstants.JSON_KEY_USE_ORIGINAL_VALUE" />) column.
    /// </summary>
    public bool UseOriginalValue { get; set; }

    /// <summary>
    /// Value of the "Visible" (<see cref="SnApiConstants.JSON_KEY_VISIBLE" />) column.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="SnApiConstants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    private string? _packageName;

    [BackingField(nameof(_packageName))]
    public string? PackageName
    {
        get { lock (_syncRoot) { return _package?.Name ?? _packageName; } }
        set => SetOptionalNavForeignKey(_syncRoot, value, ref _packageName, ref _package, p => p.Name);
    }

    private Package? _package;

    /// <summary>
    /// The source package of the type.
    /// </summary>
    public Package? Package
    {
        get { lock (_syncRoot) { return _package; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _packageName, ref _package);
    }

    private string? _scopeValue;

    /// <summary>
    /// Value of the associated record for the "Application" (<see cref="SnApiConstants.JSON_KEY_SYS_SCOPE" />) column.
    /// </summary>
    [BackingField(nameof(_scopeValue))]
    public string? ScopeValue
    {
        get { lock (_syncRoot) { return _scope?.Value ?? _scopeValue; } }
        set => SetOptionalNonEmptyNavForeignKey(_syncRoot, value, ref _scopeValue, ref _scope, s => s.Value);
    }

    private Scope? _scope;

    /// <summary>
    /// The scope for the type.
    /// </summary>
    public Scope? Scope
    {
        get { lock (_syncRoot) { return _scope; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _scopeValue, ref _scope);
    }

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

    private SncSource? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SncSource? Source
    {
        get { lock (_syncRoot) { return _source; } }
        set => SetRequiredNavProperty(_syncRoot, value, ref _sourceFqdn, ref _source, s => s.FQDN);
    }

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    private HashSet<Element> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null)
        {
            if (_name.Length switch
            {
                0 => true,
                1 => char.IsWhiteSpace(_name[0]),
                _ => _name.All(char.IsWhiteSpace),
            })
                results.Add(new ValidationResult($"{nameof(Name)} cannot be empty.", new[] { nameof(Name) }));
            if (_label.Length switch
            {
                0 => true,
                1 => char.IsWhiteSpace(_label[0]),
                _ => _label.All(char.IsWhiteSpace),
            })
                results.Add(new ValidationResult($"{nameof(Label)} cannot be empty.", new[] { nameof(Label) }));
            if (_sourceFqdn is null)
                results.Add(new ValidationResult($"{nameof(SourceFqdn)} cannot be null.", new[] { nameof(SourceFqdn) }));
        }
        return results;
    }

    public bool Equals(GlideType? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(_name, other._name));

    public override bool Equals(object? obj) => obj is GlideType other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(_name);

    public override string ToString() => nameof(GlideType) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(ScalarType), JsonValue.Create(ScalarType) },
        { nameof(ScalarLength), JsonValue.Create(ScalarLength) },
        { nameof(ClassName), JsonValue.Create(ClassName) },
        { nameof(UseOriginalValue), JsonValue.Create(UseOriginalValue) },
        { nameof(IsVisible), JsonValue.Create(IsVisible) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Package), JsonValue.Create(_packageName) },
        { nameof(Scope), JsonValue.Create(_scopeValue) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
