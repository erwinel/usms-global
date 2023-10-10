using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.CmdLineConstants;

internal class Program
{

    internal static IHost Host { get; private set; } = null!;

    internal static void WriteHelpToConsole()
    {
        string exe = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;
        Console.BackgroundColor = ConsoleColor.Black;
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(exe);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Generate typings file.");
            Console.WriteLine();
            Console.WriteLine("Command line options:");

            static void writeSwitch(char switchChar, string value, string desription, params string[] additionalDesc)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.Write($"-{switchChar}=");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(value);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(desription);
                if (additionalDesc is not null)
                    foreach (string d in additionalDesc)
                        Console.WriteLine(d);
            }

            writeSwitch(SHORTHAND_d, "fileName",
                "The Path to the typings database.",
                "This path is relative to the subdirectory containing the executable.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.DbFile)} setting in appsettings.json, if defined; otherwise it will use a database named {DEFAULT_DbFile} in the same subdirectory as the executable.");

            writeSwitch(SHORTHAND_t, "name",
                "The names of the table to generate typings for.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.Table)} setting in appsettings.json, if defined.");

            writeSwitch(SHORTHAND_u, "name",
                "The user name credentials to use when connecting to the remote instance.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.UserName)} setting in appsettings.json, if defined; otherwise, you will be prompted for the user name.");

            writeSwitch(SHORTHAND_p, "password",
                "The password credentials to use when connecting to the remote instance.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.Password)} setting in appsettings.json, if defined; otherwise, you will be prompted for the password.");

            writeSwitch(SHORTHAND_i, "id",
                "Specifies client ID in the remote ServiceNow instance's Application Registry.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.ClientId)} setting in appsettings.json, if defined; otherwise, you will be prompted for the client ID.");

            writeSwitch(SHORTHAND_x, "secret",
                "The the client secret in the remote ServiceNow instance's Application Registry.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.ClientSecret)} setting in appsettings.json, if defined; otherwise, you will be prompted for the client secret.");

            writeSwitch(SHORTHAND_r, "url",
                "The base URL of the remote ServiceNow instance.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.RemoteURL)} setting in appsettings.json, if defined; otherwise, an error message will be displayed.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{SHORTHAND_m}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(MODE_SCOPED);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(MODE_SCOPED_ABBR);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(MODE_GLOBAL);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(MODE_GLOBAL_ABBR);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Specifies output mode.");
            Console.WriteLine($"'{MODE_SCOPED}' or '{MODE_SCOPED_ABBR}' generates typings for use in scoped apps, and '{MODE_SCOPED}' or '{MODE_GLOBAL_ABBR}' generates typings for use in global scripts.");
            Console.WriteLine("The default behavior is to generate typings for use in global scripts.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{SHORTHAND_o}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("filename");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(".d.ts");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The output file name.");
            Console.WriteLine("This path is relative to the current working directory.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.Output)} setting in appsettings.json, if present; otherwise, it will write to a file named {DEFAULT_OUTPUT_FILENAME} in the current working directory.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"-{SHORTHAND_f}=true");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Force overwrite of the output file.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(SnTsTypeGenerator.AppSettings.Force)} setting in appsettings.json, if set to true; otherwise, an error message will be displayed if the output file already exists.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(SHORTHAND__3F_);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(SHORTHAND_h);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(DASH_help);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Displays this help information.");
            Console.WriteLine("If this option is used, then all other options are ignored.");
        }
        finally { Console.ResetColor(); }
    }

    private static void Main(string[] args)
    {
        for (int index = 0; index < args.Length; index++)
        {
            var a = args[index];
            if (a.Length > 0)
                switch (a[0])
                {
                    case '-':
                        switch (a.ToLower())
                        {
                            case SHORTHAND_h:
                            case SHORTHAND__3F_:
                            case DASH_help:
                                WriteHelpToConsole();
                                return; 
                        }
                        if (!a.Contains("="))
                            index++;
                        break;
                    case '/':
                        switch (a.ToLower())
                        {
                            case SLASH_h:
                            case SLASH__3F_:
                            case SLASH_help:
                                WriteHelpToConsole();
                                return; 
                        }
                        if (!a.Contains("="))
                            index++;
                        break;
                }
        }
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        if (builder.Environment.IsDevelopment())
            builder.Configuration.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true);
        builder.Logging.AddConsole();
        builder.Services.Configure<SnTsTypeGenerator.AppSettings>(builder.Configuration.GetSection(nameof(SnTsTypeGenerator)));
        SnTsTypeGenerator.AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddDbContextPool<SnTsTypeGenerator.TypingsDbContext>(options =>
            {
                var dbFile = builder.Configuration.GetSection(nameof(SnTsTypeGenerator)).Get<SnTsTypeGenerator.AppSettings>()?.DbFile;
                try
                {
                    dbFile = Path.GetFullPath(string.IsNullOrEmpty(dbFile) ? Path.Combine(builder.Environment.ContentRootPath, DEFAULT_DbFile) :
                        Path.IsPathFullyQualified(dbFile) || Path.IsPathRooted(dbFile) ? dbFile : Path.Combine(builder.Environment.ContentRootPath, dbFile));
                }
                catch { }
                options.UseSqlite(new SqliteConnectionStringBuilder
                {
                    DataSource = dbFile,
                    ForeignKeys = true,
                    Mode = SqliteOpenMode.ReadWrite
                }.ConnectionString);
            })
            .AddHostedService<SnTsTypeGenerator.MainWorkerService>()
            .AddTransient<SnTsTypeGenerator.SnClientHandlerService>()
            .AddTransient<SnTsTypeGenerator.TableAPIService>()
            .AddTransient<SnTsTypeGenerator.DataLoaderService>()
            .AddSingleton<SnTsTypeGenerator.RenderingService>();
        Host = builder.Build();
        Host.Run();
    }
}