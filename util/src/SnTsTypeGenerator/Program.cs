﻿using Microsoft.Data.Sqlite;
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
        // builder.Logging.AddConsole();
        builder.Logging.ClearProviders();
        // builder.Logging.AddConsole();
        // builder.Logging.AddDebug();
        // string path = Path.Combine(builder.Environment.ContentRootPath, DEFAULT_LogDir);
        // // if (File.Exists(path))
        //     // TODO: Log fatal
        // if (!Directory.Exists(path))
        //     Directory.CreateDirectory(path);
        Log.Logger = new LoggerConfiguration()
            // .ReadFrom.Configuration(builder.Configuration)
            // .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Logging.AddSerilog();
        // builder.Services.AddSingleton(Log.Logger);
        builder.Services.Configure<SnTsTypeGenerator.Services.AppSettings>(builder.Configuration.GetSection(nameof(SnTsTypeGenerator)));
        SnTsTypeGenerator.Services.AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddDbContextPool<SnTsTypeGenerator.Services.TypingsDbContext>(options =>
            {
                var dbFile = builder.Configuration.GetSection(nameof(SnTsTypeGenerator)).Get<SnTsTypeGenerator.Services.AppSettings>()?.DbFile;
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
            .AddHostedService<SnTsTypeGenerator.Services.MainWorkerService>()
            .AddTransient<SnTsTypeGenerator.Services.SnClientHandlerService>()
            .AddTransient<SnTsTypeGenerator.Services.TableAPIService>()
            .AddTransient<SnTsTypeGenerator.Services.DataLoaderService>()
            .AddSingleton<SnTsTypeGenerator.Services.RenderingService>();
        Host = builder.Build();
        Host.Run();
    }
}