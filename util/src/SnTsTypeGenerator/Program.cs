using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using static SnTsTypeGenerator.Services.CmdLineConstants;

internal class Program
{
    internal static IHost Host { get; private set; } = null!;

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        if (builder.Environment.IsDevelopment())
            builder.Configuration.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true);
        builder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
        builder.Services.Configure<SnTsTypeGenerator.Services.AppSettings>(builder.Configuration.GetSection(nameof(SnTsTypeGenerator)));
        SnTsTypeGenerator.Services.AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddDbContextPool<SnTsTypeGenerator.Services.TypingsDbContext>((serviceProvider, options) =>
        {
            if (builder.Environment.IsDevelopment())
                options.EnableSensitiveDataLogging(true);
            var dbFile = builder.Configuration.GetSection(nameof(SnTsTypeGenerator)).Get<SnTsTypeGenerator.Services.AppSettings>()?.DbFile;
            try
            {
                if (string.IsNullOrEmpty(dbFile))
                    try { dbFile = Path.Combine(builder.Environment.ContentRootPath, DEFAULT_DbFile); }
                    catch
                    {
                        dbFile = $"{(string.IsNullOrEmpty(builder.Environment.ContentRootPath) ? "." : builder.Environment.ContentRootPath)}/{DEFAULT_DbFile}";
                        throw;
                    }
                else
                    dbFile = Path.GetFullPath(dbFile);
            }
            catch (System.Security.SecurityException exc)
            {
                SnTsTypeGenerator.LoggerMessages.LogDbFileAccessError(serviceProvider.GetService<ILogger<Program>>(), dbFile!, exc);
                dbFile = null;
            }
            catch (UnauthorizedAccessException exc)
            {
                SnTsTypeGenerator.LoggerMessages.LogDbFileAccessError(serviceProvider.GetService<ILogger<Program>>(), dbFile!, exc);
                dbFile = null;
            }
            catch (NotSupportedException exc)
            {
                SnTsTypeGenerator.LoggerMessages.LogDbfilePathInvalid(serviceProvider.GetService<ILogger<Program>>(), dbFile!, exc);
                dbFile = null;
            }
            catch (PathTooLongException exc)
            {
                SnTsTypeGenerator.LoggerMessages.LogDbfilePathTooLong(serviceProvider.GetService<ILogger<Program>>(), dbFile!, exc);
                dbFile = null;
            }
            //codeql[cs/catch-of-all-exceptions] Won't fix.
            catch (Exception exc)
            {
                SnTsTypeGenerator.LoggerMessages.LogDbfileValidationError(serviceProvider.GetService<ILogger<Program>>(), dbFile!, exc);
                dbFile = null;
            }
            options.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = dbFile,
                ForeignKeys = true,
                Mode = SqliteOpenMode.ReadWrite
            }.ConnectionString);
        })
            .AddHostedService<SnTsTypeGenerator.Services.MainWorkerService>()
            .AddTransient<SnTsTypeGenerator.Services.SnClientHandlerService>()
            .AddTransient<SnTsTypeGenerator.Services.GlideTypesService>()
            .AddTransient<SnTsTypeGenerator.Services.TableAPIService>()
            .AddTransient<SnTsTypeGenerator.Services.DataLoaderService>()
            .AddSingleton<SnTsTypeGenerator.Services.RemoteUriService>()
            .AddSingleton<SnTsTypeGenerator.Services.RenderingService>();
        Host = builder.Build();
        Host.Run();
    }
}