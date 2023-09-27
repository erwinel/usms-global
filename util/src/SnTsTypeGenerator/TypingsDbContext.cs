using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator;

/// <summary>
/// The type definitions database context.
/// </summary>
public class TypingsDbContext : DbContext
{
    private readonly ILogger<TypingsDbContext> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public TypingsDbContext(DbContextOptions<TypingsDbContext> options, ILogger<TypingsDbContext> logger, IServiceScopeFactory scopeFactory) : base(options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
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
            bool execDbInitCommands<T>(IEnumerable<string> commands)
            {
                foreach (string query in commands)
                {
                    using SqliteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;
                    try { _ = command.ExecuteNonQuery(); }
                    catch (Exception exception)
                    {
                        _logger.LogCriticalDbInitializationFailure(query, typeof(OutputFile), dbFile, exception);
                        return true;
                    }
                }
                return false;
            }
            if (execDbInitCommands<OutputFile>(OutputFile.GetDbInitCommands()) || execDbInitCommands<SourceInfo>(SourceInfo.GetDbInitCommands()) || execDbInitCommands<SysPackage>(SysPackage.GetDbInitCommands()) ||
                    execDbInitCommands<SysScope>(SysScope.GetDbInitCommands()) || execDbInitCommands<GlideType>(GlideType.GetDbInitCommands()) || execDbInitCommands<TableInfo>(TableInfo.GetDbInitCommands()) ||
                    execDbInitCommands<ElementInfo>(ElementInfo.GetDbInitCommands()))
                transaction.Rollback();
            else
                transaction.Commit();
        }
    }

    private void OnBeforeSave(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(OnBeforeSave));
        using IServiceScope serviceScope = _scopeFactory.CreateScope();
        foreach (EntityEntry e in ChangeTracker.Entries())
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            if (e.Entity is IValidatableObject entity)
                switch (e.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        DbContextServiceProvider serviceProvider = new(this, serviceScope.ServiceProvider, e);
                        ValidationContext validationContext = new(entity, serviceProvider, null);
                        _logger.LogValidatingEntity(e.State, e.Metadata, entity);
                        try { Validator.ValidateObject(entity, validationContext, true); }
                        catch (ValidationException validationException)
                        {
                            LoggerMessages.LogEntityValidationFailure(_logger, e.Metadata, entity, validationException);
                            _logger.LogEntityValidationFailure(e.Metadata, entity, validationException);
                            throw;
                        }
                        _logger.LogValidationCompleted(e.State, e.Metadata, entity);
                        break;
                }
        }
    }

    public override int SaveChanges()
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChanges));
        OnBeforeSave();
        int returnValue = base.SaveChanges();
        _logger.LogDbSaveChangesCompletedTrace(false, null, returnValue);
        return returnValue;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChanges), nameof(acceptAllChangesOnSuccess), acceptAllChangesOnSuccess);
        OnBeforeSave();
        int returnValue = base.SaveChanges(acceptAllChangesOnSuccess);
        _logger.LogDbSaveChangesCompletedTrace(false, acceptAllChangesOnSuccess, returnValue);
        return returnValue;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChangesAsync), nameof(acceptAllChangesOnSuccess), acceptAllChangesOnSuccess);
        OnBeforeSave();
        int returnValue = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        _logger.LogDbSaveChangesCompletedTrace(true, acceptAllChangesOnSuccess, returnValue);
        return returnValue;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using IDisposable? scope = _logger.BeginExecuteMethodScope(nameof(SaveChangesAsync));
        OnBeforeSave();
        int returnValue = await base.SaveChangesAsync(cancellationToken);
        _logger.LogDbSaveChangesCompletedTrace(true, null, returnValue);
        return returnValue;
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

    internal class DbContextServiceProvider : IServiceProvider
    {
        private readonly object _entity;
        private readonly TypingsDbContext _dbContext;
        private readonly IServiceProvider _backingServiceProvider;

        internal DbContextServiceProvider(TypingsDbContext dbContext, IServiceProvider backingServiceProvider, object entity)
        {
            _dbContext = dbContext;
            _entity = entity;
            _backingServiceProvider = backingServiceProvider;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType is null)
                return null;
            if (serviceType.IsInstanceOfType(_entity))
                return _entity;
            if (serviceType.IsInstanceOfType(_dbContext._logger))
                return _dbContext._logger;
            return _backingServiceProvider.GetService(serviceType);
        }
    }
}
