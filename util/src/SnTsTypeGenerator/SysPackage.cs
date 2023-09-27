using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;
/// <summary>
/// Represents an item from the "Package" (sys_package) table.
/// </summary>
public class SysPackage
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

    private string _value = string.Empty;

    /// <summary>
    /// Value of the package reference.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_value))]
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
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

    private Guid? _outputId;

    /// <summary>
    /// Foreign key associated with <see cref="Output"/> .
    /// </summary>
    [BackingField(nameof(_outputId))]
    public Guid? OutputId
    {
        get => _output?.Id ?? _outputId;
        set
        {
            lock (_syncRoot)
            {
                if (value.HasValue)
                {
                    if (!(_outputId.HasValue && value.Value.Equals(_outputId.Value)))
                    {
                        if (_output is null)
                            _outputId = value;
                        else if (value.Value.Equals(_output.Id))
                            _outputId = null;
                        else
                            _output = null;
                    }
                }
                else if (_outputId.HasValue)
                {
                    _outputId = null;
                    _output = null;
                }
            }
        }
    }

    private OutputFile? _output;

    /// <summary>
    /// Specifies an optional explicit output file.
    /// </summary>
    public OutputFile? Output
    {
        get => _output;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _output is null : _output is not null && ReferenceEquals(_output, value))
                    return;

                _output = value;
                _outputId = null;
            }
        }
    }

    internal SysPackage? FromRefElement(JsonElement? source)
    {
        throw new NotImplementedException();
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

    internal static void OnBuildEntity(EntityTypeBuilder<SysPackage> builder)
    {
        _ = builder.HasKey(s => s.Name);
        _ = builder.Property(nameof(Name)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(ShortDescription)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(SourceFqdn)).UseCollation(COLLATION_NOCASE);
        _ = builder.HasOne(t => t.Source).WithMany(s => s.Packages).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(t => t.Output).WithMany(s => s.Packages).HasForeignKey(t => t.OutputId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(SysPackage)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ShortDescription)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(OutputId)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(SysPackage)}_{nameof(OutputFile)}"" REFERENCES ""{nameof(OutputFile)}""(""{nameof(OutputFile.Id)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(SysPackage)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(SourceInfo)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(SysPackage)}"" PRIMARY KEY(""{nameof(Name)}"")
)";
    }
}