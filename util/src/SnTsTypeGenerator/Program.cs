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

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        var env = builder.Environment;
        if (env.IsDevelopment())
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
            .AddTransient<RenderingService>();
        Host = builder.Build();
        Host.Run();
    }
}