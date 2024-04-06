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
    internal const string SHORTHAND_h = "h";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SHORTHAND__3F_ = "?";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Dbfile" /></c> setting.
    /// </summary>
    public const char SHORTHAND_d = 'd';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Table" /></c> setting.
    /// </summary>
    public const char SHORTHAND_t = 't';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.EmitBaseTypes" /></c> setting.
    /// </summary>
    public const char SHORTHAND_b = 'b';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.IncludeReferenced" /></c> setting.
    /// </summary>
    public const char SHORTHAND_i = 'i';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.UserName" /></c> setting.
    /// </summary>
    public const char SHORTHAND_u = 'u';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Password" /></c> setting.
    /// </summary>
    public const char SHORTHAND_p = 'p';

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
    /// Gets the command line option for the <c><see cref="AppSettings.GetPackageGroups" /></c> setting.
    /// </summary>
    public const string SHORTHAND_get_package_groups = "get-package-groups";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.GetRemoteSources" /></c> setting.
    /// </summary>
    public const string SHORTHAND_get_remote_sources = "get-remote-sources";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.BaselineInit" /></c> setting.
    /// </summary>
    public const char SHORTHAND_n = 'n';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ForceOverwrite" /></c> setting.
    /// </summary>
    public const char SHORTHAND_f = 'f';

    /// <summary>
    /// Gets the alternate command line option for the <c><see cref="AppSettings.ForceOverwrite" /></c> setting.
    /// </summary>
    public const string SHORTHAND_force = "force";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.GlobalScope" /></c> setting.
    /// </summary>
    public const char SHORTHAND_g = 'g';
}