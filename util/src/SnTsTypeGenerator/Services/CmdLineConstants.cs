namespace SnTsTypeGenerator.Services;

public static class CmdLineConstants
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"Typings.db";

    internal const string DEFAULT_OUTPUT_FILENAME = "types.d.ts";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string DASH_help = "--help";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SHORTHAND_h = "-h";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SHORTHAND__3F_ = "-?";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SLASH_h = "/h";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SLASH_help = "/help";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SLASH__3F_ = "/?";
    
    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Dbfile" /></c> setting.
    /// </summary>
    public const char SHORTHAND_d = 'd';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Table" /></c> setting.
    /// </summary>
    public const char SHORTHAND_t = 't';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.UserName" /></c> setting.
    /// </summary>
    public const char SHORTHAND_u = 'u';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Password" /></c> setting.
    /// </summary>
    public const char SHORTHAND_p = 'p';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientId" /></c> setting.
    /// </summary>
    public const char SHORTHAND_i = 'i';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientSecret" /></c> setting.
    /// </summary>
    public const char SHORTHAND_x = 'x';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.RemoteURL" /></c> setting.
    /// </summary>
    public const char SHORTHAND_r = 'r';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Output" /></c> setting.
    /// </summary>
    public const char SHORTHAND_o = 'o';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Force" /></c> setting.
    /// </summary>
    public const char SHORTHAND_f = 'f';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Mode" /></c> setting.
    /// </summary>
    public const char SHORTHAND_m = 'm';

    /// <summary>
    /// Gets the <see cref="AppSettings.Mode"/> equivalent to <c><see cref="MODE_SCOPED" /></c>.
    /// </summary>
    public const string MODE_SCOPED_ABBR = "s";

    /// <summary>
    /// Gets the <see cref="AppSettings.Mode"/> equivalent to <c><see cref="MODE_GLOBAL" /></c>.
    /// </summary>
    public const string MODE_GLOBAL_ABBR = "g";

    /// <summary>
    /// Value of <see cref="AppSettings.Mode"/> when the render mode is for global-scoped scripts.
    /// </summary>
    public const string MODE_GLOBAL = "global";
    
    /// <summary>
    /// Value of <see cref="AppSettings.Mode"/> when the render mode is for scoped app scripts.
    /// </summary>
    public const string MODE_SCOPED = "scoped";
}