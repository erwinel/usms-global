using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Table" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_DB_OBJECT" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Tables))]
public sealed class Table : IEquatable<Table>, IValidatableObject
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

    /// <summary>
    /// Value of the "Extensible" (<see cref="Services.SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    public bool IsExtendable { get; set; }

    #region AccessibleFrom Property

    private string _accessibleFrom = string.Empty;

    /// <summary>
    /// Value of the "Accessible from" (<see cref="Services.SnApiConstants.JSON_KEY_ACCESS" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_accessibleFrom))]
    public string AccessibleFrom
    {
        get => _accessibleFrom;
        set => _accessibleFrom = value ?? string.Empty;
    }

    #endregion

    #region ExtensionModel Property

    private string? _extensionModel;
    /// <summary>
    /// Value of the "Extension model" (<see cref="Services.SnApiConstants.JSON_KEY_EXTENSION_MODEL" />) column.
    /// </summary>
    [BackingField(nameof(_extensionModel))]
    public string? ExtensionModel { get => _extensionModel; set => _extensionModel = value.NullIfWhiteSpace(); }

    #endregion

    #region NumberPrefix Property

    private string? _numberPrefix;
    /// <summary>
    /// Associated value of the "Auto number" (<see cref="Services.SnApiConstants.JSON_KEY_NUMBER_REF" />) column.
    /// </summary>
    [BackingField(nameof(_numberPrefix))]
    public string? NumberPrefix { get => _numberPrefix; set => _numberPrefix = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Indicates whether the table is a TypeScript interface rather than a GlideRecord type.
    /// </summary>
    public bool IsInterface { get; set; }

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
    /// The source package of the table.
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
        get { lock (_syncRoot) { return _scope?.Name ?? _scopeValue; } }
        set => SetOptionalNonEmptyNavForeignKey(_syncRoot, value, ref _scopeValue, ref _scope, s => s.Value);
    }

    private Scope? _scope;

    /// <summary>
    /// The scope of the table.
    /// </summary>
    public Scope? Scope
    {
        get { lock (_syncRoot) { return _scope; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _scopeValue, ref _scope);
    }

    #endregion

    #region SuperClass Navigation Property

    private string? _superClassName;

    /// <summary>
    /// Table name associated with the "Extends table" (<see cref="Services.SnApiConstants.JSON_KEY_SUPER_CLASS" />) column.
    /// </summary>
    [BackingField(nameof(_superClassName))]
    public string? SuperClassName
    {
        get { lock (_syncRoot) { return _superClass?.Name ?? _superClassName; } }
        set => SetOptionalNonEmptyNavForeignKey(_syncRoot, value, ref _superClassName, ref _superClass, s => s.Name);
    }

    private Table? _superClass;

    /// <summary>
    /// The extended table.
    /// </summary>
    public Table? SuperClass
    {
        get { lock (_syncRoot) { return _superClass; } }
        set => SetOptionalNavProperty(_syncRoot, value, ref _superClassName, ref _superClass);
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

    #region Derived Property

    private HashSet<Table> _derived = new();

    [NotNull]
    [BackingField(nameof(_derived))]
    public HashSet<Table> Derived { get => _derived; set => _derived = value ?? new(); }

    #endregion

    #region Elements Property

    private HashSet<Element> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

    #endregion

    #region ReferredBy Property

    private HashSet<Element> _referredBy = new();

    [NotNull]
    [BackingField(nameof(_referredBy))]
    public HashSet<Element> ReferredBy { get => _referredBy; set => _referredBy = value ?? new(); }

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

    public bool Equals(Table? other) => other is not null && (ReferenceEquals(this, other) ||
        (Services.SnApiConstants.NameComparer.Equals(_name, other._name) && _scopeValue.NoCaseEquals(other._scopeValue)));

    public override bool Equals(object? obj) => Equals(obj as Table);

    public override int GetHashCode()
    {
        unchecked
        {
            return (21 + Services.SnApiConstants.NameComparer.GetHashCode(_scopeValue ?? "")) * 7 + Services.SnApiConstants.NameComparer.GetHashCode(_name);
        }
    }

    public override string ToString() => nameof(Table) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(IsExtendable), JsonValue.Create(IsExtendable) },
        { nameof(AccessibleFrom), JsonValue.Create(_accessibleFrom) },
        { nameof(ExtensionModel), JsonValue.Create(ExtensionModel) },
        { nameof(NumberPrefix), JsonValue.Create(NumberPrefix) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(IsInterface), JsonValue.Create(IsInterface) },
        { nameof(Package), JsonValue.Create(_packageID) },
        { nameof(Scope), JsonValue.Create(_scopeValue) },
        { nameof(SuperClass), JsonValue.Create(_superClassName) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
