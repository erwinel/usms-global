using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Table" (<see cref="SnApiConstants.TABLE_NAME_SYS_DB_OBJECT" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Tables))]
public class Table : IEquatable<Table>
{
    private readonly object _syncRoot = new();

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Name" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column.
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
    /// Value of the "Label" (<see cref="SnApiConstants.JSON_KEY_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    /// <summary>
    /// Value of the "Extensible" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    public bool IsExtendable { get; set; }

    private string _accessibleFrom = string.Empty;

    /// <summary>
    /// Value of the "Accessible from" (<see cref="SnApiConstants.JSON_KEY_ACCESS" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_accessibleFrom))]
    public string AccessibleFrom
    {
        get => _accessibleFrom;
        set => _accessibleFrom = value ?? string.Empty;
    }

    /// <summary>
    /// Value of the "Extension model" (<see cref="SnApiConstants.JSON_KEY_EXTENSION_MODEL" />) column.
    /// </summary>
    public string? ExtensionModel { get; set; }

    /// <summary>
    /// Associated value of the "Auto number" (<see cref="SnApiConstants.JSON_KEY_NUMBER_REF" />) column.
    /// </summary>
    public string? NumberPrefix { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Indicates whether the table is a TypeScript interface rather than a GlideRecord type.
    /// </summary>
    public bool IsInterface { get; set; }

    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="SnApiConstants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    private string? _packageName;

    [BackingField(nameof(_packageName))]
    public string? PackageName
    {
        get { lock (_syncRoot) { return _package?.Name ?? _packageName; } }
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

    private Package? _package;

    /// <summary>
    /// The source package of the table.
    /// </summary>
    public Package? Package
    {
        get { lock(_syncRoot) { return _package; } }
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
    /// Value of the associated record for the "Application" (<see cref="SnApiConstants.JSON_KEY_SYS_SCOPE" />) column.
    /// </summary>
    [BackingField(nameof(_scopeValue))]
    public string? ScopeValue
    {
        get { lock (_syncRoot) { return _scope?.Name ?? _scopeValue; } }
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

    private Scope? _scope;

    /// <summary>
    /// The scope of the table.
    /// </summary>
    public Scope? Scope
    {
        get { lock(_syncRoot) { return _scope; } }
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

    private string? _superClassName;

    /// <summary>
    /// Table name associated with the "Extends table" (<see cref="SnApiConstants.JSON_KEY_SUPER_CLASS" />) column.
    /// </summary>
    [BackingField(nameof(_superClassName))]
    public string? SuperClassName
    {
        get { lock (_syncRoot) { return _superClass?.Name ?? _superClassName; } }
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

    private Table? _superClass;

    /// <summary>
    /// The extended table.
    /// </summary>
    public Table? SuperClass
    {
        get { lock(_syncRoot) { return _superClass; } }
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

    private string _sourceFqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [BackingField(nameof(_sourceFqdn))]
    public string SourceFqdn
    {
        get { lock (_syncRoot) { return _source?.FQDN ?? _sourceFqdn; } }
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

    private SncSource? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SncSource? Source
    {
        get { lock(_syncRoot) { return _source; } }
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

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    private HashSet<Table> _derived = new();

    [NotNull]
    [BackingField(nameof(_derived))]
    public virtual HashSet<Table> Derived { get => _derived; set => _derived = value ?? new(); }

    private HashSet<Element> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

    private HashSet<Element> _referredBy = new();

    [NotNull]
    [BackingField(nameof(_referredBy))]
    public virtual HashSet<Element> ReferredBy { get => _referredBy; set => _referredBy = value ?? new(); }

    public bool Equals(Table? other) => other is not null && (ReferenceEquals(this, other) ||
        (Services.SnApiConstants.NameComparer.Equals(_name, other._name) && _scopeValue.NoCaseEquals(other._scopeValue)));

    public override bool Equals(object? obj) => Equals(obj as Table);

    public override int GetHashCode()
    {
        unchecked
        {
            return (21 + Services.SnApiConstants.NameComparer.GetHashCode(_scopeValue ?? "")) * 7 + Services.SnApiConstants.NameComparer.GetHashCode(_name);
        }
    }

    public override string ToString() => nameof(Table) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(Label), JsonValue.Create(_label) },
        { nameof(IsExtendable), JsonValue.Create(IsExtendable) },
        { nameof(AccessibleFrom), JsonValue.Create(_accessibleFrom) },
        { nameof(ExtensionModel), JsonValue.Create(ExtensionModel) },
        { nameof(NumberPrefix), JsonValue.Create(NumberPrefix) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(IsInterface), JsonValue.Create(IsInterface) },
        { nameof(Package), JsonValue.Create(_packageName) },
        { nameof(Scope), JsonValue.Create(_scopeValue) },
        { nameof(SuperClass), JsonValue.Create(_superClassName) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
