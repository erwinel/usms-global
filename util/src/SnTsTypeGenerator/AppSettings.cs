
using Microsoft.Extensions.Configuration;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public class AppSettings
{
    /// <summary>
    /// Gets the command line option for the <c><see cref="Dbfile" /></c> setting.
    /// </summary>
    public const string SHORTHAND_d = "-d";

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in
    /// the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Table" /></c> setting.
    /// </summary>
    public const string SHORTHAND_t = "-t";

    /// <summary>
    /// Comma-separated database table names to generate typings for.
    /// </summary>
    public string? Table { get; set; }

    /// <summary>
    /// Database table names to generate typings for.
    /// </summary>
    public List<string>? Tables { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="UserName" /></c> setting.
    /// </summary>
    public const string SHORTHAND_u = "-u";

    /// <summary>
    /// Login user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Password" /></c> setting.
    /// </summary>
    public const string SHORTHAND_p = "-p";

    /// <summary>
    /// Password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="ClientId" /></c> setting.
    /// </summary>
    public const string SHORTHAND_i = "-i";

    /// <summary>
    /// Gets or sets the client ID in the remote ServiceNow instance's Application Registry.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="ClientSecret" /></c> setting.
    /// </summary>
    public const string SHORTHAND_x = "-x";

    /// <summary>
    /// Gets or sets the client secret in the remote ServiceNow instance's Application Registry.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="RemoteURL" /></c> setting.
    /// </summary>
    public const string SHORTHAND_r = "-r";

    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURL { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Output" /></c> setting.
    /// </summary>
    public const string SHORTHAND_o = "-o";

    /// <summary>
    /// The output file name.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Force" /></c> setting.
    /// </summary>
    public const string SHORTHAND_f = "-f";

    /// <summary>
    /// Force overwrite of output file.
    /// </summary>
    public bool? Force { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Scoped" /></c> setting.
    /// </summary>
    public const string SHORTHAND_s = "-s";

    /// <summary>
    /// Generate typings for use in scoped applications.
    /// </summary>
    public bool? Scoped { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Global" /></c> setting.
    /// </summary>
    public const string SHORTHAND_g = "-g";

    /// <summary>
    /// Generate typings for use in global applications.
    /// </summary>
    public bool? Global { get; set; }

    /// <summary>
    /// Gets the command line option for the <c><see cref="Help" /></c> setting.
    /// </summary>
    public const string SHORTHAND_h = "-h";

    /// <summary>
    /// Gets the command line switch for the <see cref="Help" /> application option option.
    /// </summary>
    public const string SHORTHAND__3F_ = "-?";

    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.</remarks>
    public bool? Help { get; set; }

    private static readonly Dictionary<string, string> _switchMappings = new()
    {
        { SHORTHAND_d, $"{nameof(SnTsTypeGenerator)}:{nameof(DbFile)}" },
        { SHORTHAND_t, $"{nameof(SnTsTypeGenerator)}:{nameof(Table)}" },
        { SHORTHAND_u, $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { SHORTHAND_p, $"{nameof(SnTsTypeGenerator)}:{nameof(Password)}" },
        { SHORTHAND_r, $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { SHORTHAND_g, $"{nameof(SnTsTypeGenerator)}:{nameof(Global)}" },
        { SHORTHAND_s, $"{nameof(SnTsTypeGenerator)}:{nameof(Scoped)}" },
        { SHORTHAND_h, $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { SHORTHAND__3F_, $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" }
    };

    internal static void Configure(string[] args, ConfigurationManager configuration)
    {
        configuration.AddCommandLine(args, _switchMappings);
    }
}
