using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Field class" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_GLIDE_OBJECT" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Types))]
public sealed class GlideType : IEquatable<GlideType>, IValidatableObject
{
    private readonly object _syncRoot = new();

    #region Name Property

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Name" (<see cref="Services.SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    #endregion

    #region Label Property

    private string _label = string.Empty;

    /// <summary>
    /// Value of the "Label" (<see cref="Services.SnApiConstants.JSON_KEY_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    #endregion

    #region ScalarType Property

    private string? _scalarType;
    /// <summary>
    /// Value of the "Extends" (<see cref="Services.SnApiConstants.JSON_KEY_SCALAR_TYPE" />) column.
    /// </summary>
    [BackingField(nameof(_scalarType))]
    public string? ScalarType { get => _scalarType; set => _scalarType = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Value of the "Length" (<see cref="Services.SnApiConstants.JSON_KEY_SCALAR_LENGTH" />) column.
    /// </summary>
    public int? ScalarLength { get; set; }

    #region ClassName Property

    private string? _className;
    /// <summary>
    /// Value of the "Class name" (<see cref="Services.SnApiConstants.JSON_KEY_CLASS_NAME" />) column.
    /// </summary>
    [BackingField(nameof(_className))]
    public string? ClassName { get => _className; set => _className = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Value of the "Use original value" (<see cref="Services.SnApiConstants.JSON_KEY_USE_ORIGINAL_VALUE" />) column.
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

    #region Package Navigation Property

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    private string? _packageID;

    [BackingField(nameof(_packageID))]
    public string? PackageID
    {
        get { lock (_syncRoot) { return _package?.ID ?? _packageID; } }
        set => SetOptionalNavForeignKey(_syncRoot, value, ref _packageID, ref _package, p => p.ID);
    }

    private Package? _package;

    /// <summary>
    /// The source package of the type.
    /// </summary>
    public Package? Package
    {
        get { lock (_syncRoot) { return _package; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _packageID, ref _package);
    }

    #endregion

    #region Scope Navigation Property

    private string? _scopeValue;

    /// <summary>
    /// Value of the associated record for the "Application" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_SCOPE" />) column.
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

    private SncSource? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SncSource? Source
    {
        get { lock (_syncRoot) { return _source; } }
        set => SetRequiredNavProperty(_syncRoot, value, ref _sourceFqdn, ref _source, s => s.FQDN);
    }

    #endregion

    #region SysID Property

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    #endregion

    #region GlobalElementType Property

    private string? _globalElementType;

    /// <summary>
    /// Gets the TypeScript GlideElement type name used in globally-scoped scripts.
    /// </summary>
    /// <value>The TypeScript name to use when referring to the corresponding GlideElement object in globally-scoped scripts or <see langword="null"/> to use the default type name.</value>
    /// <remarks>This should correspond to the name of a TypeScript definition that is assignable from the 'GlideElement' type,
    /// and is defined in any of the <c>*.d.ts</c> files in the <c>Resources/ts/global</c> folder.</remarks>
    [BackingField(nameof(_globalElementType))]
    public string? GlobalElementType { get => _globalElementType; set => _globalElementType = value.NullIfWhiteSpace(); }

    #endregion

    #region ScopedElementType Property

    private string? _scopedElementType;

    /// <summary>
    /// Gets the TypeScript GlideElement type name used in scoped applications.
    /// </summary>
    /// <value>The TypeScript name to use when referring to the corresponding GlideElement object in scoped applications or <see langword="null"/> to use the default type name.</value>
    /// <remarks>This should correspond to the name of a TypeScript definition that is assignable from the 'GlideElement' type,
    /// and is defined in any of the <c>*.d.ts</c> files in the <c>Resources/ts/scoped</c> folder.</remarks>
    [BackingField(nameof(_scopedElementType))]
    public string? ScopedElementType { get => _scopedElementType; set => _scopedElementType = value.NullIfWhiteSpace(); }

    #endregion

    #region Elements Property

    private HashSet<Element> _elements = new();

    /// <summary>
    /// Gets the elements (columns) that use this type.
    /// </summary>
    /// <returns>The <see cref="Element"/> objects that reference this type.</returns>
    [NotNull]
    [BackingField(nameof(_elements))]
    public HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

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
        { nameof(Package), JsonValue.Create(_packageID) },
        { nameof(Scope), JsonValue.Create(_scopeValue) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
