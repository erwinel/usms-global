using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

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

    private string _sysId = string.Empty;

    /// <summary>
    /// Value of the package reference.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysId))]
    public string SysId
    {
        get => _sysId;
        set => _sysId = value ?? string.Empty;
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
}