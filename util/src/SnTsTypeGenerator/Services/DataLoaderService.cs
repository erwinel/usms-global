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
    private TypingsDbContext _dbContext;
    private readonly TableAPIService _tableAPIService;
    private readonly ILogger<DataLoaderService> _logger;
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
                switch (name)
                {
                    case TYPE_NAME_boolean:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "boolean";
                        type.SysID = "17d3ba81bf3320001875647fcf0739dc";
                        break;
                    case "currency":
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "decimal";
                        type.ClassName = "Currency";
                        type.ScalarLength = 20;
                        type.SysID = "fd2cb3b40a0a0b3000977fec84409b73";
                        break;
                    case TYPE_NAME_document_id:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "GUID";
                        type.ClassName = "com.glide.script.glide_elements.GlideElementDocumentId";
                        type.ScalarLength = 32;
                        type.SysID = "a887c8b20a0a0b4a00258cf907c43960";
                        break;
                    case TYPE_NAME_domain_id:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "GUID";
                        type.ClassName = "com.glide.script.glide_elements.GlideElementDomainId";
                        type.ScalarLength = 32;
                        type.SysID = "db52cccc0a7b7b7b0170604fc254959d";
                        break;
                    case TYPE_NAME_due_date:
                        type.IsVisible = true;
                        type.ScalarType = "datetime";
                        type.ClassName = "GlideDueDate";
                        type.SysID = "2eb30326c61122b8011a74ef6a7c9ee8";
                        break;
                    case TYPE_NAME_glide_action_list:
                        type.ScalarType = "string";
                        type.ClassName = "GlideActionList";
                        type.ScalarLength = 1024;
                        type.SysID = "355be32bbfa00100421cdc2ecf073929";
                        break;
                    case TYPE_NAME_glide_date_time:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "datetime";
                        type.ClassName = "GlideDateTime";
                        type.SysID = "a54edbb1c0a80006014da86b91525bf3";
                        break;
                    case TYPE_NAME_glide_date:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "date";
                        type.ClassName = "GlideDate";
                        type.SysID = "a3a73875c611227800e6f970ea4c7410";
                        break;
                    case TYPE_NAME_glide_duration:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "datetime";
                        type.ClassName = "GlideDuration";
                        type.SysID = "a946bf95c61122780075303f75c339c4";
                        break;
                    case TYPE_NAME_integer:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "integer";
                        type.SysID = "aab367c1bf3320001875647fcf073909";
                        break;
                    case "price":
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "decimal";
                        type.ClassName = "Price";
                        type.ScalarLength = 20;
                        type.SysID = "fd39244f0a0a0b30009a9072cfd1b037";
                        break;
                    case TYPE_NAME_reference:
                        type.UseOriginalValue = true;
                        type.IsVisible = true;
                        type.ScalarType = "GUID";
                        type.SysID = "52a227c1bf3320001875647fcf07396a";
                        break;
                    case TYPE_NAME_timer:
                        type.UseOriginalValue = true;
                        type.ScalarType = "datetime";
                        type.ClassName = "com.glide.glideobject.GlideDuration";
                        type.SysID = "0569687fc611227801e0338f57aa2ab4";
                        break;
                    default:
                        type.ScalarType = "string";
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
                                type.UseOriginalValue = true;
                                break;
                            case "conditions":
                            case TYPE_NAME_glide_list:
                            case TYPE_NAME_journal_input:
                            case TYPE_NAME_journal_list:
                            case "user_image":
                                type.IsVisible = true;
                                break;
                            default:
                                type.IsVisible = true;
                                type.UseOriginalValue = true;
                                break;
                        }
                        switch (name)
                        {
                            case "collection":
                                type.SysID = "a0e0cbd2c3203000bac1addbdfba8f59";
                                break;
                            case "documentation_field":
                                type.ClassName = "GlideElementTranslatedField";
                                type.ScalarLength = 80;
                                type.SysID = "7c5689e1bfd030001875647fcf07392e";
                                break;
                            case "domain_path":
                                type.ScalarLength = 255;
                                type.SysID = "7b17c631c30310006e06addbdfba8fc1";
                                break;
                            case "email":
                                type.SysID = "bb3227aec0a80164012fb063bf06ebbf";
                                break;
                            case "expression":
                                type.SysID = "0ab2b62665914510f8771b76afcd57d8";
                                break;
                            case TYPE_NAME_GUID:
                                type.ScalarLength = 32;
                                type.SysID = "4f04a7c1bf3320001875647fcf07396b";
                                break;
                            case "multi_two_lines":
                                type.SysID = "c0ff45bfc611227a0020ea6c4111077d";
                                break;
                            case "ph_number":
                                type.SysID = "4fea0202c611228e01ff351048e59ee1";
                                break;
                            case "sys_class_name":
                                type.ClassName = "com.glide.glideobject.SysClassName";
                                type.SysID = "0f66a28cc6112275013d0fea88ffa3f9";
                                break;
                            case "sys_class_path":
                                type.SysID = "38805f73672222008db1bcb532415a8b";
                                break;
                            case "translated_field":
                                type.ClassName = "GlideElementTranslatedField";
                                type.SysID = "c12757ecc0a8016400a0744011ed8262";
                                break;
                            case TYPE_NAME_user_input:
                                type.ClassName = "GlideUserInput";
                                type.SysID = "b7545d990a0a0a0a006e1f8f9d6fa835";
                                break;
                            case "user_roles":
                                type.ScalarLength = 255;
                                type.SysID = "6670773ec0a80165010ede48f3caa831";
                                break;
                            case "variables":
                                type.SysID = "8c31e3c1bf3320001875647fcf0739d9";
                                break;
                            case "version":
                                type.SysID = "89966d16c3321100bac14ddcddba8f2b";
                                break;
                            case "conditions":
                                type.ScalarLength = 4000;
                                type.SysID = "4ab756cec611229b006b4082e623d8ac";
                                break;
                            case TYPE_NAME_glide_list:
                                type.ClassName = "GlideList";
                                type.ScalarLength = 1024;
                                type.SysID = "e3f24e9cc0a80166007d6acccef476e9";
                                break;
                            case TYPE_NAME_journal_input:
                                type.ClassName = "Journal";
                                type.SysID = "50b082d6c61122760136e69d0e2852de";
                                break;
                            case TYPE_NAME_journal_list:
                                type.ClassName = "Journal";
                                type.SysID = "50b0a5e3c6112276017eea10c160a249";
                                break;
                            case "user_image":
                                type.ClassName = "UserImage";
                                type.SysID = "e5760b18c611228100a4effee22075fa";
                                break;
                            case "choice":
                                type.SysID = "5217a7c1bf3320001875647fcf0739b7";
                                break;
                            case "field_name":
                                type.ScalarLength = 80;
                                type.SysID = "4df9be04c0a8016401a50976ae00ed2e";
                                break;
                            case TYPE_NAME_journal:
                                type.ClassName = "Journal";
                                type.SysID = "ee0f99bea9fe5f6201e4186985822523";
                                break;
                            case "password":
                                type.ClassName = "Password";
                                type.SysID = "3393a388c61122770033c78ba16d3fcd";
                                break;
                            case "script":
                                type.ScalarLength = 4000;
                                type.SysID = "4ab7aca2c611229b00e53ca27ac105d9";
                                break;
                            case "table_name":
                                type.ScalarLength = 80;
                                type.SysID = "e0baa043c0a8016501553d18374ae67a";
                                break;
                            case "translated_text":
                                type.SysID = "e62dabc1bf3320001875647fcf073967";
                                break;
                            case "workflow":
                                type.ScalarLength = 80;
                                type.SysID = "deb943ec7f00000101aea727f742df58";
                                break;
                            default:
                                type.SysID = "747127c1bf3320001875647fcf0739e0";
                                break;
                        }
                        break;
                }
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

    private async Task<Package?> FromPackageRefAsync(PackageRef? pkgRef, SncSource source, CancellationToken cancellationToken)
    {
        if (pkgRef is null)
            return null;
        string sys_id = pkgRef.SysID;
        if (_packageIdMap.TryGetValue(sys_id, out string? id))
            return await _dbContext.Packages.FirstOrDefaultAsync(p => p.ID == id, cancellationToken);
        var packageRecord = await _tableAPIService.GetPackageByIDAsync(sys_id, cancellationToken);
        Package? package;
        if (packageRecord is null)
        {
            if (_scopeIdMap.TryGetValue(sys_id, out id))
            {
                Scope? scope = await _dbContext.Scopes.FirstOrDefaultAsync(p => p.Value == id, cancellationToken);
                if (scope is null)
                    return null;
                id = scope.ID;
                if ((package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.ID == id, cancellationToken)) is null)
                {
                    package = new()
                    {
                        ID = id,
                        Name = scope.Name,
                        Version = scope.Version,
                        SysID = sys_id,
                        LastUpdated = DateTime.Now,
                        Source = source
                    };
                    await _dbContext.Packages.AddAsync(package, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                var name = pkgRef.Name;
                if (name is null || (package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Name == name)) is null)
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
                Source = source
            };
            await _dbContext.Packages.AddAsync(package, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
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

    public DataLoaderService(TypingsDbContext dbContext, TableAPIService tableAPIService, ILogger<DataLoaderService> logger)
    {
        _dbContext = dbContext;
        _tableAPIService = tableAPIService;
        _logger = logger;
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