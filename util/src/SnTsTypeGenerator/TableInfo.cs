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

[Table(nameof(TableInfo))]
public class TableInfo
{
    private readonly object _syncRoot = new();

    private string _sysID = string.Empty;

    [NotNull]
    [BackingField(nameof(_sysID))]
    public string SysID
    {
        get => _sysID;
        set => _sysID = value ?? string.Empty;
    }

    private string _name = string.Empty;

    [Key]
    [BackingField(nameof(_name))]
    public string Name
    {
        get => _name;
        set => _name = value ?? string.Empty;
    }

    private string _label = string.Empty;

    [NotNull]
    [BackingField(nameof(_label))]
    public string Label
    {
        get => _label;
        set => _label = value ?? string.Empty;
    }
    
    public bool IsExtendable { get; set; }

    private string? _scopeValue;

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

    public string? NumberPrefix { get; set; }

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

    internal static void OnBuildEntity(EntityTypeBuilder<TableInfo> builder)
    {
        builder.HasKey(t => t.Name);
        builder.HasIndex(t => t.SysID).IsUnique();
        builder.HasOne(t => t.Scope).WithMany(s => s.Tables).HasForeignKey(t => t.ScopeValue).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.SuperClass).WithMany(s => s.Derived).HasForeignKey(t => t.SuperClassName).OnDelete(DeleteBehavior.Restrict);
    }

    internal static IEnumerable<string> GetDbInitCommands()
    {
        yield return @$"CREATE TABLE IF NOT EXISTS ""{nameof(TableInfo)}"" (
    ""{nameof(Name)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(Label)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(SysID)}"" NVARCHAR NOT NULL COLLATE NOCASE,
    ""{nameof(NumberPrefix)}"" NVARCHAR DEFAULT NULL COLLATE NOCASE,
    ""{nameof(IsExtendable)}"" BIT NOT NULL DEFAULT 0,
    ""{nameof(ScopeValue)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SysScope)}"" REFERENCES ""{nameof(SysScope)}""(""{nameof(SysScope.Value)}"") ON DELETE RESTRICT COLLATE NOCASE,
    ""{nameof(SuperClassName)}"" NVARCHAR DEFAULT NULL CONSTRAINT ""FK_{nameof(TableInfo)}_{nameof(SuperClass)}"" REFERENCES ""{nameof(TableInfo)}""(""{nameof(Name)}"") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT ""PK_{nameof(TableInfo)}"" PRIMARY KEY(""{nameof(Name)}""),
    CONSTRAINT ""UK_{nameof(TableInfo)}_{nameof(SysID)}"" UNIQUE(""{nameof(SysID)}"")
)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(SysID)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(SysID)}\" COLLATE NOCASE)";
        yield return $"CREATE INDEX \"IDX_{nameof(TableInfo)}_{nameof(IsExtendable)}\" ON \"{nameof(TableInfo)}\" (\"{nameof(IsExtendable)}\")";
    }

    private async Task RenderFieldsAsync(IndentedTextWriter writer, TypingsDbContext dbContext, Func<ElementInfo, string, Task> renderPropertyAsync, CancellationToken cancellationToken)
    {
        string @namespace = GetNamespace();
        var tableName = Name;
        EntityEntry<TableInfo> entry = dbContext.Tables.Entry(this);
        var elements = await entry.GetRelatedCollectionAsync(e => e.Elements, cancellationToken);
        TableInfo? superClass = await entry.GetReferencedEntityAsync(e => e.SuperClass, () => SuperClassName is not null, cancellationToken);
        string extends;
        if (superClass is not null)
        {
            extends = $"extends {superClass.GetInterfaceTypeString(@namespace)}{{";
            elements = elements.NewElements(await superClass.GetRelatedCollectionAsync(dbContext.Tables, t => t.Elements, cancellationToken));
        }
        else if (elements.ExtendsBaseRecord())
        {
            elements = elements.GetNonBaseRecordElements();
            extends = "extends IBaseRecord {";
        }
        else
            extends = "{";
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
        if (elements.Any())
        {
            await writer.WriteAsync($"export interface {tableName} {extends}");
            var indent = writer.Indent + 1;
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

    internal async Task RenderFieldsGlobalAsync(IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        await RenderFieldsAsync(writer, dbContext, async (e, ns) => await e.RenderPropertyGlobalAsync(writer, ns, cancellationToken), cancellationToken);
    }
    
    internal async Task RenderFieldsScopedAsync(IndentedTextWriter writer, TypingsDbContext dbContext, CancellationToken cancellationToken)
    {
        await RenderFieldsAsync(writer, dbContext, async (e, ns) => await e.RenderPropertyScopedAsync(writer, ns, cancellationToken), cancellationToken);
    }
}