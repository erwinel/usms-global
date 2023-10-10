using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator;

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

            writeSwitch(AppSettings.SHORTHAND_d, "fileName",
                "The Path to the typings database.",
                "This path is relative to the subdirectory containing the executable.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.DbFile)} setting in appsettings.json, if defined; otherwise it will use a database named {Constants.DEFAULT_DbFile} in the same subdirectory as the executable.");

            writeSwitch(AppSettings.SHORTHAND_t, "name",
                "The names of the table to generate typings for.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Table)} setting in appsettings.json, if defined.");

            writeSwitch(AppSettings.SHORTHAND_u, "name",
                "The user name credentials to use when connecting to the remote instance.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.UserName)} setting in appsettings.json, if defined; otherwise, you will be prompted for the user name.");

            writeSwitch(AppSettings.SHORTHAND_p, "password",
                "The password credentials to use when connecting to the remote instance.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Password)} setting in appsettings.json, if defined; otherwise, you will be prompted for the password.");

            writeSwitch(AppSettings.SHORTHAND_i, "id",
                "Specifies client ID in the remote ServiceNow instance's Application Registry.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.ClientId)} setting in appsettings.json, if defined; otherwise, you will be prompted for the client ID.");

            writeSwitch(AppSettings.SHORTHAND_x, "secret",
                "The the client secret in the remote ServiceNow instance's Application Registry.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.ClientSecret)} setting in appsettings.json, if defined; otherwise, you will be prompted for the client secret.");

            writeSwitch(AppSettings.SHORTHAND_r, "url",
                "The base URL of the remote ServiceNow instance.",
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.RemoteURL)} setting in appsettings.json, if defined; otherwise, an error message will be displayed.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_m}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(AppSettings.MODE_SCOPED);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(AppSettings.MODE_SCOPED_ABBR);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(AppSettings.MODE_GLOBAL);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(AppSettings.MODE_GLOBAL_ABBR);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Specifies output mode.");
            Console.WriteLine($"'{AppSettings.MODE_SCOPED}' or '{AppSettings.MODE_SCOPED_ABBR}' generates typings for use in scoped apps, and '{AppSettings.MODE_SCOPED}' or '{AppSettings.MODE_GLOBAL_ABBR}' generates typings for use in global scripts.");
            Console.WriteLine("The default behavior is to generate typings for use in global scripts.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_o}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("filename");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(".d.ts");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The output file name.");
            Console.WriteLine("This path is relative to the current working directory.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Output)} setting in appsettings.json, if present; otherwise, it will write to a file named {Constants.DEFAULT_OUTPUT_FILENAME} in the current working directory.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"-{AppSettings.SHORTHAND_f}=true");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Force overwrite of the output file.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Force)} setting in appsettings.json, if set to true; otherwise, an error message will be displayed if the output file already exists.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(CmdLineConstants.DASH__3F_);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(CmdLineConstants.DASH_H);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(CmdLineConstants.DASH_HELP);
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
                            case CmdLineConstants.DASH_H:
                            case CmdLineConstants.DASH__3F_:
                            case CmdLineConstants.DASH_HELP:
                                WriteHelpToConsole();
                                return; 
                        }
                        if (!a.Contains("="))
                            index++;
                        break;
                    case '/':
                        switch (a.ToLower())
                        {
                            case CmdLineConstants.SLASH_H:
                            case CmdLineConstants.SLASH__3F_:
                            case CmdLineConstants.SLASH_HELP:
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
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(SnTsTypeGenerator)));
        AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddDbContextPool<TypingsDbContext>(options =>
            {
                var dbFile = builder.Configuration.GetSection(nameof(SnTsTypeGenerator)).Get<AppSettings>()?.DbFile;
                try
                {
                    dbFile = Path.GetFullPath(string.IsNullOrEmpty(dbFile) ? Path.Combine(builder.Environment.ContentRootPath, Constants.DEFAULT_DbFile) :
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
            .AddHostedService<MainWorkerService>()
            .AddTransient<SnClientHandlerService>()
            .AddTransient<TableAPIService>()
            .AddTransient<DataLoaderService>()
            .AddSingleton<RenderingService>();
        Host = builder.Build();
        Host.Run();
    }
}