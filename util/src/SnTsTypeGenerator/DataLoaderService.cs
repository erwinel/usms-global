using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator;

/// <summary>
/// Loads data from the database and from the remote ServiceNow instance.
/// </summary>
public sealed class DataLoaderService : IDisposable
{
    private readonly TypingsDbContext _dbContext;
    private TableAPIService? _tableAPIService;
    private readonly ILogger<DataLoaderService> _logger;
    private readonly Dictionary<string, string> _scopeIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _tableIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _packageIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, SourceInfo> _sourceCache = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _tableAPIService is not null && _tableAPIService.InitSuccessful && _dbContext.InitSuccessful;

    private async Task<SourceInfo> GetSourceAsync(string fqdn, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_sourceCache.TryGetValue(fqdn, out SourceInfo? source))
            return source;
        if ((source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.FQDN == fqdn, cancellationToken)) is null)
        {
            source = new()
            {
                FQDN = fqdn,
                Label = fqdn,
                IsPersonalDev = false,
                LastAccessed = DateTime.Now
            };
            await _dbContext.Sources.AddAsync(source, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            _sourceCache.Add(fqdn, source);
            await _dbContext.SaveChangesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }
        return source;
    }

    private async Task<SysPackage> GetPackageAsync(SysPackage package, CancellationToken cancellationToken)
    {
        if (_packageIdMap.TryGetValue(package.SysId, out string? name))
            return await _dbContext.Packages.FirstAsync(t => t.Name == name, cancellationToken);
        package.Source = await GetSourceAsync(package.SourceFqdn, cancellationToken);
        package.LastUpdated = DateTime.Now;
        await _dbContext.Packages.AddAsync(package, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        _packageIdMap.Add(package.SysId, package.Name);
        await _dbContext.SaveChangesAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return package;
    }

    private async Task<SysScope> GetScopeAsync(SysScope scope, CancellationToken cancellationToken)
    {
        if (_scopeIdMap.TryGetValue(scope.SysID, out string? value))
            return await _dbContext.Scopes.FirstAsync(t => t.Value == value, cancellationToken);
        SysScope? retrieved = await _tableAPIService!.GetScopeByIDAsync(scope.SysID, cancellationToken);
        if (retrieved is null)
            scope.Value = scope.SysID;
        else
        {
            value = scope.Value;
            if ((retrieved = await _dbContext.Scopes.FirstOrDefaultAsync(t => t.Value == value, cancellationToken)) is not null)
            {
                _scopeIdMap.Add(scope.SysID, value);
                return retrieved;
            }
        }
        scope.Source = await GetSourceAsync(scope.SourceFqdn, cancellationToken);
        scope.LastUpdated = DateTime.Now;
        await _dbContext.Scopes.AddAsync(scope, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        _scopeIdMap.Add(scope.SysID, scope.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return scope;
    }

    private async Task<GlideType> GetGlideTypeAsync(GlideType type, CancellationToken cancellationToken)
    {
        string name = type.Name;
        GlideType? result = await _dbContext.Types.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (result is not null)
            return result;
        if ((result = await _tableAPIService!.GetGlideTypeByNameAsync(name, cancellationToken)) is not null)
        {
            type = result;
            if (type.Scope is not null)
                type.Scope = await GetScopeAsync(type.Scope, cancellationToken);
            if (type.Package is not null)
                type.Package = await GetPackageAsync(type.Package, cancellationToken);
        }

        type.Source = await GetSourceAsync(type.SourceFqdn, cancellationToken);
        type.LastUpdated = DateTime.Now;
        await _dbContext.Types.AddAsync(type, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await _dbContext.SaveChangesAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return type;
    }

    private async Task SaveTableAsync(TableInfo tableInfo, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (tableInfo.Scope is not null)
            tableInfo.Scope = await GetScopeAsync(tableInfo.Scope, cancellationToken);
        if (tableInfo.Package is not null)
            tableInfo.Package = await GetPackageAsync(tableInfo.Package, cancellationToken);
        SourceInfo source = await GetSourceAsync(tableInfo.SourceFqdn, cancellationToken);
        tableInfo.Source = source;
        tableInfo.LastUpdated = DateTime.Now;
        TableInfo? superClass = tableInfo.SuperClass;
        tableInfo.SuperClass = null;
        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogAddingTableToDb(tableInfo.Name);
        await _dbContext.Tables.AddAsync(tableInfo, cancellationToken);
        _tableIdMap.Add(tableInfo.SysID, tableInfo.Name);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (superClass is not null && (tableInfo.SuperClass = await GetTableAsync(superClass, cancellationToken)) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        cancellationToken.ThrowIfCancellationRequested();

        ElementInfo[] elements = await _tableAPIService!.GetElementsByTableNameAsync(tableInfo.Name, cancellationToken);
        if (elements.Length == 0)
        {
            _logger.LogNewTableSaveCompleteTrace(tableInfo.Name);
            return;
        }

        if (tableInfo.SuperClass is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
            elements = elements.GetBaseElements(await tableInfo.SuperClass.GetRelatedCollectionAsync(_dbContext.Tables, t => t.Elements, cancellationToken)).Where(a =>
            {
                (ElementInfo e, ElementInfo? b, bool isTypeOverride) = a;
                return isTypeOverride || b is null || e.IsActive != b.IsActive || e.IsArray != b.IsArray || e.MaxLength != b.MaxLength || e.IsDisplay != b.IsDisplay || e.SizeClass != b.SizeClass ||
                    e.IsMandatory != b.IsMandatory || e.IsPrimary != b.IsPrimary || e.IsReadOnly != b.IsReadOnly || e.IsCalculated != b.IsCalculated || e.IsUnique != b.IsUnique ||
                    !comparer.Equals(e.Label, b.Label) || ((e.Comments is null) ? b.Comments is not null : b.Comments is null || !comparer.Equals(e.Comments, b.Comments)) ||
                    ((e.DefaultValue is null) ? b.DefaultValue is not null : b.DefaultValue is null || !comparer.Equals(e.Comments, b.DefaultValue)) ||
                    ((e.PackageName is null) ? b.PackageName is not null : b.PackageName is null || !comparer.Equals(e.Comments, b.PackageName)) ||
                    ((e.ScopeValue is null) ? b.ScopeValue is not null : b.ScopeValue is null || !comparer.Equals(e.Comments, b.ScopeValue));
            }).Select(a => a.Inherited).ToArray();
            if (elements.Length == 0)
            {
                _logger.LogNewTableSaveCompleteTrace(tableInfo.Name);
                return;
            }
        }

        foreach (ElementInfo e in elements)
        {
            e.Table = tableInfo;
            e.Source = source;
        }

        foreach (var g in elements.Where(e => e.Package is not null).GroupBy(e => e.Package!.SysId))
        {
            ElementInfo[] arr = g.ToArray();
            cancellationToken.ThrowIfCancellationRequested();
            SysPackage p = await GetPackageAsync(arr[0].Package!, cancellationToken);
            foreach (ElementInfo e in arr)
                e.Package = p;
        }

        foreach (var g in elements.Where(e => e.Scope is not null).GroupBy(e => e.Scope!.Value))
        {
            ElementInfo[] arr = g.ToArray();
            cancellationToken.ThrowIfCancellationRequested();
            SysScope s = await GetScopeAsync(arr[0].Scope!, cancellationToken);
            foreach (ElementInfo e in arr)
                e.Scope = s;
        }

        foreach (var g in elements.Where(e => e.Type is not null).GroupBy(e => e.Type!.Name))
        {
            ElementInfo[] arr = g.ToArray();
            cancellationToken.ThrowIfCancellationRequested();
            GlideType t = await GetGlideTypeAsync(arr[0].Type!, cancellationToken);
            foreach (ElementInfo e in arr)
                e.Type = t;
        }

        foreach (var g in elements.Where(e => e.Reference is not null).GroupBy(e => e.Reference!.SysID))
        {
            ElementInfo[] arr = g.ToArray();
            cancellationToken.ThrowIfCancellationRequested();
            TableInfo t = await GetTableAsync(arr[0].Reference!, cancellationToken);
            foreach (ElementInfo e in arr)
                e.Reference = t;
        }

        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogAddingElementsToDatabase(tableInfo.Name);
        await _dbContext.Elements.AddRangeAsync(elements, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogNewTableSaveCompleteTrace(tableInfo.Name);
    }

    private async Task<TableInfo> GetTableAsync(TableInfo table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_tableIdMap.TryGetValue(table.SysID, out string? name))
            return await _dbContext.Tables.FirstAsync(t => t.Name == name, cancellationToken);
        TableInfo? result = await _tableAPIService!.GetTableByIdAsync(table.SysID, cancellationToken);
        if (result is not null)
            table = result;
        cancellationToken.ThrowIfCancellationRequested();
        await SaveTableAsync(table, cancellationToken);
        return table;
    }

    /// <summary>
    /// Loads information for the table that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="TableInfo"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no table was found in the database or in the remote ServiceNow instance.</returns>
    internal async Task<TableInfo?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_tableAPIService is null)
            throw new ObjectDisposedException(nameof(TableAPIService));
        TableInfo? tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (tableInfo is null && (tableInfo = await _tableAPIService.GetTableByNameAsync(name, cancellationToken)) is not null)
            await SaveTableAsync(tableInfo, cancellationToken);
        return tableInfo;
    }

    public DataLoaderService(TypingsDbContext dbContext, TableAPIService tableAPIService, ILogger<DataLoaderService> logger)
    {
        _dbContext = dbContext;
        _tableAPIService = tableAPIService;
        _logger = logger;
    }

    private void Dispose(bool disposing)
    {
        TableAPIService? tableAPIService = _tableAPIService;
        _tableAPIService = null;
        if (tableAPIService is not null && disposing)
            tableAPIService.Dispose();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}