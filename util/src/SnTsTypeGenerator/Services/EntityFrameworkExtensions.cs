using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

public static class EntityFrameworkExtensions
{
    private static readonly GlideType _stringType = new() { Name = TYPE_NAME_string, Label = "String" };
    private static readonly GlideType _dateTimeType = new() { Name = TYPE_NAME_glide_date_time, Label = "Date/Time" };

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
    public static async Task<IEnumerable<TProperty>> GetRelatedCollectionAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellationToken)
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
    public static async Task<IEnumerable<TProperty>> GetRelatedCollectionAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellationToken)
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
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this EntityEntry<TEntity>? entry, Expression<Func<TEntity, TProperty?>> propertyExpression,
            CancellationToken cancellationToken)
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
    public static async Task<EntityEntry<TProperty>?> GetReferencedEntryAsync<TEntity, TProperty>(this TEntity? entity, DbSet<TEntity> dbSet, Expression<Func<TEntity, TProperty?>> propertyExpression,
            CancellationToken cancellationToken)
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
        return result is null ? (await dbSet.Entry(entity).GetReferencedEntryAsync(Expression.Lambda<Func<TEntity, TProperty?>>(Expression.Call(propertyAccessor.Method)), cancellationToken))?.Entity : result;
    }

    public static bool IsIdenticalTo(this ElementInfo x, ElementInfo y)
    {
        if (ReferenceEquals(x, y))
            return true;
        return ReferenceEquals(x, y) || (x.IsActive == y.IsActive && x.IsArray == y.IsArray && x.IsCalculated == y.IsCalculated && x.IsDisplay == y.IsDisplay && x.IsMandatory == y.IsMandatory &&
            x.IsPrimary == y.IsPrimary && x.IsReadOnly == y.IsReadOnly && x.IsUnique == y.IsUnique && (x.MaxLength.HasValue ? y.MaxLength.HasValue && x.MaxLength.Value == y.MaxLength.Value : !y.MaxLength.HasValue) &&
            (x.SizeClass.HasValue ? y.SizeClass.HasValue && x.SizeClass.Value == y.SizeClass.Value : !y.MaxLength.HasValue) && NameComparer.Equals(x.SysID, y.SysID) && NameComparer.Equals(x.Name, y.Name) &&
            NameComparer.Equals(x.Label, y.Label) && x.Comments.NoCaseEquals(y.Comments) && x.DefaultValue.NoCaseEquals(y.DefaultValue) && x.PackageName.NoCaseEquals(y.PackageName));
    }
    
    internal static async Task<IEnumerable<ElementInheritance>> GetAllElementInheritancesAsync(this EntityEntry<TableInfo> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<TableInfo>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
            return elements.Select(e => new ElementInheritance(e, null));
        var super = await GetAllElementInheritancesAsync(superClass, cancellationToken);
        return elements.Select(e =>
        {
            var n = e.Name;
            return new ElementInheritance(e, super.FirstOrDefault(s => s.Element.Name == n)?.Element);
        }).Concat(super.Where(s =>
        {
            var n = s.Element.Name;
            return !elements.Any(e => e.Name == n);
        })).OrderBy(i => i.Element.Name, NameComparer);
    }

    internal static async Task<IEnumerable<ElementInfo>> GetAllElementsAsync(this EntityEntry<TableInfo> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<TableInfo>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
            return elements;
        var super = await GetAllElementsAsync(superClass, cancellationToken);
        return elements.Concat(super.Where(s =>
        {
            var n = s.Name;
            return !elements.Any(e => e.Name == n);
        })).OrderBy(e => e.Name, NameComparer);
    }

    internal static async Task<IEnumerable<ElementInheritance>> GetElementsAsync(this EntityEntry<TableInfo> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<TableInfo>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
            return elements.Select(e => new ElementInheritance(e, null));
        var super = await GetAllElementInheritancesAsync(superClass, cancellationToken);
        return elements.Select(e =>
        {
            var n = e.Name;
            return new ElementInheritance(e, super.FirstOrDefault(s => s.Element.Name == n)?.Element);
        }).Where(i =>
        {
            var s = i.Super;
            return s is null || !i.Element.IsIdenticalTo(s);
        }).OrderBy(i => i.Element.Name, NameComparer);
    }

    public static bool ExtendsBaseRecord(this IEnumerable<ElementInfo> elements) => elements is not null && elements.Any(e => e.Name == JSON_KEY_SYS_ID && e.TypeName == TYPE_NAME_GUID) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_BY && e.TypeName == TYPE_NAME_string) && elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_ON && e.TypeName == TYPE_NAME_glide_date_time) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_MOD_COUNT && e.TypeName == TYPE_NAME_integer) && elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_BY && e.TypeName == TYPE_NAME_string) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_ON && e.TypeName == TYPE_NAME_glide_date_time);

    public static string GetNamespace(this TableInfo tableInfo) => string.IsNullOrWhiteSpace(tableInfo.ScopeValue) ? DEFAULT_NAMESPACE : tableInfo.ScopeValue;

    public static string GetShortName(this TableInfo tableInfo)
    {
        if (string.IsNullOrWhiteSpace(tableInfo.ScopeValue) || tableInfo.ScopeValue == DEFAULT_NAMESPACE || !tableInfo.Name.StartsWith(tableInfo.ScopeValue))
            return tableInfo.Name;
        int len = tableInfo.ScopeValue.Length + 1;
        if (tableInfo.Name.Length <= len || tableInfo.Name[tableInfo.ScopeValue.Length] != '_')
            return tableInfo.Name;
        return tableInfo.Name[len..];
    }

    public static string GetGlideRecordTypeString(this TableInfo tableInfo, string targetNs)
    {
        if (string.IsNullOrWhiteSpace(tableInfo.ScopeValue) || tableInfo.ScopeValue == DEFAULT_NAMESPACE)
            return $"{NS_NAME_GlideRecord}.{tableInfo.Name}";
        if (targetNs == tableInfo.ScopeValue)
            return $"{NS_NAME_record}.{tableInfo.GetShortName()}";
        return $"{tableInfo.ScopeValue}.{NS_NAME_record}.{tableInfo.GetShortName()}";
    }

    public static string GetGlideElementTypeString(this TableInfo tableInfo, string targetNs)
    {
        if (string.IsNullOrWhiteSpace(tableInfo.ScopeValue) || tableInfo.ScopeValue == DEFAULT_NAMESPACE)
            return $"{NS_NAME_GlideElement}.{tableInfo.Name}";
        if (targetNs == tableInfo.ScopeValue)
            return $"{NS_NAME_element}.{tableInfo.GetShortName()}";
        return $"{tableInfo.ScopeValue}.{NS_NAME_element}.{tableInfo.GetShortName()}";
    }

    public static string GetInterfaceTypeString(this TableInfo tableInfo, string targetNs)
    {
        if (string.IsNullOrWhiteSpace(tableInfo.ScopeValue) || tableInfo.ScopeValue == DEFAULT_NAMESPACE)
            return $"{NS_NAME_tableFields}.{tableInfo.Name}";
        if (targetNs == tableInfo.ScopeValue)
            return $"{NS_NAME_fields}.{tableInfo.GetShortName()}";
        return $"{tableInfo.ScopeValue}.{NS_NAME_fields}.{tableInfo.GetShortName()}";
    }
}
