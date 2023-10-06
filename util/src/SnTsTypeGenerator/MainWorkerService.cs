using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScope _scope;
    private readonly ImmutableArray<string> _tableNames;

    // private readonly AppSettings _appSettings;
    // private readonly TypingsDbContext _dbContext;
    // private readonly DataLoaderService _dataLoader;
    // private readonly RenderingService _renderer;

    // public MainWorkerService(ILogger<MainWorkerService> logger, IHostApplicationLifetime appLifetime, IOptions<AppSettings> appSettings)
    public MainWorkerService(ILogger<MainWorkerService> logger, IServiceProvider services, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _scope = services.CreateScope();
        AppSettings _appSettings = appSettings.Value;
        if (_appSettings.Help.HasValue && _appSettings.Help.Value)
            WriteHelpToConsole();
        else
        {
            var tableNames = _appSettings.Table?.Split(',').Where(t => !string.IsNullOrEmpty(t));
            if ((tableNames is not null && tableNames.Any()) || ((tableNames = _appSettings.Tables?.Where(t => !string.IsNullOrEmpty(t))) is not null))
            {
                _tableNames = tableNames.Select(n => n.Trim().ToLower()).Distinct().ToImmutableArray();
                return;
            }
            _logger.LogNoTableNamesSpecifiedWarning();
        }
        
        _tableNames = ImmutableArray.Create<string>();
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

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_d}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("fileName");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The Path to the typings database.");
            Console.WriteLine("This path is relative to the subdirectory containing the executable.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.DbFile)} setting in appsettings.json, if defined; otherwise it will use a database named {DEFAULT_DbFile} in the same subdirectory as the executable.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_t}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("name");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(",name,...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The names of the table to generate typings for.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Table)} setting in appsettings.json, if defined.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_u}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("login");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The user name credentials to use when connecting to the remote instance.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.UserName)} setting in appsettings.json, if defined; otherwise, you will be prompted for the user name.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_p}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("password");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The password credentials to use when connecting to the remote instance.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Password)} setting in appsettings.json, if defined; otherwise, you will be prompted for the password.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_i}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("id");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Specifies client ID in the remote ServiceNow instance's Application Registry.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.ClientId)} setting in appsettings.json, if defined; otherwise, you will be prompted for the client ID.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_x}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("secret");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The the client secret in the remote ServiceNow instance's Application Registry.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.ClientSecret)} setting in appsettings.json, if defined; otherwise, you will be prompted for the client secret.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_r}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("url");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The base URL of the remote ServiceNow instance.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.RemoteURL)} setting in appsettings.json, if defined; otherwise, an error message will be displayed.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_s}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("true");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Generate typings for use with scoped apps.");
            Console.WriteLine($"This cannot be used with the -{AppSettings.SHORTHAND_g}=true option.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Scoped)} setting in appsettings.json, if it is set to true.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_g}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("true");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Generate typings for use with scoped apps.");
            Console.WriteLine($"This cannot be used with the -{AppSettings.SHORTHAND_s}=true option.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Global)} setting in appsettings.json, if it is set to true.");
            Console.WriteLine($"This is the default behaviour if neither this option, the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Global)} setting, the -{AppSettings.SHORTHAND_s}=true option, nor the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Scoped)} is present.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_o}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("filename.d.ts");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The output file name.");
            Console.WriteLine("This path is relative to the current working directory.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Output)} setting in appsettings.json, if present; otherwise, it will write to a file named {DEFAULT_OUTPUT_FILENAME} in the current working directory.");

            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_f}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("filename.d.ts");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Force overwrite of the output file.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Force)} setting in appsettings.json, if set to true; otherwise, an error message will be displayed if the output file already exists.");

            Console.Write($"-{AppSettings.SHORTHAND__3F_}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("true");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"-{AppSettings.SHORTHAND_h}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("true");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Displays this help information.");
            Console.WriteLine("If this option is used, then all other options are ignored.");
        }
        finally { Console.ResetColor(); }
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        DataLoaderService _dataLoader = _scope.ServiceProvider.GetRequiredService<DataLoaderService>();
        RenderingService _renderer = _scope.ServiceProvider.GetRequiredService<RenderingService>();

        if (!(_dataLoader.InitSuccessful && _renderer.InitSuccessful))
            return;

        Collection<TableInfo> toRender = new();
        foreach (string name in _tableNames)
        {
            if (stoppingToken.IsCancellationRequested)
                return;
            try
            {
                var tableInfo = await _dataLoader.GetTableByNameAsync(name, stoppingToken);
                if (tableInfo is not null)
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

    public override void Dispose()
    {
        _scope.Dispose();
        base.Dispose();
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