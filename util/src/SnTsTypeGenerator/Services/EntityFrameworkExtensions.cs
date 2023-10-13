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

    public static IEnumerable<(ElementInfo Inherited, ElementInfo? Base, bool IsTypeOverride)> GetBaseElements(this IEnumerable<ElementInfo> inheritedElements, IEnumerable<ElementInfo> baseElements)
    {
        return inheritedElements.Select<ElementInfo, (ElementInfo Inherited, ElementInfo? Base, bool IsTypeOverride)>(e =>
        {
            string name = e.Name;
            ElementInfo? b = baseElements.FirstOrDefault(o => o.Name == name);
            if (b is null)
                return (e, null, false);
            string tn = e.TypeName;
            string? r = e.RefTableName;
            if (r is null)
                return (e, b, b.RefTableName is not null || !NameComparer.Equals(b.TypeName, tn));
            return (e, b, b.RefTableName is null || !(NameComparer.Equals(b.TypeName, tn) && NameComparer.Equals(b.RefTableName, r)));
        });
    }

    internal static async Task<IEnumerable<ElementInfo>> GetAllElementsAsync(this EntityEntry<TableInfo> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<TableInfo>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
            return elements;
        var superElements = (await superClass.GetAllElementsAsync(cancellationToken)).ToArray();
        if (superElements.Length == 0)
            return elements;
        return elements.Concat(superElements.Where(e => !elements.Any(b => NameComparer.Equals(b.Name, e.Name) && NameComparer.Equals(b.TypeName, e.TypeName))));
    }

    internal static bool Overrides(this ElementInfo element, ElementInfo superElement, out bool isNew)
    {
        if (!NameComparer.Equals(element.Name, superElement.Name))
        {
            isNew = false;
            return false;
        }
        isNew = !NameComparer.Equals(element.TypeName, superElement.TypeName);
        return element.IsActive != superElement.IsActive || element.IsArray != superElement.IsArray || element.IsDisplay != superElement.IsDisplay || element.IsMandatory != superElement.IsMandatory ||
            element.IsPrimary != superElement.IsPrimary || element.IsReadOnly != superElement.IsReadOnly || element.IsCalculated != superElement.IsCalculated || element.IsUnique != superElement.IsUnique ||
            element.MaxLength.HasValue ? superElement.MaxLength.HasValue && element.MaxLength!.Value == superElement.MaxLength.Value : !superElement.MaxLength.HasValue ||
            element.SizeClass.HasValue ? superElement.SizeClass.HasValue && element.SizeClass!.Value == superElement.SizeClass.Value : !superElement.SizeClass.HasValue ||
            !(element.Comments.NoCaseEquals(superElement.Comments) && element.DefaultValue.NoCaseEquals(superElement.DefaultValue) && element.PackageName.NoCaseEquals(superElement.PackageName) &&
                element.RefTableName.NoCaseEquals(superElement.RefTableName) && NameComparer.Equals(element.Label, superElement.Label));
    }

    public static bool ExtendsBaseRecord(this IEnumerable<ElementInfo> elements) => elements is not null && elements.Any(e => e.Name == JSON_KEY_SYS_ID && e.TypeName == TYPE_NAME_GUID) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_BY && e.TypeName == TYPE_NAME_string) && elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_ON && e.TypeName == TYPE_NAME_glide_date_time) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_MOD_COUNT && e.TypeName == TYPE_NAME_integer) && elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_BY && e.TypeName == TYPE_NAME_string) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_ON && e.TypeName == TYPE_NAME_glide_date_time);

    public static async Task<bool> ExtendsBaseRecordAsync(this EntityEntry<TableInfo> entity, CancellationToken cancellationToken) => entity is not null &&
        (await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken)).ExtendsBaseRecord();

    // BUG: Database now contains a record for IBaseRecord
    internal static async Task<(IEnumerable<ElementInheritance> Inheritances, bool ExtendsBaseRecord)> GetElementInheritancesAsync(this EntityEntry<TableInfo> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        TableInfo table = entity.Entity;
        EntityEntry<TableInfo>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        if (superClass is null)
        {
            ElementInfo? sys_id = elements.FirstOrDefault(e => e.Name == JSON_KEY_SYS_ID && e.TypeName == TYPE_NAME_GUID);
            if (sys_id is not null)
            {
                ElementInfo? created_by = elements.FirstOrDefault(e => e.Name == JSON_KEY_SYS_CREATED_BY && e.TypeName == TYPE_NAME_string);
                if (created_by is not null)
                {
                    ElementInfo? created_on = elements.FirstOrDefault(e => e.Name == JSON_KEY_SYS_CREATED_ON && e.TypeName == TYPE_NAME_glide_date_time);
                    if (created_on is not null)
                    {
                        ElementInfo? mod_count = elements.FirstOrDefault(e => e.Name == JSON_KEY_SYS_MOD_COUNT && e.TypeName == TYPE_NAME_integer);
                        if (mod_count is not null)
                        {
                            ElementInfo? updated_by = elements.FirstOrDefault(e => e.Name == JSON_KEY_SYS_UPDATED_BY && e.TypeName == TYPE_NAME_string);
                            if (updated_by is not null)
                            {
                                ElementInfo? updated_on = elements.FirstOrDefault(e => e.Name == JSON_KEY_SYS_UPDATED_ON && e.TypeName == TYPE_NAME_glide_date_time);
                                if (updated_on is not null)
                                    return (new ElementInheritance[]
                                    {
                                        new(sys_id, new() { IsActive = true, IsPrimary = true, Label = "Sys ID", MaxLength = 32, Name = JSON_KEY_SYS_ID, ScopeValue = DEFAULT_NAMESPACE, SysID = sys_id.SysID,
                                            TypeName = TYPE_NAME_GUID, TableName = TS_NAME_BASERECORD, SourceFqdn = sys_id.SourceFqdn }),
                                        new(created_by, new() { IsActive = true, Label = "Created by", MaxLength = 40, Name = JSON_KEY_SYS_CREATED_BY, ScopeValue = DEFAULT_NAMESPACE, SysID = created_by.SysID,
                                            TypeName = TYPE_NAME_string, TableName = TS_NAME_BASERECORD, SourceFqdn = created_by.SourceFqdn }),
                                        new(created_on, new() { IsActive = true, Label = "Created", MaxLength = 40, Name = JSON_KEY_SYS_CREATED_ON, ScopeValue = DEFAULT_NAMESPACE, SysID = created_on.SysID,
                                            TypeName = TYPE_NAME_glide_date_time, TableName = TS_NAME_BASERECORD, SourceFqdn = created_on.SourceFqdn }),
                                        new(mod_count, new() { IsActive = true, Label = "Updates", MaxLength = 40, Name = JSON_KEY_SYS_MOD_COUNT, ScopeValue = DEFAULT_NAMESPACE, SysID = mod_count.SysID,
                                            TypeName = TYPE_NAME_integer, TableName = TS_NAME_BASERECORD, SourceFqdn = mod_count.SourceFqdn }),
                                        new(updated_by, new() { IsActive = true, Label = "Updated by", MaxLength = 40, Name = JSON_KEY_SYS_UPDATED_BY, ScopeValue = DEFAULT_NAMESPACE, SysID = updated_by.SysID,
                                            TypeName = TYPE_NAME_string, TableName = TS_NAME_BASERECORD, SourceFqdn = updated_by.SourceFqdn }),
                                        new(updated_on, new() { IsActive = true, Label = "Updated", MaxLength = 40, Name = JSON_KEY_SYS_UPDATED_ON, ScopeValue = DEFAULT_NAMESPACE, SysID = updated_on.SysID,
                                            TypeName = TYPE_NAME_glide_date_time, TableName = TS_NAME_BASERECORD, SourceFqdn = updated_on.SourceFqdn })
                                    }, true);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            var superElements = (await superClass.GetAllElementsAsync(cancellationToken)).ToArray();
            if (superElements.Length > 0)
                return (elements.Select(e => new ElementInheritance(e, superElements.FirstOrDefault(b => NameComparer.Equals(b.Name, e.Name)))), false);
        }
        return (elements.Select(e => new ElementInheritance(e, null)), false);
    }
}
