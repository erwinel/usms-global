using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SnTsTypeGenerator;

/// <summary>
/// Represents an item from the "Dictionary Entry" (<see cref="Constants.TABLE_NAME_SYS_DICTIONARY" />) table.
/// </summary>
[Table(nameof(ElementInfo))]
public class ElementInfo
{
    private readonly object _syncRoot = new();

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

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Column name" (<see cref="Constants.JSON_KEY_ELEMENT" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    private string _label = string.Empty;

    /// <summary>
    /// Value of the "Column label" (<see cref="Constants.JSON_KEY_COLUMN_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    /// <summary>
    /// Value of the "Active" (<see cref="Constants.JSON_KEY_ACTIVE" />) column.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Value of the "Array" (<see cref="Constants.JSON_KEY_ARRAY" />) column.
    /// </summary>
    public bool IsArray { get; set; }

    /// <summary>
    /// Value of the "Max length" (<see cref="Constants.JSON_KEY_MAX_LENGTH" />) column.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Value of the "Comments" (<see cref="Constants.JSON_KEY_COMMENTS" />) column.
    /// </summary>
    public string? Comments { get; set; }
    
    /// <summary>
    /// Value of the "Default value" (<see cref="Constants.JSON_KEY_DEFAULT_VALUE" />) column.
    /// </summary>
    public string? DefaultValue { get; set; }
    
    /// <summary>
    /// Value of the "Display" (<see cref="Constants.JSON_KEY_DISPLAY" />) column.
    /// </summary>
    public bool IsDisplay { get; set; }

    /// <summary>
    /// Value of the "Sizeclass" (<see cref="Constants.JSON_KEY_SIZECLASS" />) column.
    /// </summary>
    public int? SizeClass { get; set; }

    /// <summary>
    /// Value of the "Mandatory" (<see cref="Constants.JSON_KEY_MANDATORY" />) column.
    /// </summary>
    public bool IsMandatory { get; set; }

    /// <summary>
    /// Value of the "Primary" (<see cref="Constants.JSON_KEY_PRIMARY" />) column.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Value of the "Read only" (<see cref="Constants.JSON_KEY_READ_ONLY" />) column.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Value of the "Calculated" (<see cref="Constants.JSON_KEY_VIRTUAL" />) column.
    /// </summary>
    public bool IsCalculated { get; set; }

    /// <summary>
    /// Value of the "Unique" (<see cref="Constants.JSON_KEY_UNIQUE" />) column.
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    private string? _packageName;

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="Constants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    [BackingField(nameof(_packageName))]
    public string? PackageName
    {
        get => _package?.Name ?? _packageName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_packageName is not null)
                    {
                        _packageName = null;
                        _package = null;
                    }
                }
                else if (_packageName is null || !value.Equals(_packageName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_package is null)
                        _packageName = value;
                    else if (value.Equals(_package.Name, StringComparison.InvariantCultureIgnoreCase))
                        _packageName = null;
                    else
                        _package = null;
                }
            }
        }
    }

    private SysPackage? _package;
    
    public SysPackage? Package
    {
        get => _package;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _package is null : _package is not null && ReferenceEquals(_package, value))
                    return;

                _package = value;
                _packageName = null;
            }
        }
    }

    private string? _scopeValue;

    /// <summary>
    /// Value of the associated record for the "Application" (<see cref="Constants.JSON_KEY_SYS_SCOPE" />) column.
    /// </summary>
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

    private string _tableName = string.Empty;

    /// <summary>
    /// Value of the "Table" (<see cref="Constants.JSON_KEY_NAME" />) column.
    /// </summary>
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

    /// <summary>
    /// Name of the related value for the "Type" (<see cref="Constants.JSON_KEY_INTERNAL_TYPE" />) column.
    /// </summary>
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

    /// <summary>
    /// The value of the associated name for the "Reference" (<see cref="Constants.JSON_KEY_REFERENCE" />) column.
    /// </summary>
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

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(ElementInfo)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(IsActive)}"" BIT NOT NULL DEFAULT 1,
    ""{nameof(IsArray)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(MaxLength)}"" INT DEFAULT NULL,
    ""{nameof(SizeClass)}"" INT DEFAULT NULL,
    ""{nameof(Comments)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(DefaultValue)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(IsDisplay)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsMandatory)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsPrimary)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsReadOnly)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsCalculated)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(IsUnique)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(LastUpdated)}"" DATETIME NOT NULL,
    ""{nameof(PackageName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SysPackage)}"" REFERENCES ""{nameof(SysPackage)}""(""{nameof(SysPackage.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TableName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(Table)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TypeName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(GlideType)}"" REFERENCES ""{nameof(GlideType)}""(""{nameof(GlideType.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(RefTableName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(TableInfo)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(TableInfo.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(SourceInfo)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
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
        builder.HasOne(t => t.Source).WithMany(s => s.Elements).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Table).WithMany(s => s.Elements).HasForeignKey(t => t.TableName).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Type).WithMany(s => s.Elements).HasForeignKey(t => t.TypeName).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Package).WithMany(s => s.Elements).HasForeignKey(t => t.PackageName).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Scope).WithMany(s => s.Elements).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Reference).WithMany(s => s.ReferredBy).HasForeignKey(t => t.RefTableName).OnDelete(DeleteBehavior.Restrict);
    }

    internal async Task<ElementInfo?> FromElementAsync(JsonElement source, RemoteLoaderService loaderService, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal async Task RenderPropertyGlobalAsync(IndentedTextWriter writer, string @namespace, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal async Task RenderPropertyScopedAsync(IndentedTextWriter writer, string @namespace, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
