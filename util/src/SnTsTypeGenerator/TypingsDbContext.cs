using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnTsTypeGenerator;

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
        _ = modelBuilder.Entity<GlideType>(GlideType.OnBuildEntity)
            .Entity<TableInfo>(TableInfo.OnBuildEntity)
            .Entity<SysScope>(SysScope.OnBuildEntity)
            .Entity<ElementInfo>(ElementInfo.OnBuildEntity);
    }

    public virtual DbSet<GlideType> Types { get; set; } = null!;

    public virtual DbSet<TableInfo> Tables { get; set; } = null!;

    public virtual DbSet<SysScope> Scopes { get; set; } = null!;

    public virtual DbSet<SysPackage> Packages { get; set; } = null!;

    public virtual DbSet<ElementInfo> Elements { get; set; } = null!;

    public virtual DbSet<SourceInfo> Sources { get; set; } = null!;
}
