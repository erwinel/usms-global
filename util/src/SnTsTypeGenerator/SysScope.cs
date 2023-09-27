using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

/// <summary>
/// Represents an item from the "Application" (<see cref="Constants.TABLE_NAME_SYS_SCOPE" />) table.
/// </summary>
[Table(nameof(SysScope))]
public class SysScope
{
    private readonly object _syncRoot = new();

    private string _value = string.Empty;

    [Key]
    [BackingField(nameof(_value))]
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
    }

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Name" (<see cref="Constants.JSON_KEY_NAME" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    public string? ShortDescription { get; set; }

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="Constants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

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
        get => _source?.FQDN ?? _sourceFqdn;
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

    private SourceInfo? _source;
    
    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SourceInfo? Source
    {
        get => _source;
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

    internal static SysScope? FromElement(JsonElement element)
    {
        throw new NotImplementedException();
    }

    internal static void OnBuildEntity(EntityTypeBuilder<SysScope> builder)
    {
        _ = builder.HasKey(s => s.Value);
        _ = builder.HasIndex(s => s.SysID).IsUnique();
        _ = builder.Property(nameof(Value)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Name)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ShortDescription)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SysID)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SourceFqdn)).UseCollation(COLLATION_NOCASE);
        _ = builder.HasOne(t => t.Source).WithMany(s => s.Scopes).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(SysScope)}"" (
    ""{nameof(Value)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ShortDescription)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(SysScope)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(SourceInfo)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(SysScope)}"" PRIMARY KEY(""{nameof(Value)}""),
    CONSTRAINT ""UK_{nameof(SysScope)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(SysScope)}_{nameof(SysID)}\" ON \"{nameof(SysScope)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
    }
}
