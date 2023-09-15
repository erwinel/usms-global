using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SnTsTypeGenerator;

[Table(nameof(ElementInfo))]
public class ElementInfo
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

    internal const string COLNAME_Name = "element";

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

    public bool IsActive { get; set; }

    public bool IsArray { get; set; }

    public int? MaxLength { get; set; }

    public string? Comments { get; set; }
    
    public string? DefaultValue { get; set; }
    
    public bool IsDisplay { get; set; }

    public bool IsMandatory { get; set; }

    public bool IsPrimary { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsUnique { get; set; }

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

    internal const string COLNAME_Table = "name";

    private string _tableName = string.Empty;

    [BackingField(nameof(_tableName))]
    public string TableName
    {
        get => _table?.Name ?? _tableName;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_table is null || !value.Equals(_table.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    _tableName = value;
                    _table = null;
                }
            }
        }
    }

    private TableInfo? _table;
    
    public TableInfo? Table
    {
        get => _table;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_table is null)
                        return;
                    _tableName = _table.Name;
                }
                else
                {
                    if (_table is not null && ReferenceEquals(_table, value))
                        return;
                    _table = value;
                    _tableName = string.Empty;
                }
            }
        }
    }

    private string _typeName = string.Empty;

    [BackingField(nameof(_typeName))]
    public string TypeName
    {
        get => _type?.Name ?? _typeName;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_type is null || !value.Equals(_type.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    _typeName = value;
                    _type = null;
                }
            }
        }
    }

    private GlideType? _type;
    
    public GlideType? Type
    {
        get => _type;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_type is null)
                        return;
                    _typeName = _type.Name;
                }
                else
                {
                    if (_type is not null && ReferenceEquals(_type, value))
                        return;
                    _type = value;
                    _typeName = string.Empty;
                }
            }
        }
    }

    private string? _refTableName;

    [BackingField(nameof(_refTableName))]
    public string? RefTableName
    {
        get => _reference?.Name ?? _refTableName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_refTableName is not null)
                    {
                        _refTableName = null;
                        _reference = null;
                    }
                }
                else if (_refTableName is null || !value.Equals(_refTableName, StringComparison.InvariantCultureIgnoreCase))
                {

                    if (_reference is null)
                        _refTableName = value;
                    else if (value.Equals(_reference.Name, StringComparison.InvariantCultureIgnoreCase))
                        _refTableName = null;
                    else
                        _reference = null;
                }
            }
        }
    }

    private TableInfo? _reference;
    public TableInfo? Reference
    {
        get => _reference;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _reference is null : _reference is not null && ReferenceEquals(_reference, value))
                    return;

                _reference = value;
                _refTableName = null;
            }
        }
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(ElementInfo)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(IsActive)}"" BIT NOT NULL DEFAULT 1,
    ""{nameof(IsArray)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(MaxLength)}"" INT DEFAULT NULL,
    ""{nameof(Comments)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(DefaultValue)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(IsDisplay)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsMandatory)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsPrimary)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsReadOnly)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsUnique)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TableName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(Table)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TypeName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(GlideType)}"" REFERENCES ""{nameof(GlideType)}""(""{nameof(GlideType.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(RefTableName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(TableInfo)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(TableInfo.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(ElementInfo)}"" PRIMARY KEY(""{nameof(Name)}""),
    CONSTRAINT ""UK_{nameof(ElementInfo)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(SysID)}\" ON \"{nameof(ElementInfo)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(IsActive)}\" ON \"{nameof(ElementInfo)}\" (\"{nameof(IsActive)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(IsDisplay)}\" ON \"{nameof(ElementInfo)}\" (\"{nameof(IsDisplay)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(IsPrimary)}\" ON \"{nameof(ElementInfo)}\" (\"{nameof(IsPrimary)}\")";
    }

    internal static void OnBuildEntity(EntityTypeBuilder<ElementInfo> builder)
    {
        builder.HasKey(t => t.Name);
        builder.HasIndex(t => t.SysID).IsUnique();
        builder.HasOne(t => t.Table).WithMany(s => s.Elements).HasForeignKey(t => t.TableName).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Type).WithMany(s => s.Elements).HasForeignKey(t => t.TypeName).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Scope).WithMany(s => s.Elements).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Reference).WithMany(s => s.ReferredBy).HasForeignKey(t => t.RefTableName).OnDelete(DeleteBehavior.Restrict);
    }
}