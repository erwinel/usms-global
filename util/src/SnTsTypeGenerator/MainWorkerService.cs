using System.Collections.Immutable;
using System.Collections.ObjectModel;
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
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ImmutableArray<string> _tableNames;

    public MainWorkerService(ILogger<MainWorkerService> logger, IServiceProvider services, IOptions<AppSettings> appSettings, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _scope = services.CreateScope();
        _applicationLifetime = applicationLifetime;
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
            Console.WriteLine(exe);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Generate typings file.");
            Console.WriteLine();
            Console.WriteLine("Command line options:");

            void writeSwitch(char switchChar, string value, string desription, params string[] additionalDesc)
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
                $"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.DbFile)} setting in appsettings.json, if defined; otherwise it will use a database named {DEFAULT_DbFile} in the same subdirectory as the executable.");

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
            Console.WriteLine("Specifies output mode.",
                $"'{AppSettings.MODE_SCOPED}' or '{AppSettings.MODE_SCOPED_ABBR}' generates typings for use in scoped apps, and '{AppSettings.MODE_SCOPED}' or '{AppSettings.MODE_GLOBAL_ABBR}' generates typings for use in global scripts.",
                "The default behavior is to generate typings for use in global scripts.");

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
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Output)} setting in appsettings.json, if present; otherwise, it will write to a file named {DEFAULT_OUTPUT_FILENAME} in the current working directory.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{AppSettings.SHORTHAND_f}=true");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Force overwrite of the output file.");
            Console.WriteLine($"If this option is not present, then this will use the {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Force)} setting in appsettings.json, if set to true; otherwise, an error message will be displayed if the output file already exists.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"-{AppSettings.SHORTHAND__3F_}=true");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tor");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"-{AppSettings.SHORTHAND_h}=true");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Displays this help information.");
            Console.WriteLine("If this option is used, then all other options are ignored.");
        }
        finally { Console.ResetColor(); }
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (_tableNames.Length == 0 || stoppingToken.IsCancellationRequested)
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
        catch (OperationCanceledException) { throw; }
        catch (Exception error)
        {
            _logger.LogUnexpectedServiceException<MainWorkerService>(error);
        }
        finally
        {
            if (!stoppingToken.IsCancellationRequested)
                _applicationLifetime.StopApplication();
        }
    }
}