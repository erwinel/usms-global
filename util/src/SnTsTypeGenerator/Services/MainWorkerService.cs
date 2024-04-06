using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnTsTypeGenerator.Models;
using SQLitePCL;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public sealed class MainWorkerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScope _scope;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly AppSettings _appSettings;

    // private readonly bool _includeReferenced;
    // private readonly ImmutableArray<string> _tableNames;
    // private readonly bool _showHelp;
    // private readonly MainWorkerOperation _operation;

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

            // TODO: Document SHORTHAND_g

            writeBooleanSwitch(SHORTHAND_g, "Emits typings for globally-scoped scripting, versus typings for scoped applications.");

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

            writeBooleanSwitch(SHORTHAND_f, "Overwrites the existing output file if it already exist.",
                "If this option is not present, and the output file already exists, this will return with an error.");

            writeBooleanSwitch(SHORTHAND_n, "All newly found packages that are active and not licensable on the remote instance are to be considered baseline.");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine($"=-{SHORTHAND_get_package_groups}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Display package groups from database and app settings.");

            // TODO: Document SHORTHAND_get_remote_sources

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

    private async Task<IEnumerable<Table>> GetTablesToRender(IEnumerable<string> tableNames, DataLoaderService dataLoader, CancellationToken stoppingToken)
    {
        Collection<Table> tables = new();
        foreach (string name in tableNames.Select(n => n.Trim().ToLower()).Distinct())
        {
            if (stoppingToken.IsCancellationRequested)
                return tables;
            var tableInfo = await _logger.WithActivityScopeAsync(LogActivityType.GetTableFromArg, name, () => dataLoader.GetTableByNameAsync(name, stoppingToken));
            if (tableInfo is not null)
                tables.Add(tableInfo);
        }
        return (stoppingToken.IsCancellationRequested || !(_appSettings.IncludeReferenced ?? false)) ? tables : await dataLoader.LoadAllReferencedAsync(tables, stoppingToken);
    }

    private async Task GetRemoteSourcesAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        await TypingsDbContext.InitializeAsync(_scope, stoppingToken);
        using TypingsDbContext dbContext = _scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
        if (!dbContext.InitSuccessful) return;
        var totalCount = 0;
        await foreach (var sncSource in dbContext.Sources.OrderBy(s => s.Label).AsAsyncEnumerable())
        {
            var entry = dbContext.Sources.Entry(sncSource);
            Console.Write("https://");
            Console.WriteLine(sncSource.FQDN);
            Console.Write("    Label: ");
            Console.WriteLine(sncSource.Label.AsNonEmpty(sncSource.FQDN));
            Console.Write("    Is PDI: ");
            Console.WriteLine(sncSource.IsPersonalDev ? "Yes" : "No");
            Console.Write("    Last Accessed:");
            Console.WriteLine(sncSource.LastAccessed.ToLocalTime().ToString("yyyy-mm-dd HH:mm:ss"));
            Console.WriteLine("    Packages: {0}", (await entry.GetRelatedCollectionAsync(s => s.Packages, stoppingToken)).Count());
            Console.WriteLine("    Scopes: {0}", (await entry.GetRelatedCollectionAsync(s => s.Scopes, stoppingToken)).Count());
            Console.WriteLine("    Tables: {0}", (await entry.GetRelatedCollectionAsync(s => s.Tables, stoppingToken)).Count());
            Console.WriteLine("    Types: {0}", (await entry.GetRelatedCollectionAsync(s => s.Types, stoppingToken)).Count());
            Console.WriteLine("    Columns: {0}", (await entry.GetRelatedCollectionAsync(s => s.Elements, stoppingToken)).Count());
            totalCount++;
        }
        switch (totalCount)
        {
            case 0:
                Console.WriteLine("No remote sources.");
                break;
            case 1:
                Console.WriteLine();
                Console.WriteLine("1 remote source.");
                break;
            default:
                Console.WriteLine();
                Console.WriteLine("{0} remote sources.", totalCount);
                break;
        }
    }

    private async Task GetPackageGroupsAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        await TypingsDbContext.InitializeAsync(_scope, stoppingToken);
        using TypingsDbContext dbContext = _scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
        if (!dbContext.InitSuccessful) return;
        var totalCount = 0;
        await foreach (var packageGroup in dbContext.PackageGroups.OrderBy(s => s.Name).AsAsyncEnumerable())
        {
            var entry = dbContext.PackageGroups.Entry(packageGroup);
            Console.WriteLine(packageGroup.Name);
            Console.Write("    Is Baseline: ");
            Console.WriteLine(packageGroup.IsBaseline ? "Yes" : "No");
            Console.Write("    Last Updated:");
            Console.WriteLine(packageGroup.LastUpdated.ToLocalTime().ToString("yyyy-mm-dd HH:mm:ss"));
            Console.WriteLine("    Packages: {0}", (await entry.GetRelatedCollectionAsync(s => s.Packages, stoppingToken)).Count());
            totalCount++;
        }
        switch (totalCount)
        {
            case 0:
                Console.WriteLine("No package groups.");
                break;
            case 1:
                Console.WriteLine();
                Console.WriteLine("1 package group.");
                break;
            default:
                Console.WriteLine();
                Console.WriteLine("{0} package groups.", totalCount);
                break;
        }
    }

    private async Task ModifySourceAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        await TypingsDbContext.InitializeAsync(_scope, stoppingToken);
        using TypingsDbContext dbContext = _scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
        if (!dbContext.InitSuccessful) return;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            if (_appSettings.ShowHelp())
                WriteHelpToConsole();
            else if (_appSettings.GetRemoteSources ?? false)
                await GetRemoteSourcesAsync(stoppingToken);
            else if (_appSettings.GetPackageGroups ?? false)
                await GetPackageGroupsAsync(stoppingToken);
            else if (_appSettings.ModifySource ?? false)
                await ModifySourceAsync(stoppingToken);
            else
            {
                var tableNames = _appSettings.Table?.Split(',').Where(t => !string.IsNullOrEmpty(t));
                if ((tableNames is not null && tableNames.Any()) || ((tableNames = _appSettings.Tables?.Where(t => !string.IsNullOrEmpty(t))) is not null))
                {
                    await TypingsDbContext.InitializeAsync(_scope, stoppingToken);
                    DataLoaderService dataLoader = _scope.ServiceProvider.GetRequiredService<DataLoaderService>();
                    RenderingService renderer = _scope.ServiceProvider.GetRequiredService<RenderingService>();

                    if (!(dataLoader.InitSuccessful && renderer.InitSuccessful))
                        return;
                    IEnumerable<Table> toRender = await _logger.WithActivityScopeAsync(LogActivityType.LoadTables, () => GetTablesToRender(tableNames, dataLoader, stoppingToken));
                    if (!stoppingToken.IsCancellationRequested)
                        await _logger.WithActivityScopeAsync(LogActivityType.RenderTypes, () => renderer.RenderAsync(toRender, stoppingToken));
                }
                else
                    _logger.LogNoTableNamesSpecified();
            }
            // if (_tableNames.Length == 0)
            //     return;

            // await TypingsDbContext.InitializeAsync(_scope, stoppingToken);
            // DataLoaderService dataLoader = _scope.ServiceProvider.GetRequiredService<DataLoaderService>();
            // RenderingService renderer = _scope.ServiceProvider.GetRequiredService<RenderingService>();

            // if (!(dataLoader.InitSuccessful && renderer.InitSuccessful))
            //     return;
            // IEnumerable<Table> toRender = await _logger.WithActivityScopeAsync(LogActivityType.LoadTables, () => GetTablesToRender(dataLoader, stoppingToken));
            // if (!stoppingToken.IsCancellationRequested)
            //     await _logger.WithActivityScopeAsync(LogActivityType.RenderTypes, () => renderer.RenderAsync(toRender, stoppingToken));
        }
        catch (OperationCanceledException) { throw; }
        //codeql[cs/catch-of-all-exceptions]
        catch (Exception error)
        {
            if (!stoppingToken.IsCancellationRequested && _logger.IsNotLogged(error))
                _logger.LogUnexpectedException(error);
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
        _appSettings = settingsOption.Value;
    }
}
