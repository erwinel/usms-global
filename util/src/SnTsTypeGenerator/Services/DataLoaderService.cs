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

    internal async Task<TableInfo> GetBaseRecordTypeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_dbContext is null)
            throw new ObjectDisposedException(nameof(DataLoaderService));
        TableInfo? tableInfo = await _dbContext.Tables.FirstOrDefaultAsync(t => t.Name == TS_NAME_BASERECORD && t.IsInterface, cancellationToken);
        if (tableInfo is null)
        {
            string sourceFqdn = _tableAPIService.SourceFqdn;
            tableInfo = await AddTableAsync(new(
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
                SourceFqdn: sourceFqdn), cancellationToken);
            _ = await AddElementsAsync(new Element[] {
                new(
                    Name: JSON_KEY_SYS_ID,
                    Label: "Sys ID",
                    SysID: Guid.Empty.ToString("N"),
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Value: TYPE_NAME_GUID, DisplayValue: "Sys ID (GUID)"),
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
                    Type: new(Value: TYPE_NAME_string, DisplayValue: "String"),
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
                    Type: new(Value: TYPE_NAME_glide_date_time, DisplayValue: "Date/Time"),
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
                    Type: new(Value: TYPE_NAME_integer, DisplayValue: "Integer"),
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
                    Name: JSON_KEY_SYS_CREATED_BY,
                    Label: "Updated by",
                    SysID: "ef0b4750753d4f6c82499a605b490af4",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Value: TYPE_NAME_string, DisplayValue: "String"),
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
                    Label: "Updated",
                    SysID: "3f68a52adc8a4c5a960ec2a9a2bd9fd6",
                    Reference: null,
                    IsReadOnly: false,
                    Type: new(Value: TYPE_NAME_glide_date_time, DisplayValue: "Date/Time"),
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
            }, tableInfo, cancellationToken);
        }
        return tableInfo;
    }

    private async Task<ElementInfo[]> AddElementsAsync(IEnumerable<Element> elements, TableInfo table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!elements.Any())
            return Array.Empty<ElementInfo>();
        var source = (await table.GetReferencedEntityAsync(_dbContext!.Tables, t => t.Source, cancellationToken))!;
        _logger.LogAddingElementsToDatabase(table.Name);
        var items = elements.Select<Element, (ElementInfo Element, RecordRef? Package, RecordRef? Reference, RecordRef? Type, string SourceFqdn)>(e => (new ElementInfo()
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
        foreach (var g in items.GroupBy(a => a.SourceFqdn))
        {
            var s = await GetSourceAsync(g.Key, cancellationToken);
            foreach (var a in g)
                a.Element.Source = s;
        }
        foreach (var g in items.Where(a => a.Package is not null).GroupBy(a => a.Package!.DisplayValue))
        {
            var e = g.First();
            var p = await FromPackageRefAsync(e.Package, e.Element.Source!, cancellationToken);
            if (p is not null)
                foreach (var a in g)
                    a.Element.Package = p;
        }
        foreach (var g in items.Where(a => a.Type is not null).GroupBy(a => a.Type!.DisplayValue))
        {
            var e = g.First();
            var t = await FromGlideTypeRefAsync(e.Type!, e.Element.Source!, cancellationToken);
            if (t is not null)
                foreach (var a in g)
                    a.Element.Type = t;
        }
        foreach (var g in items.Where(a => a.Reference is not null).GroupBy(a => a.Reference!.Value))
        {
            var e = g.First();
            var t = await FromTableRefAsync(g.First().Reference!, cancellationToken);
            if (t is not null)
                foreach (var a in g)
                    a.Element.Reference = t;
        }
        var result = items.Select(e => e.Element).ToArray();
        await _dbContext!.Elements.AddRangeAsync(result, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task<TableInfo> AddTableAsync(Table table, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SourceInfo source = await GetSourceAsync(table.SourceFqdn, cancellationToken);
        TableInfo tableInfo = new()
        {
            Name = table.Name,
            AccessibleFrom = table.AccessibleFrom,
            ExtensionModel = table.ExtensionModel,
            IsExtendable = table.IsExtendable,
            IsInterface = false,
            Label = table.Label,
            LastUpdated = DateTime.Now,
            NumberPrefix = table.NumberPrefix,
            Source = source,
            SysID = table.SysID,
            Package = await FromPackageRefAsync(table.Package, source, cancellationToken),
            Scope = await FromScopeRefAsync(table.Scope, source, cancellationToken)
        };
        _logger.LogAddingTableToDb(tableInfo.Name);
        await _dbContext!.Tables.AddAsync(tableInfo, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _tableIdMap.Add(tableInfo.SysID, tableInfo.Name);
        _logger.LogNewTableSaveCompleted(tableInfo.Name);
        TableInfo? superClass = (table.SuperClass is null) ? null : await FromTableRefAsync(table.SuperClass, cancellationToken);
        IEnumerable<ElementInfo> superElements;
        var elements = (await _tableAPIService.GetElementsByTableNameAsync(table.Name, cancellationToken)).ToArray();
        if (superClass is null)
        {
            if (elements.ExtendsBaseRecord())
            {
                superClass = await GetBaseRecordTypeAsync(cancellationToken);
                tableInfo.SuperClass = superClass;
                _dbContext.Tables.Update(tableInfo);
                await _dbContext.SaveChangesAsync(cancellationToken);
                superElements = await tableInfo.GetRelatedCollectionAsync(_dbContext.Tables, e => e.Elements, cancellationToken);
            }
            else
                return tableInfo;
        }
        else
        {
            tableInfo.SuperClass = superClass;
            _dbContext.Tables.Update(tableInfo);
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (elements.Length == 0)
                return tableInfo;
            if (!(superElements = (await _dbContext.Tables.Entry(tableInfo).GetAllElementInheritancesAsync(cancellationToken)).Select(e => e.Element)).Any())
            {
                await AddElementsAsync(elements, tableInfo, cancellationToken);
                return tableInfo;
            }
        }

        await AddElementsAsync(elements.Where(e => !superElements.Any(s => e.IsIdenticalTo(s))), tableInfo, cancellationToken);

        return tableInfo;
    }

    private async Task<GlideType> FromGlideTypeRefAsync(RecordRef type, SourceInfo source, CancellationToken cancellationToken)
    {
        string name = type.Value;
        GlideType? result = await _dbContext!.Types.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (result is null)
        {
            var record = await _tableAPIService.GetGlideTypeByNameAsync(name, cancellationToken);
            if (record is not null)
            {
                source = await GetSourceAsync(record.SourceFqdn, cancellationToken);
                result = new()
                {
                    Name = name,
                    Label = record.Label,
                    ScalarType = record.ScalarType,
                    ScalarLength = record.ScalarLength,
                    ClassName = record.ClassName,
                    UseOriginalValue = record.UseOriginalValue,
                    IsVisible = record.IsVisible,
                    LastUpdated = DateTime.Now,
                    SysID = record.SysID,
                    Source = source,
                    Package = await FromPackageRefAsync(record.Package, source, cancellationToken),
                    Scope = await FromScopeRefAsync(record.Scope, source, cancellationToken)
                };
            }
            else
            {
                result = new()
                {
                    Name = name,
                    Label = type.DisplayValue,
                    LastUpdated = DateTime.Now,
                    Source = source
                };
                switch (name)
                {
                    case TYPE_NAME_boolean:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "boolean";
                        result.SysID = "17d3ba81bf3320001875647fcf0739dc";
                        break;
                    case "currency":
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "decimal";
                        result.ClassName = "Currency";
                        result.ScalarLength = 20;
                        result.SysID = "fd2cb3b40a0a0b3000977fec84409b73";
                        break;
                    case TYPE_NAME_document_id:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "GUID";
                        result.ClassName = "com.glide.script.glide_elements.GlideElementDocumentId";
                        result.ScalarLength = 32;
                        result.SysID = "a887c8b20a0a0b4a00258cf907c43960";
                        break;
                    case TYPE_NAME_domain_id:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "GUID";
                        result.ClassName = "com.glide.script.glide_elements.GlideElementDomainId";
                        result.ScalarLength = 32;
                        result.SysID = "db52cccc0a7b7b7b0170604fc254959d";
                        break;
                    case TYPE_NAME_due_date:
                        result.IsVisible = true;
                        result.ScalarType = "datetime";
                        result.ClassName = "GlideDueDate";
                        result.SysID = "2eb30326c61122b8011a74ef6a7c9ee8";
                        break;
                    case TYPE_NAME_glide_action_list:
                        result.ScalarType = "string";
                        result.ClassName = "GlideActionList";
                        result.ScalarLength = 1024;
                        result.SysID = "355be32bbfa00100421cdc2ecf073929";
                        break;
                    case TYPE_NAME_glide_date_time:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "datetime";
                        result.ClassName = "GlideDateTime";
                        result.SysID = "a54edbb1c0a80006014da86b91525bf3";
                        break;
                    case TYPE_NAME_glide_date:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "date";
                        result.ClassName = "GlideDate";
                        result.SysID = "a3a73875c611227800e6f970ea4c7410";
                        break;
                    case TYPE_NAME_glide_duration:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "datetime";
                        result.ClassName = "GlideDuration";
                        result.SysID = "a946bf95c61122780075303f75c339c4";
                        break;
                    case TYPE_NAME_integer:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "integer";
                        result.SysID = "aab367c1bf3320001875647fcf073909";
                        break;
                    case "price":
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "decimal";
                        result.ClassName = "Price";
                        result.ScalarLength = 20;
                        result.SysID = "fd39244f0a0a0b30009a9072cfd1b037";
                        break;
                    case TYPE_NAME_reference:
                        result.UseOriginalValue = true;
                        result.IsVisible = true;
                        result.ScalarType = "GUID";
                        result.SysID = "52a227c1bf3320001875647fcf07396a";
                        break;
                    case TYPE_NAME_timer:
                        result.UseOriginalValue = true;
                        result.ScalarType = "datetime";
                        result.ClassName = "com.glide.glideobject.GlideDuration";
                        result.SysID = "0569687fc611227801e0338f57aa2ab4";
                        break;
                    default:
                        result.ScalarType = "string";
                        switch (name)
                        {
                            case "collection":
                            case "documentation_field":
                            case "domain_path":
                            case "email":
                            case "expression":
                            case TYPE_NAME_GUID:
                            case "multi_two_lines":
                            case "ph_number":
                            case "sys_class_name":
                            case "sys_class_path":
                            case "translated_field":
                            case "user_input":
                            case "user_roles":
                            case "variables":
                            case "version":
                                result.UseOriginalValue = true;
                                break;
                            case "conditions":
                            case TYPE_NAME_glide_list:
                            case TYPE_NAME_journal_input:
                            case TYPE_NAME_journal_list:
                            case "user_image":
                                result.IsVisible = true;
                                break;
                            default:
                                result.IsVisible = true;
                                result.UseOriginalValue = true;
                                break;
                        }
                        switch (name)
                        {
                            case "collection":
                                result.SysID = "a0e0cbd2c3203000bac1addbdfba8f59";
                                break;
                            case "documentation_field":
                                result.ClassName = "GlideElementTranslatedField";
                                result.ScalarLength = 80;
                                result.SysID = "7c5689e1bfd030001875647fcf07392e";
                                break;
                            case "domain_path":
                                result.ScalarLength = 255;
                                result.SysID = "7b17c631c30310006e06addbdfba8fc1";
                                break;
                            case "email":
                                result.SysID = "bb3227aec0a80164012fb063bf06ebbf";
                                break;
                            case "expression":
                                result.SysID = "0ab2b62665914510f8771b76afcd57d8";
                                break;
                            case TYPE_NAME_GUID:
                                result.ScalarLength = 32;
                                result.SysID = "4f04a7c1bf3320001875647fcf07396b";
                                break;
                            case "multi_two_lines":
                                result.SysID = "c0ff45bfc611227a0020ea6c4111077d";
                                break;
                            case "ph_number":
                                result.SysID = "4fea0202c611228e01ff351048e59ee1";
                                break;
                            case "sys_class_name":
                                result.ClassName = "com.glide.glideobject.SysClassName";
                                result.SysID = "0f66a28cc6112275013d0fea88ffa3f9";
                                break;
                            case "sys_class_path":
                                result.SysID = "38805f73672222008db1bcb532415a8b";
                                break;
                            case "translated_field":
                                result.ClassName = "GlideElementTranslatedField";
                                result.SysID = "c12757ecc0a8016400a0744011ed8262";
                                break;
                            case TYPE_NAME_user_input:
                                result.ClassName = "GlideUserInput";
                                result.SysID = "b7545d990a0a0a0a006e1f8f9d6fa835";
                                break;
                            case "user_roles":
                                result.ScalarLength = 255;
                                result.SysID = "6670773ec0a80165010ede48f3caa831";
                                break;
                            case "variables":
                                result.SysID = "8c31e3c1bf3320001875647fcf0739d9";
                                break;
                            case "version":
                                result.SysID = "89966d16c3321100bac14ddcddba8f2b";
                                break;
                            case "conditions":
                                result.ScalarLength = 4000;
                                result.SysID = "4ab756cec611229b006b4082e623d8ac";
                                break;
                            case TYPE_NAME_glide_list:
                                result.ClassName = "GlideList";
                                result.ScalarLength = 1024;
                                result.SysID = "e3f24e9cc0a80166007d6acccef476e9";
                                break;
                            case TYPE_NAME_journal_input:
                                result.ClassName = "Journal";
                                result.SysID = "50b082d6c61122760136e69d0e2852de";
                                break;
                            case TYPE_NAME_journal_list:
                                result.ClassName = "Journal";
                                result.SysID = "50b0a5e3c6112276017eea10c160a249";
                                break;
                            case "user_image":
                                result.ClassName = "UserImage";
                                result.SysID = "e5760b18c611228100a4effee22075fa";
                                break;
                            case "choice":
                                result.SysID = "5217a7c1bf3320001875647fcf0739b7";
                                break;
                            case "field_name":
                                result.ScalarLength = 80;
                                result.SysID = "4df9be04c0a8016401a50976ae00ed2e";
                                break;
                            case TYPE_NAME_journal:
                                result.ClassName = "Journal";
                                result.SysID = "ee0f99bea9fe5f6201e4186985822523";
                                break;
                            case "password":
                                result.ClassName = "Password";
                                result.SysID = "3393a388c61122770033c78ba16d3fcd";
                                break;
                            case "script":
                                result.ScalarLength = 4000;
                                result.SysID = "4ab7aca2c611229b00e53ca27ac105d9";
                                break;
                            case "table_name":
                                result.ScalarLength = 80;
                                result.SysID = "e0baa043c0a8016501553d18374ae67a";
                                break;
                            case "translated_text":
                                result.SysID = "e62dabc1bf3320001875647fcf073967";
                                break;
                            case "workflow":
                                result.ScalarLength = 80;
                                result.SysID = "deb943ec7f00000101aea727f742df58";
                                break;
                            default:
                                result.SysID = "747127c1bf3320001875647fcf0739e0";
                                break;
                        }
                        break;
                }
            }
            await _dbContext.Types.AddAsync(result, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    private async Task<TableInfo?> FromTableRefAsync(RecordRef? tableRef, CancellationToken cancellationToken)
    {
        if (tableRef is null)
            return null;
        if (_tableIdMap.TryGetValue(tableRef.Value, out string? name))
            return await _dbContext!.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        var table = await _tableAPIService.GetTableByIdAsync(tableRef.Value, cancellationToken);
        if (table is null)
            return null;
        name = table.Name;
        var tableInfo = await _dbContext!.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        if (tableInfo is null)
            return await AddTableAsync(table, cancellationToken);
        _tableIdMap.Add(tableRef.Value, tableInfo.Name);
        return tableInfo;
    }

    private async Task<SysPackage?> FromPackageRefAsync(RecordRef? pkgRef, SourceInfo source, CancellationToken cancellationToken)
    {
        if (pkgRef is null)
            return null;
        if (_packageIdMap.TryGetValue(pkgRef.Value, out string? name))
            return await _dbContext!.Packages.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        name = pkgRef.DisplayValue;
        SysPackage? package = await _dbContext!.Packages.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        if (package is null)
        {
            package = new()
            {
                Name = pkgRef.DisplayValue,
                SysId = pkgRef.Value,
                LastUpdated = DateTime.Now,
                Source = source
            };
            await _dbContext!.Packages.AddAsync(package, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        _packageIdMap.Add(pkgRef.Value, package.Name);
        return package;
    }

    private async Task<SysScope?> FromScopeRefAsync(RecordRef? scopeRef, SourceInfo source, CancellationToken cancellationToken)
    {
        if (scopeRef is null)
            return null;
        if (_scopeIdMap.TryGetValue(scopeRef.Value, out string? value))
            return await _dbContext!.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken);
        var scope = await _tableAPIService.GetScopeByIDAsync(scopeRef.Value, cancellationToken);
        SysScope? result;
        if (scope is null)
        {
            value = scopeRef.DisplayValue;
            result = new()
            {
                Name = value,
                Value = value,
                LastUpdated = DateTime.Now,
                SysID = scopeRef.Value,
                Source = source
            };
            await _dbContext!.Scopes.AddAsync(result, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            value = scope.Value;
            if ((result = await _dbContext!.Scopes.FirstOrDefaultAsync(s => s.Value == value, cancellationToken)) is null)
            {
                result = new()
                {
                    Name = scope.Name,
                    Value = value,
                    LastUpdated = DateTime.Now,
                    SysID = scope.SysID,
                    Source = source,
                    ShortDescription = scope.ShortDescription
                };
                await _dbContext!.Scopes.AddAsync(result, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        _scopeIdMap.Add(scopeRef.Value, value);
        return result;
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