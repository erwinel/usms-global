
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public class AppSettings
{
    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />.
    /// The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile { get; set; }

    /// <summary>
    /// Comma-separated database table names to generate typings for.
    /// </summary>
    public string? Table { get; set; }

    /// <summary>
    /// Database table names to generate typings for.
    /// </summary>
    public List<string>? Tables { get; set; }

    /// <summary>
    /// Emits typings for base types.
    /// </summary>
    public bool? EmitBaseTypes { get; set; }

    /// <summary>
    /// Emits referenced types as well.
    /// </summary>
    public bool? IncludeReferenced { get; set; }

    /// <summary>
    /// Login user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the client ID in the remote ServiceNow instance's Application Registry.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret in the remote ServiceNow instance's Application Registry.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURL { get; set; }

    /// <summary>
    /// Indicates whether to modify the target remote source.
    /// </summary>
    /// <remarks>Only the <see cref="RemoteURL"/>, <see cref="NewURL"/>, <see cref="IsPdi"/>, and <see cref="RemoteLabel"/> options are used when this is <see langword="true"/>.</remarks>
    // TODO: Add command line switch and help text
    public bool? ModifySource { get; set; }

    /// <summary>
    /// Used with the <see cref="ModifySource"/> option to change the remote ServiceNow instance URI from the value of this option to <see cref="RemoteURL"/>.
    /// </summary>
    // TODO: Add command line switch and help text
    public string? ExistingURL { get; set; }

    /// <summary>
    /// Indicates whether the remote instance is a Personal Developer Instance.
    /// </summary>
    // TODO: Add command line switch and help text
    public bool? IsPdi { get; set; }

    /// <summary>
    /// The descriptive name to use for the remote instance.
    /// </summary>
    // TODO: Add command line switch and help text
    public string? RemoteLabel { get; set; }

    /// <summary>
    /// The name of the package group for all new packages that could not be automatically grouped.
    /// </summary>
    // TODO: Add command line switch and help text
    public string? DefaultPackageGroup { get; set; }

    /// <summary>
    /// The output file name.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Force overwrite of output file.
    /// </summary>
    public bool? ForceOverwrite { get; set; }

    /// <summary>
    /// Indicates whether the typings are generated for globally-scoped scripting.
    /// </summary>
    public bool? GlobalScope { get; set; }

    /// <summary>
    /// All newly found packages that are active and not licensable on the remote instance are to be considered baseline.
    /// </summary>
    public bool? BaselineInit { get; set; }

    /// <summary>
    /// Gets package groups contained in <see cref="TypingsDbContext.PackageGroups"/> table and the <see cref="DefaultPackageGroups"/> setting.
    /// </summary>
    public bool? GetPackageGroups { get; set; }

    /// <summary>
    /// Gets remote sources contained in <see cref="TypingsDbContext.Sources"/> table.
    /// </summary>
    public bool? GetRemoteSources { get; set; }

    /// <summary>
    /// Gets the glide type mappings to refer to when adding new rows to the <see cref="Models.GlideType"/> table.
    /// </summary>
    public List<KnownGlideType>? KnownGlideTypes { get; set; }

    /// /// <summary>
    /// Gets the glide type mappings to refer to when adding new rows to the <see cref="Models.GlideType"/> table.
    /// </summary>
    public List<DefaultPackageGroup>? DefaultPackageGroups { get; set; }

    public bool? Help { get; set; }

    public bool ShowHelp() => Help ?? false;

    private ReadOnlyDictionary<string, KnownGlideType>? _knownGlideTypes = null;
    public ReadOnlyDictionary<string, KnownGlideType> GetKnownGlideTypes()
    {
        if (_knownGlideTypes is not null) return _knownGlideTypes;
        Dictionary<string, KnownGlideType> result = new(StringComparer.InvariantCultureIgnoreCase);
        _knownGlideTypes = new(result);
        if (KnownGlideTypes is not null)
            foreach (KnownGlideType t in KnownGlideTypes.Where(t => t is not null && !string.IsNullOrEmpty(t.Name)))
            {
                if (!result.ContainsKey(t.Name))
                    result.Add(t.Name, t);
            }
        foreach (KnownGlideType t in KnownGlideType.GetDefaultKnownTypes())
        {
            if (result.TryGetValue(t.Name, out KnownGlideType? existing))
            {
                if (string.IsNullOrWhiteSpace(existing.Label) && !string.IsNullOrWhiteSpace(t.Label))
                    existing.Label = t.Label;
                if (string.IsNullOrWhiteSpace(existing.ScalarType) && !string.IsNullOrWhiteSpace(t.ScalarType))
                    existing.ScalarType = t.ScalarType;
                if (string.IsNullOrWhiteSpace(existing.GlobalElementType) && !string.IsNullOrWhiteSpace(t.GlobalElementType))
                    existing.GlobalElementType = t.GlobalElementType;
                if (string.IsNullOrWhiteSpace(existing.ScopedElementType) && !string.IsNullOrWhiteSpace(t.ScopedElementType))
                    existing.ScopedElementType = t.ScopedElementType;
                if (!existing.ScalarLength.HasValue && t.ScalarLength.HasValue)
                    existing.ScalarLength = t.ScalarLength;
            }
            else
                result.Add(t.Name, t);
        }
        return _knownGlideTypes;
    }

    private static readonly Dictionary<string, string> _valueSwitchMappings = new()
    {
        { $"-{SHORTHAND_d}", $"{nameof(SnTsTypeGenerator)}:{nameof(DbFile)}" },
        { $"--{nameof(DbFile)}", $"{nameof(SnTsTypeGenerator)}:{nameof(DbFile)}" },
        { $"-{SHORTHAND_t}", $"{nameof(SnTsTypeGenerator)}:{nameof(Table)}" },
        { $"--{nameof(Table)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Table)}" },
        { $"-{SHORTHAND_u}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"--{nameof(UserName)}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"-{SHORTHAND_p}", $"{nameof(SnTsTypeGenerator)}:{nameof(Password)}" },
        { $"--{nameof(Password)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Password)}" },
        { $"-{SHORTHAND_x}", $"{nameof(SnTsTypeGenerator)}:{nameof(ClientSecret)}" },
        { $"--{nameof(ClientSecret)}", $"{nameof(SnTsTypeGenerator)}:{nameof(ClientSecret)}" },
        { $"-{SHORTHAND_r}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"--{nameof(RemoteURL)}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"-{SHORTHAND_o}", $"{nameof(SnTsTypeGenerator)}:{nameof(Output)}" },
        { $"--{nameof(Output)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Output)}" }
    };

    private static readonly Dictionary<string, string> _booleanSwitchMappings = new()
    {
        { $"-{SHORTHAND_i}", $"{nameof(SnTsTypeGenerator)}:{nameof(IncludeReferenced)}" },
        { $"--{nameof(IncludeReferenced)}", $"{nameof(SnTsTypeGenerator)}:{nameof(IncludeReferenced)}" },
        { $"-{SHORTHAND_b}", $"{nameof(SnTsTypeGenerator)}:{nameof(EmitBaseTypes)}" },
        { $"--{nameof(EmitBaseTypes)}", $"{nameof(SnTsTypeGenerator)}:{nameof(EmitBaseTypes)}" },
        { $"-{SHORTHAND_f}", $"{nameof(SnTsTypeGenerator)}:{nameof(ForceOverwrite)}" },
        { $"--{SHORTHAND_force}", $"{nameof(SnTsTypeGenerator)}:{nameof(ForceOverwrite)}" },
        { $"--{nameof(ForceOverwrite)}", $"{nameof(SnTsTypeGenerator)}:{nameof(ForceOverwrite)}" },
        { $"-{SHORTHAND_n}", $"{nameof(SnTsTypeGenerator)}:{nameof(BaselineInit)}" },
        { $"--{nameof(BaselineInit)}", $"{nameof(SnTsTypeGenerator)}:{nameof(BaselineInit)}" },
        { $"--{SHORTHAND_get_package_groups}", $"{nameof(SnTsTypeGenerator)}:{nameof(GetPackageGroups)}" },
        { $"--{SHORTHAND_get_remote_sources}", $"{nameof(SnTsTypeGenerator)}:{nameof(GetRemoteSources)}" },
        { $"-{SHORTHAND_h}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { $"-{SHORTHAND__3F_}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { $"--{nameof(Help)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" }
    };

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new AltCommandLineConfigurationSource(args?.ToImmutableArray() ?? ImmutableArray<string>.Empty, _booleanSwitchMappings, _valueSwitchMappings));
    }
}
