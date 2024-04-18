using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models;

[Table(nameof(Services.TypingsDbContext.PackageGroups))]
public class PackageGroup : IValidatableObject, IEquatable<PackageGroup>
{
    #region Name Property

    private string _name = string.Empty;

    /// <summary>
    /// Gets or sets the package name.
    /// </summary>
    [BackingField(nameof(_name))]
    [Key]
    public string Name
    {
        get => _name;
        set => _name = value.EmptyIfWhiteSpace();
    }

    #endregion

    /// <summary>
    /// Indicates whether the package group is considered a baseline package group.
    /// </summary>
    public bool IsBaseline { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null)
        {
            if (string.IsNullOrWhiteSpace(_name))
                results.Add(new ValidationResult($"{nameof(Name)} cannot be empty.", new[] { nameof(Name) }));
        }
        return results;
    }

    public bool Equals(PackageGroup? other) => other is not null && (ReferenceEquals(this, other) || NameComparer.Equals(_name, other._name));

    public override bool Equals(object? obj) => obj is PackageGroup other && Equals(other);

    public override int GetHashCode() => NameComparer.GetHashCode(_name);

    #region Packages Property

    private HashSet<Package> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public HashSet<Package> Packages { get => _packages; set => _packages = value ?? new(); }

    #endregion

    public override string ToString() => nameof(PackageGroup) + new JsonObject()
    {
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(IsBaseline), JsonValue.Create(IsBaseline) }
    }.ToJsonString();
}