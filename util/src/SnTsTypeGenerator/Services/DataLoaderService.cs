using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

/// <summary>
/// Loads data from the database and from the remote ServiceNow instance.
/// </summary>
public sealed class DataLoaderService : IDisposable
{
    private TypingsDbContext _dbContext;
    private readonly TableAPIService _tableAPIService;
    private readonly ILogger<DataLoaderService> _logger;
    private readonly ReadOnlyDictionary<string, KnownGlideType> _knownGlideTypes;
    private readonly bool _baselineInit;
    private readonly Dictionary<string, string> _tableIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _scopeIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _packageIdMap = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, SncSource> _sourceCache = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _tableAPIService is not null && _tableAPIService.InitSuccessful && (_dbContext?.InitSuccessful ?? false);

    private async Task<SncSource> GetSourceAsync(string fqdn, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_sourceCache.TryGetValue(fqdn, out SncSource? source))
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
        else
            _sourceCache.Add(fqdn, source);
        return source;
    }

    /// <summary>
    /// Gets the table entity representing the <see cref="TS_NAME_BASERECORD"/>.
    /// </summary>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="Table"/> entity representing the <see cref="TS_NAME_BASERECORD"/>.</returns>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">An entity validation error is encountered before saving to the database.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save. This is usually because the data in the database has been modified since it was loaded into memory.</exception>
    internal async Task<Table> GetBaseRecordTypeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_dbContext is null)
            throw new ObjectDisposedException(nameof(DataLoaderService));
        Table? table = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == TS_NAME_BASERECORD && t.IsInterface, cancellationToken);
        if (table is null)
        {
            string sourceFqdn = _tableAPIService.SourceFqdn;
            table = await AddTableAsync(new(
                Name: TS_NAME_BASERECORD,
                Label: TS_NAME_BASERECORD,
                SysID: Guid.Empty.ToString("N"),
                IsExtendable: true,
                NumberPrefix: null,
                Package: null,
                Scope: null,
                SuperClass: null,
                AccessibleFrom: string.Empty,
                ExtensionModel: null,
                SourceFqdn: sourceFqdn), true, cancellationToken);
            _ = await AddElementsAsync(new ElementRecord[] {
                new(
                    Name: JSON_KEY_SYS_ID,
                    Label: "Sys ID",
                    SysID: Guid.Empty.ToString("N"),
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Name: TYPE_NAME_GUID, Label: "Sys ID (GUID)"),
                    MaxLength: 32,
                    IsActive: true,
                    IsUnique: false,
                    IsPrimary: true,
                    IsCalculated: false,
                    SizeClass: null,
                    IsMandatory: false,
                    IsArray: false,
                    Comments: null,
                    IsDisplay: false,
                    DefaultValue: null,
                    Package: null,
                    SourceFqdn: sourceFqdn),
                new(
                    Name: JSON_KEY_SYS_CREATED_BY,
                    Label: "Created by",
                    SysID: "9be67479a3b34cf395f500f3c165a9af",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Name: TYPE_NAME_string, Label: "String"),
                    MaxLength: 40,
                    IsActive: true,
                    IsUnique: false,
                    IsPrimary: false,
                    IsCalculated: false,
                    SizeClass: null,
                    IsMandatory: false,
                    IsArray: false,
                    Comments: null,
                    IsDisplay: false,
                    DefaultValue: null,
                    Package: null,
                    SourceFqdn: sourceFqdn),
                new(
                    Name: JSON_KEY_SYS_CREATED_ON,
                    Label: "Created",
                    SysID: "6bd533127c67405d998d3cb50f44419a",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Name: TYPE_NAME_glide_date_time, Label: "Date/Time"),
                    MaxLength: null,
                    IsActive: true,
                    IsUnique: false,
                    IsPrimary: false,
                    IsCalculated: false,
                    SizeClass: null,
                    IsMandatory: false,
                    IsArray: false,
                    Comments: null,
                    IsDisplay: false,
                    DefaultValue: null,
                    Package: null,
                    SourceFqdn: sourceFqdn),
                new(
                    Name: JSON_KEY_SYS_MOD_COUNT,
                    Label: "Updates",
                    SysID: "75a55d94320c4041a7e4a1e14813de27",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Name: TYPE_NAME_integer, Label: "Integer"),
                    MaxLength: null,
                    IsActive: false,
                    IsUnique: false,
                    IsPrimary: false,
                    IsCalculated: false,
                    SizeClass: null,
                    IsMandatory: false,
                    IsArray: false,
                    Comments: null,
                    IsDisplay: false,
                    DefaultValue: null,
                    Package: null,
                    SourceFqdn: sourceFqdn),
                new(
                    Name: JSON_KEY_SYS_UPDATED_BY,
                    Label: "Updated by",
                    SysID: "ef0b4750753d4f6c82499a605b490af4",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Name: TYPE_NAME_string, Label: "String"),
                    MaxLength: 40,
                    IsActive: true,
                    IsUnique: false,
                    IsPrimary: false,
                    IsCalculated: false,
                    SizeClass: null,
                    IsMandatory: false,
                    IsArray: false,
                    Comments: null,
                    IsDisplay: false,
                    DefaultValue: null,
                    Package: null,
                    SourceFqdn: sourceFqdn),
                new(
                    Name: JSON_KEY_SYS_UPDATED_ON,
                    Label: "Updated",
                    SysID: "3f68a52adc8a4c5a960ec2a9a2bd9fd6",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Name: TYPE_NAME_glide_date_time, Label: "Date/Time"),
                    MaxLength: null,
                    IsActive: true,
                    IsUnique: false,
                    IsPrimary: false,
                    IsCalculated: false,
                    SizeClass: null,
                    IsMandatory: false,
                    IsArray: false,
                    Comments: null,
                    IsDisplay: false,
                    DefaultValue: null,
                    Package: null,
                    SourceFqdn: sourceFqdn)
            }, table, cancellationToken);
        }
        return table;
    }

    private async Task<Element[]> AddElementsAsync(IEnumerable<ElementRecord> elements, Table table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!elements.Any())
            return Array.Empty<Element>();
        var source = (await table.GetReferencedEntityAsync(_dbContext.Tables, t => t.Source, cancellationToken))!;
        _logger.LogAddingElementsToDatabase(table.Name);
        var entities = elements.Select<ElementRecord, (Element Element, PackageRef? Package, TableRef? Reference, TypeRef? Type, string SourceFqdn)>(e =>
        (new Element()
        {
            Name = e.Name,
            Comments = e.Comments,
            DefaultValue = e.DefaultValue,
            IsActive = e.IsActive,
            IsArray = e.IsArray,
            IsCalculated = e.IsCalculated,
            IsDisplay = e.IsDisplay,
            IsMandatory = e.IsMandatory,
            IsPrimary = e.IsPrimary,
            IsReadOnly = e.IsReadOnly,
            IsUnique = e.IsUnique,
            Label = e.Label,
            LastUpdated = DateTime.Now,
            MaxLength = e.MaxLength,
            SizeClass = e.SizeClass,
            SysID = e.SysID,
            Table = table,
            Source = source
        }, e.Package, e.Reference, e.Type, e.SourceFqdn));
        foreach (var g in entities.GroupBy(a => a.SourceFqdn))
        {
            var s = await GetSourceAsync(g.Key, cancellationToken);
            foreach (var a in g)
                a.Element.Source = s;
        }
        foreach (var g in entities.Where(a => a.Package is not null).GroupBy(a => a.Package!.SysID))
        {
            var e = g.First();
            var p = await FromPackageRefAsync(e.Package, e.Element.Source!, cancellationToken);
            if (p is not null)
                foreach (var a in g)
                    a.Element.Package = p;
        }
        foreach (var g in entities.Where(a => a.Type is not null).GroupBy(a => a.Type!.Name))
        {
            var e = g.First();
            var t = await FromGlideTypeRefAsync(e.Type!, e.Element.Source!, cancellationToken);
            if (t is not null)
                foreach (var a in g)
                    a.Element.Type = t;
        }
        foreach (var g in entities.Where(a => a.Reference is not null).GroupBy(a => a.Reference!.Name))
        {
            var e = g.First();
            var t = await FromTableRefAsync(e.Reference!, cancellationToken);
            if (t is not null)
                foreach (var a in g)
                    a.Element.Reference = t;
        }
        var result = entities.Select(e => e.Element).ToArray();
        await _dbContext.Elements.AddRangeAsync(result, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task<Table> AddTableAsync(TableRecord @record, CancellationToken cancellationToken) => await AddTableAsync(@record, false, cancellationToken);

    private async Task<Table> AddTableAsync(TableRecord @record, bool isInterface, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SncSource source = await GetSourceAsync(@record.SourceFqdn, cancellationToken);
        Table table = new()
        {
            Name = @record.Name,
            AccessibleFrom = @record.AccessibleFrom,
            ExtensionModel = @record.ExtensionModel,
            IsExtendable = @record.IsExtendable,
            IsInterface = isInterface,
            Label = @record.Label,
            LastUpdated = DateTime.Now,
            NumberPrefix = @record.NumberPrefix,
            Source = source,
            SysID = @record.SysID,
            Package = await FromPackageRefAsync(@record.Package, source, cancellationToken),
            Scope = await FromScopeRefAsync(@record.Scope, source, cancellationToken)
        };
        _logger.LogAddingTableToDb(table.Name);
        await _dbContext.Tables.AddAsync(table, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _tableIdMap.Add(table.SysID, table.Name);
        _logger.LogNewTableSaveCompleted(table.Name);
        Table? superClass = (@record.SuperClass is null) ? null : await FromSuperClassRefAsync(@record.SuperClass, cancellationToken);
        IEnumerable<Element> superElements;
        var elements = (await _tableAPIService.GetElementsByTableNameAsync(@record.Name, cancellationToken)).ToArray();
        if (superClass is null)
        {
            if (elements.ExtendsBaseRecord())
            {
                superClass = await GetBaseRecordTypeAsync(cancellationToken);
                table.SuperClass = superClass;
                _dbContext.Tables.Update(table);
                await _dbContext.SaveChangesAsync(cancellationToken);
                superElements = await superClass.GetRelatedCollectionAsync(_dbContext.Tables, e => e.Elements, cancellationToken);
            }
            else
                return table;
        }
        else
        {
            table.SuperClass = superClass;
            _dbContext.Tables.Update(table);
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (elements.Length == 0)
                return table;
            var entry = _dbContext.Tables.Entry(superClass);
            var inh = await entry.GetAllElementInheritancesAsync(cancellationToken);
            if (!(superElements = inh.Select(e => e.Element)).Any())
            {
                await AddElementsAsync(elements, table, cancellationToken);
                return table;
            }
        }

        await AddElementsAsync(elements.Where(e => !superElements.Any(s => e.IsIdenticalTo(s))), table, cancellationToken);

        return table;
    }

    private async Task<GlideType> FromGlideTypeRefAsync(TypeRef typeRef, SncSource source, CancellationToken cancellationToken)
    {
        string name = typeRef.Name;
        GlideType? type = await _dbContext.Types.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (type is null)
        {
            var typeRecord = await _tableAPIService.GetGlideTypeByNameAsync(name, cancellationToken);
            if (typeRecord is not null)
            {
                source = await GetSourceAsync(typeRecord.SourceFqdn, cancellationToken);
                type = new()
                {
                    Name = name,
                    Label = typeRecord.Label,
                    ScalarType = typeRecord.ScalarType,
                    ScalarLength = typeRecord.ScalarLength,
                    ClassName = typeRecord.ClassName,
                    UseOriginalValue = typeRecord.UseOriginalValue,
                    IsVisible = typeRecord.IsVisible,
                    LastUpdated = DateTime.Now,
                    SysID = typeRecord.SysID,
                    Source = source,
                    Package = await FromPackageRefAsync(typeRecord.Package, source, cancellationToken),
                    Scope = await FromScopeRefAsync(typeRecord.Scope, source, cancellationToken)
                };
                if (_knownGlideTypes.TryGetValue(name, out KnownGlideType? knownGlideType))
                {
                    if (!string.IsNullOrWhiteSpace(knownGlideType.GlobalElementType))
                        type.GlobalElementType = knownGlideType.GlobalElementType;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.ScopedElementType))
                        type.ScopedElementType = knownGlideType.ScopedElementType;
                }
            }
            else
            {
                type = new()
                {
                    Name = name,
                    Label = typeRef.Label,
                    LastUpdated = DateTime.Now,
                    Source = source
                };
                if (_knownGlideTypes.TryGetValue(name, out KnownGlideType? knownGlideType))
                {
                    if (string.IsNullOrWhiteSpace(typeRef.Label))
                        type.Label = string.IsNullOrWhiteSpace(knownGlideType.Label) ? name : knownGlideType.Label;
                    type.ScalarLength = knownGlideType.ScalarLength;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.ScalarType))
                        type.ScalarType = knownGlideType.ScalarType;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.GlobalElementType))
                        type.GlobalElementType = knownGlideType.GlobalElementType;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.ScopedElementType))
                        type.ScopedElementType = knownGlideType.ScopedElementType;
                }
                else if (string.IsNullOrWhiteSpace(typeRef.Label))
                    type.Label = name;
            }
            await _dbContext.Types.AddAsync(type, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return type;
    }

    private async Task<Table?> FromTableRefAsync(TableRef? tableRef, CancellationToken cancellationToken)
    {
        if (tableRef is null)
            return null;
        string name = tableRef.Name;
        var table = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (table is not null)
            return table;
        var tableRecord = await _tableAPIService.GetTableByNameAsync(name, cancellationToken);
        tableRecord ??= new(
                Name: name,
                Label: tableRef.Label,
                SysID: Guid.NewGuid().ToString("N"),
                IsExtendable: true,
                NumberPrefix: null,
                Package: null,
                Scope: null,
                SuperClass: null,
                AccessibleFrom: string.Empty,
                ExtensionModel: null,
                SourceFqdn: _tableAPIService.SourceFqdn);
        return await AddTableAsync(tableRecord, cancellationToken);
    }

    private async Task<Table?> FromSuperClassRefAsync(SuperClassRef? tableRef, CancellationToken cancellationToken)
    {
        if (tableRef is null)
            return null;
        string sys_id = tableRef.SysID;
        if (_tableIdMap.TryGetValue(sys_id, out string? name))
            return await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        var tableRecord = await _tableAPIService.GetTableByIdAsync(sys_id, cancellationToken);
        if (tableRecord is null) // TODO: Add warning for table not found
            return null;
        return await AddTableAsync(tableRecord, cancellationToken);
    }

    // BUG: Package groups can have more than one package name to them.
    private async Task<PackageGroup> GetPackageGroupAsync(string packageName, string @namespace, bool activeAndNotLicensable, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(packageName)) packageName = GLOBAL_NAMESPACE;
        if (string.IsNullOrWhiteSpace(@namespace)) @namespace = GLOBAL_NAMESPACE;
        var packageGroup = await _dbContext.PackageGroups.FirstOrDefaultAsync(g => g.PackageName == packageName && g.Namespace == @namespace, cancellationToken);
        if (packageGroup is not null) return packageGroup;
        string fileName;
        fileName = NameComparer.Equals(packageName, @namespace) ? packageName : $"{packageName}-{@namespace}";
        
        if (ReservedBaseNames.Contains(fileName, NameComparer) || (await _dbContext.PackageGroups.FirstOrDefaultAsync(g => g.PackageName == packageName && g.Namespace == @namespace, cancellationToken)) is not null)
        {
            string baseName = fileName;
            int pos = 1;
            fileName = $"{fileName}-{pos}";
            while ((await _dbContext.PackageGroups.FirstOrDefaultAsync(g => g.PackageName == packageName && g.Namespace == @namespace, cancellationToken)) is not null)
            {
                pos++;
                fileName = $"{fileName}-{pos}";
            }
        }

        packageGroup = new()
        {
            PackageName = packageName,
            Namespace = @namespace,
            IsBaseline = activeAndNotLicensable && _baselineInit
        };
        throw new NotImplementedException();
    }

    private async Task<Package?> FromPackageRefAsync(PackageRef? pkgRef, SncSource source, CancellationToken cancellationToken)
    {
        if (pkgRef is null)
            return null;
        string sys_id = pkgRef.SysID;
        if (_packageIdMap.TryGetValue(sys_id, out string? id))
            return await _dbContext.Packages.FirstOrDefaultAsync(p => p.ID == id, cancellationToken);
        var packageRecord = await _tableAPIService.GetPackageByIDAsync(sys_id, cancellationToken);
        Package? package;
        string pkgName;
        if (packageRecord is null)
        {
            if (_scopeIdMap.TryGetValue(sys_id, out id))
            {
                Scope? scope = await _dbContext.Scopes.FirstOrDefaultAsync(p => p.Value == id, cancellationToken);
                if (scope is null)
                    return null;
                id = scope.ID;
                pkgName = pkgRef.Name.AsNonEmpty(scope.Value);
                if ((package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.ID == id, cancellationToken)) is null)
                {
                    package = new()
                    {
                        ID = id,
                        Name = scope.Name,
                        Version = scope.Version,
                        SysID = sys_id,
                        LastUpdated = DateTime.Now,
                        Source = source,
                        Group = await GetPackageGroupAsync(pkgName, scope.Name, false, cancellationToken)
                    };
                    await _dbContext.Packages.AddAsync(package, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                var name = pkgRef.Name.AsNonEmpty(GLOBAL_NAMESPACE);
                if (string.IsNullOrWhiteSpace(pkgRef.Name))
                {
                    pkgName = GLOBAL_NAMESPACE;
                    package = new()
                    {
                        ID = pkgName,
                        Name = pkgName,
                        SysID = sys_id,
                        LastUpdated = DateTime.Now,
                        Source = source,
                        Group = await GetPackageGroupAsync(pkgName, pkgName, false, cancellationToken)
                    };
                    await _dbContext.Packages.AddAsync(package, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else if ((package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Name == name, cancellationToken)) is null)
                    return null;
                id = package.ID;
            }
        }
        else
        {
            id = packageRecord.ID;
            package = new()
            {
                ID = id,
                Name = packageRecord.Name,
                Version = string.Empty,
                SysID = sys_id,
                LastUpdated = DateTime.Now,
                Source = source,
                // BUG: Need to re-do logic for getting scope and package
                Group = await GetPackageGroupAsync(packageRecord.Name, packageRecord.Name, packageRecord.Active && !packageRecord.Licensable, cancellationToken)
            };
            // if (packageRecord is ScopeRecord scopeRecord)
            // {

            // }
            // else
            // {

            // }
            await _dbContext.Packages.AddAsync(package, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            // if (packageRecord is PluginRecord pluginRecord && pluginRecord.ParentID is not null)
            // {
            // TODO: Ensure parent is in DB and set parent
            // }
        }
        _packageIdMap.Add(sys_id, id);
        return package;
    }

    private async Task<Scope?> FromScopeRefAsync(ScopeRef? scopeRef, SncSource source, CancellationToken cancellationToken)
    {
        if (scopeRef is null)
            return null;
        var sys_id = scopeRef.ID;
        if (_scopeIdMap.TryGetValue(sys_id, out string? value))
            return await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken);
        var scopeRecord = await _tableAPIService.GetScopeByIDAsync(sys_id, cancellationToken);
        Scope? scope;
        if (scopeRecord is null)
        {
            if ((value = scopeRef.Name) == null || (scope = await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Name == value, cancellationToken)) is null)
                return null; // TODO: Warn not found
        }
        else
        {
            value = scopeRecord.Value;
            if ((scope = await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken)) is null)
            {
                scope = new()
                {
                    Name = scopeRecord.Name,
                    Value = value,
                    LastUpdated = DateTime.Now,
                    SysID = scopeRecord.SysID,
                    Source = source,
                    ShortDescription = scopeRecord.ShortDescription
                };
                await _dbContext.Scopes.AddAsync(scope, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        _scopeIdMap.Add(sys_id, value);
        return scope;
    }

    /// <summary>
    /// Loads information for the table that matches the specified name.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The <see cref="Table"/> record that matches the specified <paramref name="name"/> or <see langword="null" /> if no table was found in the database or in the remote ServiceNow instance.</returns>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">An entity validation error is encountered before saving to the database.</exception>
    /// <exception cref="DbUpdateException">An error is encountered while saving to the database.</exception>
    /// <exception cref="DbUpdateConcurrencyException">A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save. This is usually because the data in the database has been modified since it was loaded into memory.</exception>
    internal async Task<Table?> GetTableByNameAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_dbContext is null)
            throw new ObjectDisposedException(nameof(DataLoaderService));
        Table? table = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (table is null)
            return await _logger.WithActivityScope(LogActivityType.AddNewTable, name, async () =>
            {
                var response = await _tableAPIService.GetTableByNameAsync(name, cancellationToken);
                if (response is not null)
                    table = await AddTableAsync(response, cancellationToken);
                return table;
            });
        return table;
    }

    internal async Task<IEnumerable<Table>> LoadAllReferencedAsync(IEnumerable<Table> tables, CancellationToken cancellationToken)
    {
        TypingsDbContext? dbContext = _dbContext ?? throw new ObjectDisposedException(nameof(DataLoaderService));
        return await dbContext.LoadAllReferencedAsync(tables, cancellationToken);
    }

    public DataLoaderService(TypingsDbContext dbContext, TableAPIService tableAPIService, IOptions<AppSettings> appSettingsOptions, ILogger<DataLoaderService> logger)
    {
        _dbContext = dbContext;
        _tableAPIService = tableAPIService;
        _logger = logger;
        var appSettings = appSettingsOptions.Value;
        _knownGlideTypes = appSettings.GetKnownGlideTypes();
        _baselineInit = appSettings.BaselineInit ?? false;
    }

    private void Dispose(bool disposing)
    {
        TypingsDbContext? dbContext = _dbContext;
        _dbContext = null!;
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