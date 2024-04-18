using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace SnTsTypeGenerator.Models;

[Table(nameof(Services.TypingsDbContext.SourceInstances))]
public sealed class SourceInstance : IEquatable<SourceInstance>, IValidatableObject
{
    #region FQDN Property

    private string _fqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [Key]
    [BackingField(nameof(_fqdn))]
    public string FQDN
    {
        get => _fqdn;
        set => _fqdn = value.EmptyIfWhiteSpace();
    }

    #endregion

    #region Label Property

    private string _label = string.Empty;

    /// <summary>
    /// The display name.
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
    /// Indicates whether this instanced is a personal developer instance.
    /// </summary>
    public bool IsPersonalDev { get; set; }

    /// <summary>
    /// Date and time that the current source ServiceNow instance was accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; }

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

    #region Scopes Property

    private HashSet<Scope> _scopes = new();

    [NotNull]
    [BackingField(nameof(_scopes))]
    public HashSet<Scope> Scopes { get => _scopes; set => _scopes = value ?? new(); }

    #endregion

    #region Packages Property

    private HashSet<Package> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public HashSet<Package> Packages { get => _packages; set => _packages = value ?? new(); }

    #endregion

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null)
        {
            if (string.IsNullOrWhiteSpace(_fqdn))
                results.Add(new ValidationResult($"{nameof(FQDN)} cannot be empty.", new[] { nameof(FQDN) }));
            if (string.IsNullOrWhiteSpace(_label))
                results.Add(new ValidationResult($"{nameof(Label)} cannot be empty.", new[] { nameof(Label) }));
        }
        return results;
    }

    public bool Equals(SourceInstance? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(_fqdn, other._fqdn));

    public override bool Equals(object? obj) => obj is SourceInstance other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(_fqdn);

    public override string ToString() => nameof(SourceInstance) + new JsonObject()
    {
        { nameof(FQDN), JsonValue.Create(_fqdn) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(IsPersonalDev), JsonValue.Create(IsPersonalDev) },
        { nameof(LastAccessed), JsonValue.Create(LastAccessed) }
    }.ToJsonString();
}
