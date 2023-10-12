using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SnTsTypeGenerator.Models;

[Table(nameof(TypingsDbContext.Sources))]
public class SourceInfo
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

    private HashSet<TableInfo> _tables = new();

    [NotNull]
    [BackingField(nameof(_tables))]
    public virtual HashSet<TableInfo> Tables { get => _tables; set => _tables = value ?? new(); }

    private HashSet<ElementInfo> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<ElementInfo> Elements { get => _elements; set => _elements = value ?? new(); }

    private HashSet<SysScope> _scopes = new();

    [NotNull]
    [BackingField(nameof(_scopes))]
    public virtual HashSet<SysScope> Scopes { get => _scopes; set => _scopes = value ?? new(); }

    private HashSet<SysPackage> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public virtual HashSet<SysPackage> Packages { get => _packages; set => _packages = value ?? new(); }
}