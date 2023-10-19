using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace SnTsTypeGenerator.Models;
/// <summary>
/// Represents an item from the "Package" (sys_package) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Packages))]
public class Package : IEquatable<Package>
{
    private readonly object _syncRoot = new();

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

    public string? ShortDescription { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    private string _sourceFqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [BackingField(nameof(_sourceFqdn))]
    public string SourceFqdn
    {
        get { lock(_syncRoot) { return _source?.FQDN ?? _sourceFqdn; } }
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_source is null || !value.Equals(_source.FQDN, StringComparison.InvariantCultureIgnoreCase))
                {
                    _sourceFqdn = value;
                    _source = null;
                }
            }
        }
    }

    private SncSource? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SncSource? Source
    {
        get { lock(_syncRoot) { return _source; } }
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_source is null)
                        return;
                    _sourceFqdn = _source.FQDN;
                }
                else
                {
                    if (_source is not null && ReferenceEquals(_source, value))
                        return;
                    _source = value;
                    _sourceFqdn = string.Empty;
                }
            }
        }
    }

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
