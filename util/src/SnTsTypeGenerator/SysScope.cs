using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SnTsTypeGenerator;

[Table(nameof(SysScope))]
public class SysScope
{
    private string _value = string.Empty;

    [Key]
    [BackingField(nameof(_value))]
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
    }

    private string _name = string.Empty;

    [NotNull]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    public string? ShortDescription { get; set; }

    private string _sysID = string.Empty;

    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
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
        builder.HasKey(s => s.Value);
        builder.HasIndex(s => s.SysID).IsUnique();
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(SysScope)}"" (
    ""{nameof(Value)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(SysScope)}"" PRIMARY KEY(""{nameof(Value)}""),
    CONSTRAINT ""UK_{nameof(SysScope)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(SysScope)}_{nameof(SysID)}\" ON \"{nameof(SysScope)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
    }
}
