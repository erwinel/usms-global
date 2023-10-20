using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;
/// <summary>
/// Represents an item from the "Package" (sys_package) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Packages))]
public sealed class Package : IEquatable<Package>, IValidatableObject
{
    private readonly object _syncRoot = new();

    #region Name Property

    private string _name = string.Empty;

    /// <summary>
    /// Display name of the package.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    #endregion

    #region ShortDescription Property

    private string? _shortDescription;
    [BackingField(nameof(_shortDescription))]
    public string? ShortDescription { get => _shortDescription; set => _shortDescription = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

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

    #region SysId Property

    private string _sysId = string.Empty;

    /// <summary>
    /// Value of the package reference.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysId))]
    public string SysId
    {
        get => _sysId;
        set => _sysId = value ?? string.Empty;
    }

    #endregion

    #region Types Property

    private HashSet<GlideType> _types = new();

    [NotNull]
    [BackingField(nameof(_types))]
    public HashSet<GlideType> Types { get => _types; set => _types = value ?? new(); }

    #endregion

    #region Tables Property

    private HashSet<Table> _tables = new();

    [NotNull]
    [BackingField(nameof(_tables))]
    public HashSet<Table> Tables { get => _tables; set => _tables = value ?? new(); }

    #endregion

    #region Elements Property

    private HashSet<Element> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

    #endregion

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null && _sourceFqdn is null)
            results.Add(new ValidationResult($"{nameof(SourceFqdn)} cannot be null.", new[] { nameof(SourceFqdn) }));
        return results;
    }

    public bool Equals(Package? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(_name, other._name));

    public override bool Equals(object? obj) => obj is Package other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(_name);

    public override string ToString() => nameof(Package) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(ShortDescription), JsonValue.Create(ShortDescription) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysId), JsonValue.Create(_sysId) }
    }.ToJsonString();
}
