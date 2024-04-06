using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    private readonly RemoteUriService _remoteUri;
    private readonly ILogger<DataLoaderService> _logger;
    private readonly object _syncRoot = new();
    private SncSource? _currentSourceEntity;
    private EntityEntry<SncSource>? _currentSourceEntry;
    private readonly ReadOnlyDictionary<string, KnownGlideType> _knownGlideTypes;
    private readonly bool _baselineInit;
    private readonly Dictionary<string, string> _tableIdMap = new(NameComparer);
    private readonly Dictionary<string, string> _scopeIdMap = new(NameComparer);
    private readonly Dictionary<string, string> _packageIdMap = new(NameComparer);
    private readonly Dictionary<string, SncSource> _sourceCache = new(NameComparer);
    private readonly Dictionary<string, string> _packageGroupMap = new(NameComparer);
    private readonly ImmutableArray<string> _defaultBaselinePackageGroups;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => (_remoteUri?.InitSuccessful ?? false) && (_tableAPIService?.InitSuccessful ?? false) && (_dbContext?.InitSuccessful ?? false);

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
            var source = await EnsureCurrentSourceAsync(cancellationToken);
            string sourceFqdn = source.FQDN;
            table = new()
            {
                Name = TS_NAME_BASERECORD,
                Label = TS_NAME_BASERECORD,
                SysID = Guid.Empty.ToString("N"),
                IsExtendable = true,
                IsInterface = true,
                Source = source
            };
            await AddTableAsync(table, null, cancellationToken);
            _ = await AddElementsAsync(new RemoteDictionaryEntry[] {
                new(
                    Name: JSON_KEY_SYS_ID,
                    Label: "Sys ID",
                    SysID: Guid.Empty.ToString("N"),
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(TYPE_NAME_GUID, "Sys ID (GUID)"),
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
                    Scope: null),
                new(
                    Name: JSON_KEY_SYS_CREATED_BY,
                    Label: "Created by",
                    SysID: "9be67479a3b34cf395f500f3c165a9af",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(TYPE_NAME_string, "String"),
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
                    Scope: null),
                new(
                    Name: JSON_KEY_SYS_CREATED_ON,
                    Label: "Created",
                    SysID: "6bd533127c67405d998d3cb50f44419a",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(TYPE_NAME_glide_date_time, "Date/Time"),
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
                    Scope: null),
                new(
                    Name: JSON_KEY_SYS_MOD_COUNT,
                    Label: "Updates",
                    SysID: "75a55d94320c4041a7e4a1e14813de27",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(TYPE_NAME_integer, "Integer"),
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
                    Scope: null),
                new(
                    Name: JSON_KEY_SYS_UPDATED_BY,
                    Label: "Updated by",
                    SysID: "ef0b4750753d4f6c82499a605b490af4",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(TYPE_NAME_string, "String"),
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
                    Scope: null),
                new(
                    Name: JSON_KEY_SYS_UPDATED_ON,
                    Label: "Updated",
                    SysID: "3f68a52adc8a4c5a960ec2a9a2bd9fd6",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(TYPE_NAME_glide_date_time, "Date/Time"),
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
                    Scope: null)
            }, table, cancellationToken);
        }
        return table;
    }

    private async Task<Element[]> AddElementsAsync(IEnumerable<RemoteDictionaryEntry> elements, Table table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!elements.Any())
            return Array.Empty<Element>();
        var source = (await table.GetReferencedEntityAsync(_dbContext.Tables, t => t.Source, cancellationToken))!;
        _logger.LogAddingElementsToDatabase(table.Name);
        var entities = elements.Select<RemoteDictionaryEntry, (Element Element, RemoteRef? Package, RemoteRef? Reference, RemoteRef? Type, string SourceFqdn)>(e =>
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
        }, e.Package, e.Reference, e.Type, source.FQDN));
        foreach (var g in entities.GroupBy(a => a.SourceFqdn))
        {
            var s = await GetSourceAsync(g.Key, cancellationToken);
            foreach (var a in g)
                a.Element.Source = s;
        }
        foreach (var g in entities.Where(a => a.Package is not null).GroupBy(a => a.Package!.Value))
        {
            var e = g.First();
            var p = await FromPackageRefAsync(e.Package, e.Element.Source!, cancellationToken);
            if (p is not null)
                foreach (var a in g)
                    a.Element.Package = p;
        }
        foreach (var g in entities.Where(a => a.Type is not null).GroupBy(a => a.Type!.Value))
        {
            var e = g.First();
            var t = await FromFieldClassRefAsync(e.Type!, e.Element.Source!, cancellationToken);
            if (t is not null)
                foreach (var a in g)
                    a.Element.Type = t;
        }
        foreach (var g in entities.Where(a => a.Reference is not null).GroupBy(a => a.Reference!.Value))
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

    private async Task<Table> AddTableAsync(RemoteTable remoteTable, CancellationToken cancellationToken) => await AddTableAsync(remoteTable, false, cancellationToken);

    private async Task AddTableAsync(Table table, RemoteRef? superClassRef, CancellationToken cancellationToken)
    {
        _logger.LogAddingTableToDb(table.Name);
        await _dbContext.Tables.AddAsync(table, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _tableIdMap.Add(table.SysID, table.Name);
        _logger.LogNewTableSaveCompleted(table.Name);
        Table? superClass = (superClassRef is null) ? null : await FromSuperClassRefAsync(superClassRef, cancellationToken);
        IEnumerable<Element> superElements;
        var elements = (await _tableAPIService.GetDictionaryEntryRecordsByTableNameAsync(table.Name, cancellationToken)).ToArray();
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
                return;
        }
        else
        {
            table.SuperClass = superClass;
            _dbContext.Tables.Update(table);
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (elements.Length == 0)
                return;
            var entry = _dbContext.Tables.Entry(superClass);
            var inh = await entry.GetAllElementInheritancesAsync(cancellationToken);
            if (!(superElements = inh.Select(e => e.Element)).Any())
            {
                await AddElementsAsync(elements, table, cancellationToken);
                return;
            }
        }

        await AddElementsAsync(elements.Where(e => !superElements.Any(s => e.IsIdenticalTo(s))), table, cancellationToken);

    }

    private async Task<Table> AddTableAsync(RemoteTable remoteTable, bool isInterface, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SncSource source = await EnsureCurrentSourceAsync(cancellationToken);
        Table table = new()
        {
            Name = remoteTable.Name,
            AccessibleFrom = remoteTable.AccessibleFrom,
            ExtensionModel = remoteTable.ExtensionModel,
            IsExtendable = remoteTable.IsExtendable,
            IsInterface = isInterface,
            Label = remoteTable.Label,
            LastUpdated = DateTime.Now,
            NumberPrefix = remoteTable.NumberPrefix,
            Source = source,
            SysID = remoteTable.SysID,
            Package = await FromPackageRefAsync(remoteTable.Package, source, cancellationToken),
            Scope = await FromScopeRefAsync(remoteTable.Scope, source, cancellationToken)
        };

        await AddTableAsync(table, remoteTable.SuperClass, cancellationToken);
        return table;
    }

    private async Task<Table> AddTableFromRemoteRecordAsync(RemoteTable remoteTable, bool isInterface, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SncSource source = await EnsureCurrentSourceAsync(cancellationToken);
        Table table = new()
        {
            Name = remoteTable.Name,
            AccessibleFrom = remoteTable.AccessibleFrom,
            ExtensionModel = remoteTable.ExtensionModel,
            IsExtendable = remoteTable.IsExtendable,
            IsInterface = isInterface,
            Label = remoteTable.Label,
            LastUpdated = DateTime.Now,
            NumberPrefix = remoteTable.NumberPrefix,
            Source = source,
            SysID = remoteTable.SysID,
            Package = await FromPackageRefAsync(remoteTable.Package, source, cancellationToken),
            Scope = await FromScopeRefAsync(remoteTable.Scope, source, cancellationToken)
        };
        _logger.LogAddingTableToDb(table.Name);
        await _dbContext.Tables.AddAsync(table, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _tableIdMap.Add(table.SysID, table.Name);
        _logger.LogNewTableSaveCompleted(table.Name);
        Table? superClass = (remoteTable.SuperClass is null) ? null : await FromSuperClassRefAsync(remoteTable.SuperClass, cancellationToken);
        IEnumerable<Element> superElements;
        var elements = (await _tableAPIService.GetDictionaryEntryRecordsByTableNameAsync(remoteTable.Name, cancellationToken)).ToArray();
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

    private async Task<GlideType> FromFieldClassRefAsync(RemoteRef remoteRef, SncSource source, CancellationToken cancellationToken)
    {
        string name = remoteRef.Value;
        GlideType? type = await _dbContext.Types.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (type is null)
        {
            var fieldClass = await _tableAPIService.GetFieldClassRecordByNameAsync(name, cancellationToken);
            if (fieldClass is not null)
            {
                type = new()
                {
                    Name = name,
                    Label = fieldClass.Label,
                    ScalarType = fieldClass.ScalarType,
                    ScalarLength = fieldClass.ScalarLength,
                    ClassName = fieldClass.ClassName,
                    UseOriginalValue = fieldClass.UseOriginalValue,
                    IsVisible = fieldClass.IsVisible,
                    LastUpdated = DateTime.Now,
                    SysID = fieldClass.SysID,
                    Source = source,
                    Package = await FromPackageRefAsync(fieldClass.Package, source, cancellationToken),
                    Scope = await FromScopeRefAsync(fieldClass.Scope, source, cancellationToken)
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
                    Label = remoteRef.Display ?? name,
                    LastUpdated = DateTime.Now,
                    Source = source
                };
                if (_knownGlideTypes.TryGetValue(name, out KnownGlideType? knownGlideType))
                {
                    if (string.IsNullOrWhiteSpace(remoteRef.Display))
                        type.Label = string.IsNullOrWhiteSpace(knownGlideType.Label) ? name : knownGlideType.Label;
                    type.ScalarLength = knownGlideType.ScalarLength;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.ScalarType))
                        type.ScalarType = knownGlideType.ScalarType;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.GlobalElementType))
                        type.GlobalElementType = knownGlideType.GlobalElementType;
                    if (!string.IsNullOrWhiteSpace(knownGlideType.ScopedElementType))
                        type.ScopedElementType = knownGlideType.ScopedElementType;
                }
                else if (string.IsNullOrWhiteSpace(remoteRef.Display))
                    type.Label = name;
            }
            await _dbContext.Types.AddAsync(type, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return type;
    }

    private async Task<SncSource> GetCurrentSourceAsync(CancellationToken cancellationToken)
    {
        if (!_dbContext.InitSuccessful && _remoteUri.InitSuccessful) throw new InvalidOperationException();
        cancellationToken.ThrowIfCancellationRequested();
        if (_currentSourceEntity is not null) return _currentSourceEntity;
        var fqdn = _remoteUri.Fqdn;
        if ((_currentSourceEntity = _dbContext.Sources.FirstOrDefault(e => e.FQDN == fqdn)) is not null)
        {
            if (_remoteUri.Label is not null && _remoteUri.Label != _currentSourceEntity.Label)
                _currentSourceEntity.Label = _remoteUri.Label;
            else if (_currentSourceEntity.IsPersonalDev == _remoteUri.IsPdi)
                return _currentSourceEntity;
            _currentSourceEntity.IsPersonalDev = _remoteUri.IsPdi;
            _dbContext.Sources.Update(_currentSourceEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _currentSourceEntity;
        }
        var oldFqdn = _remoteUri.OldFqdn;
        if (fqdn != oldFqdn && (_currentSourceEntity = _dbContext.Sources.FirstOrDefault(e => e.FQDN == oldFqdn)) is not null)
        {
            _currentSourceEntity.FQDN = fqdn;
            if (_remoteUri.Label is not null && _remoteUri.Label != _currentSourceEntity.Label)
                _currentSourceEntity.Label = _remoteUri.Label;
            _currentSourceEntity.IsPersonalDev = _remoteUri.IsPdi;
            _dbContext.Sources.Update(_currentSourceEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _currentSourceEntity;
        }
        _currentSourceEntity = new()
        {
            Label = _remoteUri.Label ?? fqdn,
            FQDN = fqdn,
            IsPersonalDev = _remoteUri.IsPdi,
            LastAccessed = DateTime.Now
        };
        await _dbContext.Sources.AddAsync(_currentSourceEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return _currentSourceEntity;
    }

    internal async Task<SncSource> EnsureCurrentSourceAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Monitor.Enter(_syncRoot);
        try { return await GetCurrentSourceAsync(cancellationToken); }
        finally { Monitor.Exit(_syncRoot); }
    }

    internal async Task<EntityEntry<SncSource>> EnsureCurrentSourceEntryAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Monitor.Enter(_syncRoot);
        try
        {
            _currentSourceEntry ??= _dbContext.Sources.Entry(await GetCurrentSourceAsync(cancellationToken));
            return _currentSourceEntry;
        }
        finally { Monitor.Exit(_syncRoot); }
    }

    private async Task<Table?> FromTableRefAsync(RemoteRef? remoteRef, CancellationToken cancellationToken)
    {
        if (remoteRef is null)
            return null;
        string name = remoteRef.Value;
        var tableEntity = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (tableEntity is not null)
            return tableEntity;
        var remoteTable = await _tableAPIService.GetTableRecordByNameAsync(name, cancellationToken);
        if (remoteTable is not null)
            return await AddTableAsync(remoteTable, cancellationToken);
        Table table = new()
        {
            Name = name,
            AccessibleFrom = string.Empty,
            IsExtendable = true,
            Label = remoteRef.Display ?? name,
            LastUpdated = DateTime.Now,
            Source = await EnsureCurrentSourceAsync(cancellationToken),
            SysID = Guid.NewGuid().ToString("N")
        };
        await AddTableAsync(table, null, cancellationToken);
        return table;
    }

    private async Task<Table?> FromSuperClassRefAsync(RemoteRef? tableRef, CancellationToken cancellationToken)
    {
        if (tableRef is null)
            return null;
        string sys_id = tableRef.Value;
        if (_tableIdMap.TryGetValue(sys_id, out string? name))
            return await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        var tableRecord = await _tableAPIService.GetTableRecordByIdAsync(sys_id, cancellationToken);
        if (tableRecord is null) // TODO: Add warning for table not found
            return null;
        return await AddTableAsync(tableRecord, cancellationToken);
    }

    // BUG: Package groups can have more than one package name to them.
    private async Task<PackageGroup> GetPackageGroupAsync(string packageName, string @namespace, bool activeAndNotLicensable, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<Package?> FromPackageRefAsync(RemoteRef? pkgRef, SncSource source, CancellationToken cancellationToken)
    {
        if (pkgRef is null)
            return null;
        string sys_id = pkgRef.Value;
        if (_packageIdMap.TryGetValue(sys_id, out string? id))
            return await _dbContext.Packages.FirstOrDefaultAsync(p => p.ID == id, cancellationToken);
        var packageRecord = await _tableAPIService.GetPackageRecordByIDAsync(sys_id, cancellationToken);
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
                pkgName = pkgRef.Display.AsNonEmpty(scope.Value);
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
                var name = pkgRef.Display.AsNonEmpty(GLOBAL_NAMESPACE);
                if (string.IsNullOrWhiteSpace(pkgRef.Display))
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

    private async Task<Scope?> FromScopeRefAsync(RemoteRef? scopeRef, SncSource source, CancellationToken cancellationToken)
    {
        if (scopeRef is null)
            return null;
        var sys_id = scopeRef.Value;
        if (_scopeIdMap.TryGetValue(sys_id, out string? value))
            return await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken);
        var scopeRecord = await _tableAPIService.GetApplicationRecordByIDAsync(sys_id, cancellationToken);
        Scope? scope;
        if (scopeRecord is null)
        {
            if ((value = scopeRef.Display) == null || (scope = await _dbContext.Scopes.FirstOrDefaultAsync(s => s.Name == value, cancellationToken)) is null)
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
                var response = await _tableAPIService.GetTableRecordByNameAsync(name, cancellationToken);
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

    public DataLoaderService(TypingsDbContext dbContext, RemoteUriService remoteUriService, TableAPIService tableAPIService, IOptions<AppSettings> appSettingsOptions, ILogger<DataLoaderService> logger)
    {
        _dbContext = dbContext;
        _remoteUri = remoteUriService;
        _tableAPIService = tableAPIService;
        _logger = logger;
        var appSettings = appSettingsOptions.Value;
        _knownGlideTypes = appSettings.GetKnownGlideTypes();
        _baselineInit = appSettings.BaselineInit ?? false;
        if (appSettings.DefaultPackageGroups is not null)
        {
            List<string> baselinePackageGroups = new();
            Dictionary<string, string> pkgMap = new(NameComparer);
            foreach (DefaultPackageGroup g in appSettings.DefaultPackageGroups)
            {
                if (g is null || g.Packages is null || g.Packages.Count == 0) continue;
                var n = g.Name.AsWhitespaceNormalizedOrDefaultIfEmpty(GLOBAL_NAMESPACE);
                if (g.IsBaseline.HasValue)
                {
                    if (g.IsBaseline.Value)
                        baselinePackageGroups.Add(n);
                    else
                        baselinePackageGroups.Remove(n);
                }
                foreach (var p in g.Packages.Select(v => v.AsWhitespaceNormalizedOrDefaultIfEmpty(GLOBAL_NAMESPACE)))
                {
                    if (_packageGroupMap.ContainsKey(p))
                        _packageGroupMap[p] = n;
                    else
                        _packageGroupMap.Add(p, n);
                }
            }
            _defaultBaselinePackageGroups = baselinePackageGroups.ToArray().Distinct(NameComparer).ToArray().ToImmutableArray();
        }
        else
            _defaultBaselinePackageGroups = ImmutableArray<string>.Empty;
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