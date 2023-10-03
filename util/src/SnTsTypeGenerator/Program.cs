using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator;

internal class Program
{
    private static IHost _host = null!;
    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        AppSettings.Configure(args, builder.Configuration);
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        var env = builder.Environment;
        if (env.IsDevelopment())
            builder.Configuration.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true);
        builder.Configuration.AddCommandLine(args);
        builder.Logging.AddConsole();
        builder.Services
            .Configure<AppSettings>(builder.Configuration.GetSection(nameof(SnTsTypeGenerator)))
            .AddDbContextPool<TypingsDbContext>(options =>
            {
                var dbFile = builder.Configuration.Get<AppSettings>()?.DbFile;
                if (string.IsNullOrEmpty(dbFile))
                    dbFile = AppSettings.DEFAULT_DbFile;
                try { dbFile = new FileInfo((Path.IsPathFullyQualified(dbFile) || Path.IsPathRooted(dbFile)) ? dbFile : Path.Combine(builder.Environment.ContentRootPath, dbFile)).FullName; }
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
        _host = builder.Build();
        _host.Run();
    }
}
