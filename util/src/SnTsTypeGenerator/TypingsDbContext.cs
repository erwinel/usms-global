using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator;

/// <summary>
/// The type definitions database context.
/// </summary>
public class TypingsDbContext : DbContext
{
    private readonly ILogger<TypingsDbContext> _logger;

    public TypingsDbContext(DbContextOptions<TypingsDbContext> options, ILogger<TypingsDbContext> logger) : base(options)
    {
        _logger = logger;
        SqliteConnectionStringBuilder csb;
        string dbFile = Database.GetConnectionString()!;
        try
        {
            dbFile = (csb = new(dbFile)).DataSource;
            if (File.Exists(dbFile))
                return;
        }
        catch (Exception exception)
        {
            _logger.LogCriticalDbfileValidationError(dbFile, exception);
            return;
        }
        csb.Mode = SqliteOpenMode.ReadWriteCreate;
        SqliteConnection connection;
        try
        {
            connection = new(csb.ConnectionString);
            connection.Open();
        }
        catch (Exception exception)
        {
            _logger.LogCriticalDbfileAccessError(dbFile, exception);
            return;
        }
        using (connection)
        {
            var transaction = connection.BeginTransaction();
            foreach (string query in OutputFile.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(OutputFile), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            foreach (string query in SourceInfo.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(SourceInfo), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            foreach (string query in SysPackage.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(SysPackage), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            foreach (string query in SysScope.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(SysScope), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            foreach (string query in GlideType.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(GlideType), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            foreach (string query in TableInfo.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(TableInfo), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            foreach (string query in ElementInfo.GetDbInitCommands())
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;
                try { _ = command.ExecuteNonQuery(); }
                catch (Exception exception)
                {
                    _logger.LogCriticalDbInitializationFailureError(query, typeof(ElementInfo), dbFile, exception);
                    transaction.Rollback();
                    return;
                }
            }
            transaction.Commit();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<OutputFile>(OutputFile.OnBuildEntity)
            .Entity<SourceInfo>(SourceInfo.OnBuildEntity)
            .Entity<SysPackage>(SysPackage.OnBuildEntity)
            .Entity<SysScope>(SysScope.OnBuildEntity)
            .Entity<GlideType>(GlideType.OnBuildEntity)
            .Entity<TableInfo>(TableInfo.OnBuildEntity)
            .Entity<ElementInfo>(ElementInfo.OnBuildEntity);
    }

    /// <summary>
    /// Gets the explicit output file definitions.
    /// </summary>
    public virtual DbSet<OutputFile> OutputFiles { get; set; } = null!;

    /// <summary>
    /// Gets the source ServiceNow instance information records.
    /// </summary>
    public virtual DbSet<SourceInfo> Sources { get; set; } = null!;

    /// <summary>
    /// Gets the source packages.
    /// </summary>
    public virtual DbSet<SysPackage> Packages { get; set; } = null!;

    /// <summary>
    /// Gets the scope definitions.
    /// </summary>
    public virtual DbSet<SysScope> Scopes { get; set; } = null!;

    /// <summary>
    /// Gets the ServiceNow column types.
    /// </summary>
    public virtual DbSet<GlideType> Types { get; set; } = null!;

    /// <summary>
    /// Gets the ServiceNow tables.
    /// </summary>
    public virtual DbSet<TableInfo> Tables { get; set; } = null!;

    /// <summary>
    /// Gets the ServiceNow table elements (columns).
    /// </summary>
    public virtual DbSet<ElementInfo> Elements { get; set; } = null!;
}
