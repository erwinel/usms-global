namespace SnTsTypeGenerator;

public static class CmdLineConstants
{
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
    /// Gets the command line option for the <c><see cref="Dbfile" /></c> setting.
    /// </summary>
    public const char SHORTHAND_d = 'd';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Table" /></c> setting.
    /// </summary>
    public const char SHORTHAND_t = 't';

    /// <summary>
    /// Gets the command line option for the <c><see cref="UserName" /></c> setting.
    /// </summary>
    public const char SHORTHAND_u = 'u';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Password" /></c> setting.
    /// </summary>
    public const char SHORTHAND_p = 'p';

    /// <summary>
    /// Gets the command line option for the <c><see cref="ClientId" /></c> setting.
    /// </summary>
    public const char SHORTHAND_i = 'i';

    /// <summary>
    /// Gets the command line option for the <c><see cref="ClientSecret" /></c> setting.
    /// </summary>
    public const char SHORTHAND_x = 'x';

    /// <summary>
    /// Gets the command line option for the <c><see cref="RemoteURL" /></c> setting.
    /// </summary>
    public const char SHORTHAND_r = 'r';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Output" /></c> setting.
    /// </summary>
    public const char SHORTHAND_o = 'o';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Force" /></c> setting.
    /// </summary>
    public const char SHORTHAND_f = 'f';

    /// <summary>
    /// Gets the command line option for the <c><see cref="Mode" /></c> setting.
    /// </summary>
    public const char SHORTHAND_m = 'm';

    /// <summary>
    /// Gets the command line option value equivalent to <c><see cref="MODE_SCOPED" /></c>.
    /// </summary>
    public const string MODE_SCOPED_ABBR = "s";

    /// <summary>
    /// Gets the command line option value equivalent to <c><see cref="MODE_GLOBAL" /></c>.
    /// </summary>
    public const string MODE_GLOBAL_ABBR = "g";

    /// <summary>
    /// Value of <see cref="Mode"/> when the render mode is for global-scoped scripts.
    /// </summary>
    public const string MODE_GLOBAL = "global";
    
    /// <summary>
    /// Value of <see cref="Mode"/> when the render mode is for scoped app scripts.
    /// </summary>
    public const string MODE_SCOPED = "scoped";
}