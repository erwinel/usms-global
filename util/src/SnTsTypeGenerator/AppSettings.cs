
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
    public const string SHORTHAND_DbFile = "";

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Table" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_Table = "";

    /// <summary>
    /// Database table names to generate typings for.
    /// </summary>
    public List<string>? Table { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="UserName" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_UserName = "";

    /// <summary>
    /// Login user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Password" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_Password = "";

    /// <summary>
    /// Password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="ClientId" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_ClientId = "-i";

    /// <summary>
    /// Gets or sets the client ID from the ServiceNow Application Registry.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="ClientSecret" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_ClientSecret = "-i";

    /// <summary>
    /// Gets or sets the client secret from the ServiceNow Application Registry.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="RemoteURL" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_RemoteURL = "";

    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURL { get; set; }

    /// <summary>
    /// Gets the shorthand for the <c>--<see cref="Help" /></c> command line option.
    /// </summary>
    public const string SHORTHAND_Help1 = "-h";

    /// <summary>
    /// Gets the command line switch for the <see cref="Help" /> application option option.
    /// </summary>
    public const string SHORTHAND_Help2 = "-?";
    
    /// <summary>
    /// Gets or sets the value indicating whether to write help information to the console.
    /// </summary>
    /// <remarks>If this option is used, then all other options are ignored.</remarks>
    public bool? Help { get; set; }

    private static readonly Dictionary<string, string> _switchMappings = new()
    {
        { SHORTHAND_DbFile, nameof(DbFile) },
        { SHORTHAND_Table, nameof(Table) },
        { SHORTHAND_UserName, nameof(UserName) },
        { SHORTHAND_Password, nameof(Password) },
        { SHORTHAND_ClientId, nameof(ClientId) },
        { SHORTHAND_ClientSecret, nameof(ClientSecret) },
        { SHORTHAND_RemoteURL, nameof(RemoteURL) },
        { SHORTHAND_Help1, nameof(Help) },
        { SHORTHAND_Help2, nameof(Help) }
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
            Console.Write($"\t{SHORTHAND_DbFile}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [fileName]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(DbFile)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[fileName]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the Path to the typings database.");
            Console.WriteLine("\t\t\tThis path is relative to the subdirectory containing the executable.");
            Console.WriteLine("\t\t\tThe default behavior is use a database named {DEFAULT_DbFile} in the same subdirectory as the executable.");
            Console.WriteLine();
            Console.Write($"\t{SHORTHAND_Table} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [comma_separated_table_names]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(Table)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[comma_separated_table_names]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the names of the table to generate typings for.");
            Console.WriteLine();
            Console.Write($"\t{SHORTHAND_UserName} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [user_name]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(UserName)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[user_name]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the user name credentials to use when connedting to the remote instance.");
            Console.WriteLine();
            Console.WriteLine($"\t{SHORTHAND_Password}");
            Console.WriteLine("\t\tor");
            Console.WriteLine($"\t--{nameof(Password)}");
            Console.WriteLine("\t\t\tSpecifies the password credentials to use when connedting to the remote instance.");
            Console.WriteLine();
            Console.Write($"\t{SHORTHAND_RemoteURL} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [url]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(RemoteURL)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[url]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the base URL of the remote ServiceNow instance.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t{SHORTHAND_Help2}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t{SHORTHAND_Help1}");
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
