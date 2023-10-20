using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static SnTsTypeGenerator.Models.EntityAccessors;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Represents an item from the "Application" (<see cref="SnApiConstants.TABLE_NAME_SYS_SCOPE" />) table.
/// </summary>
[Table(nameof(Services.TypingsDbContext.Scopes))]
public sealed class Scope : IEquatable<Scope>, IValidatableObject
{
    private readonly object _syncRoot = new();

    #region Value Property

    private string _value = string.Empty;

    [Key]
    [BackingField(nameof(_value))]
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
    }

    #endregion

    #region Name Property

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Name" (<see cref="SnApiConstants.JSON_KEY_NAME" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    #endregion

    #region ShortDescription Property

    private string? _shortDescription;
    [BackingField(nameof(_shortDescription))]
    public string? ShortDescription { get => _shortDescription; set => _shortDescription = value.NullIfWhiteSpace(); }

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

    #region SysID Property

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
            if (_value.Length switch
            {
                0 => true,
                1 => char.IsWhiteSpace(_value[0]),
                _ => _value.All(char.IsWhiteSpace),
            })
                results.Add(new ValidationResult($"{nameof(Value)} cannot be empty.", new[] { nameof(Value) }));
            if (_name.Length switch
            {
                0 => true,
                1 => char.IsWhiteSpace(_name[0]),
                _ => _name.All(char.IsWhiteSpace),
            })
                results.Add(new ValidationResult($"{nameof(Name)} cannot be empty.", new[] { nameof(Name) }));
            if (_sourceFqdn is null)
                results.Add(new ValidationResult($"{nameof(SourceFqdn)} cannot be null.", new[] { nameof(SourceFqdn) }));
        }
        return results;
    }

    public bool Equals(Scope? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(_value, other._value));

    public override bool Equals(object? obj) => obj is Scope other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(_value);

    public override string ToString() => nameof(Scope) + new JsonObject()
    {
        { nameof(Value), JsonValue.Create(_value) },
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(ShortDescription), JsonValue.Create(ShortDescription) },
        { nameof(LastUpdated), JsonValue.Create(LastUpdated) },
        { nameof(Source), JsonValue.Create(_sourceFqdn) },
        { nameof(SysID), JsonValue.Create(_sysID) }
    }.ToJsonString();
}
