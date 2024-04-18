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
    /// Gets or sets the value corresponding to the "Name" (<see cref="Services.SnApiConstants.JSON_KEY_NAME" />) column.
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
    /// Gets or sets the value corresponding to the "Label" (<see cref="Services.SnApiConstants.JSON_KEY_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value.EmptyIfWhiteSpace();
    }

    #endregion

    #region ScalarType Property

    private string? _scalarType;

    /// <summary>
    /// Gets or sets the value corresponding to the "Extends" (<see cref="Services.SnApiConstants.JSON_KEY_SCALAR_TYPE" />) column.
    /// </summary>
    [BackingField(nameof(_scalarType))]
    public string? ScalarType { get => _scalarType; set => _scalarType = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Gets or sets the value corresponding to the "Length" (<see cref="Services.SnApiConstants.JSON_KEY_SCALAR_LENGTH" />) column.
    /// </summary>
    public int? ScalarLength { get; set; }

    #region UnderlyingType Property

    private string? _underlyingType;

    /// <summary>
    /// Gets or sets the value representing the type of object returned by the getGlideObject() method, which may be derived from of the "Class name" (<see cref="Services.SnApiConstants.JSON_KEY_CLASS_NAME" />) column.
    /// </summary>
    [BackingField(nameof(_underlyingType))]
    public string? UnderlyingType { get => _underlyingType; set => _underlyingType = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Gets or sets the value corresponding to the "Use original value" (<see cref="Services.SnApiConstants.JSON_KEY_USE_ORIGINAL_VALUE" />) column.
    /// </summary>
    public bool UseOriginalValue { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the "Visible" (<see cref="Services.SnApiConstants.JSON_KEY_VISIBLE" />) column.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_CASE_SENSITIVE" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool CaseSensitive { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_ENCODE_UTF8" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool EncodeUtf8 { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_OMIT_SYS_ORIGINAL" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool OmitSysOriginal { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_EDGE_ENCRYPTION_ENABLED" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool EdgeEncryptionEnabled { get; set; }

    #region Serializer Property

    private string? _serializer;

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_SERIALIZER" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    [BackingField(nameof(_serializer))]
    public string? Serializer { get => _serializer; set => _serializer = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_IS_MULTI_TEXT" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool IsMultiText { get; set; }

    #region PdfCellType Property

    private string? _pdfCellType;

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_PDF_CELL_TYPE" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    [BackingField(nameof(_pdfCellType))]
    public string? PdfCellType { get => _pdfCellType; set => _pdfCellType = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_NO_SORT" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool NoSort { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_NO_DATA_REPLICATE" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool NoDataReplicate { get; set; }

    /// <summary>
    /// Gets or sets the value corresponding to the <see cref="Services.SnApiConstants.JSON_KEY_NO_AUDIT" /> attribute from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    public bool NoAudit { get; set; }

    #region Attributes Property

    private string? _attributes;

    /// <summary>
    /// Gets or sets the unparsed, URI-encoded values from the "Attributes" (<see cref="Services.SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.
    /// </summary>
    [BackingField(nameof(_attributes))]
    public string? Attributes { get => _attributes; set => _attributes = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Gets or sets the date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    #region Package Navigation Property

    /// <summary>
    /// Gets or sets the name of the associated record for the "Package" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_PACKAGE" />) column.
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
    /// Gets or sets the source package of the type.
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
    /// Gets or sets the unique identifier of the associated record for the "Application" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_SCOPE" />) column.
    /// </summary>
    [BackingField(nameof(_scopeValue))]
    public string? ScopeValue
    {
        get { lock (_syncRoot) { return _scope?.Value ?? _scopeValue; } }
        set => SetOptionalNonEmptyNavForeignKey(_syncRoot, value, ref _scopeValue, ref _scope, s => s.Value);
    }

    private Scope? _scope;

    /// <summary>
    /// Gets or sets the scope for the type.
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
    /// Gets or sets the value corresponding to the "Sys ID" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    #endregion

    #region ElementType Property

    private string? _elementType;

    /// <summary>
    /// Gets the TypeScript GlideElement type name.
    /// </summary>
    /// <value>The TypeScript name to use when referring to the corresponding GlideElement object or <see langword="null"/> to use the default type name.</value>
    /// <remarks>This should correspond to the name of a TypeScript definition in the <c>ElementTypes.d.ts</c> file in the <c>Resources/ts/global</c> or <c>Resources/ts/scoped</c> folder.</remarks>
    [BackingField(nameof(_elementType))]
    public string? ElementType { get => _elementType; set => _elementType = value.NullIfWhiteSpace(); }

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
            if (string.IsNullOrWhiteSpace(SourceFqdn))
                results.Add(new ValidationResult($"{nameof(SourceFqdn)} cannot be empty.", new[] { nameof(SourceFqdn) }));
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
        { nameof(UnderlyingType), JsonValue.Create(UnderlyingType) },
        { nameof(UseOriginalValue), JsonValue.Create(UseOriginalValue) },
        { nameof(IsVisible), JsonValue.Create(IsVisible) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Package), JsonValue.Create(_packageID) },
        { nameof(Scope), JsonValue.Create(_scopeValue) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
