
using Microsoft.Extensions.Configuration;

namespace SnTsTypeGenerator;

public class AppSettings
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"Typings.db";

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

    public const string DEFAULT_NAMESPACE = "global";

    public const string DEFAULT_OUTPUT_FILENAME = "types.d.ts";

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

    internal static void WriteHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Generate typings file.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Command line options:");

            Console.Write($"\t-{SHORTHAND_d}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" fileName");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(DbFile)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("fileName");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the Path to the typings database.");
            Console.WriteLine("\t\t\tThis path is relative to the subdirectory containing the executable.");
            Console.WriteLine("\t\t\tThe default behavior is use a database named {DEFAULT_DbFile} in the same subdirectory as the executable.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_t}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(" name");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(",name,...");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Table)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("name");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(",name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the names of the table to generate typings for.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_u}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" login");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(UserName)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("login");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the user name credentials to use when connecting to the remote instance.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_p}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" password");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Password)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("password");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the password credentials to use when connecting to the remote instance.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_r}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" url");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(RemoteURL)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("url");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the base URL of the remote ServiceNow instance.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_s}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Scoped)}");
            Console.WriteLine("\t\t\tGenerate typings for use with scoped apps.");
            Console.WriteLine($"\t\t\tCannot be used with the --{nameof(Global)} (-{SHORTHAND_g}) option.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_g}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Global)}");
            Console.WriteLine("\t\t\tGenerate typings for use with global apps (this is the default behavior).");
            Console.WriteLine($"\t\t\tCannot be used with the --{nameof(Scoped)} (-{SHORTHAND_s}) option.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_o}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" filename.d.ts");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Output)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("filename.d.ts");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t\t\tSpecifies the output file name. The default is {DEFAULT_OUTPUT_FILENAME}.");
            Console.WriteLine();

            Console.Write($"\t-{SHORTHAND_f}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Force)}");
            Console.WriteLine("\t\t\tForce overwrite of the output file.");
            Console.WriteLine();

            Console.WriteLine($"\t{SHORTHAND__3F_}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t{SHORTHAND_h}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t--{nameof(Help)}");
            Console.WriteLine();
            Console.WriteLine("\t\t\tDisplays this help information.");
            Console.WriteLine("\t\t\tIf this option is used, then all other options are ignored.");
        }
        finally { Console.ResetColor(); }
    }
}
