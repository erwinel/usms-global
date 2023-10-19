using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public sealed class MainWorkerService : BackgroundService
{
    private const string STAGE_NAME_LOAD = "Load";
    private const string STAGE_NAME_RENDER = "Render";
    private readonly ILogger _logger;
    private readonly IServiceScope _scope;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly bool _includeReferenced;
    private readonly ImmutableArray<string> _tableNames;
    private readonly bool _showHelp;

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
            Console.WriteLine("Options specified as command line switches will override those specified in the [Application settings](./appsettings.json) file.");
            Console.WriteLine("Command line options:");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{SHORTHAND_t}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("name");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(",name,name...");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This specifies the names of the tables to render typings for.");
            Console.WriteLine("Multiple table names are separated by commas with no spaces.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.Write($"-{SHORTHAND_m}=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("scoped");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("s");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("global");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("g");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Determines whether the typings file is generated for use with scoped applications or for globally-scoped scripting.");
            Console.WriteLine("Valid values are \"scoped\" or \"s\" for targeting scoped applications, and \"global\", or \"g\" for global-scope mode. If this is not specified, the default behavior is to generate typings for scoped applications.");

            static void writeValueSwitch(char switchChar, string value, string desription, params string[] additionalDesc)
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

            writeValueSwitch(SHORTHAND_r, "url",
                "The base URL of the source ServiceNow instance, which should not include any path, query or fragment.",
                "If this is not specified, you will be prompted for the URL.");

            writeValueSwitch(SHORTHAND_u, "user_name",
                "The account name to use as account credentials on the source ServiceNow instance.",
                "If this is not specified, you will be prompted for the user name.");

            writeValueSwitch(SHORTHAND_p, "*password*",
                "The password to use as account credentials on on the source ServiceNow instance.",
                "If this is not specified, you will be prompted for the password");

            writeValueSwitch(SHORTHAND_x, "*secret*",
                "The Client Secrent from the Application Registry entry in the target ServiceNow instance.",
                "If this is not specified, and the Client ID is specified in the application settings, you will be prompted for the client secret");

            writeValueSwitch(SHORTHAND_o, "filename",
                "The file output path, relative to the current working directory. This defaults to a file named \"types.d.ts\".",
                "If the file extension is omitted, it will have the default extension of \".d.ts\".");

            static void writeBooleanSwitch(char switchChar, string desription, params string[] additionalDesc)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine($"-{switchChar}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(desription);
                if (additionalDesc is not null)
                    foreach (string d in additionalDesc)
                        Console.WriteLine(d);
            }

            writeBooleanSwitch(SHORTHAND_f, "Overwrites the existing output file if it already exist.",
                "If this option is not present, and the output file already exists, this will return wiht an error.");

            writeValueSwitch(SHORTHAND_d, "filename",
                "Specifies the database file, relative to the current working directory. This defaults to a file named \"Typings.db\" in the same subdirectory as the application executable.");

            writeBooleanSwitch(SHORTHAND_b, "Includes the \"$$GlideElement.Reference<TFields, TRecord>\" type definition and the \"$$tableFields.IBaseRecord\" interface in the output.");

            writeBooleanSwitch(SHORTHAND_i, "Includes the parent type definitions and type definitions referenced by elements, which are not explicitly included in the -{SHORTHAND_t} command line argument or {nameof(SnTsTypeGenerator)}:{nameof(AppSettings.Tables)} setting.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(SHORTHAND__3F_);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  or");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(SHORTHAND_h);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  or");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"--{nameof(AppSettings.Help)}");
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

            if (_showHelp)
            {
                WriteHelpToConsole();
                return;
            }

            DataLoaderService _dataLoader = _scope.ServiceProvider.GetRequiredService<DataLoaderService>();
            RenderingService _renderer = _scope.ServiceProvider.GetRequiredService<RenderingService>();

            if (!(_dataLoader.InitSuccessful && _renderer.InitSuccessful))
                return;
            IEnumerable<Table> toRender;

            using (var scope = _logger.BeginScope(STAGE_NAME_LOAD))
            {
                Collection<Table> tables = new();
                foreach (string name in _tableNames)
                {
                    if (stoppingToken.IsCancellationRequested)
                        return;
                    try
                    {
                        var tableInfo = await _dataLoader.GetTableByNameAsync(name, stoppingToken);
                        if (tableInfo is not null)
                            tables.Add(tableInfo);
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
                toRender = (stoppingToken.IsCancellationRequested || !_includeReferenced) ? tables : await _dataLoader.LoadAllReferencedAsync(tables, stoppingToken);
            }
            if (!stoppingToken.IsCancellationRequested)
            {
                using var scope2 = _logger.BeginScope(STAGE_NAME_RENDER);
                await _renderer.RenderAsync(toRender, stoppingToken);
            }
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

    public MainWorkerService(ILogger<MainWorkerService> logger, IServiceProvider services, IOptions<AppSettings> settingsOption, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _scope = services.CreateScope();
        _applicationLifetime = applicationLifetime;
        AppSettings appSettings = settingsOption.Value;
        _includeReferenced = appSettings.IncludeReferenced ?? false;
        _showHelp = appSettings.ShowHelp();
        var tableNames = appSettings.Table?.Split(',').Where(t => !string.IsNullOrEmpty(t));
        if ((tableNames is not null && tableNames.Any()) || ((tableNames = appSettings.Tables?.Where(t => !string.IsNullOrEmpty(t))) is not null))
        {
            _tableNames = tableNames.Select(n => n.Trim().ToLower()).Distinct().ToImmutableArray();
            return;
        }
        if (!_showHelp)
            _logger.LogNoTableNamesSpecified();
        _tableNames = ImmutableArray.Create<string>();
    }
}