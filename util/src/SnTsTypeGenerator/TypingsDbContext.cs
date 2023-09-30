using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

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
        _ = modelBuilder.Entity<OutputFile>(builder =>
            {
                _ = builder.HasKey(s => s.Id);
                _ = builder.HasIndex(s => s.Name).IsUnique();
                _ = builder.Property(nameof(OutputFile.Label)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(OutputFile.Name)).UseCollation(COLLATION_NOCASE);
            })
            .Entity<SourceInfo>(builder =>
            {
                _ = builder.HasKey(s => s.FQDN);
                _ = builder.HasIndex(t => t.IsPersonalDev);
                _ = builder.Property(nameof(SourceInfo.FQDN)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SourceInfo.Label)).UseCollation(COLLATION_NOCASE);
            })
            .Entity<SysPackage>(builder =>
            {
                _ = builder.HasKey(s => s.Name);
                _ = builder.Property(nameof(SysPackage.Name)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysPackage.SysId)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysPackage.ShortDescription)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysPackage.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                _ = builder.HasOne(t => t.Source).WithMany(s => s.Packages).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Output).WithMany(s => s.Packages).HasForeignKey(t => t.OutputId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            })
            .Entity<SysScope>(builder =>
            {
                _ = builder.HasKey(s => s.Value);
                _ = builder.HasIndex(s => s.SysID).IsUnique();
                _ = builder.Property(nameof(SysScope.Value)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysScope.Name)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysScope.ShortDescription)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysScope.SysID)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(SysScope.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                _ = builder.HasOne(t => t.Source).WithMany(s => s.Scopes).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
            })
            .Entity<GlideType>(builder =>
            {
                _ = builder.HasKey(t => t.Name);
                _ = builder.HasIndex(t => t.SysID).IsUnique();
                _ = builder.Property(nameof(GlideType.Name)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.Label)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.SysID)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.ScalarType)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.ClassName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.PackageName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.ScopeValue)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(GlideType.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                _ = builder.HasOne(t => t.Source).WithMany(s => s.Types).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Package).WithMany(s => s.Types).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Scope).WithMany(s => s.Types).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
            })
            .Entity<TableInfo>(builder =>
            {
                _ = builder.HasKey(t => t.Name);
                _ = builder.HasIndex(t => t.SysID).IsUnique();
                _ = builder.HasIndex(t => t.IsExtendable);
                _ = builder.Property(nameof(TableInfo.Name)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.Label)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.SysID)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.NumberPrefix)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.AccessibleFrom)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.ExtensionModel)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.PackageName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.ScopeValue)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.SuperClassName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(TableInfo.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                _ = builder.HasOne(t => t.Source).WithMany(s => s.Tables).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Package).WithMany(s => s.Tables).HasForeignKey(t => t.PackageName).OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Scope).WithMany(s => s.Tables).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.SuperClass).WithMany(s => s.Derived).HasForeignKey(t => t.SuperClassName).OnDelete(DeleteBehavior.Restrict);
            })
            .Entity<ElementInfo>(builder =>
            {
                _ = builder.HasKey(t => t.Name);
                _ = builder.HasIndex(t => t.SysID).IsUnique();
                _ = builder.Property(nameof(ElementInfo.Name)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.Label)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.SysID)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.Comments)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.DefaultValue)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.PackageName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.ScopeValue)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.TableName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.TypeName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.RefTableName)).UseCollation(COLLATION_NOCASE);
                _ = builder.Property(nameof(ElementInfo.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                _ = builder.HasOne(t => t.Source).WithMany(s => s.Elements).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Table).WithMany(s => s.Elements).HasForeignKey(t => t.TableName).IsRequired().OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Type).WithMany(s => s.Elements).HasForeignKey(t => t.TypeName).IsRequired().OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Package).WithMany(s => s.Elements).HasForeignKey(t => t.PackageName).OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Scope).WithMany(s => s.Elements).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
                _ = builder.HasOne(t => t.Reference).WithMany(s => s.ReferredBy).HasForeignKey(t => t.RefTableName).OnDelete(DeleteBehavior.Restrict);
            });
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
