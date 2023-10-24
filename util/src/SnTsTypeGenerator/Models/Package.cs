using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an record from the "Sys Plugins" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_PLUGINS" />), "Custom Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_APP" />),
/// "Store Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_STORE_APP" />), or "Application" (<see cref="Services.SnApiConstants.TABLE_NAME_SYS_SCOPE" />) table.
/// https://dev93009.service-now.com/nav_to.do?uri=sys_db_object.do?sys_id=c7795303b4232110320f8dc279c80442
/// </summary>
[Table(nameof(Services.TypingsDbContext.Packages))]
public sealed class Package : IValidatableObject
{
    private readonly object _syncRoot = new();

    #region ID Property

    private string _id = string.Empty;

    /// <summary>
    /// Value of the "Class name" (<see cref="Services.SnApiConstants.JSON_KEY_SOURCE" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_id))]
    public string ID
    {
        get => _id;
        set => _id = value ?? string.Empty;
    }

    #endregion

    #region Name Property

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Class name" (<see cref="Services.SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    #endregion

    #region Version Property

    private string? _version;

    /// <summary>
    /// Value of the "Class name" (<see cref="Services.SnApiConstants.JSON_KEY_VERSION" />) column.
    /// </summary>
    [BackingField(nameof(_version))]
    public string? Version { get => _version; set => _version = value.NullIfWhiteSpace(); }

    #endregion

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    #region Source Navigation Property

    private string _sourceFqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [BackingField(nameof(_sourceFqdn))]
    public string SourceFqdn
    {
        get { lock (_syncRoot) { return _source?.FQDN ?? _sourceFqdn; } }
        set => SetRequiredNonEmptyNavForeignKey(_syncRoot, value, ref _sourceFqdn, ref _source, s => s.FQDN);
    }

    private SncSource? _source;

    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SncSource? Source
    {
        get { lock (_syncRoot) { return _source; } }
        set => SetRequiredNavProperty(_syncRoot, value, ref _sourceFqdn, ref _source, s => s.FQDN);
    }

    #endregion

    #region SysId Property

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Class name" (<see cref="Services.SnApiConstants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    #endregion

    #region Elements Property

    private HashSet<Element> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public HashSet<Element> Elements { get => _elements; set => _elements = value ?? new(); }

    #endregion

    #region Types Property

    private HashSet<GlideType> _types = new();

    [NotNull]
    [BackingField(nameof(_types))]
    public HashSet<GlideType> Types { get => _types; set => _types = value ?? new(); }

    #endregion

    #region Tables Property

    private HashSet<Table> _tables = new();

    [NotNull]
    [BackingField(nameof(_tables))]
    public HashSet<Table> Tables { get => _tables; set => _tables = value ?? new(); }

    #endregion

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null)
        {
            if (_id.Length == 0)
                results.Add(new ValidationResult($"{nameof(ID)} cannot be empty.", new[] { nameof(SourceFqdn) }));
            if (_sourceFqdn.Length == 0)
                results.Add(new ValidationResult($"{nameof(SourceFqdn)} cannot be empty.", new[] { nameof(SourceFqdn) }));
        }
        return results;
    }

    public bool Equals(Package? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(ID, other.ID));

    public override bool Equals(object? obj) => obj is Package other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(ID);

    public override string ToString() => nameof(Package) + new JsonObject()
    {
        { nameof(ID), JsonValue.Create(ID) },
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
