
using Microsoft.Extensions.Configuration;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public class AppSettings
{
    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Dbfile" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_d = "-d";

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in
    /// the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Table" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_t = "-t";

    /// <summary>
    /// Database table names to generate typings for.
    /// </summary>
    public List<string>? Table { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="UserName" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_u = "-u";

    /// <summary>
    /// Login user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Password" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_p = "-p";

    /// <summary>
    /// Password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the client ID from the ServiceNow Application Registry.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret from the ServiceNow Application Registry.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="RemoteURL" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_r = "-r";

    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURL { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Output" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_o = "-o";

    /// <summary>
    /// The output file name.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Force" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_f = "-f";

    /// <summary>
    /// Force overwrite of output file.
    /// </summary>
    public bool? Force { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Scoped" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_s = "-s";

    /// <summary>
    /// Generate typings for use in scoped applications.
    /// </summary>
    public bool? Scoped { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Global" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_g = "-g";

    /// <summary>
    /// Generate typings for use in global applications.
    /// </summary>
    public bool? Global { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Help" /></c> command line option.
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
        { SHORTHAND_d, nameof(DbFile) },
        { SHORTHAND_t, nameof(Table) },
        { SHORTHAND_u, nameof(UserName) },
        { SHORTHAND_p, nameof(Password) },
        { SHORTHAND_r, nameof(RemoteURL) },
        { SHORTHAND_g, nameof(Global) },
        { SHORTHAND_s, nameof(Scoped) },
        { SHORTHAND_h, nameof(Help) },
        { SHORTHAND__3F_, nameof(Help) }
    };

    internal static void Configure(string[] args, ConfigurationManager configuration)
    {
        configuration.AddCommandLine(args, _switchMappings);
    }
}
