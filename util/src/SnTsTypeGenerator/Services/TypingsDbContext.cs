using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
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

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => Database.CanConnect();

    private static IEnumerable<string> GetSncSourceDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Sources)}"" (
    ""{nameof(SncSource.FQDN)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SncSource.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SncSource.IsPersonalDev)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(SncSource.LastAccessed)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    CONSTRAINT ""PK_{nameof(Sources)}"" PRIMARY KEY(""{nameof(SncSource.FQDN)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(Sources)}_{nameof(SncSource.IsPersonalDev)}\" ON \"{nameof(Sources)}\" (\"{nameof(SncSource.IsPersonalDev)}\")";
    }

    private static IEnumerable<string> GetPackageDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Packages)}"" (
    ""{nameof(Package.ID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Package.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Package.Version)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Package.IsBaseline)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Package.LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(Package.ParentID)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Packages)}_{nameof(Package.Parent)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(Package.ID)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Package.SourceFqdn)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(Packages)}_{nameof(Package.Source)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SncSource.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Package.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(Packages)}"" PRIMARY KEY(""{nameof(Package.ID)}"")
)";
    }

    private static IEnumerable<string> GetScopeDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Scopes)}"" (
    ""{nameof(Scope.Value)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Scope.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Scope.Version)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Scope.ID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Scope.ShortDescription)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(Scope.LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(Scope.SourceFqdn)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(Scopes)}_{nameof(Scope.Source)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SncSource.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Scope.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(Scopes)}"" PRIMARY KEY(""{nameof(Scope.Value)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(Scopes)}_{nameof(Scope.SysID)}\" ON \"{nameof(Scopes)}\" (\"{nameof(Scope.SysID)}\" COLLATE NOCASE)";
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
    ""{nameof(GlideType.GlobalElementType)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(GlideType.ScopedElementType)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(GlideType.LastUpdated)}"" DATETIME NOT NULL,
    ""{nameof(GlideType.PackageID)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Types)}_{nameof(GlideType.Package)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(Package.ID)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(GlideType.ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Types)}_{nameof(GlideType.Scope)}"" REFERENCES ""{nameof(Scopes)}""(""{nameof(Scope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(GlideType.SourceFqdn)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(Types)}_{nameof(GlideType.Source)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SncSource.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(Types)}"" PRIMARY KEY(""{nameof(GlideType.Name)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(Types)}_{nameof(GlideType.SysID)}\" ON \"{nameof(Types)}\" (\"{nameof(GlideType.SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(Types)}_{nameof(GlideType.UseOriginalValue)}\" ON \"{nameof(Types)}\" (\"{nameof(GlideType.UseOriginalValue)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(Types)}_{nameof(GlideType.IsVisible)}\" ON \"{nameof(Types)}\" (\"{nameof(GlideType.IsVisible)}\")";
    }

    private static IEnumerable<string> GetTableDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Tables)}"" (
    ""{nameof(Table.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Table.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Table.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Table.NumberPrefix)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(Table.AccessibleFrom)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Table.ExtensionModel)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(Table.IsExtendable)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Table.LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(Table.IsInterface)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Table.PackageID)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Tables)}_{nameof(Table.Package)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(Package.ID)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Table.ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Tables)}_{nameof(Table.Scope)}"" REFERENCES ""{nameof(Scopes)}""(""{nameof(Scope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Table.SuperClassName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Tables)}_{nameof(Table.SuperClass)}"" REFERENCES ""{nameof(Tables)}""(""{nameof(Table.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Table.SourceFqdn)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(Tables)}_{nameof(Table.Source)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SncSource.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(Tables)}"" PRIMARY KEY(""{nameof(Table.Name)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(Tables)}_{nameof(Table.SysID)}\" ON \"{nameof(Tables)}\" (\"{nameof(Table.SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(Tables)}_{nameof(Table.IsExtendable)}\" ON \"{nameof(Tables)}\" (\"{nameof(Table.IsExtendable)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(Tables)}_{nameof(Table.IsInterface)}\" ON \"{nameof(Tables)}\" (\"{nameof(Table.IsInterface)}\")";
    }

    private static IEnumerable<string> GetElementDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(Elements)}"" (
    ""{nameof(Element.Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Element.Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Element.SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Element.IsActive)}"" BIT NOT NULL DEFAULT 1,
    ""{nameof(Element.IsArray)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.MaxLength)}"" INT DEFAULT NULL,
    ""{nameof(Element.SizeClass)}"" INT DEFAULT NULL,
    ""{nameof(Element.Comments)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(Element.DefaultValue)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(Element.IsDisplay)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.IsMandatory)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.IsPrimary)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.IsReadOnly)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.IsCalculated)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.IsUnique)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(Element.LastUpdated)}"" DATETIME NOT NULL,
    ""{nameof(Element.PackageID)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Elements)}_{nameof(Element.Package)}"" REFERENCES ""{nameof(Packages)}""(""{nameof(Package.ID)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Element.TableName)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(Elements)}_{nameof(Element.Table)}"" REFERENCES ""{nameof(Tables)}""(""{nameof(Table.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Element.TypeName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Elements)}_{nameof(Element.Type)}"" REFERENCES ""{nameof(Types)}""(""{nameof(GlideType.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Element.RefTableName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(Elements)}_{nameof(Element.Reference)}"" REFERENCES ""{nameof(Tables)}""(""{nameof(Table.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(Element.SourceFqdn)}"" NVARCHAR NOT NULL CONSTRAINT ""FK_{nameof(Elements)}_{nameof(Element.Source)}"" REFERENCES ""{nameof(Sources)}""(""{nameof(SncSource.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(Element)}"" PRIMARY KEY(""{nameof(Element.Name)}"", ""{nameof(Element.TableName)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(Element)}_{nameof(Element.SysID)}\" ON \"{nameof(Elements)}\" (\"{nameof(Element.SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(Element)}_{nameof(Element.IsActive)}\" ON \"{nameof(Elements)}\" (\"{nameof(Element.IsActive)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(Element)}_{nameof(Element.IsDisplay)}\" ON \"{nameof(Elements)}\" (\"{nameof(Element.IsDisplay)}\")";
        yield return $"CREATE INDEX \"IDX_{nameof(Element)}_{nameof(Element.IsPrimary)}\" ON \"{nameof(Elements)}\" (\"{nameof(Element.IsPrimary)}\")";
    }

    internal async static Task InitializeAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        using var dbContext = scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
        await dbContext.InitializeAsync(cancellationToken);
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        SqliteConnectionStringBuilder csb;
        string connectionString = Database.GetConnectionString()!;
        string path;
        try { path = (csb = new(connectionString)).DataSource; }
        catch (ArgumentException exc) { throw new DbInitializationException("Invalid connection string", exc, connectionString); }
        if (string.IsNullOrEmpty(path))
            return;
        FileInfo dbFile;
        try { dbFile = new(path); }
        catch (System.Security.SecurityException exception)
        {
            throw new DbfileAccessException("Insufficent permissions to access database.", exception, path);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new DbfileAccessException("Access to database is denied.", exception, path);
        }
        catch (PathTooLongException exception)
        {
            throw new DbfileAccessException("The database path, file name, or both exceed the system-defined maximum length.", exception, path);
        }
        catch (NotSupportedException exception)
        {
            throw new DbfileAccessException("The database path is invalid.", exception, path);
        }
        if (dbFile.Exists)
            return;
        if (dbFile.Directory is null || !dbFile.Directory.Exists)
            throw new DbFileDirectoryNotFoundException(dbFile.DirectoryName);
        csb.Mode = SqliteOpenMode.ReadWriteCreate;
        await _logger.WithActivityScopeAsync(LogActivityType.InitializeDatabase, async () =>
        {
            using SqliteConnection connection = new(csb.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            async Task executeDbInitCommandsAsync<T>(IEnumerable<string> commands)
            {
                foreach (string query in commands)
                {
                    using SqliteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;
                    try { _ = await command.ExecuteNonQueryAsync(cancellationToken); }
                    catch (System.Data.Common.DbException exception)
                    {
                        throw new DbInitializationException(dbFile.FullName, typeof(T), exception);
                    }
                }
            }
            using var transaction = connection.BeginTransaction();
            try
            {
                await _logger.WithActivityScope(LogActivityType.InitializeDbTable, nameof(Sources), () => executeDbInitCommandsAsync<SncSource>(GetSncSourceDbInitCommands()));
                await _logger.WithActivityScope(LogActivityType.InitializeDbTable, nameof(Packages), () => executeDbInitCommandsAsync<Package>(GetPackageDbInitCommands()));
                await _logger.WithActivityScope(LogActivityType.InitializeDbTable, nameof(Scopes), () => executeDbInitCommandsAsync<Scope>(GetScopeDbInitCommands()));
                await _logger.WithActivityScope(LogActivityType.InitializeDbTable, nameof(Types), () => executeDbInitCommandsAsync<GlideType>(GetGlideTypeDbInitCommands()));
                await _logger.WithActivityScope(LogActivityType.InitializeDbTable, nameof(Tables), () => executeDbInitCommandsAsync<Table>(GetTableDbInitCommands()));
                await _logger.WithActivityScope(LogActivityType.InitializeDbTable, nameof(Elements), () => executeDbInitCommandsAsync<Element>(GetElementDbInitCommands()));
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            transaction.Commit();
        });
    }

    public TypingsDbContext(DbContextOptions<TypingsDbContext> options) : base(options)
    {
        _logger = Program.Host.Services.GetRequiredService<ILogger<TypingsDbContext>>();
        _scopeFactory = Program.Host.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private void OnBeforeSave(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        using var scope = _logger.BeginActivityScope(LogActivityType.ValidateBeforeSave);
        using IServiceScope serviceScope = _scopeFactory.CreateScope();
        foreach (EntityEntry e in ChangeTracker.Entries())
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            switch (e.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    DbContextServiceProvider serviceProvider = new(this, serviceScope.ServiceProvider, e);
                    ValidationContext validationContext = new(e.Entity, serviceProvider, null);
                    _logger.LogValidatingEntity(e.State, e.Metadata, e.Entity);
                    try { Validator.ValidateObject(e.Entity, validationContext, true); }
                    catch (ValidationException validationException)
                    {
                        LoggerMessages.LogEntityValidationFailure(_logger, e.Metadata, e.Entity, validationException);
                        _logger.LogEntityValidationFailure(e.Metadata, e.Entity, validationException);
                        throw;
                    }
                    _logger.LogValidationCompleted(e.State, e.Metadata, e.Entity);
                    break;
            }
        }
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="ValidationException">An entity validation error is encountered before saving to the database.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save. This is usually because the data in the database has been modified since it was loaded into memory.</exception>
    public override int SaveChanges() => _logger.WithActivityScope(LogActivityType.SaveDbChanges, () =>
    {
        OnBeforeSave();
        using var scope = _logger.BeginActivityScope(LogActivityType.SaveDbChanges);
        int returnValue;
        returnValue = base.SaveChanges();
        _logger.LogDbSaveChangesCompleted(false, null, returnValue);
        return returnValue;
    });

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="ChangeTracker.AcceptAllChanges"/> is called after the changes have been sent successfully to the database.</param>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="ValidationException">An entity validation error is encountered before saving to the database.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save. This is usually because the data in the database has been modified since it was loaded into memory.</exception>
    public override int SaveChanges(bool acceptAllChangesOnSuccess) => _logger.WithActivityScope(LogActivityType.SaveDbChanges, acceptAllChangesOnSuccess, () =>
    {
        OnBeforeSave();
        using var scope = _logger.BeginActivityScope(LogActivityType.SaveDbChanges);
        int returnValue = base.SaveChanges(acceptAllChangesOnSuccess);
        _logger.LogDbSaveChangesCompleted(false, acceptAllChangesOnSuccess, returnValue);
        return returnValue;
    });

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="ChangeTracker.AcceptAllChanges"/> is called after the changes have been sent successfully to the database.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="ValidationException">An entity validation error is encountered before saving to the database.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save. This is usually because the data in the database has been modified since it was loaded into memory.</exception>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSave(cancellationToken);
        using var scope = _logger.BeginActivityScope(LogActivityType.SaveDbChanges);
        int returnValue = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        _logger.LogDbSaveChangesCompleted(true, acceptAllChangesOnSuccess, returnValue);
        return returnValue;
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="ValidationException">An entity validation error is encountered before saving to the database.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save. This is usually because the data in the database has been modified since it was loaded into memory.</exception>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSave(cancellationToken);
        using var scope = _logger.BeginActivityScope(LogActivityType.SaveDbChanges);
        int returnValue = await base.SaveChangesAsync(cancellationToken);
        _logger.LogDbSaveChangesCompleted(true, null, returnValue);
        return returnValue;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.WithActivityScope(LogActivityType.ModelCreating, () =>
        {
            _ = modelBuilder.Entity<SncSource>(builder =>
                {
                    _ = builder.HasKey(s => s.FQDN);
                    _ = builder.HasIndex(t => t.IsPersonalDev);
                    _ = builder.Property(nameof(SncSource.FQDN)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(SncSource.Label)).UseCollation(COLLATION_NOCASE);
                })
                .Entity<Package>(builder =>
                {
                    _ = builder.HasKey(s => s.ID);
                    _ = builder.Property(nameof(Package.ID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Package.Name)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Package.Version)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Package.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Package.SysID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.HasOne(t => t.Source).WithMany(s => s.Packages).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Parent).WithMany(s => s.Children).HasForeignKey(t => t.ParentID).OnDelete(DeleteBehavior.Restrict);
                })
                .Entity<Scope>(builder =>
                {
                    _ = builder.HasKey(s => s.Value);
                    _ = builder.HasIndex(s => s.SysID).IsUnique();
                    _ = builder.Property(nameof(Scope.Value)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Scope.Name)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Scope.Version)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Scope.ID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Scope.ShortDescription)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Scope.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Scope.SysID)).UseCollation(COLLATION_NOCASE);
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
                    _ = builder.Property(nameof(GlideType.PackageID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(GlideType.ScopeValue)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(GlideType.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                    _ = builder.HasOne(t => t.Source).WithMany(s => s.Types).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Package).WithMany(s => s.Types).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Scope).WithMany(s => s.Types).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
                })
                .Entity<Table>(builder =>
                {
                    _ = builder.HasKey(t => t.Name);
                    _ = builder.HasIndex(t => t.SysID).IsUnique();
                    _ = builder.HasIndex(t => t.IsExtendable);
                    _ = builder.HasIndex(t => t.IsInterface);
                    _ = builder.Property(nameof(Table.Name)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.Label)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.SysID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.NumberPrefix)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.AccessibleFrom)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.ExtensionModel)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.PackageID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.ScopeValue)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.SuperClassName)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Table.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                    _ = builder.HasOne(t => t.Source).WithMany(s => s.Tables).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Package).WithMany(s => s.Tables).HasForeignKey(t => t.PackageID).OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Scope).WithMany(s => s.Tables).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.SuperClass).WithMany(s => s.Derived).HasForeignKey(t => t.SuperClassName).OnDelete(DeleteBehavior.Restrict);
                })
                .Entity<Element>(builder =>
                {
                    _ = builder.HasKey(nameof(Element.Name), nameof(Element.TableName));
                    _ = builder.HasIndex(t => t.SysID).IsUnique();
                    _ = builder.Property(nameof(Element.Name)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.Label)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.SysID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.Comments)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.DefaultValue)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.PackageID)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.TableName)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.TypeName)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.RefTableName)).UseCollation(COLLATION_NOCASE);
                    _ = builder.Property(nameof(Element.SourceFqdn)).UseCollation(COLLATION_NOCASE);
                    _ = builder.HasOne(t => t.Source).WithMany(s => s.Elements).HasForeignKey(t => t.SourceFqdn).IsRequired().OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Table).WithMany(s => s.Elements).HasForeignKey(t => t.TableName).IsRequired().OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Type).WithMany(s => s.Elements).HasForeignKey(t => t.TypeName).IsRequired().OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Package).WithMany(s => s.Elements).HasForeignKey(t => t.PackageID).OnDelete(DeleteBehavior.Restrict);
                    _ = builder.HasOne(t => t.Reference).WithMany(s => s.ReferredBy).HasForeignKey(t => t.RefTableName).OnDelete(DeleteBehavior.Restrict);
                });
        });
    }

    /// <summary>
    /// Gets the source ServiceNow instance information records.
    /// </summary>
    public virtual DbSet<SncSource> Sources { get; set; } = null!;

    /// <summary>
    /// Gets the source packages.
    /// </summary>
    public virtual DbSet<Package> Packages { get; set; } = null!;

    /// <summary>
    /// Gets the scope definitions.
    /// </summary>
    public virtual DbSet<Scope> Scopes { get; set; } = null!;

    /// <summary>
    /// Gets the ServiceNow column types.
    /// </summary>
    public virtual DbSet<GlideType> Types { get; set; } = null!;

    /// <summary>
    /// Gets the ServiceNow tables.
    /// </summary>
    public virtual DbSet<Table> Tables { get; set; } = null!;

    /// <summary>
    /// Gets the ServiceNow table elements (columns).
    /// </summary>
    public virtual DbSet<Element> Elements { get; set; } = null!;
}
