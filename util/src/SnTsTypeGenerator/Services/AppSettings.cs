
using System.Collections.Immutable;
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
    /// The output file name.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Force overwrite of output file.
    /// </summary>
    public bool? Force { get; set; }

    /// <summary>
    /// The output mode.
    /// Should be <see cref="MODE_SCOPED"/>, <see cref="MODE_SCOPED_ABBR"/>, <see cref="MODE_GLOBAL"/>, or <see cref="MODE_GLOBAL_ABBR"/>.
    /// </summary>
    /// <remarks>The default behavior is <see cref="MODE_GLOBAL"/>.</remarks>
    public string? Mode { get; set; }

    public bool? Help { get; set; }

    public bool ShowHelp() => Help ?? false;

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
        { $"-{SHORTHAND_r}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"--{nameof(RemoteURL)}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"-{SHORTHAND_m}", $"{nameof(SnTsTypeGenerator)}:{nameof(Mode)}" },
        { $"--{nameof(Mode)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Mode)}" }
    };

    private static readonly Dictionary<string, string> _booleanSwitchMappings = new()
    {
        { $"-{SHORTHAND_i}", $"{nameof(SnTsTypeGenerator)}:{nameof(IncludeReferenced)}" },
        { $"--{nameof(IncludeReferenced)}", $"{nameof(SnTsTypeGenerator)}:{nameof(IncludeReferenced)}" },
        { $"-{SHORTHAND_b}", $"{nameof(SnTsTypeGenerator)}:{nameof(EmitBaseTypes)}" },
        { $"--{nameof(EmitBaseTypes)}", $"{nameof(SnTsTypeGenerator)}:{nameof(EmitBaseTypes)}" },
        { $"-{SHORTHAND_f}", $"{nameof(SnTsTypeGenerator)}:{nameof(Force)}" },
        { $"--{nameof(Force)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Force)}" },
        { $"-{SHORTHAND_h}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { $"-{SHORTHAND__3F_}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { $"--{nameof(Help)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" }
    };

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new BoolOptCommandLineConfigurationSource(args?.ToImmutableArray() ?? ImmutableArray<string>.Empty, _booleanSwitchMappings, _valueSwitchMappings));
    }
}
