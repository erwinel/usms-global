using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
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
            Path = $"{URI_PATH_API}/{tableName}/{id}",
            Query = $"{URI_PARAM_DISPLAY_VALUE}=all",
            Fragment = null
        }.Uri;
    }
    
    public static async Task<string?> GetJsonResponseAsync(this HttpClientHandler? clientHandler, Uri requestUri, ILogger logger, CancellationToken cancellationToken)
    {
        if (clientHandler is null)
            return null;
        using HttpClient httpClient = new(clientHandler);
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception)
        {
            logger.LogHttpRequestFailed(requestUri, exception);
            return null;
        }
        try { return await response.Content.ReadAsStringAsync(cancellationToken); }
        catch (Exception exception)
        {
            logger.LogGetResponseContentFailed(requestUri, exception);
            return null;
        }
    }

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

    /// <summary>
    /// Gets the target of a link property.
    /// </summary>
    /// <param name="clientHandler">The HTTP message handler.</param>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The link property name.</param>
    /// <param name="lookupFunc">The function that retrieves the associated return value from the given link value.</param>
    /// <param name="createFunc">The function that creates the return value from the results of a remote query.</param>
    /// <param name="logger">The logger to use.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="T">The return object type.</typeparam>
    // BUG: Need add sysparm_display_value=all query parameter to request URI so the returned the property schema is { display_value: string, value: string, link?: string; }
    [Obsolete("Do not use")]
    public static async Task<T?> GetLinkedObjectAsync<T>(this HttpClientHandler clientHandler, JsonElement element, string name, Func<string, Task<T?>> lookupFunc, Func<JsonElement, Task<T?>> createFunc, ILogger logger, CancellationToken cancellationToken) where T : class
    {
        if (!element.TryGetProperty(name, out JsonElement linkElement) || linkElement.ValueKind != JsonValueKind.Object)
            return null;
        string? link;
        T? result;
        Uri? uri;
        string responseBody;
        if (linkElement.TryGetPropertyAsNonEmptyString(JSON_KEY_VALUE, out string? value))
        {
            if ((result = await lookupFunc(value)) is not null)
                return result;
            if (!(linkElement.TryGetPropertyAsNonEmptyString(JSON_KEY_LINK, out link) && Uri.TryCreate(link, UriKind.Absolute, out uri)))
                return null;
            using HttpClient client = new(clientHandler);
            HttpRequestMessage msg = new(HttpMethod.Get, uri);
            msg.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await client.SendAsync(msg, cancellationToken);
            try { response.EnsureSuccessStatusCode(); }
            catch (HttpRequestException exception)
            {
                logger.LogHttpRequestFailed(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
            try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception exception)
            {
                logger.LogGetResponseContentFailed(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
        }
        else if (linkElement.TryGetPropertyAsNonEmptyString(JSON_KEY_LINK, out link) && Uri.TryCreate(link, UriKind.Absolute, out uri))
        {
            using HttpClient client = new(clientHandler);
            HttpRequestMessage msg = new(HttpMethod.Get, uri);
            msg.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await client.SendAsync(msg, cancellationToken);
            try { response.EnsureSuccessStatusCode(); }
            catch (HttpRequestException exception)
            {
                logger.LogHttpRequestFailed(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
            try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception exception)
            {
                logger.LogGetResponseContentFailed(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
        }
        else
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            logger.LogInvalidHttpResponse(uri, responseBody);
            return null;
        }
        
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            logger.LogJsonCouldNotBeParsed(uri, responseBody, exception);
            return null;
        }
        using (doc)
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
                return await createFunc(doc.RootElement);
        }
        return null;
    }

    [Obsolete("Do not use")]
    public static bool TryGetProperty(this JsonElement jsonObj, string propertyName, string innerPropertyName, out JsonElement value) => jsonObj.TryGetProperty(propertyName, out value) && jsonObj.TryGetProperty(innerPropertyName, out value);

    [Obsolete("Bad name")]
    public static bool TryGetPropertyValueElement(this JsonElement jsonObj, string propertyName, [NotNullWhen(true)] out JsonElement valueElement) =>
        jsonObj.TryGetProperty(propertyName, out valueElement) && valueElement.ValueKind == JsonValueKind.Object && valueElement.TryGetProperty(JSON_KEY_VALUE, out valueElement);

    [Obsolete("Bad name")]
    public static bool GetNestedValueElementAsBoolean(this JsonElement source, string name, bool defaultValue = false) => source.TryGetPropertyValueElement(name, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0,
        _ => defaultValue,
    } : defaultValue;

    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsBoolean(this JsonElement jsonObj, string propertyName, [NotNullWhen(true)] out bool result)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.True:
                    result = true;
                    return true;
                case JsonValueKind.False:
                    result = false;
                    return true;
                case JsonValueKind.Number:
                    result = element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0;
                    return true;
            }
        result = false;
        return false;
    }

    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsBoolean(this JsonElement jsonObj, string propertyName, string innerPropertyName, [NotNullWhen(true)] out bool result)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, innerPropertyName, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.True:
                    result = true;
                    return true;
                case JsonValueKind.False:
                    result = false;
                    return true;
                case JsonValueKind.Number:
                    result = element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0;
                    return true;
            }
        result = false;
        return false;
    }
    
    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsBoolean(this JsonElement jsonObj, string propertyName, [NotNullWhen(true)] out bool value, out string? display_value)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element) && element.TryGetPropertyAsBoolean(JSON_KEY_VALUE, out value))
        {
            display_value = element.TryGetProperty(JSON_KEY_DISPLAY_VALUE, out JsonElement d) ? d.GetString() : null;
            return true;
        }
        value = false;
        display_value = null;
        return false;
    }

    /// <summary>
    /// Gets the boolean value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="defaultValue">The default value to use if the property does not exist or it is not <see cref="JsonValueKind.True"/> or <see cref="JsonValueKind.False"/></param>
    /// <returns><see langword="true" /> if the property exists and it is <see cref="JsonValueKind.True"/>; <see langword="false" /> if it is <see cref="JsonValueKind.False"/>; otherwise, the value of <paramref name="defaultValue"/>.</returns>
    [Obsolete("Do not use")]
    public static bool GetPropertyAsBoolean(this JsonElement jsonObj, string propertyName, bool defaultValue = false) => jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0,
        _ => defaultValue,
    } : defaultValue;

    [Obsolete("Do not use")]
    public static bool GetPropertyAsBoolean(this JsonElement jsonObj, string propertyName, string innerPropertyName, bool defaultValue = false) => jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, innerPropertyName, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0,
        _ => defaultValue,
    } : defaultValue;

    /// <summary>
    /// Tries to get the string value of a property.
    /// </summary>
    /// <param name="jsonObj">The source JSON object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="result">The string value of the property or <see langword="null" /> if the property does not exist or it is not a <see cref="JsonValueKind.String"/>.</param>
    /// <returns><see langword="true" /> if the property exists and it is a <see cref="JsonValueKind.String"/>; otherwise, <see langword="false" />.</returns>
    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsString(this JsonElement jsonObj, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element) && element.ValueKind == JsonValueKind.String)
            return (result = element.GetString()) is not null;
        result = null;
        return false;
    }

    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsString(this JsonElement jsonObj, string propertyName, string innerPropertyName, [NotNullWhen(true)] out string? result)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, innerPropertyName, out JsonElement element) && element.ValueKind == JsonValueKind.String)
            return (result = element.GetString()) is not null;
        result = null;
        return false;
    }

    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsString(this JsonElement jsonObj, string propertyName, [NotNullWhen(true)] out string? value, out string? display_value)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element) && element.TryGetPropertyAsString(JSON_KEY_VALUE, out value))
        {
            display_value = element.TryGetProperty(JSON_KEY_DISPLAY_VALUE, out JsonElement d) ? d.GetString() : null;
            return true;
        }
        value = display_value = null;
        return false;
    }

    /// <summary>
    /// Gets the string value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="defaultValue">The default value to use if the property does not exist or it is not a <see cref="JsonValueKind.String"/>.</param>
    /// <returns>The value of the property if it exists and it is a <see cref="JsonValueKind.String"/>; otherwise, the value of <paramref name="defaultValue"/>.</returns>
    [Obsolete("Do not use")]
    public static string GetPropertyAsString(this JsonElement source, string propertyName, string defaultValue = "") => source.ValueKind == JsonValueKind.Object && source.TryGetProperty(propertyName, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.Null or JsonValueKind.Undefined => defaultValue,
        _ => element.GetString() ?? defaultValue,
    } : defaultValue;

    [Obsolete("Do not use")]
    public static string GetPropertyAsString(this JsonElement source, string propertyName, string innerPropertyName, string defaultValue) => source.ValueKind == JsonValueKind.Object && source.TryGetProperty(propertyName, innerPropertyName, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.Null or JsonValueKind.Undefined => defaultValue,
        _ => element.GetString() ?? defaultValue,
    } : defaultValue;

    [Obsolete("Bad name")]
    public static bool TryGetNestedValueElementAsNonEmptyString(this JsonElement source, string name, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetPropertyValueElement(name, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    break;
                default:
                    var s = element.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        result = s;
                        return true;
                    }
                    break;
            }
            result = null;
            return false;
    }
    /// <summary>
    /// Tries to get a non-empty string property value.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="result">The non-empty string value of the property or <see langword="null" /> if the property does not exist, is not a <see cref="JsonValueKind.String"/> or is empty.</param>
    /// <returns><see langword="true" /> if the property exists, is a <see cref="JsonValueKind.String"/>, and is not empty; otherwise, <see langword="false" />.</returns>
    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsNonEmptyString(this JsonElement source, string name, [NotNullWhen(true)] out string? result)
    {
        if (source.ValueKind == JsonValueKind.Object && source.TryGetProperty(name, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    break;
                default:
                    var s = element.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        result = s;
                        return true;
                    }
                    break;
            }
            result = null;
            return false;
    }

    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsNonEmptyString(this JsonElement source, string propertyName, string innerPropertyName, [NotNullWhen(true)] out string? result)
    {
        if (source.ValueKind == JsonValueKind.Object && source.TryGetProperty(propertyName, innerPropertyName, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    break;
                default:
                    var s = element.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        result = s;
                        return true;
                    }
                    break;
            }
            result = null;
            return false;
    }

    [Obsolete("Do not use")]
    public static bool TryGetPropertyAsNonEmptyString(this JsonElement jsonObj, string propertyName, [NotNullWhen(true)] out string? value, out string? display_value)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element) && element.TryGetPropertyAsString(JSON_KEY_VALUE, out value) && !string.IsNullOrWhiteSpace(value))
        {
            display_value = element.TryGetProperty(JSON_KEY_DISPLAY_VALUE, out JsonElement d) ? d.GetString() : null;
            return true;
        }
        value = display_value = null;
        return false;
    }

    [Obsolete("Bad name")]
    public static string GetNestedValueElementAsNonEmptyString(this JsonElement source, string name, string defaultValue)
    {
        if (source.TryGetPropertyValueElement(name, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return defaultValue;
                default:
                    var s = element.GetString();
                    return string.IsNullOrWhiteSpace(s) ? defaultValue : s;
            }
        return defaultValue;
    }

    /// <summary>
    /// Gets the string value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="defaultValue">The default value to use if the property does not exist, is not a <see cref="JsonValueKind.String"/>, or is empty.</param>
    /// <returns>The value of the property if it exists, is a <see cref="JsonValueKind.String"/>, and is not empty; otherwise, the value of <paramref name="defaultValue"/>.</returns>
    [Obsolete("Do not use")]
    public static string GetPropertyAsNonEmptyString(this JsonElement jsonObj, string propertyName, string defaultValue)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return defaultValue;
                default:
                    var s = element.GetString();
                    return string.IsNullOrWhiteSpace(s) ? defaultValue : s;
            }
        return defaultValue;
    }
    
    [Obsolete("Do not use")]
    public static string GetPropertyAsNonEmptyString(this JsonElement jsonObj, string propertyName, string innerPropertyName, string defaultValue)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, innerPropertyName, out JsonElement element))
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return defaultValue;
                default:
                    var s = element.GetString();
                    return string.IsNullOrWhiteSpace(s) ? defaultValue : s;
            }
        return defaultValue;
    }
    
    [Obsolete("Do not use")]
    public static string GetPropertyAsNonEmptyString(this JsonElement jsonObj, string propertyName, string defaultValue, out string? display_value)
    {
        if (jsonObj.ValueKind == JsonValueKind.Object && jsonObj.TryGetProperty(propertyName, out JsonElement element) && element.TryGetPropertyAsString(JSON_KEY_VALUE, out string? value) && !string.IsNullOrWhiteSpace(value))
        {
            display_value = element.TryGetProperty(JSON_KEY_DISPLAY_VALUE, out JsonElement d) ? d.GetString() : null;
            return value;
        }
        display_value = null;
        return defaultValue;
    }
}
