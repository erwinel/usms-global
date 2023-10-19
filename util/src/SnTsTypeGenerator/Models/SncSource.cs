using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace SnTsTypeGenerator.Models;

[Table(nameof(Services.TypingsDbContext.Sources))]
public class SncSource : IEquatable<SncSource>
{
    private string _fqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [Key]
    [BackingField(nameof(_fqdn))]
    public string FQDN
    {
        get => _fqdn;
        set => _fqdn = value ?? string.Empty;
    }

    private string _label = string.Empty;

    /// <summary>
    /// The display name.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    /// <summary>
    /// Indicates whether this instanced is a personal developer instance.
    /// </summary>
    public bool IsPersonalDev { get; set; }

    /// <summary>
    /// Date and time that the current source ServiceNow instance was accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; }

    private HashSet<GlideType> _types = new();

    [NotNull]
    [BackingField(nameof(_types))]
    public virtual HashSet<GlideType> Types { get => _types; set => _types = value ?? new(); }

    private HashSet<Table> _tables = new();

    [NotNull]
    [BackingField(nameof(_tables))]
    public virtual HashSet<Table> Tables { get => _tables; set => _tables = value ?? new(); }

    private HashSet<Element> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

    private HashSet<Scope> _scopes = new();

    [NotNull]
    [BackingField(nameof(_scopes))]
    public virtual HashSet<Scope> Scopes { get => _scopes; set => _scopes = value ?? new(); }

    private HashSet<Package> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public virtual HashSet<Package> Packages { get => _packages; set => _packages = value ?? new(); }

    public bool Equals(SncSource? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(_fqdn, other._fqdn));

    public override bool Equals(object? obj) => obj is SncSource other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(_fqdn);

    public override string ToString() => nameof(SncSource) + new JsonObject()
    {
        { nameof(FQDN), JsonValue.Create(_fqdn) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(IsPersonalDev), JsonValue.Create(IsPersonalDev) },
        { nameof(LastAccessed), JsonValue.Create(LastAccessed) }
    }.ToJsonString();
}
