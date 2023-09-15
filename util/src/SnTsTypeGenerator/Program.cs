using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

internal class Program
{
    private static IHost _host = null!;

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        builder.Configuration.AddCommandLine(args);
        builder.Logging.AddConsole();
        builder.Services
            .Configure<SnTsTypeGenerator.CommandSettings>(builder.Configuration)
            .Configure<SnTsTypeGenerator.AppSettings>(builder.Configuration.GetSection(nameof(SnTsTypeGenerator)))
            .AddDbContextPool<SnTsTypeGenerator.TypingsDbContext>(options =>
            {
                var dbFile = builder.Configuration.Get<SnTsTypeGenerator.AppSettings>()?.DbFile;
                if (string.IsNullOrEmpty(dbFile) && string.IsNullOrEmpty(dbFile = builder.Configuration.Get<SnTsTypeGenerator.CommandSettings>()?.Dbfile))
                    dbFile = SnTsTypeGenerator.AppSettings.DEFAULT_DbFile;
                try { dbFile = new FileInfo((Path.IsPathFullyQualified(dbFile) || Path.IsPathRooted(dbFile)) ? dbFile : Path.Combine(builder.Environment.ContentRootPath, dbFile)).FullName; }
                catch { }
                options.UseSqlite(new SqliteConnectionStringBuilder
                {
                    DataSource = dbFile,
                    ForeignKeys = true,
                    Mode = SqliteOpenMode.ReadWrite
                }.ConnectionString);
            })
            .AddHostedService<SnTsTypeGenerator.MainWorkerService>();
        _host = builder.Build();
        _host.Run();
    }
}
