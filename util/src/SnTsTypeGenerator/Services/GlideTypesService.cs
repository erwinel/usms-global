using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

public class GlideTypesService
{
    private readonly ILogger<GlideTypesService> _logger;
    private readonly IFileInfo _jsClassMappingsFile;
    private readonly IFileInfo _glideTypesFile;
    private ReadOnlyDictionary<string, string>? _jsClassMap;
    private ReadOnlyDictionary<string, DefaultGlideType>? _defaultGlideTypes;

    private async Task<ReadOnlyDictionary<string, string>> GetClassMapAsync(CancellationToken cancellationToken)
    {
        Monitor.Enter(_jsClassMappingsFile);
        try
        {
            if (_jsClassMap is null)
            {
                Dictionary<string, string> map = new(NameComparer);
                if (_jsClassMappingsFile.Exists)
                {
                    try
                    {
                        using var stream = _jsClassMappingsFile.CreateReadStream();
                        var mappings = JsonSerializer.DeserializeAsyncEnumerable<JsClassMapping>(stream, JsonSerializerOptions.Default, cancellationToken);
                        int index = -1;
                        await foreach (var item in mappings)
                        {
                            index++;
                            if (item is null) continue;
                            if (item is null || string.IsNullOrWhiteSpace(item.JsClass) || string.IsNullOrWhiteSpace(item.PackageName))
                                _logger.LogInvalidJsClassMapping(_jsClassMappingsFile.Name, index);
                            else if (map.ContainsKey(item.JsClass))
                                _logger.LogDuplicateJsClassMapping(item.JsClass, _jsClassMappingsFile.Name, index);
                            else
                                map.Add(item.JsClass, item.PackageName);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogJsonFileAccessError(_jsClassMappingsFile.Name, exception);
                    }
                }
                foreach (var item in JsClassMapping.GetDefaultJsClassMappings())
                    if (!map.ContainsKey(item.JsClass))
                        map.Add(item.JsClass, item.PackageName);
                _jsClassMap = new(map);
            }
        }
        finally { Monitor.Exit(_jsClassMappingsFile); }
        return _jsClassMap;
    }

    internal async Task WithJavaClassMapAsync(Action<ReadOnlyDictionary<string, string>> action, CancellationToken cancellationToken) => action(await GetClassMapAsync(cancellationToken));

    internal async Task WithJavaClassMapAsync(Func<ReadOnlyDictionary<string, string>, Task> asyncAction, CancellationToken cancellationToken) => await asyncAction(await GetClassMapAsync(cancellationToken));

    internal async Task WithJavaClassMapAsync(Func<ReadOnlyDictionary<string, string>, CancellationToken, Task> asyncAction, CancellationToken cancellationToken) => await asyncAction(await GetClassMapAsync(cancellationToken), cancellationToken);

    internal async Task<TResult> FromJavaClassMapAsync<TResult>(Func<ReadOnlyDictionary<string, string>, TResult> func, CancellationToken cancellationToken) => func(await GetClassMapAsync(cancellationToken));

    internal async Task<TResult> FromJavaClassMapAsync<TResult>(Func<ReadOnlyDictionary<string, string>, Task<TResult>> asyncFunc, CancellationToken cancellationToken) => await asyncFunc(await GetClassMapAsync(cancellationToken));

    internal async Task<TResult> FromJavaClassMapAsync<TResult>(Func<ReadOnlyDictionary<string, string>, CancellationToken, Task<TResult>> asyncFunc, CancellationToken cancellationToken) => await asyncFunc(await GetClassMapAsync(cancellationToken), cancellationToken);

    internal async Task<string?> GetJavaClassAsync(string jsType, CancellationToken cancellationToken)
    {
        return (await GetClassMapAsync(cancellationToken)).TryGetValue(jsType, out var result) ? result : null;
    }

    private async Task<ReadOnlyDictionary<string, DefaultGlideType>> GetDefaultGlideTypeMapAsync(CancellationToken cancellationToken)
    {
        Monitor.Enter(_glideTypesFile);
        try
        {
            if (_defaultGlideTypes is null)
            {
                Dictionary<string, DefaultGlideType> map = new(NameComparer);
                if (_glideTypesFile.Exists)
                {
                    try
                    {
                        using var stream = _glideTypesFile.CreateReadStream();
                        var mappings = JsonSerializer.DeserializeAsyncEnumerable<KnownGlideType>(stream, JsonSerializerOptions.Default, cancellationToken);
                        int index = -1;
                        await foreach (var item in mappings)
                        {
                            index++;
                            if (item is null) continue;
                            if (item is null || string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.JsClass))
                                _logger.LogInvalidGlideTypeJson(_glideTypesFile.Name, index);
                            else if (map.ContainsKey(item.Name))
                                _logger.LogDuplicateGlideTypeJson(item.Name, _glideTypesFile.Name, index);
                            else
                                map.Add(item.JsClass, new DefaultGlideType(
                                    JsClass: item.JsClass,
                                    Label: item.Label.AsWhitespaceNormalizedOrDefaultIfEmpty(item.Name),
                                    ScalarType: item.ScalarType.NullIfWhiteSpace(),
                                    ScalarLength: item.ScalarLength,
                                    UnderlyingType: item.UnderlyingType.NullIfWhiteSpace(),
                                    Visible: item.Visible,
                                    DoNotUseOriginalValue: item.DoNotUseOriginalValue,
                                    CaseSensitive: item.CaseSensitive,
                                    EncodeUtf8: item.EncodeUtf8,
                                    OmitSysOriginal: item.OmitSysOriginal,
                                    EdgeEncryptionEnabled: item.EdgeEncryptionEnabled,
                                    Serializer: item.Serializer.NullIfWhiteSpace(),
                                    IsMultiText: item.IsMultiText,
                                    PdfCellType: item.PdfCellType.NullIfWhiteSpace(),
                                    NoSort: item.NoSort,
                                    NoDataReplicate: item.NoDataReplicate,
                                    NoAudit: item.NoAudit,
                                    Attributes: (item.Attributes?.Where(i => !string.IsNullOrWhiteSpace(i)).Distinct(NameComparer).ToArray() ?? Array.Empty<string>()).ToImmutableArray()));
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogJsonFileAccessError(_glideTypesFile.Name, exception);
                    }
                }
                foreach (var item in KnownGlideType.GetDefaultKnownTypes())
                    if (map.TryGetValue(item.Name, out DefaultGlideType? existing))
                    {
                        bool changed = existing.Label == item.Name && !string.IsNullOrWhiteSpace(item.Label);
                        if (changed)
                            existing = existing with { Label = item.Label! };
                        if (existing.ScalarType is null && !string.IsNullOrWhiteSpace(item.ScalarType))
                        {
                            changed = true;
                            existing = existing with { ScalarType = item.ScalarType };
                        }
                        if (!existing.ScalarLength.HasValue && item.ScalarLength.HasValue)
                        {
                            changed = true;
                            existing = existing with { ScalarLength = item.ScalarLength };
                        }
                        if (existing.UnderlyingType is null && !string.IsNullOrWhiteSpace(item.UnderlyingType))
                        {
                            changed = true;
                            existing = existing with { UnderlyingType = item.UnderlyingType };
                        }
                        if (!existing.Visible.HasValue && item.Visible.HasValue)
                        {
                            changed = true;
                            existing = existing with { Visible = item.Visible };
                        }
                        if (!existing.DoNotUseOriginalValue.HasValue && item.DoNotUseOriginalValue.HasValue)
                        {
                            changed = true;
                            existing = existing with { DoNotUseOriginalValue = item.DoNotUseOriginalValue };
                        }
                        if (!existing.CaseSensitive.HasValue && item.CaseSensitive.HasValue)
                        {
                            changed = true;
                            existing = existing with { CaseSensitive = item.CaseSensitive };
                        }
                        if (!existing.EncodeUtf8.HasValue && item.EncodeUtf8.HasValue)
                        {
                            changed = true;
                            existing = existing with { EncodeUtf8 = item.EncodeUtf8 };
                        }
                        if (!existing.OmitSysOriginal.HasValue && item.OmitSysOriginal.HasValue)
                        {
                            changed = true;
                            existing = existing with { OmitSysOriginal = item.OmitSysOriginal };
                        }
                        if (!existing.EdgeEncryptionEnabled.HasValue && item.EdgeEncryptionEnabled.HasValue)
                        {
                            changed = true;
                            existing = existing with { EdgeEncryptionEnabled = item.EdgeEncryptionEnabled };
                        }
                        if (existing.Serializer is null && !string.IsNullOrWhiteSpace(item.Serializer))
                        {
                            changed = true;
                            existing = existing with { Serializer = item.Serializer };
                        }
                        if (!existing.IsMultiText.HasValue && item.IsMultiText.HasValue)
                        {
                            changed = true;
                            existing = existing with { IsMultiText = item.IsMultiText };
                        }
                        if (string.IsNullOrWhiteSpace(existing.PdfCellType) && !string.IsNullOrWhiteSpace(item.PdfCellType))
                        {
                            changed = true;
                            existing = existing with { PdfCellType = item.PdfCellType };
                        }
                        if (!existing.NoSort.HasValue && item.NoSort.HasValue)
                        {
                            changed = true;
                            existing = existing with { NoSort = item.NoSort };
                        }
                        if (!existing.NoDataReplicate.HasValue && item.NoDataReplicate.HasValue)
                        {
                            changed = true;
                            existing = existing with { NoDataReplicate = item.NoDataReplicate };
                        }
                        if (!existing.NoAudit.HasValue && item.NoAudit.HasValue)
                        {
                            changed = true;
                            existing = existing with { NoAudit = item.NoAudit };
                        }

                        if (item.Attributes is not null && item.Attributes.Count > 0)
                        {
                            if (existing.Attributes.Length == 0)
                            {
                                changed = true;
                                existing = existing with { Attributes = item.Attributes.ToArray().ToImmutableArray() };
                            }
                            else
                            {
                                var comparer = StringComparer.InvariantCultureIgnoreCase;
                                var toAdd = item.Attributes.Where(s => !string.IsNullOrWhiteSpace(s) && !existing.Attributes.Contains(s, comparer)).ToArray();
                                if (toAdd.Length > 0)
                                {
                                    changed = true;
                                    existing = existing with { Attributes = item.Attributes.Concat(toAdd).ToArray().ToImmutableArray() };
                                }
                            }
                        }
                        if (changed)
                            map[item.Name] = existing;
                    }
                    else
                        map.Add(item.JsClass, new DefaultGlideType(
                            JsClass: item.JsClass,
                            Label: item.Label.AsWhitespaceNormalizedOrDefaultIfEmpty(item.Name),
                            ScalarType: item.ScalarType,
                            ScalarLength: item.ScalarLength,
                            UnderlyingType: item.UnderlyingType,
                            Visible: item.Visible,
                            DoNotUseOriginalValue: item.DoNotUseOriginalValue,
                            CaseSensitive: item.CaseSensitive,
                            EncodeUtf8: item.EncodeUtf8,
                            OmitSysOriginal: item.OmitSysOriginal,
                            EdgeEncryptionEnabled: item.EdgeEncryptionEnabled,
                            Serializer: item.Serializer,
                            IsMultiText: item.IsMultiText,
                            PdfCellType: item.PdfCellType,
                            NoSort: item.NoSort,
                            NoDataReplicate: item.NoDataReplicate,
                            NoAudit: item.NoAudit,
                            Attributes: (item.Attributes?.ToArray() ?? Array.Empty<string>()).ToImmutableArray()));
                _defaultGlideTypes = new(map);
            }
        }
        finally { Monitor.Exit(_glideTypesFile); }
        return _defaultGlideTypes;
    }

    internal async Task WithDefaultGlideTypeMapAsync(Action<ReadOnlyDictionary<string, DefaultGlideType>> action, CancellationToken cancellationToken) => action(await GetDefaultGlideTypeMapAsync(cancellationToken));

    internal async Task WithDefaultGlideTypeMapAsync(Func<ReadOnlyDictionary<string, DefaultGlideType>, Task> asyncAction, CancellationToken cancellationToken) => await asyncAction(await GetDefaultGlideTypeMapAsync(cancellationToken));

    internal async Task WithDefaultGlideTypeMapAsync(Func<ReadOnlyDictionary<string, DefaultGlideType>, CancellationToken, Task> asyncAction, CancellationToken cancellationToken) => await asyncAction(await GetDefaultGlideTypeMapAsync(cancellationToken), cancellationToken);

    internal async Task<TResult> FromDefaultGlideTypeMapAsync<TResult>(Func<ReadOnlyDictionary<string, DefaultGlideType>, TResult> func, CancellationToken cancellationToken) => func(await GetDefaultGlideTypeMapAsync(cancellationToken));

    internal async Task<TResult> FromDefaultGlideTypeMapAsync<TResult>(Func<ReadOnlyDictionary<string, DefaultGlideType>, Task<TResult>> asyncFunc, CancellationToken cancellationToken) => await asyncFunc(await GetDefaultGlideTypeMapAsync(cancellationToken));

    internal async Task<TResult> FromDefaultGlideTypeMapAsync<TResult>(Func<ReadOnlyDictionary<string, DefaultGlideType>, CancellationToken, Task<TResult>> asyncFunc, CancellationToken cancellationToken) => await asyncFunc(await GetDefaultGlideTypeMapAsync(cancellationToken), cancellationToken);

    internal async Task<DefaultGlideType?> GetDefaultGlideTypeAsync(string name, CancellationToken cancellationToken)
    {
        return (await GetDefaultGlideTypeMapAsync(cancellationToken)).TryGetValue(name, out var result) ? result : null;
    }

    public GlideTypesService(IHostEnvironment hostEnvironment, ILogger<GlideTypesService> logger)
    {
        _logger = logger;
        _jsClassMappingsFile = hostEnvironment.ContentRootFileProvider.GetFileInfo("JsClassMappings.json");
        _glideTypesFile = hostEnvironment.ContentRootFileProvider.GetFileInfo("GlideTypes.json");
    }
}