using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public static class ExtensionMethods
{
    /// <summary>
    /// Indicates whether the entry exists in the target database.
    /// </summary>
    /// <param name="entry">The <see cref="EntityEntry"/> to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="entry"/> is not <see langword="null"/> and its <see cref="EntityEntry.State"/>
    /// is <see cref="EntityState.Unchanged"/> or <see cref="EntityState.Modified"/>; otherwise, <see langword="false"/>.</returns>
    public static bool ExistsInDb(this EntityEntry? entry)
    {
        if (entry is null)
            return false;
        return entry.State switch
        {
            EntityState.Unchanged or EntityState.Modified => true,
            _ => false,
        };
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related items for a navigation property.
    /// </summary>
    /// <param name="entry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity objects.</returns>
    public static async Task<IEnumerable<TProperty>> GetRelatedCollectionAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entry is null)
            return Enumerable.Empty<TProperty>();
        CollectionEntry<TEntity, TProperty> collectionEntry = entry.Collection(propertyExpression);
        if (!collectionEntry.IsLoaded && entry.ExistsInDb())
            await collectionEntry.LoadAsync(cancellationToken);
        return collectionEntry.CurrentValue ?? Enumerable.Empty<TProperty>();
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related items for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity objects.</returns>
    public static async Task<IEnumerable<TProperty>> GetRelatedCollectionAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return Enumerable.Empty<TProperty>();
        return await dbSet.Entry(entity).GetRelatedCollectionAsync(propertyExpression, cancellationToken);
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The entry related entity or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entry.Reference(propertyExpression);
        if (referenceEntry.TargetEntry is not null)
            return referenceEntry.TargetEntry;
        if (!referenceEntry.IsLoaded && referenceEntry.Metadata is INavigation nav && nav.ForeignKey.Properties.Any(p => entry.Property(p).CurrentValue is not null))
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.TargetEntry;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity object.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The entry related entity or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return null;
        return await dbSet.Entry(entity).GetReferencedEntryAsync(propertyExpression, cancellationToken);
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entityEntry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity object or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entry.Reference(propertyExpression);
        if (referenceEntry.TargetEntry is not null)
            return referenceEntry.CurrentValue;
        if (!referenceEntry.IsLoaded && referenceEntry.Metadata is INavigation nav && nav.ForeignKey.Properties.Any(p => entry.Property(p).CurrentValue is not null))
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.CurrentValue;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity object.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity object or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Func<TEntity, TProperty?> propertyAccessor, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return null;
        TProperty? result = propertyAccessor(entity);
        return (result is null) ? (await dbSet.Entry(entity).GetReferencedEntryAsync(Expression.Lambda<Func<TEntity, TProperty?>>(Expression.Call(propertyAccessor.Method)), cancellationToken))?.Entity : result;
    }

    /// <summary>
    /// Gets elements that do not inherit from a super class.
    /// </summary>
    /// <param name="source">The elements of the inheriting class.</param>
    /// <param name="inherited">The elements of the inherited class.</param>
    /// <returns>The elements from <paramref name="source"/> where <paramref name="inherited"/> has no element that  matching the same <see cref="ElementInfo.Name"/> and <see cref="ElementInfo.TypeName"/>.</returns>
    public static IEnumerable<ElementInfo> NewElements(this IEnumerable<ElementInfo> source, IEnumerable<ElementInfo>? inherited) => (inherited is null) ? source : source.Where(e =>
    {
        string n = e.Name;
        string t = e.TypeName;
        return !inherited.Any(i => i.Name  == n && i.TypeName == t);
    });
    
    /// <summary>
    /// Gets elements that inherit from a super class with different properties.
    /// </summary>
    /// <param name="source">The elements of the inheriting class.</param>
    /// <param name="inherited">The elements of the inherited class.</param>
    /// <returns>The elements from <paramref name="source"/> where <paramref name="inherited"/> matches an element with the <see cref="ElementInfo.Name"/> and <see cref="ElementInfo.TypeName"/>, but has at least one other property that differs.</returns>
    public static IEnumerable<ElementInfo> OverriddenElements(this IEnumerable<ElementInfo> source, IEnumerable<ElementInfo>? inherited) => (inherited is null) ? Enumerable.Empty<ElementInfo>() : source.Where(e =>
    {
        string n = e.Name;
        string t = e.TypeName;
        ElementInfo? ie = inherited.FirstOrDefault(i => i.Name  == n && i.TypeName == t);
        return ie is not null && (ie.Comments != e.Comments || ie.DefaultValue != e.DefaultValue || ie.IsActive != ie.IsActive || ie.IsDisplay != ie.IsDisplay ||
            ie.IsReadOnly != ie.IsReadOnly || ie.IsUnique != ie.IsUnique || ie.Label != ie.Label || ie.MaxLength != ie.MaxLength || ie.IsArray != ie.IsArray);
    });
    
    /// <summary>
    /// Gets the properties that aren't implemented by the IBaseRecord type.
    /// </summary>
    /// <param name="source">The source elements.</param>
    /// <returns>Elements representing properties that aren't implemented by the IBaseRecord type.</returns>
    public static IEnumerable<ElementInfo> GetNonBaseRecordElements(this IEnumerable<ElementInfo> source) => source.Where(e => e.Name switch
    {
        JSON_KEY_SYS_ID => e.TypeName != TYPE_NAME_GUID && e.IsPrimary,
        JSON_KEY_SYS_CREATED_BY or JSON_KEY_SYS_UPDATED_BY => e.TypeName != TYPE_NAME_STRING,
        JSON_KEY_SYS_CREATED_ON or JSON_KEY_SYS_UPDATED_ON => e.TypeName != TYPE_NAME_GLIDE_DATE_TIME,
        JSON_KEY_SYS_MOD_COUNT => e.TypeName != TYPE_NAME_INTEGER,
        _ => true,
    });
    
    /// <summary>
    /// Tests whether all fields implemented by the IBaseRecord type are present.
    /// </summary>
    /// <param name="source">The source elements.</param>
    /// <returns><see langword="true" /> if all fields implemented by the IBaseRecord type are present; otherwise <see langword="false" />.</returns>
    public static bool ExtendsBaseRecord(this IEnumerable<ElementInfo>? elements) => elements is not null &&
        elements.Any(e => e.Name == JSON_KEY_SYS_ID && e.TypeName == TYPE_NAME_GUID && e.IsPrimary) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_BY && e.TypeName == TYPE_NAME_STRING) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_ON && e.TypeName == TYPE_NAME_GLIDE_DATE_TIME) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_MOD_COUNT && e.TypeName == TYPE_NAME_INTEGER) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_BY && e.TypeName == TYPE_NAME_STRING) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_ON && e.TypeName == TYPE_NAME_GLIDE_DATE_TIME);
    
    /// <summary>
    /// Surrounds a string with quotes if it contains spaces or specific symbols.
    /// </summary>
    /// <param name="value">The source string value.</param>
    /// <returns>A JSON-quoted string if the value contains spaces or special characters.</returns>
    public static string SmartQuoteJson(this string? value)
    {
        if (value is null)
            return "null";
        if (value.Length == 0)
            return "\"\"";
        string result = JsonValue.Create(value)!.ToJsonString();
        return (!value.Any(c => char.IsWhiteSpace(c)) && value.Length == result.Length - 2) ? value : result;
    }

    public static async Task<JsonNode?> GetAPIJsonResult(this HttpClientHandler handler, Uri requestUri, ILogger logger, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        logger.LogAPIRequestStart(requestUri);
        using HttpClient httpClient = new(handler);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception) { throw new RequestFailedException(requestUri, exception); }
        string responseBody;
        try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
        catch (Exception exception) { throw new GetResponseContentFailedException(requestUri, exception); }
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(responseBody)) { throw new InvalidHttpResponseException(requestUri, responseBody); }
        JsonNode? result;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            result = doc.RootElement.ValueKind switch
            {
                JsonValueKind.Undefined or JsonValueKind.Null => null,
                JsonValueKind.Array => JsonArray.Create(doc.RootElement),
                JsonValueKind.Object => JsonObject.Create(doc.RootElement),
                _ => JsonValue.Create(doc.RootElement),
            };
        }
        catch (JsonException exception) { throw new ResponseParsingException(requestUri, responseBody, exception); }
        logger.LogAPIRequestCompleted(requestUri, result);
        return result;
    }
    
    /// <summary>
    /// Creates a table API URI.
    /// </summary>
    /// <param name="baseUri">The base URI.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="element">The element name.</param>
    /// <param name="value">The element value.</param>
    /// <returns>A <see cref="URI"/> to look up a table based upon a element name and value.</returns>
    public static Uri ToTableApiUri(this Uri baseUri, string tableName, string element, string value)
    {
        value = Uri.EscapeDataString($"{element}={value}");
        return new UriBuilder(baseUri)
        {
            Path = $"{URI_PATH_API}/{tableName}",
            Query = $"{URI_PARAM_QUERY}={value}&{URI_PARAM_DISPLAY_VALUE}=all",
            Fragment = null
        }.Uri;
    }

    public static Uri ToTableApiUri(this Uri baseUri, string tableName, string id)
    {
        return new UriBuilder(baseUri)
        {
            Path = $"{URI_PATH_API}/{tableName}/{Uri.EscapeDataString(id)}",
            Query = $"{URI_PARAM_DISPLAY_VALUE}=all",
            Fragment = null
        }.Uri;
    }
    
    // public static async Task<string?> GetJsonResponseAsync(this HttpClientHandler? clientHandler, Uri requestUri, ILogger logger, CancellationToken cancellationToken)
    // {
    //     if (clientHandler is null)
    //         return null;
    //     using HttpClient httpClient = new(clientHandler);
    //     HttpRequestMessage message = new(HttpMethod.Get, requestUri);
    //     message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
    //     using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
    //     if (cancellationToken.IsCancellationRequested)
    //         return null;
    //     try { response.EnsureSuccessStatusCode(); }
    //     catch (HttpRequestException exception)
    //     {
    //         logger.LogHttpRequestFailed(requestUri, exception);
    //         return null;
    //     }
    //     try { return await response.Content.ReadAsStringAsync(cancellationToken); }
    //     catch (Exception exception)
    //     {
    //         logger.LogGetResponseContentFailed(requestUri, exception);
    //         return null;
    //     }
    // }

    public static bool TryGetPropertyValue(this JsonObject source, string propertyName, string innerPropertyName, out JsonNode? jsonNode) =>
        source.TryGetPropertyValue(propertyName, out jsonNode) && jsonNode is JsonObject obj && obj.TryGetPropertyValue(innerPropertyName, out jsonNode);
    
    public static bool TryGetPropertyAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
            return jsonValue.TryGetValue(out result);
        result = null;
        return false;
    }

    public static bool TryGetPropertyAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
            return jsonValue.TryGetValue(out result) && !string.IsNullOrWhiteSpace(result);
        result = null;
        return false;
    }

    public static string? GetPropertyAsString(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue && jsonValue.TryGetValue(out string? result)) ? result : null;

    public static string GetPropertyAsNonEmpty(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue && jsonValue.TryGetValue(out string? result) && !string.IsNullOrWhiteSpace(result)) ? result : string.Empty;

    public static bool TryCoercePropertyAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out result))
                result = jsonValue.ToString();
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryCoercePropertyAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out result))
                result = jsonValue.ToString();
            return !string.IsNullOrWhiteSpace(result);
        }
        result = null;
        return false;
    }

    public static string? CoercePropertyAsStringOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? (jsonValue.TryGetValue(out string? result) ? result : jsonValue.ToString()) : null;

    public static string? CoercePropertyAsNonEmptyOrNull(this JsonObject source, string propertyName)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out string? result))
                result = jsonValue.ToString();
            if (!string.IsNullOrWhiteSpace(result))
                return result;
        }
        return null;
    }

    public static string CoercePropertyAsString(this JsonObject source, string propertyName, string defaultValue = "") =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? (jsonValue.TryGetValue(out string? result) ? result : jsonValue.ToString()) : defaultValue;

    public static string CoercePropertyAsNonEmpty(this JsonObject source, string propertyName, string defaultValue = "")
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (!jsonValue.TryGetValue(out string? result))
                result = jsonValue.ToJsonString();
            if (!string.IsNullOrWhiteSpace(result))
                return result;
        }
        return defaultValue;
    }

    public static bool TryCoercePropertyAsInt(this JsonObject source, string propertyName, [NotNullWhen(true)] out int? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue(out result))
                return true;
            if (jsonValue.TryGetValue(out string? s) && int.TryParse(s, out int i))
            {
                result = i;
                return true;
            }
        }
        result = null;
        return false;
    }

    public static int? CoercePropertyAsIntOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? (jsonValue.TryGetValue(out int? result) ? result :
            (jsonValue.TryGetValue(out string? s) && int.TryParse(s, out int i)) ? i : null) : null;

    public static int CoercePropertyAsInt(this JsonObject source, string propertyName, int defaultValue = 0) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? (jsonValue.TryGetValue(out int? result) ? result.Value :
            (jsonValue.TryGetValue(out string? s) && int.TryParse(s, out int i)) ? i : defaultValue) : defaultValue;
    
    public static bool TryCoercePropertyAsBoolean(this JsonObject source, string propertyName, [NotNullWhen(true)] out bool? result)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue(out result))
                return true;
            if (jsonValue.TryGetValue(out string? s) && bool.TryParse(s, out bool b))
            {
                result = b;
                return true;
            }
        }
        result = null;
        return false;
    }

    public static bool? CoercePropertyAsBooleanOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? (jsonValue.TryGetValue(out bool? result) ? result :
            (jsonValue.TryGetValue(out string? s) && bool.TryParse(s, out bool b)) ? b : null) : null;

    public static bool CoercePropertyAsBoolean(this JsonObject source, string propertyName, bool defaultValue = false) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue jsonValue) ? (jsonValue.TryGetValue(out bool? result) ? result.Value :
            (jsonValue.TryGetValue(out string? s) && bool.TryParse(s, out bool b)) ? b : defaultValue) : defaultValue;
    
    public static bool TryGetFieldAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
            display_value = null;
        }
        else
            value = display_value = null;
        return false;
    }

    public static bool TryGetFieldAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
            display_value = null;
        }
        else
            value = display_value = null;
        return false;
    }

    public static bool TryGetFieldAsString(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsString(JSON_KEY_VALUE, out value);
        value = null;
        return false;
    }

    public static bool TryGetFieldAsNonEmpty(this JsonObject source, string propertyName, [NotNullWhen(true)] out string? value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsString(JSON_KEY_VALUE, out value);
        value = null;
        return false;
    }

    public static string? GetFieldAsStringOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static string? GetFieldAsNonEmptyOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static string? GetFieldAsStringOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsStringOrNull(JSON_KEY_VALUE) : null;

    public static string? GetFieldAsNonEmptyOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsStringOrNull(JSON_KEY_VALUE) : null;

    public static string GetFieldAsString(this JsonObject source, string propertyName, string defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static string GetFieldAsNonEmpty(this JsonObject source, string propertyName, string defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static string GetFieldAsString(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return string.Empty;
    }

    public static string GetFieldAsNonEmpty(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsString(JSON_KEY_VALUE, out string? value))
                return value;
        }
        else
            display_value = null;
        return string.Empty;
    }

    public static string GetFieldAsString(this JsonObject source, string propertyName, string defaultValue = "") =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.GetFieldAsString(JSON_KEY_VALUE, defaultValue) : defaultValue;

    public static string GetFieldAsNonEmpty(this JsonObject source, string propertyName, string defaultValue = "") =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.GetFieldAsString(JSON_KEY_VALUE, defaultValue) : defaultValue;

    public static bool TryGetFieldAsInt(this JsonObject source, string propertyName, [NotNullWhen(true)] out int? value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
        }
        else
            value = null;
        display_value = null;
        return false;
    }

    public static bool TryGetFieldAsInt(this JsonObject source, string propertyName, [NotNullWhen(true)] out int? value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out value);
        value = null;
        return false;
    }

    public static int? GetFieldAsIntOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out int? value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static int? GetFieldAsIntOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsIntOrNull(JSON_KEY_VALUE) : null;

    public static int GetFieldAsInt(this JsonObject source, string propertyName, int defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out int? value))
                return value.Value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static int GetFieldAsInt(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsInt(JSON_KEY_VALUE, out int? value))
                return value.Value;
        }
        else
            display_value = null;
        return 0;
    }

    public static int GetFieldAsInt(this JsonObject source, string propertyName, int defaultValue = 0) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.GetFieldAsInt(JSON_KEY_VALUE, defaultValue) : defaultValue;

    public static bool TryGetFieldAsBoolean(this JsonObject source, string propertyName, [NotNullWhen(true)] out bool? value, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out value))
            {
                display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
                return true;
            }
        }
        else
            value = null;
        display_value = null;
        return false;
    }

    public static bool TryGetFieldAsBoolean(this JsonObject source, string propertyName, [NotNullWhen(true)] out bool? value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
            return field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out value);
        value = null;
        return false;
    }

    public static bool? GetFieldAsBooleanOrNull(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool? value))
                return value;
        }
        else
            display_value = null;
        return null;
    }

    public static bool? GetFieldAsBooleanOrNull(this JsonObject source, string propertyName) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field) ? field.CoercePropertyAsBooleanOrNull(JSON_KEY_VALUE) : null;

    public static bool GetFieldAsBoolean(this JsonObject source, string propertyName, bool defaultValue, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool? value))
                return value.Value;
        }
        else
            display_value = null;
        return defaultValue;
    }

    public static bool GetFieldAsBoolean(this JsonObject source, string propertyName, out string? display_value)
    {
        if (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field)
        {
            display_value = field.TryCoercePropertyAsString(JSON_KEY_DISPLAY_VALUE, out string? s) ? s : null;
            if (field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool? value))
                return value.Value;
        }
        else
            display_value = null;
        return false;
    }

    public static bool GetFieldAsBoolean(this JsonObject source, string propertyName, bool defaultValue = false) =>
        (source.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject field && field.TryCoercePropertyAsBoolean(JSON_KEY_VALUE, out bool? value)) ? value.Value : defaultValue;

    public static string ToDisplayName(this HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.Continue => "client can continue with its request",
        HttpStatusCode.SwitchingProtocols => "Switching Protocols",
        HttpStatusCode.EarlyHints => "Early Hints",
        HttpStatusCode.NonAuthoritativeInformation => "Non Authoritative Information",
        HttpStatusCode.NoContent => "No Content",
        HttpStatusCode.ResetContent => "Reset Content",
        HttpStatusCode.PartialContent => "Partial Content",
        HttpStatusCode.MultiStatus => "Multi Status",
        HttpStatusCode.AlreadyReported => "Already Reported",
        HttpStatusCode.IMUsed => "IM Used",
        HttpStatusCode.RedirectMethod => "Redirect Method",
        HttpStatusCode.NotModified => "Not Modified",
        HttpStatusCode.UseProxy => "Use Proxy",
        HttpStatusCode.RedirectKeepVerb => "Redirect Keep Verb",
        HttpStatusCode.PermanentRedirect => "Permanent Redirect",
        HttpStatusCode.BadRequest => "Bad Request",
        HttpStatusCode.PaymentRequired => "Payment Required",
        HttpStatusCode.NotFound => "Not Found",
        HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
        HttpStatusCode.NotAcceptable => "Not Acceptable",
        HttpStatusCode.ProxyAuthenticationRequired => "Proxy Authentication Required",
        HttpStatusCode.RequestTimeout => "Request Timeout",
        HttpStatusCode.LengthRequired => "Length Required",
        HttpStatusCode.PreconditionFailed => "Precondition Failed",
        HttpStatusCode.RequestEntityTooLarge => "Request Entity Too Large",
        HttpStatusCode.RequestUriTooLong => "Request URI Too Long",
        HttpStatusCode.UnsupportedMediaType => "Unsupported Media Type",
        HttpStatusCode.RequestedRangeNotSatisfiable => "Requested Range Not Satisfiable",
        HttpStatusCode.ExpectationFailed => "Expectation Failed",
        HttpStatusCode.MisdirectedRequest => "Misdirected Request",
        HttpStatusCode.UnprocessableEntity => "Unprocessable Entity",
        HttpStatusCode.FailedDependency => "Failed Dependency",
        HttpStatusCode.UpgradeRequired => "Upgrade Required",
        HttpStatusCode.PreconditionRequired => "Precondition Required",
        HttpStatusCode.TooManyRequests => "Too Many Requests",
        HttpStatusCode.RequestHeaderFieldsTooLarge => "Request Header Fields Too Large",
        HttpStatusCode.UnavailableForLegalReasons => "Unavailable For Legal Reasons",
        HttpStatusCode.InternalServerError => "Internal Server Error",
        HttpStatusCode.NotImplemented => "Not Implemented",
        HttpStatusCode.BadGateway => "Bad Gateway",
        HttpStatusCode.ServiceUnavailable => "Service Unavailable",
        HttpStatusCode.GatewayTimeout => "Gateway Timeout",
        HttpStatusCode.HttpVersionNotSupported => "Http Version Not Supported",
        HttpStatusCode.VariantAlsoNegotiates => "Variant Also Negotiates",
        HttpStatusCode.InsufficientStorage => "Insufficient Storage",
        HttpStatusCode.LoopDetected => "Loop Detected",
        HttpStatusCode.NotExtended => "Not Extended",
        HttpStatusCode.NetworkAuthenticationRequired => "Network Authentication Required",
        _ => statusCode.ToString("F"),
    };

    public static string? ToDescription(this HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.Continue => "client can continue with its request",
        HttpStatusCode.SwitchingProtocols => "Protocol version or protocol is being changed.",
        HttpStatusCode.Processing => "Server has accepted the complete request but hasn't completed it yet.",
        HttpStatusCode.EarlyHints => "Server is likely to send a final response with the header fields included in the informational response.",
        HttpStatusCode.OK => "Request succeeded.",
        HttpStatusCode.Created => "New resource created before response was sent.",
        HttpStatusCode.Accepted => "Request accepted for further processing.",
        HttpStatusCode.NonAuthoritativeInformation => "Returned meta information is from a cached copy.",
        HttpStatusCode.NoContent => "Request has been successfully processed; Response is intentionally blank.",
        HttpStatusCode.ResetContent => "Client should reset (not reload) the current resource.",
        HttpStatusCode.PartialContent => "Response is partial.",
        HttpStatusCode.MultiStatus => "Multiple status codes for single response from WebDAV operation.",
        HttpStatusCode.AlreadyReported => "Members of WebDAV binding previously enumerated in preceding multistatus response not included.",
        HttpStatusCode.IMUsed => "Response represents result of one or more instance-manipulations applied to current instance.",
        HttpStatusCode.Ambiguous => "Requested information has multiple representations.",
        HttpStatusCode.Moved => "Requested information moved to URI specified in Location header.",
        HttpStatusCode.Found or HttpStatusCode.RedirectKeepVerb or HttpStatusCode.PermanentRedirect => "Requested information is located at URI specified in Location header.",
        HttpStatusCode.RedirectMethod => "Redirect to the specified in Location header.",
        HttpStatusCode.NotModified => "Cached copy is up to date.",
        HttpStatusCode.UseProxy => "Use proxy server at URI specified in Location header.",
        HttpStatusCode.Unused => "Extension to HTTP/1.1 specification not fully specified.",
        HttpStatusCode.BadRequest => "Request not understood.",
        HttpStatusCode.Unauthorized => "Requested resource requires authentication.",
        HttpStatusCode.Forbidden => "Request fulfillment refused.",
        HttpStatusCode.NotFound => "Requested resource does not exist.",
        HttpStatusCode.MethodNotAllowed => "Request method not allowed on requested resource.",
        HttpStatusCode.NotAcceptable => "Accept headers not available as representation of resource.",
        HttpStatusCode.ProxyAuthenticationRequired => "Requested proxy requires authentication.",
        HttpStatusCode.RequestTimeout => "Request not sent within expected timeframe.",
        HttpStatusCode.Conflict => "Request not carried out due to conflict.",
        HttpStatusCode.Gone => "Requested resource no longer available.",
        HttpStatusCode.LengthRequired => "Content-length header missing.",
        HttpStatusCode.RequestedRangeNotSatisfiable => "Range of requested data cannot be returned.",
        HttpStatusCode.ExpectationFailed => "Expect header not be met by server.",
        HttpStatusCode.MisdirectedRequest => "Request directed at server that is not able to produce a response.",
        HttpStatusCode.UnprocessableEntity => "Well-formed request unable to be followed due to semantic errors.",
        HttpStatusCode.Locked => "Source or destination resource is locked.",
        HttpStatusCode.FailedDependency => "Method couldn't be performed on resource due to dependency upon another action.",
        HttpStatusCode.UpgradeRequired => "Client should switch to a different protocol.",
        HttpStatusCode.PreconditionRequired => "Request must be conditional.",
        HttpStatusCode.NotImplemented => "Requested function not supported.",
        HttpStatusCode.BadGateway => "Proxy server received a bad response from another proxy or the origin server.",
        HttpStatusCode.ServiceUnavailable => "Server temporarily unavailable.",
        HttpStatusCode.GatewayTimeout => "Proxy server timed out while waiting for a response.",
        HttpStatusCode.VariantAlsoNegotiates => "Chosen variant resource not a proper endpoint in negotiation process because is configured to engage in transparent content negotiation.",
        HttpStatusCode.InsufficientStorage => "Server is unable to store the representation needed to complete the request.",
        HttpStatusCode.LoopDetected => "Operation terminated due to infinite loop.",
        HttpStatusCode.NotExtended => "Further request extensions required for fulfillment.",
        HttpStatusCode.NetworkAuthenticationRequired => "Authentication required for network access.",
        _ => null,
    };
}
