
using Microsoft.Extensions.Configuration;
using static SnTsTypeGenerator.CmdLineConstants;

namespace SnTsTypeGenerator;

public class AppSettings
{
    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in
    /// the <see cref="DEFAULT_DbFile" /> constant.</remarks>
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

    private static readonly Dictionary<string, string> _switchMappings = new()
    {
        { $"-{SHORTHAND_d}", $"{nameof(SnTsTypeGenerator)}:{nameof(DbFile)}" },
        { $"-{SHORTHAND_t}", $"{nameof(SnTsTypeGenerator)}:{nameof(Table)}" },
        { $"-{SHORTHAND_u}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"-{SHORTHAND_p}", $"{nameof(SnTsTypeGenerator)}:{nameof(Password)}" },
        { $"-{SHORTHAND_r}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"-{SHORTHAND_m}", $"{nameof(SnTsTypeGenerator)}:{nameof(Mode)}" }
    };

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.AddCommandLine(args, _switchMappings);
    }
}
