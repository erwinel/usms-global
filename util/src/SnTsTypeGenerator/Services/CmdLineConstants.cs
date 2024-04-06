namespace SnTsTypeGenerator.Services;

public static class CmdLineConstants
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"Typings.db";

    internal const string DEFAULT_OUTPUT_FILENAME = "types.d.ts";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.RemoteURL" /></c> setting.
    /// </summary>
    public const char SHORTHAND_r = 'r';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientSecret" /></c> setting.
    /// </summary>
    public const string SHORTHAND_remote_url = "remote-url";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Table" /></c> setting.
    /// </summary>
    public const char SHORTHAND_t = 't';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.GlobalScope" /></c> setting.
    /// </summary>
    public const char SHORTHAND_g = 'g';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Dbfile" /></c> setting.
    /// </summary>
    public const char SHORTHAND_d = 'd';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Dbfile" /></c> setting.
    /// </summary>
    public const string SHORTHAND_db_file = "db-file";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.EmitBaseTypes" /></c> setting.
    /// </summary>
    public const char SHORTHAND_b = 'b';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.EmitBaseTypes" /></c> setting.
    /// </summary>
    public const string SHORTHAND_emit_base_types = "emit-base-types";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.IncludeReferenced" /></c> setting.
    /// </summary>
    public const char SHORTHAND_i = 'i';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.IncludeReferenced" /></c> setting.
    /// </summary>
    public const string SHORTHAND_include_referenced = "include-referenced";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.DefaultPackageGroup" /></c> setting.
    /// </summary>
    public const string SHORTHAND_default_pkg = "default-pkg";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.BaselineInit" /></c> setting.
    /// </summary>
    public const char SHORTHAND_n = 'n';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.BaselineInit" /></c> setting.
    /// </summary>
    public const string SHORTHAND_baseline_init = "baseline-init";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.UserName" /></c> setting.
    /// </summary>
    public const char SHORTHAND_u = 'u';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.UserName" /></c> setting.
    /// </summary>
    public const string SHORTHAND_user_name = "user-name";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.UserName" /></c> setting.
    /// </summary>
    public const string SHORTHAND_login = "login";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Password" /></c> setting.
    /// </summary>
    public const char SHORTHAND_p = 'p';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientId" /></c> setting.
    /// </summary>
    public const char SHORTHAND_c = 'c';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientId" /></c> setting.
    /// </summary>
    public const string SHORTHAND_client_id = "client-id";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientSecret" /></c> setting.
    /// </summary>
    public const char SHORTHAND_x = 'x';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ClientSecret" /></c> setting.
    /// </summary>
    public const string SHORTHAND_client_secret = "client-secret";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.Output" /></c> setting.
    /// </summary>
    public const char SHORTHAND_o = 'o';

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ForceOverwrite" /></c> setting.
    /// </summary>
    public const char SHORTHAND_f = 'f';

    /// <summary>
    /// Gets the alternate command line option for the <c><see cref="AppSettings.ForceOverwrite" /></c> setting.
    /// </summary>
    public const string SHORTHAND_force = "force";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.ModifySource" /></c> setting.
    /// </summary>
    public const char SHORTHAND_m = 'm';

    /// <summary>
    /// Gets the alternate command line option for the <c><see cref="AppSettings.ModifySource" /></c> setting.
    /// </summary>
    public const string SHORTHAND_modify_source = "modify-source";

    /// <summary>
    /// Gets the alternate command line option for the <c><see cref="AppSettings.ExistingURL" /></c> setting.
    /// </summary>
    public const string SHORTHAND_existing_url = "existing-url";

    /// <summary>
    /// Gets the alternate command line option for the <c><see cref="AppSettings.IsPdi" /></c> setting.
    /// </summary>
    public const string SHORTHAND_pdi = "pdi";

    /// <summary>
    /// Gets the alternate command line option for the <c><see cref="AppSettings.RemoteLabel" /></c> setting.
    /// </summary>
    public const string SHORTHAND_remote_label = "remote-label";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.GetPackageGroups" /></c> setting.
    /// </summary>
    public const string SHORTHAND_get_package_groups = "get-package-groups";

    /// <summary>
    /// Gets the command line option for the <c><see cref="AppSettings.GetRemoteSources" /></c> setting.
    /// </summary>
    public const string SHORTHAND_get_remote_sources = "get-remote-sources";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SHORTHAND_h = "h";

    /// <summary>
    /// Command line switch for showing help.
    /// </summary>
    internal const string SHORTHAND__3F_ = "?";

}