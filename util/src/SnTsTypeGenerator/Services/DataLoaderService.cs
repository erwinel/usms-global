using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Models;
using SnTsTypeGenerator.Models.TableAPI;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

/// <summary>
/// Loads data from the database and from the remote ServiceNow instance.
/// </summary>
public sealed class DataLoaderService : IDisposable
{
    private TypingsDbContext? _dbContext;
    private readonly TableAPIService _tableAPIService;
    private readonly ILogger<DataLoaderService> _logger;
    private readonly Dictionary<string, string> _scopeIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _tableIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _packageIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, SourceInfo> _sourceCache = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _tableAPIService is not null && _tableAPIService.InitSuccessful && (_dbContext?.InitSuccessful ?? false);

    private async Task<SourceInfo> GetSourceAsync(string fqdn, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_sourceCache.TryGetValue(fqdn, out SourceInfo? source))
            return source;
        if ((source = await _dbContext!.Sources.FirstOrDefaultAsync(s => s.FQDN == fqdn, cancellationToken)) is null)
        {
            source = new()
            {
                FQDN = fqdn,
                Label = fqdn,
                IsPersonalDev = false,
                LastAccessed = DateTime.Now
            };
            await _dbContext!.Sources.AddAsync(source, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            _sourceCache.Add(fqdn, source);
            await _dbContext!.SaveChangesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }
        else
            _sourceCache.Add(fqdn, source);
        return source;
    }

    private async Task<SysPackage> GetPackageAsync(SysPackage package, CancellationToken cancellationToken)
    {
        if (_packageIdMap.TryGetValue(package.SysId, out string? name))
            return await _dbContext!.Packages.FirstAsync(t => t.Name == name, cancellationToken);
        name = package.Name;
        SysPackage? result = await _dbContext!.Packages.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        if (result is not null)
        {
            _packageIdMap.Add(package.SysId, name);
            return package;
        }
        package.Source = await GetSourceAsync(package.SourceFqdn, cancellationToken);
        package.LastUpdated = DateTime.Now;
        await _dbContext!.Packages.AddAsync(package, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        _packageIdMap.Add(package.SysId, package.Name);
        await _dbContext!.SaveChangesAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return package;
    }

    private async Task<SysScope> GetScopeAsync(SysScope scope, CancellationToken cancellationToken)
    {
        if (_scopeIdMap.TryGetValue(scope.SysID, out string? value))
            return await _dbContext!.Scopes.FirstAsync(t => t.Value == value, cancellationToken);
        var retrieved = await _tableAPIService!.GetScopeByIDAsync(scope.SysID, cancellationToken);
        if (retrieved is null)
            scope.Value = scope.SysID;
        else
        {
            var existing = await retrieved.ToDbEntityAsync(_dbContext, _scopeIdMap, cancellationToken);
            if (existing is not null)
                return existing;
            value = retrieved.Value;
        }
        scope.Source = await GetSourceAsync(scope.SourceFqdn, cancellationToken);
        scope.LastUpdated = DateTime.Now;
        await _dbContext!.Scopes.AddAsync(scope, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        _scopeIdMap.Add(scope.SysID, scope.Value);
        await _dbContext!.SaveChangesAsync(cancellationToken);
        return scope;
    }

    private async Task<GlideType> GetGlideTypeAsync(GlideType type, CancellationToken cancellationToken)
    {
        string name = type.Name;
        GlideType? result = await _dbContext!.Types.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (result is not null)
            return result;
        var response = await _tableAPIService!.GetGlideTypeByNameAsync(name, cancellationToken);
        if (response is not null)
        {
            type = await response.ToDbEntityAsync(_dbContext, _scopeIdMap, _packageIdMap, cancellationToken);
        }

        type.Source = await GetSourceAsync(type.SourceFqdn, cancellationToken);
        type.LastUpdated = DateTime.Now;
        await _dbContext!.Types.AddAsync(type, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await _dbContext!.SaveChangesAsync(cancellationToken);
        return type;
    }

    private async Task SaveTableAsync(Table table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // _logger.LogAddingTableToDb(table.Name);
        // if (elements.Length == 0)
        // {
        //     _logger.LogNewTableSaveCompleteTrace(table.Name);
        //     return;
        // }

        // if (superClass is not null)
        // {
        //     cancellationToken.ThrowIfCancellationRequested();

        //     var super = await _dbContext.Tables.Entry(superClass).GetAllElementsAsync(cancellationToken);
        //     if ((elements = elements.Where(e =>
        //     {
        //         var n = e.Name;
        //         var se = super.FirstOrDefault(s => s.Name == n);
        //         return se is null || !e.IsIdenticalTo(se);
        //     }).ToArray()).Length == 0)
        //     {
        //         _logger.LogNewTableSaveCompleteTrace(table.Name);
        //         return;
        //     }
        // }

        // cancellationToken.ThrowIfCancellationRequested();
        // _logger.LogAddingElementsToDatabase(table.Name);
        // await _dbContext!.Elements.AddRangeAsync(elements, cancellationToken);
        // cancellationToken.ThrowIfCancellationRequested();
        // await _dbContext!.SaveChangesAsync(cancellationToken);
        // _logger.LogNewTableSaveCompleteTrace(table.Name);
    }

    internal async Task<TableInfo> GetBaseRecordTypeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_dbContext is null)
            throw new ObjectDisposedException(nameof(DataLoaderService));
        TableInfo? tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == TS_NAME_BASERECORD && t.IsInterface, cancellationToken);
        if (tableInfo is null)
        {
            string sourceFqdn = _tableAPIService.SourceFqdn;
            tableInfo = await AddTableAsync(new(TS_NAME_BASERECORD, TS_NAME_BASERECORD, Guid.Empty.ToString("N"), true, null, null, null, null, string.Empty, null, sourceFqdn), cancellationToken);
            await AddElementsAsync(new Element[] {
                new(JSON_KEY_SYS_ID, "Sys ID", Guid.Empty.ToString("N"), null, false, new GlideTypeRef(TYPE_NAME_GUID, "Sys ID (GUID)", sourceFqdn), 32, true, false, true, false, null, false, false, null, false, null, null, sourceFqdn),
                new(JSON_KEY_SYS_CREATED_BY, "Created by", "9be67479a3b34cf395f500f3c165a9af", null, false, new GlideTypeRef(TYPE_NAME_string, "String", sourceFqdn), 40, true, false,
                    false, false, null, false, false, null, false, null, null, sourceFqdn),
                new(JSON_KEY_SYS_CREATED_ON, "Created", "6bd533127c67405d998d3cb50f44419a", null, false, new GlideTypeRef(TYPE_NAME_glide_date_time, "Date/Time", sourceFqdn), null, true,
                    false, false, false, null, false, false, null, false, null, null, sourceFqdn),
                new(JSON_KEY_SYS_MOD_COUNT, "Updates", "75a55d94320c4041a7e4a1e14813de27", null, false, new GlideTypeRef(TYPE_NAME_integer, "Integer", sourceFqdn), null, false, false,
                    false, false, null, false, false, null, false, null, null, sourceFqdn),
                new(JSON_KEY_SYS_CREATED_BY, "Updated by", "ef0b4750753d4f6c82499a605b490af4", null, false, new GlideTypeRef(TYPE_NAME_string, "String", sourceFqdn), 40, true, false,
                    false, false, null, false, false, null, false, null, null, sourceFqdn),
                new(JSON_KEY_SYS_CREATED_ON, "Updated", "3f68a52adc8a4c5a960ec2a9a2bd9fd6", null, false, new GlideTypeRef(TYPE_NAME_glide_date_time, "Date/Time", sourceFqdn), null, true,
                    false, false, false, null, false, false, null, false, null, null, sourceFqdn)
            }, tableInfo, cancellationToken);
        }
        return tableInfo;
    }

    private async Task<IEnumerable<ElementInfo>> AddElementsAsync(IEnumerable<Element> elements, TableInfo table, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<TableInfo> AddTableAsync(Table table, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
        if (_dbContext is null)
            throw new ObjectDisposedException(nameof(DataLoaderService));
        TableInfo? tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (tableInfo is null)
        {
            var response = await _tableAPIService.GetTableByNameAsync(name, cancellationToken);
            if (response is not null)
                tableInfo = await AddTableAsync(response, cancellationToken);
        }
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
        TypingsDbContext? dbContext = _dbContext;
        _dbContext = null;
        if (disposing && dbContext is not null)
            dbContext.Dispose();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}