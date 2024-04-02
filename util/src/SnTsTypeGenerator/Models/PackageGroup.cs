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
    #region FileName Property

    private string _fileName = string.Empty;

    /// <summary>
    /// Gets or sets the package name.
    /// </summary>
    [BackingField(nameof(_fileName))]
    public string FileName
    {
        get => _fileName;
        set => _fileName = value.EmptyIfWhiteSpace();
    }

    #endregion

    #region PackageName Property

    private string _packageName = string.Empty;

    // BUG: Package groups can have more than one package name to them. Maybe use 'DefaultPackageName'?
    /// <summary>
    /// Gets or sets the package name.
    /// </summary>
    [BackingField(nameof(_packageName))]
    public string PackageName
    {
        get => _packageName;
        set => _packageName = value.EmptyIfWhiteSpace();
    }

    #endregion

    #region Namespace Property

    private string _namespace = GLOBAL_NAMESPACE;

    /// <summary>
    /// Gets or sets the namespace scope.
    /// </summary>
    [BackingField(nameof(_namespace))]
    public string Namespace
    {
        get => _namespace;
        set => _namespace = value.AsNonEmpty(GLOBAL_NAMESPACE);
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
            if (string.IsNullOrWhiteSpace(_packageName))
                results.Add(new ValidationResult($"{nameof(PackageName)} cannot be empty.", new[] { nameof(PackageName) }));
        }
        return results;
    }

    public bool Equals(PackageGroup? other) => other is not null && (ReferenceEquals(this, other) || Services.SnApiConstants.NameComparer.Equals(_packageName, other._packageName));

    public override bool Equals(object? obj) => obj is PackageGroup other && Equals(other);

    public override int GetHashCode() => Services.SnApiConstants.NameComparer.GetHashCode(_packageName);

    #region Packages Property

    private HashSet<Package> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public HashSet<Package> Packages { get => _packages; set => _packages = value ?? new(); }

    #endregion

    public override string ToString() => nameof(PackageGroup) + new JsonObject()
    {
        { nameof(PackageName), JsonValue.Create(_packageName) },
        { nameof(IsBaseline), JsonValue.Create(IsBaseline) }
    }.ToJsonString();
}