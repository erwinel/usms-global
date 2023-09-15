using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SnTsTypeGenerator;

[Table(nameof(TableInfo))]
public class TableInfo
{
    private readonly object _syncRoot = new();

    internal const string COLNAME_SysID = "sys_id";

    private string _sysID = string.Empty;

    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    internal const string COLNAME_Name = "name";

    private string _name = string.Empty;

    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    internal const string COLNAME_Label = "label";

    private string _label = string.Empty;

    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }
    
    internal const string COLNAME_IsExtendable = "is_extendable";

    public bool IsExtendable { get; set; }

    internal const string COLNAME_Scope = "scope";

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

    internal const string COLNAME_SuperClass = "super_class";

    private string? _superClassName;


    [BackingField(nameof(_superClassName))]
    public string? SuperClassName
    {
        get => _superClass?.Name ?? _superClassName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_superClassName is not null)
                    {
                        _superClassName = null;
                        _superClass = null;
                    }
                }
                else if (_superClassName is null || !value.Equals(_superClassName, StringComparison.InvariantCultureIgnoreCase))
                {

                    if (_superClass is null)
                        _superClassName = value;
                    else if (value.Equals(_superClass.Name, StringComparison.InvariantCultureIgnoreCase))
                        _superClassName = null;
                    else
                        _superClass = null;
                }
            }
        }
    }

    private TableInfo? _superClass;

    public TableInfo? SuperClass
    {
        get => _superClass;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _superClass is null : _superClass is not null && ReferenceEquals(_superClass, value))
                    return;

                _superClass = value;
                _superClassName = null;
            }
        }
    }

    internal const string COLNAME_NumberPrefix = "number_ref";

    public string? NumberPrefix { get; set; }

    private HashSet<TableInfo> _derived = new();

    [NotNull]
    [BackingField(nameof(_derived))]
    public virtual HashSet<TableInfo> Derived { get => _derived; set => _derived = value ?? new(); }

    private HashSet<ElementInfo> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<ElementInfo> Elements { get => _elements; set => _elements = value ?? new(); }

    private HashSet<ElementInfo> _referredBy = new();

    [NotNull]
    [BackingField(nameof(_referredBy))]
    public virtual HashSet<ElementInfo> ReferredBy { get => _referredBy; set => _referredBy = value ?? new(); }

    internal static void OnBuildEntity(EntityTypeBuilder<TableInfo> builder)
    {
        builder.HasKey(t => t.Name);
        builder.HasIndex(t => t.SysID).IsUnique();
        builder.HasOne(t => t.Scope).WithMany(s => s.Tables).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.SuperClass).WithMany(s => s.Derived).HasForeignKey(t => t.SuperClassName).OnDelete(DeleteBehavior.Restrict);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(TableInfo)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(NumberPrefix)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(IsExtendable)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SuperClassName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SuperClass)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(TableInfo)}"" PRIMARY KEY(""{nameof(Name)}""),
    CONSTRAINT ""UK_{nameof(TableInfo)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(SysID)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(IsExtendable)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(IsExtendable)}\")";
    }
}