using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Models;
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
        SysScope? retrieved = await _tableAPIService!.GetScopeByIDAsync(scope.SysID, cancellationToken);
        if (retrieved is null)
            scope.Value = scope.SysID;
        else
        {
            value = (scope = retrieved).Value;
            if ((retrieved = await _dbContext!.Scopes.FirstOrDefaultAsync(t => t.Value == value, cancellationToken)) is not null)
            {
                _scopeIdMap.Add(scope.SysID, value);
                return retrieved;
            }
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
        await _dbContext!.Types.AddAsync(type, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await _dbContext!.SaveChangesAsync(cancellationToken);
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
        await _dbContext!.Tables.AddAsync(tableInfo, cancellationToken);
        _tableIdMap.Add(tableInfo.SysID, tableInfo.Name);
        await _dbContext!.SaveChangesAsync(cancellationToken);

        if (superClass is not null && (tableInfo.SuperClass = superClass = await GetTableAsync(superClass, cancellationToken)) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _dbContext!.SaveChangesAsync(cancellationToken);
        }
        cancellationToken.ThrowIfCancellationRequested();

        ElementInfo[] elements = await _tableAPIService!.GetElementsByTableNameAsync(tableInfo.Name, cancellationToken);
        if (elements.Length == 0)
        {
            _logger.LogNewTableSaveCompleteTrace(tableInfo.Name);
            return;
        }

        if (superClass is null && elements.ExtendsBaseRecord())
            tableInfo.SuperClass = superClass = await GetBaseRecordTypeAsync(cancellationToken);

        if (superClass is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var super = await _dbContext.Tables.Entry(superClass).GetAllElementsAsync(cancellationToken);
            if ((elements = elements.Where(e =>
            {
                var n = e.Name;
                var se = super.FirstOrDefault(s => s.Name == n);
                if (se is null || !e.IsIdenticalTo(se))
                {
                    e.Table = tableInfo;
                    e.Source = source;
                    return true;
                }
                return false;
            }).ToArray()).Length == 0)
            {
                _logger.LogNewTableSaveCompleteTrace(tableInfo.Name);
                return;
            }
        }
        else
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
        await _dbContext!.Elements.AddRangeAsync(elements, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await _dbContext!.SaveChangesAsync(cancellationToken);
        _logger.LogNewTableSaveCompleteTrace(tableInfo.Name);
    }

    private async Task<TableInfo> GetTableAsync(TableInfo table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_tableIdMap.TryGetValue(table.SysID, out string? name))
            return await _dbContext!.Tables.FirstAsync(t => t.Name == name, cancellationToken);
        name = table.Name;
        TableInfo? result = await _dbContext!.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (result is not null)
            _tableIdMap.Add(table.SysID, name);
        else
        {
            if ((result = await _tableAPIService!.GetTableByIdAsync(table.SysID, cancellationToken)) is not null)
                table = result;
            cancellationToken.ThrowIfCancellationRequested();
            await SaveTableAsync(table, cancellationToken);
        }
        return table;
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
            tableInfo = new()
            {
                SysID = "00000000000000000000000000000000",
                IsInterface = true,
                Label = "",
                LastUpdated = DateTime.Now,
                Name = TS_NAME_BASERECORD,
                SourceFqdn = sourceFqdn
            };
            await _dbContext.Tables.AddAsync(tableInfo, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Elements.AddAsync(new ElementInfo()
            {
                IsActive = true,
                IsPrimary = true,
                Label = "Sys ID",
                MaxLength = 32,
                Name = JSON_KEY_SYS_ID,
                SysID = "00000000000000000000000000000000",
                TypeName = TYPE_NAME_GUID,
                Table = tableInfo,
                SourceFqdn = sourceFqdn,
                LastUpdated = DateTime.Now
            }, cancellationToken);
            await _dbContext.Elements.AddAsync(new ElementInfo()
            {
                IsActive = true,
                Label = "Created by",
                MaxLength = 40,
                Name = JSON_KEY_SYS_CREATED_BY,
                SysID = "9be67479a3b34cf395f500f3c165a9af",
                TypeName = TYPE_NAME_string,
                Table = tableInfo,
                SourceFqdn = sourceFqdn,
                LastUpdated = DateTime.Now
            }, cancellationToken);
            await _dbContext.Elements.AddAsync(new ElementInfo()
            {
                IsActive = true,
                Label = "Created",
                MaxLength = 40,
                Name = JSON_KEY_SYS_CREATED_ON,
                SysID = "6bd533127c67405d998d3cb50f44419a",
                TypeName = TYPE_NAME_glide_date_time,
                Table = tableInfo,
                SourceFqdn = sourceFqdn,
                LastUpdated = DateTime.Now
            }, cancellationToken);
            await _dbContext.Elements.AddAsync(new ElementInfo()
            {
                IsActive = true,
                Label = "Updates",
                MaxLength = 40,
                Name = JSON_KEY_SYS_MOD_COUNT,
                SysID = "75a55d94320c4041a7e4a1e14813de27",
                TypeName = TYPE_NAME_integer,
                Table = tableInfo,
                SourceFqdn = sourceFqdn,
                LastUpdated = DateTime.Now
            }, cancellationToken);
            await _dbContext.Elements.AddAsync(new ElementInfo()
            {
                IsActive = true,
                Label = "Updated by",
                MaxLength = 40,
                Name = JSON_KEY_SYS_UPDATED_BY,
                SysID = "ef0b4750753d4f6c82499a605b490af4",
                TypeName = TYPE_NAME_string,
                Table = tableInfo,
                SourceFqdn = sourceFqdn,
                LastUpdated = DateTime.Now
            }, cancellationToken);
            await _dbContext.Elements.AddAsync(new ElementInfo()
            {
                IsActive = true,
                Label = "Updated",
                MaxLength = 40,
                Name = JSON_KEY_SYS_UPDATED_ON,
                SysID = "3f68a52adc8a4c5a960ec2a9a2bd9fd6",
                TypeName = TYPE_NAME_glide_date_time,
                Table = tableInfo,
                SourceFqdn = sourceFqdn,
                LastUpdated = DateTime.Now
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return tableInfo;
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