using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

/// <summary>
/// This defines an explicit output file base name.
/// </summary>
public class OutputFile : IValidatableObject
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

    internal static void OnBuildEntity(EntityTypeBuilder<OutputFile> builder)
    {
        _ = builder.HasKey(s => s.Id);
        _ = builder.HasIndex(s => s.Name).IsUnique();
        _ = builder.Property(nameof(Label)).UseCollation(COLLATION_NOCASE);
        _ = builder.Property(nameof(Name)).UseCollation(COLLATION_NOCASE);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(OutputFile)}"" (
    ""{nameof(Id)}"" UNIQUEIDENTIFIER NOt NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(OutputFile)}"" PRIMARY KEY(""{nameof(Id)}"")
)";
    }
    
    private HashSet<SysPackage> _packages = new();

    [NotNull]
    [BackingField(nameof(_packages))]
    public virtual HashSet<SysPackage> Packages { get => _packages; set => _packages = value ?? new(); }
}
