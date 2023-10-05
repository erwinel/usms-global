using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly TypingsDbContext _dbContext;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly DataLoaderService _dataLoader;
    private readonly RenderingService _renderer;

    // public MainWorkerService(ILogger<MainWorkerService> logger, IHostApplicationLifetime appLifetime, IOptions<AppSettings> appSettings)
    public MainWorkerService(ILogger<MainWorkerService> logger, TypingsDbContext dbContext, IOptions<AppSettings> appSettings, DataLoaderService dataLoader, RenderingService renderer)
    {
        _logger = logger;
        _dbContext = dbContext;
        _appSettings = appSettings;
        _dataLoader = dataLoader;
        _renderer = renderer;
        // appLifetime.ApplicationStarted.Register(OnStarted);
        // appLifetime.ApplicationStopping.Register(OnStopping);
        // appLifetime.ApplicationStopped.Register(OnStopped);
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

            Console.Write($"\t-{AppSettings.SHORTHAND_d}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" fileName");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.DbFile)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("fileName");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the Path to the typings database.");
            Console.WriteLine("\t\t\tThis path is relative to the subdirectory containing the executable.");
            Console.WriteLine("\t\t\tThe default behavior is use a database named {DEFAULT_DbFile} in the same subdirectory as the executable.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_t}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(" name");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(",name,...");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.Table)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("name");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(",name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the names of the table to generate typings for.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_u}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" login");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.UserName)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("login");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the user name credentials to use when connecting to the remote instance.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_p}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" password");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.Password)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("password");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the password credentials to use when connecting to the remote instance.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_r}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" url");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.RemoteURL)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("url");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t\t\tSpecifies the base URL of the remote ServiceNow instance.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_s}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.Scoped)}");
            Console.WriteLine("\t\t\tGenerate typings for use with scoped apps.");
            Console.WriteLine($"\t\t\tCannot be used with the --{nameof(AppSettings.Global)} (-{AppSettings.SHORTHAND_g}) option.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_g}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.Global)}");
            Console.WriteLine("\t\t\tGenerate typings for use with global apps (this is the default behavior).");
            Console.WriteLine($"\t\t\tCannot be used with the --{nameof(AppSettings.Scoped)} (-{AppSettings.SHORTHAND_s}) option.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_o}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" filename.d.ts");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.Output)}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("filename.d.ts");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t\t\tSpecifies the output file name. The default is {DEFAULT_OUTPUT_FILENAME}.");
            Console.WriteLine();

            Console.Write($"\t-{AppSettings.SHORTHAND_f}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"\t--{nameof(AppSettings.Force)}");
            Console.WriteLine("\t\t\tForce overwrite of the output file.");
            Console.WriteLine();

            Console.WriteLine($"\t{AppSettings.SHORTHAND__3F_}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t{AppSettings.SHORTHAND_h}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\t--{nameof(AppSettings.Help)}");
            Console.WriteLine();
            Console.WriteLine("\t\t\tDisplays this help information.");
            Console.WriteLine("\t\t\tIf this option is used, then all other options are ignored.");
        }
        finally { Console.ResetColor(); }
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        AppSettings settings = _appSettings.Value;
        if (settings.Help.HasValue && settings.Help.Value)
        {
            WriteHelpToConsole();
            return;
        }

        if (!(_dataLoader.InitSuccessful && _renderer.InitSuccessful))
            return;

        var tableNames = settings.Table?.Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>();
        if (tableNames.Any() || (tableNames = settings.DbFile?.Split(',').Where(t => !string.IsNullOrEmpty(t)) ?? Enumerable.Empty<string>()).Any())
        {
            Collection<TableInfo> toRender = new();
            foreach (string name in tableNames.Select(n => n.Trim().ToLower()).Distinct())
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                try
                {
                    var tableInfo = await _dbContext.Tables.Include(t => t.SuperClass).Include(t => t.Scope).FirstOrDefaultAsync(t => t.Name == name, stoppingToken);
                    if (tableInfo is not null || (tableInfo = await _dataLoader.GetTableByNameAsync(name, stoppingToken)) is not null)
                        toRender.Add(tableInfo);
                }
                catch (Exception exception)
                {
                    if (stoppingToken.IsCancellationRequested)
                        return;
                    if (exception is ILogTrackable logTrackable)
                    {
                        if (!logTrackable.IsLogged)
                            logTrackable.Log(_logger);
                    }
                    else
                        _logger.LogUnexpecteException(exception);
                    return;
                }
            }
            if (!stoppingToken.IsCancellationRequested)
                await _renderer.RenderAsync(toRender, stoppingToken);
        }
        else
            _logger.LogNoTableNamesSpecifiedWarning();
    }

    // public Task StopAsync(CancellationToken cancellationToken)
    // {
    //     _logger.LogInformation("StopAsync has been called.");

    //     return Task.CompletedTask;
    // }

    // private void OnStarted()
    // {
    //     _logger.LogInformation("2. OnStarted has been called.");
    // }

    // private void OnStopping()
    // {
    //     _logger.LogInformation("3. OnStopping has been called.");
    // }

    // private void OnStopped()
    // {
    //     _logger.LogInformation("5. OnStopped has been called.");
    // }
}