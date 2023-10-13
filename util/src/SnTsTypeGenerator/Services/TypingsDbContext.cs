using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Models;

namespace SnTsTypeGenerator.Services;

/// <summary>
/// The type definitions database context.
/// </summary>
public partial class TypingsDbContext : DbContext
{
    /// <summary>
    /// The Sqlite collation for case-insensitive matching.
    /// </summary>
    internal const string COLLATION_NOCASE = "NOCASE";

    /// <summary>
    /// The Sqlite code for the current date and time.
    /// </summary>
    internal const string DEFAULT_SQL_NOW = "(datetime('now','localtime'))";
    
    private readonly ILogger<TypingsDbContext> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly bool _pathValidated;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _pathValidated && Database.CanConnect();

    private static IEnumerable<string> GetOutputFileDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(OutputFiles)}"" (
    ""{nameof(OutputFile.Id)}"" UNIQUEIDENTIFIER NOt NULL COLLATE NOCASE,
    ""{nameof(OutputFile.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(OutputFile.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(OutputFile)}"" PRIMARY KEY(""{nameof(OutputFile.Id)}"")
)";
    }

    private static IEnumerable<string> GetSourceInfoDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Sources)}"" (
    ""{nameof(SourceInfo.FQDN)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SourceInfo.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SourceInfo.IsPersonalDev)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(SourceInfo.LastAccessed)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    CONSTRAINT ""PK_{nameof(SourceInfo)}"" PRIMARY KEY(""{nameof(SourceInfo.FQDN)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(SourceInfo)}_{nameof(SourceInfo.IsPersonalDev)}\" ON \"{nameof(Sources)}\" (\"{nameof(SourceInfo.IsPersonalDev)}\")";
    }

    private static IEnumerable<string> GetSysPackageDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Packages)}"" (
    ""{nameof(SysPackage.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysPackage.SysId)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysPackage.ShortDescription)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(SysPackage.LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(SysPackage.OutputId)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(SysPackage)}_{nameof(OutputFile)}"" REFERENCES ""{nameof(OutputFiles)}""(""{nameof(OutputFile.Id)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SysPackage.SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(SysPackage)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(SysPackage)}"" PRIMARY KEY(""{nameof(SysPackage.Name)}"")
)";
    }

    private static IEnumerable<string> GetSysScopeDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Scopes)}"" (
    ""{nameof(SysScope.Value)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysScope.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysScope.ShortDescription)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(SysScope.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysScope.LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(SysScope.SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(SysScope)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(SysScope)}"" PRIMARY KEY(""{nameof(SysScope.Value)}""),
    CONSTRAINT ""UK_{nameof(SysScope)}_{nameof(SysScope.SysID)}"" UNIQUE(""{nameof(SysScope.SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(SysScope)}_{nameof(SysScope.SysID)}\" ON \"{nameof(Scopes)}\" (\"{nameof(SysScope.SysID)}\" COLLATE NOCASE)";
    }

    private static IEnumerable<string> GetGlideTypeDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Types)}"" (
    ""{nameof(GlideType.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(GlideType.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(GlideType.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(GlideType.ScalarType)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(GlideType.ScalarLength)}"" INT DEFAULT NULL,
    ""{nameof(GlideType.ClassName)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(GlideType.UseOriginalValue)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(GlideType.IsVisible)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(GlideType.LastUpdated)}"" DATETIME NOT NULL,
    ""{nameof(GlideType.PackageName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SysPackage)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(SysPackage.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(GlideType.ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SysScope)}"" REFERENCES ""{nameof(Scopes)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(GlideType.SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(GlideType)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(GlideType)}"" PRIMARY KEY(""{nameof(GlideType.Name)}""),
    CONSTRAINT ""UK_{nameof(GlideType)}_{nameof(GlideType.SysID)}"" UNIQUE(""{nameof(GlideType.SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(GlideType.SysID)}\" ON \"{nameof(Types)}\" (\"{nameof(GlideType.SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(GlideType.UseOriginalValue)}\" ON \"{nameof(Types)}\" (\"{nameof(GlideType.UseOriginalValue)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(GlideType)}_{nameof(GlideType.IsVisible)}\" ON \"{nameof(Types)}\" (\"{nameof(GlideType.IsVisible)}\")";
    }

    private static IEnumerable<string> GetTableInfoDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Tables)}"" (
    ""{nameof(TableInfo.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(TableInfo.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(TableInfo.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(TableInfo.NumberPrefix)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(TableInfo.AccessibleFrom)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(TableInfo.ExtensionModel)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(TableInfo.IsExtendable)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(TableInfo.LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(TableInfo.PackageName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SysPackage)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(SysPackage.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TableInfo.ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(Scopes)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TableInfo.SuperClassName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(TableInfo.SuperClass)}"" REFERENCES ""{nameof(Tables)}""(""{nameof(TableInfo.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(TableInfo.SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(TableInfo)}"" PRIMARY KEY(""{nameof(TableInfo.Name)}""),
    CONSTRAINT ""UK_{nameof(TableInfo)}_{nameof(TableInfo.SysID)}"" UNIQUE(""{nameof(TableInfo.SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(TableInfo.SysID)}\" ON \"{nameof(Tables)}\" (\"{nameof(TableInfo.SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(TableInfo.IsExtendable)}\" ON \"{nameof(Tables)}\" (\"{nameof(TableInfo.IsExtendable)}\")";
    }

    private static IEnumerable<string> GetElementInfoDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Elements)}"" (
    ""{nameof(ElementInfo.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ElementInfo.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ElementInfo.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ElementInfo.IsActive)}"" BIT NOT NULL DEFAULT 1,
    ""{nameof(ElementInfo.IsArray)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.MaxLength)}"" INT DEFAULT NULL,
    ""{nameof(ElementInfo.SizeClass)}"" INT DEFAULT NULL,
    ""{nameof(ElementInfo.Comments)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(ElementInfo.DefaultValue)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(ElementInfo.IsDisplay)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.IsMandatory)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.IsPrimary)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.IsReadOnly)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.IsCalculated)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.IsUnique)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ElementInfo.LastUpdated)}"" DATETIME NOT NULL,
    ""{nameof(ElementInfo.PackageName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SysPackage)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(SysPackage.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ElementInfo.ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(Scopes)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ElementInfo.TableName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(ElementInfo.Table)}"" REFERENCES ""{nameof(Tables)}""(""{nameof(TableInfo.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ElementInfo.TypeName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(GlideType)}"" REFERENCES ""{nameof(Types)}""(""{nameof(GlideType.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ElementInfo.RefTableName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(TableInfo)}"" REFERENCES ""{nameof(Tables)}""(""{nameof(TableInfo.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ElementInfo.SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(ElementInfo)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(ElementInfo)}"" PRIMARY KEY(""{nameof(ElementInfo.Name)}""),
    CONSTRAINT ""UK_{nameof(ElementInfo)}_{nameof(ElementInfo.SysID)}"" UNIQUE(""{nameof(ElementInfo.SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(ElementInfo.SysID)}\" ON \"{nameof(Elements)}\" (\"{nameof(ElementInfo.SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(ElementInfo.IsActive)}\" ON \"{nameof(Elements)}\" (\"{nameof(ElementInfo.IsActive)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(ElementInfo.IsDisplay)}\" ON \"{nameof(Elements)}\" (\"{nameof(ElementInfo.IsDisplay)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(ElementInfo)}_{nameof(ElementInfo.IsPrimary)}\" ON \"{nameof(Elements)}\" (\"{nameof(ElementInfo.IsPrimary)}\")";
    }

    public TypingsDbContext(DbContextOptions<TypingsDbContext> options) : base(options)
    {
        _logger = Program.Host.Services.GetRequiredService<ILogger<TypingsDbContext>>();
        _scopeFactory = Program.Host.Services.GetRequiredService<IServiceScopeFactory>();
        SqliteConnectionStringBuilder csb;
        string connectionString = Database.GetConnectionString()!;
        FileInfo dbFile;
        try
        {
            string path = (csb = new(connectionString)).DataSource;
            dbFile = new(Path.IsPathFullyQualified(path) ? path : Path.Combine(Program.Host.Services.GetRequiredService<IHostEnvironment>().ContentRootPath, path));
        }
        catch (Exception exception)
        {
            _logger.LogDbfileValidationError(connectionString, exception);
            _pathValidated = false;
            return;
        }
        if (dbFile.Exists)
        {
            _pathValidated = true;
            return;
        }
        if (dbFile.Directory is null || !dbFile.Directory.Exists)
        {
            _logger.LogDbFileDirectoryNotFound(dbFile);
            _pathValidated = false;
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
            _logger.LogDbfileAccessError(connectionString, exception);
            _pathValidated = false;
            return;
        }
        try
        {
            using (connection)
            {
                var transaction = connection.BeginTransaction();
                bool executeDbInitCommands<T>(IEnumerable<string> commands)
                {
                    foreach (string query in commands)
                    {
                        using SqliteCommand command = connection.CreateCommand();
                        command.CommandText = query;
                        command.CommandType = System.Data.CommandType.Text;
                        try { _ = command.ExecuteNonQuery(); }
                        catch (Exception exception)
                        {
                            _logger.LogCriticalDbInitializationFailure(query, typeof(T), dbFile, exception);
                            return true;
                        }
                    }
                    return false;
                }
                if (executeDbInitCommands<OutputFile>(GetOutputFileDbInitCommands()) || executeDbInitCommands<SourceInfo>(GetSourceInfoDbInitCommands()) || executeDbInitCommands<SysPackage>(GetSysPackageDbInitCommands()) ||
                    executeDbInitCommands<SysScope>(GetSysScopeDbInitCommands()) || executeDbInitCommands<GlideType>(GetGlideTypeDbInitCommands()) || executeDbInitCommands<TableInfo>(GetTableInfoDbInitCommands()) ||
                    executeDbInitCommands<ElementInfo>(GetElementInfoDbInitCommands()))
                {
                    transaction.Rollback();
                    _pathValidated = false;
                    return;
                }
                transaction.Commit();
            }
            _pathValidated = true;
        }
        catch (Exception error)
        {
            _logger.LogCriticalDbInitializationFailure(dbFile, error);
            _pathValidated = false;
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
}
