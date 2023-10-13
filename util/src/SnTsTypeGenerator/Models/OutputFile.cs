using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// This defines an explicit output file base name.
/// </summary>
[Table(nameof(Services.TypingsDbContext.OutputFiles))]
public class OutputFile : IValidatableObject, IEquatable<OutputFile>
{
    /// <summary>
    /// Gets or sets the unique identifier for the output file.
    /// </summary>
    public Guid Id { get; set; }

    private string _label = string.Empty;

    /// <summary>
    /// Display name of the output file.
    /// </summary>
    [Key]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }

    private string _name = string.Empty;

    /// <summary>
    /// Output file base name without extension.
    /// </summary>
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var entry = validationContext.GetService(typeof(EntityEntry)) as EntityEntry;
        if (entry is not null)
        {
            if (_name.Length switch
            {
                0 => true,
                1 => char.IsWhiteSpace(_name[0]),
                _ => _name.All(char.IsWhiteSpace),
            })
                results.Add(new ValidationResult("{nameof(Name)} cannot be empty.", new[] { nameof(Name) }));
            if (_label.Length switch
            {
                0 => true,
                1 => char.IsWhiteSpace(_label[0]),
                _ => _label.All(char.IsWhiteSpace),
            })
                results.Add(new ValidationResult("{nameof(Name)} cannot be empty.", new[] { nameof(Label) }));
            if (entry.State == EntityState.Added && Id.Equals(Guid.Empty))
                Id = Guid.NewGuid();
        }
        return results;
    }

    private HashSet<SysPackage> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public virtual HashSet<SysPackage> Packages { get => _packages; set => _packages = value ?? new(); }

    public bool Equals(OutputFile? other) => other is not null && (ReferenceEquals(this, other) || Id.Equals(other.Id));

    public override bool Equals(object? obj) => Equals(obj as ElementInfo);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => nameof(OutputFile) + new JsonObject()
    {
        { nameof(Id), JsonValue.Create(Id) },
        { nameof(Name), JsonValue.Create(_name) },
        { nameof(Label), JsonValue.Create(_label) }
    }.ToJsonString();
}
