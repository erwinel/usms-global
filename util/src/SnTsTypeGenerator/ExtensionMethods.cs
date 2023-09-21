using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        if (!referenceEntry.IsLoaded)
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.TargetEntry;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="keyTest">Checks whether the associated key has a value.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The entry related entity or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, TProperty?>> propertyExpression, Func<bool> keyTest, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entry.Reference(propertyExpression);
        if (referenceEntry.TargetEntry is null && keyTest() && !referenceEntry.IsLoaded)
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
    /// <param name="entity">The parent entity object.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="keyTest">Checks whether the associated key has a value.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The entry related entity or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, TProperty?>> propertyExpression, Func<bool> keyTest, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return null;
        return await dbSet.Entry(entity).GetReferencedEntryAsync(propertyExpression, keyTest, cancellationToken);
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
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entityEntry, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entityEntry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entityEntry.Reference(propertyExpression);
        if (referenceEntry.CurrentValue is null && !referenceEntry.IsLoaded)
            await referenceEntry.LoadAsync(cancellationToken);
        return referenceEntry.CurrentValue;
    }

    /// <summary>
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entityEntry">The parent entity entry.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="keyTest">Checks whether the associated key has a value.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity object or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entityEntry, Expression<Func<TEntity, TProperty?>> propertyExpression, Func<bool> keyTest, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entityEntry is null)
            return null;
        ReferenceEntry<TEntity, TProperty> referenceEntry = entityEntry.Reference(propertyExpression);
        if (referenceEntry.CurrentValue is null && keyTest() && !referenceEntry.IsLoaded)
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
    /// Asyncrhonously loads and gets the related entity for a navigation property.
    /// </summary>
    /// <param name="entity">The parent entity object.</param>
    /// <param name="dbSet">The database context property for the parent entity's table.</param>
    /// <param name="propertyExpression">The expression for the nagivation property.</param>
    /// <param name="keyTest">Checks whether the associated key has a value.</param>
    /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
    /// <typeparam name="TEntity">The parent entity type.</typeparam>
    /// <typeparam name="TProperty">The related entity type.</typeparam>
    /// <returns>The related entity object or <see langword="null" /> if there is no related entity.</returns>
    public static async Task<TProperty?> GetReferencedEntityAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Func<TEntity, TProperty?> propertyAccessor, Func<bool> keyTest, CancellationToken cancellationToken)
         where TEntity : class
         where TProperty : class
    {
        if (entity is null)
            return null;
        TProperty? result = propertyAccessor(entity);
        return (result is null) ? (await dbSet.Entry(entity).GetReferencedEntryAsync<TEntity, TProperty>(Expression.Lambda<Func<TEntity, TProperty?>>(Expression.Call(propertyAccessor.Method)), keyTest, cancellationToken))?.Entity : result;
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
            Query = $"sysparm_query={value}",
            Fragment = null
        }.Uri;
    }

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
    public static async Task<T?> GetLinkedObjectAsync<T>(this HttpClientHandler clientHandler, JsonElement element, string name, Func<string, Task<T?>> lookupFunc, Func<JsonElement, Task<T?>> createFunc, ILogger logger, CancellationToken cancellationToken) where T : class
    {
        if (!element.TryGetProperty(name, out JsonElement linkElement) || linkElement.ValueKind != JsonValueKind.Object)
            return null;
        string? link;
        T? result;
        Uri? uri;
        string responseBody;
        if (linkElement.TryGetNonEmptyString(JSON_KEY_VALUE, out string? value))
        {
            if ((result = await lookupFunc(value)) is not null)
                return result;
            if (!(linkElement.TryGetNonEmptyString(JSON_KEY_LINK, out link) && Uri.TryCreate(link, UriKind.Absolute, out uri)))
                return null;
            using HttpClient client = new(clientHandler);
            HttpRequestMessage msg = new(HttpMethod.Get, uri);
            msg.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await client.SendAsync(msg, cancellationToken);
            try { response.EnsureSuccessStatusCode(); }
            catch (HttpRequestException exception)
            {
                logger.LogHttpRequestFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
            try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception exception)
            {
                logger.LogGetResponseContentFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
        }
        else if (linkElement.TryGetNonEmptyString(JSON_KEY_LINK, out link) && Uri.TryCreate(link, UriKind.Absolute, out uri))
        {
            using HttpClient client = new(clientHandler);
            HttpRequestMessage msg = new(HttpMethod.Get, uri);
            msg.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await client.SendAsync(msg, cancellationToken);
            try { response.EnsureSuccessStatusCode(); }
            catch (HttpRequestException exception)
            {
                logger.LogHttpRequestFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
            try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception exception)
            {
                logger.LogGetResponseContentFailedError(response.RequestMessage!.RequestUri!, exception);
                return null;
            }
        }
        else
            return null;
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            logger.LogInvalidHttpResponseError(uri, responseBody);
            return null;
        }
        
        JsonDocument doc;
        try { doc = JsonDocument.Parse(responseBody); }
        catch (JsonException exception)
        {
            logger.LogJsonCouldNotBeParsedError(uri, responseBody, exception);
            return null;
        }
        using (doc)
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
                return await createFunc(doc.RootElement);
        }
        return null;
    }

    /// <summary>
    /// Tries to get the string value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="result">The string value of the property or <see langword="null" /> if the property does not exist or it is not a <see cref="JsonValueKind.String"/>.</param>
    /// <returns><see langword="true" /> if the property exists and it is a <see cref="JsonValueKind.String"/>; otherwise, <see langword="false" />.</returns>
    public static bool TryGetStringValue(this JsonElement element, string name, [NotNullWhen(true)] out string? result)
    {
        if (element.TryGetProperty(name, out JsonElement nameElement) && nameElement.ValueKind == JsonValueKind.String)
            return (result = nameElement.GetString()) is not null;
        result = null;
        return false;
    }

    /// <summary>
    /// Gets the boolean value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The default value to use if the property does not exist or it is not <see cref="JsonValueKind.True"/> or <see cref="JsonValueKind.False"/></param>
    /// <returns><see langword="true" /> if the property exists and it is <see cref="JsonValueKind.True"/>; <see langword="false" /> if it is <see cref="JsonValueKind.False"/>; otherwise, the value of <paramref name="defaultValue"/>.</returns>
    public static bool GetBoolean(this JsonElement source, string name, bool defaultValue = false) => source.TryGetProperty(name, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => element.TryGetInt32(out int i) ? i != 0 : element.TryGetDouble(out double d) && d != 0.0,
        _ => defaultValue,
    } : defaultValue;

    /// <summary>
    /// Gets the string value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The default value to use if the property does not exist or it is not a <see cref="JsonValueKind.String"/>.</param>
    /// <returns>The value of the property if it exists and it is a <see cref="JsonValueKind.String"/>; otherwise, the value of <paramref name="defaultValue"/>.</returns>
    public static string GetString(this JsonElement source, string name, string defaultValue = "") => source.TryGetProperty(name, out JsonElement element) ? element.ValueKind switch
    {
        JsonValueKind.Null or JsonValueKind.Undefined => defaultValue,
        _ => source.GetString() ?? defaultValue,
    } : defaultValue;

    /// <summary>
    /// Tries to get a non-empty string property value.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="result">The non-empty string value of the property or <see langword="null" /> if the property does not exist, is not a <see cref="JsonValueKind.String"/> or is empty.</param>
    /// <returns><see langword="true" /> if the property exists, is a <see cref="JsonValueKind.String"/>, and is not empty; otherwise, <see langword="false" />.</returns>
    public static bool TryGetNonEmptyString(this JsonElement source, string name, [NotNullWhen(true)] out string? result)
    {
        if (source.TryGetProperty(name, out JsonElement element))
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
    /// Gets the string value of a property.
    /// </summary>
    /// <param name="element">The source JSON object.</param>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The default value to use if the property does not exist, is not a <see cref="JsonValueKind.String"/>, or is empty.</param>
    /// <returns>The value of the property if it exists, is a <see cref="JsonValueKind.String"/>, and is not empty; otherwise, the value of <paramref name="defaultValue"/>.</returns>
    public static string GetNonEmptyString(this JsonElement source, string name, string defaultValue)
    {
        if (source.TryGetProperty(name, out JsonElement element))
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
}
