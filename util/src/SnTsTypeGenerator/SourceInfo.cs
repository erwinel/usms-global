using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

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

    internal static void OnBuildEntity(EntityTypeBuilder<SourceInfo> builder)
    {
        _ = builder.HasKey(s => s.FQDN);
        _ = builder.HasIndex(t => t.IsPersonalDev);
        _ = builder.Property(nameof(FQDN)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Label)).UseCollation(COLLATION_NOCASE);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(SourceInfo)}"" (
    ""{nameof(FQDN)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(IsPersonalDev)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(LastAccessed)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    CONSTRAINT ""PK_{nameof(SourceInfo)}"" PRIMARY KEY(""{nameof(FQDN)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(SourceInfo)}_{nameof(IsPersonalDev)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(IsPersonalDev)}\")";
    }
}