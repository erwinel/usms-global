using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

/// <summary>
/// Represents an item from the "Field class" (<see cref="Constants.TABLE_NAME_SYS_GLIDE_OBJECT" />) table.
/// </summary>
[Table(nameof(GlideType))]
public class GlideType
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
    /// Value of the "Name" (<see cref="Constants.JSON_KEY_NAME" />) column.
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
    /// Value of the "Label" (<see cref="Constants.JSON_KEY_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }
    
    /// <summary>
    /// Value of the "Extends" (<see cref="Constants.JSON_KEY_SCALAR_TYPE" />) column.
    /// </summary>
    public string? ScalarType { get; set; }

    /// <summary>
    /// Value of the "Length" (<see cref="Constants.JSON_KEY_SCALAR_LENGTH" />) column.
    /// </summary>
    public int? ScalarLength { get; set; }

    /// <summary>
    /// Value of the "Class name" (<see cref="Constants.JSON_KEY_CLASS_NAME" />) column.
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Value of the "Use original value" (<see cref="Constants.JSON_KEY_USE_ORIGINAL_VALUE" />) column.
    /// </summary>
    public bool UseOriginalValue { get; set; }

    /// <summary>
    /// Value of the "Visible" (<see cref="Constants.JSON_KEY_VISIBLE" />) column.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="Constants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    private string? _packageName;

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
    
    /// <summary>
    /// The source package of the type.
    /// </summary>
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

    /// <summary>
    /// The scope for the type.
    /// </summary>
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

    private HashSet<ElementInfo> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<ElementInfo> Elements { get => _elements; set => _elements = value ?? new(); }

    internal static void OnBuildEntity(EntityTypeBuilder<GlideType> builder)
    {
        builder.HasKey(t => t.Name);
        builder.HasIndex(t => t.SysID).IsUnique();
        _ = builder.Property(nameof(Name)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Label)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SysID)).UseCollation(COLLATION_NOCASE);
        builder.HasOne(t => t.Source).WithMany(s => s.Types).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Package).WithMany(s => s.Types).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
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
    ""{nameof(IsVisible)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(LastUpdated)}"" DATETIME NOT NULL,
    ""{nameof(PackageName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SysPackage)}"" REFERENCES ""{nameof(SysPackage)}""(""{nameof(SysPackage.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(SourceInfo)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(GlideType)}"" PRIMARY KEY(""{nameof(Name)}""),
    CONSTRAINT ""UK_{nameof(GlideType)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(SysID)}\" ON \"{nameof(GlideType)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(UseOriginalValue)}\" ON \"{nameof(GlideType)}\" (\"{nameof(UseOriginalValue)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(IsVisible)}\" ON \"{nameof(GlideType)}\" (\"{nameof(IsVisible)}\")";
    }
}