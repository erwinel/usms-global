using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SnTsTypeGenerator;

[Table(nameof(GlideType))]
public class GlideType
{
    private readonly object _syncRoot = new();

    private string _sysID = string.Empty;

    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    private string _name = string.Empty;

    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    private string _label = string.Empty;

    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }
    
    public string? ScalarType { get; set; }

    public int? ScalarLength { get; set; }

    public string? ClassName { get; set; }

    public bool UseOriginalValue { get; set; }

    public bool Visible { get; set; }

    private string? _scopeValue;

    [BackingField(nameof(_scopeValue))]
    public string? ScopeValue
    {
        get => _scope?.Value ?? _scopeValue;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_scopeValue is not null)
                    {
                        _scopeValue = null;
                        _scope = null;
                    }
                }
                else if (_scopeValue is null || !value.Equals(_scopeValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_scope is null)
                        _scopeValue = value;
                    else if (value.Equals(_scope.Value, StringComparison.InvariantCultureIgnoreCase))
                        _scopeValue = null;
                    else
                        _scope = null;
                }
            }
        }
    }

    private SysScope? _scope;

    public SysScope? Scope
    {
        get => _scope;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _scope is null : _scope is not null && ReferenceEquals(_scope, value))
                    return;

                _scope = value;
                _scopeValue = null;
            }
        }
    }

    private HashSet<ElementInfo> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<ElementInfo> Elements { get => _elements; set => _elements = value ?? new(); }

    internal static void OnBuildEntity(EntityTypeBuilder<GlideType> builder)
    {
        builder.HasKey(t => t.Name);
        builder.HasIndex(t => t.SysID).IsUnique();
        builder.HasOne(t => t.Scope).WithMany(s => s.Types).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(GlideType)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ScalarType)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(ScalarLength)}"" INT DEFAULT NULL,
    ""{nameof(ClassName)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(UseOriginalValue)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Visible)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(GlideType)}"" PRIMARY KEY(""{nameof(Name)}""),
    CONSTRAINT ""UK_{nameof(GlideType)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(SysID)}\" ON \"{nameof(GlideType)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(UseOriginalValue)}\" ON \"{nameof(GlideType)}\" (\"{nameof(UseOriginalValue)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(Visible)}\" ON \"{nameof(GlideType)}\" (\"{nameof(Visible)}\")";
    }
}