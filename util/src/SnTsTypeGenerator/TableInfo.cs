using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

/// <summary>
/// Represents an item from the "Table" (<see cref="Constants.TABLE_NAME_SYS_DB_OBJECT" />) table.
/// </summary>
[Table(nameof(TableInfo))]
public class TableInfo
{
    private readonly object _syncRoot = new();

    private string _sysID = string.Empty;

    /// <summary>
    /// Value of the "Sys ID" (<see cref="Constants.JSON_KEY_SYS_ID" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    private string _name = string.Empty;

    /// <summary>
    /// Value of the "Name" (<see cref="Constants.JSON_KEY_NAME" />) column.
    /// </summary>
    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    private string _label = string.Empty;

    /// <summary>
    /// Value of the "Label" (<see cref="Constants.JSON_KEY_LABEL" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }
    
    /// <summary>
    /// Value of the "Extensible" (<see cref="Constants.JSON_KEY_NAME" />) column.
    /// </summary>
    public bool IsExtendable { get; set; }

    private string _accessibleFrom = string.Empty;

    /// <summary>
    /// Value of the "Accessible from" (<see cref="Constants.JSON_KEY_ACCESS" />) column.
    /// </summary>
    [NotNull]
    [BackingField(nameof(_accessibleFrom))]
    public string AccessibleFrom
    {
        get => _accessibleFrom;
        set => _accessibleFrom = value ?? string.Empty;
    }
    
    /// <summary>
    /// Value of the "Extension model" (<see cref="Constants.JSON_KEY_EXTENSION_MODEL" />) column.
    /// </summary>
    public string? ExtensionModel { get; set; }

    /// <summary>
    /// Associated value of the "Auto number" (<see cref="Constants.JSON_KEY_NUMBER_REF" />) column.
    /// </summary>
    public string? NumberPrefix { get; set; }

    /// <summary>
    /// Date and time that this record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// Name of the associated record for the "Package" (<see cref="Constants.JSON_KEY_SYS_PACKAGE" />) column.
    /// </summary>
    private string? _packageName;

    [BackingField(nameof(_packageName))]
    public string? PackageName
    {
        get => _package?.Name ?? _packageName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_packageName is not null)
                    {
                        _packageName = null;
                        _package = null;
                    }
                }
                else if (_packageName is null || !value.Equals(_packageName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_package is null)
                        _packageName = value;
                    else if (value.Equals(_package.Name, StringComparison.InvariantCultureIgnoreCase))
                        _packageName = null;
                    else
                        _package = null;
                }
            }
        }
    }

    private SysPackage? _package;
    
    /// <summary>
    /// The source package of the table.
    /// </summary>
    public SysPackage? Package
    {
        get => _package;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _package is null : _package is not null && ReferenceEquals(_package, value))
                    return;

                _package = value;
                _packageName = null;
            }
        }
    }

    private string? _scopeValue;

    /// <summary>
    /// Value of the associated record for the "Application" (<see cref="Constants.JSON_KEY_SYS_SCOPE" />) column.
    /// </summary>
    [BackingField(nameof(_scopeValue))]
    public string? ScopeValue
    {
        get => _scope?.Value ?? _scopeValue;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_scopeValue is not null)
                    {
                        _scopeValue = null;
                        _scope = null;
                    }
                }
                else if (_scopeValue is null || !value.Equals(_scopeValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_scope is null)
                        _scopeValue = value;
                    else if (value.Equals(_scope.Value, StringComparison.InvariantCultureIgnoreCase))
                        _scopeValue = null;
                    else
                        _scope = null;
                }
            }
        }
    }

    private SysScope? _scope;
    
    /// <summary>
    /// The scope of the table.
    /// </summary>
    public SysScope? Scope
    {
        get => _scope;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _scope is null : _scope is not null && ReferenceEquals(_scope, value))
                    return;

                _scope = value;
                _scopeValue = null;
            }
        }
    }

    private string? _superClassName;

    /// <summary>
    /// Table name associated with the "Extends table" (<see cref="Constants.JSON_KEY_SUPER_CLASS" />) column.
    /// </summary>
    [BackingField(nameof(_superClassName))]
    public string? SuperClassName
    {
        get => _superClass?.Name ?? _superClassName;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_superClassName is not null)
                    {
                        _superClassName = null;
                        _superClass = null;
                    }
                }
                else if (_superClassName is null || !value.Equals(_superClassName, StringComparison.InvariantCultureIgnoreCase))
                {

                    if (_superClass is null)
                        _superClassName = value;
                    else if (value.Equals(_superClass.Name, StringComparison.InvariantCultureIgnoreCase))
                        _superClassName = null;
                    else
                        _superClass = null;
                }
            }
        }
    }

    private TableInfo? _superClass;

    /// <summary>
    /// The extended table.
    /// </summary>
    public TableInfo? SuperClass
    {
        get => _superClass;
        set
        {
            lock (_syncRoot)
            {
                if ((value is null) ? _superClass is null : _superClass is not null && ReferenceEquals(_superClass, value))
                    return;

                _superClass = value;
                _superClassName = null;
            }
        }
    }

    private string _sourceFqdn = string.Empty;

    /// <summary>
    /// The FQDN of the source ServiceNow instance.
    /// </summary>
    [BackingField(nameof(_sourceFqdn))]
    public string SourceFqdn
    {
        get => _source?.FQDN ?? _sourceFqdn;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            lock (_syncRoot)
            {
                if (_source is null || !value.Equals(_source.FQDN, StringComparison.InvariantCultureIgnoreCase))
                {
                    _sourceFqdn = value;
                    _source = null;
                }
            }
        }
    }

    private SourceInfo? _source;
    
    /// <summary>
    /// The record representing the source ServiceNow instance.
    /// </summary>
    public SourceInfo? Source
    {
        get => _source;
        set
        {
            lock (_syncRoot)
            {
                if (value is null)
                {
                    if (_source is null)
                        return;
                    _sourceFqdn = _source.FQDN;
                }
                else
                {
                    if (_source is not null && ReferenceEquals(_source, value))
                        return;
                    _source = value;
                    _sourceFqdn = string.Empty;
                }
            }
        }
    }

    private HashSet<TableInfo> _derived = new();

    [NotNull]
    [BackingField(nameof(_derived))]
    public virtual HashSet<TableInfo> Derived { get => _derived; set => _derived = value ?? new(); }

    private HashSet<ElementInfo> _elements = new();

    [NotNull]
    [BackingField(nameof(_elements))]
    public virtual HashSet<ElementInfo> Elements { get => _elements; set => _elements = value ?? new(); }

    private HashSet<ElementInfo> _referredBy = new();

    [NotNull]
    [BackingField(nameof(_referredBy))]
    public virtual HashSet<ElementInfo> ReferredBy { get => _referredBy; set => _referredBy = value ?? new(); }

    internal string GetNamespace() => string.IsNullOrWhiteSpace(_scopeValue) ? AppSettings.DEFAULT_NAMESPACE : _scopeValue;

    internal string GetShortName()
    {
        if (string.IsNullOrWhiteSpace(_scopeValue) || _scopeValue == AppSettings.DEFAULT_NAMESPACE || !_name.StartsWith(_scopeValue))
            return _name;
        int len = _scopeValue.Length + 1;
        if (_name.Length <= len || _name[_scopeValue.Length] != '_')
            return _name;
        return _name[len..];
    }

    internal string GetGlideRecordTypeString(string targetNs)
    {
        if (string.IsNullOrWhiteSpace(_scopeValue) || _scopeValue == AppSettings.DEFAULT_NAMESPACE)
            return $"{NS_NAME_GlideRecord}.{Name}";
        if (targetNs == _scopeValue)
            return $"{NS_NAME_record}.{GetShortName()}";
        return $"{_scopeValue}.{NS_NAME_record}.{GetShortName()}";
    }

    internal string GetGlideElementTypeString(string targetNs)
    {
        if (string.IsNullOrWhiteSpace(_scopeValue) || _scopeValue == AppSettings.DEFAULT_NAMESPACE)
            return $"{NS_NAME_GlideElement}.{Name}";
        if (targetNs == _scopeValue)
            return $"{NS_NAME_element}.{GetShortName()}";
        return $"{_scopeValue}.{NS_NAME_element}.{GetShortName()}";
    }

    internal string GetInterfaceTypeString(string targetNs)
    {
        if (string.IsNullOrWhiteSpace(_scopeValue) || _scopeValue == AppSettings.DEFAULT_NAMESPACE)
            return $"{NS_NAME_tableFields}.{Name}";
        if (targetNs == _scopeValue)
            return $"{NS_NAME_fields}.{GetShortName()}";
        return $"{_scopeValue}.{NS_NAME_fields}.{GetShortName()}";
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(TableInfo)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(NumberPrefix)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(AccessibleFrom)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(ExtensionModel)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(IsExtendable)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(LastUpdated)}"" DATETIME NOT NULL DEFAULT {DEFAULT_SQL_NOW},
    ""{nameof(PackageName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SysPackage)}"" REFERENCES ""{nameof(SysPackage)}""(""{nameof(SysPackage.Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SuperClassName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SuperClass)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SourceFqdn)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SourceInfo)}"" REFERENCES ""{nameof(SourceInfo)}""(""{nameof(SourceInfo.FQDN)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(TableInfo)}"" PRIMARY KEY(""{nameof(Name)}""),
    CONSTRAINT ""UK_{nameof(TableInfo)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(SysID)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(IsExtendable)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(IsExtendable)}\")";
    }

    private async Task RenderFieldsAsync(IndentedTextWriter writer, TypingsDbContext dbContext, Func<ElementInfo, string, Task> renderPropertyAsync, Func<ElementInfo, string, Task> renderJsDocAsync, CancellationToken cancellationToken)
    {
        string @namespace = GetNamespace();
        var tableName = Name;
        EntityEntry<TableInfo> entry = dbContext.Tables.Entry(this);
        var elements = (await entry.GetRelatedCollectionAsync(e => e.Elements, cancellationToken)).ToArray();
        TableInfo? superClass = await entry.GetReferencedEntityAsync(e => e.SuperClass, cancellationToken);
        string extends;
        ElementInfo[] jsDocElements;
        if (superClass is not null)
        {
            extends = $"extends {superClass.GetInterfaceTypeString(@namespace)}{{";
            if (elements.Length > 0)
            {
                var baseElements = await superClass.GetRelatedCollectionAsync(dbContext.Tables, t => t.Elements, cancellationToken);
                if (baseElements.Any())
                {
                    var withBase = elements.GetBaseElements(baseElements);
                    jsDocElements = withBase.Where(a => a.Base is not null && !a.IsTypeOverride).Select(a => a.Inherited).ToArray();
                    elements = withBase.Where(a => a.Base is null || a.IsTypeOverride).Select(a => a.Inherited).ToArray();
                }
                else
                    jsDocElements = Array.Empty<ElementInfo>();
            }
            else
                jsDocElements = Array.Empty<ElementInfo>();
        }
        else if (elements.ExtendsBaseRecord())
        {
            jsDocElements = Array.Empty<ElementInfo>();
            elements = elements.GetNonBaseRecordElements().ToArray();
            extends = "extends IBaseRecord {";
        }
        else
        {
            jsDocElements = Array.Empty<ElementInfo>();
            extends = "{";
        }
        tableName = GetShortName();
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("/**");
        if (tableName == Name)
            await writer.WriteLineAsync($" * {((string.IsNullOrWhiteSpace(Label) || Label == tableName) ? tableName : Label.SmartQuoteJson())} glide record fields.");
        else
            await writer.WriteLineAsync($" * {((string.IsNullOrWhiteSpace(Label) || Label == tableName) ? tableName : Label.SmartQuoteJson())} ({Name}) glide record fields.");
        await writer.WriteLineAsync($" * @see {{@link {GetGlideRecordTypeString(@namespace)}}}");
        await writer.WriteLineAsync($" * @see {{@link {GetGlideElementTypeString(@namespace)}}}");
        await writer.WriteLineAsync(" */");
        if (elements.Length > 0 || jsDocElements.Length > 0)
        {
            await writer.WriteAsync($"export interface {tableName} {extends}");
            var indent = writer.Indent + 1;
            foreach (ElementInfo e in jsDocElements)
            {
                writer.Indent = indent;
                await renderJsDocAsync(e, @namespace);
            }
            foreach (ElementInfo e in elements)
            {
                writer.Indent = indent;
                await renderPropertyAsync(e, @namespace);
            }
            writer.Indent = indent - 1;
            await writer.WriteLineAsync("}");
        }
        else
            await writer.WriteLineAsync($"export interface {tableName} {extends} }}");
    }

    internal async Task RenderFieldsGlobalAsync(IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken) =>
        await RenderFieldsAsync(writer, dbContext, async (e, ns) => await e.RenderPropertyGlobalAsync(writer, ns, cancellationToken),
            async (e, ns) => await e.RenderJsDocGlobalAsync(writer, ns, cancellationToken), cancellationToken);

    internal async Task RenderFieldsScopedAsync(IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken) =>
        await RenderFieldsAsync(writer, dbContext, async (e, ns) => await e.RenderPropertyScopedAsync(writer, ns, cancellationToken),
            async (e, ns) => await e.RenderJsDocScopedAsync(writer, ns, cancellationToken), cancellationToken);
}