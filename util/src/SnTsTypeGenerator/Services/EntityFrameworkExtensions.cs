using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

public static class EntityFrameworkExtensions
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
        var entry = dbSet.Entry(entity);
        return await entry.GetRelatedCollectionAsync(propertyExpression, cancellationToken);
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

    public static bool IsIdenticalTo(this Element x, Element y)
    {
        if (ReferenceEquals(x, y))
            return true;
        return ReferenceEquals(x, y) || (x.IsActive == y.IsActive && x.IsArray == y.IsArray && x.IsCalculated == y.IsCalculated && x.IsDisplay == y.IsDisplay && x.IsMandatory == y.IsMandatory &&
            x.IsPrimary == y.IsPrimary && x.IsReadOnly == y.IsReadOnly && x.IsUnique == y.IsUnique && (x.MaxLength.HasValue ? y.MaxLength.HasValue && x.MaxLength.Value == y.MaxLength.Value : !y.MaxLength.HasValue) &&
            (x.SizeClass.HasValue ? y.SizeClass.HasValue && x.SizeClass.Value == y.SizeClass.Value : !y.MaxLength.HasValue) && NameComparer.Equals(x.SysID, y.SysID) && NameComparer.Equals(x.Name, y.Name) &&
            NameComparer.Equals(x.Label, y.Label) && x.Comments.NoCaseEquals(y.Comments) && x.DefaultValue.NoCaseEquals(y.DefaultValue) && x.PackageName.NoCaseEquals(y.PackageName));
    }

    public static bool IsIdenticalTo(this ElementRecord x, Element y)
    {
        if (ReferenceEquals(x, y))
            return true;
        return ReferenceEquals(x, y) || (x.IsActive == y.IsActive && x.IsArray == y.IsArray && x.IsCalculated == y.IsCalculated && x.IsDisplay == y.IsDisplay && x.IsMandatory == y.IsMandatory &&
            x.IsPrimary == y.IsPrimary && x.IsReadOnly == y.IsReadOnly && x.IsUnique == y.IsUnique && (x.MaxLength.HasValue ? y.MaxLength.HasValue && x.MaxLength.Value == y.MaxLength.Value : !y.MaxLength.HasValue) &&
            (x.SizeClass.HasValue ? y.SizeClass.HasValue && x.SizeClass.Value == y.SizeClass.Value : !y.MaxLength.HasValue) && NameComparer.Equals(x.SysID, y.SysID) && NameComparer.Equals(x.Name, y.Name) &&
            NameComparer.Equals(x.Label, y.Label) && x.Comments.NoCaseEquals(y.Comments) && x.DefaultValue.NoCaseEquals(y.DefaultValue) && (x.Package?.DisplayValue).NoCaseEquals(y.PackageName));
    }

    internal static async Task<IEnumerable<ElementInheritance>> GetAllElementInheritancesAsync(this EntityEntry<Table> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<Table>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (superClass is null)
            return elements.Select(e => new ElementInheritance(e, null));
        var super = await GetAllElementInheritancesAsync(superClass, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return elements.Select(e =>
        {
            var n = e.Name;
            return new ElementInheritance(e, super.FirstOrDefault(s => NameComparer.Equals(s.Element.Name, n))?.Element);
        }).Concat(super.Where(s =>
        {
            var n = s.Element.Name;
            return !elements.Any(e => NameComparer.Equals(e.Name, n));
        })).OrderBy(i => i.Element.Name, NameComparer);
    }

    internal static async Task<IEnumerable<Element>> GetAllElementsAsync(this EntityEntry<Table> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<Table>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (superClass is null)
            return elements;
        var super = await GetAllElementsAsync(superClass, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return elements.Concat(super.Where(s =>
        {
            var n = s.Name;
            return !elements.Any(e => NameComparer.Equals(e.Name, n));
        })).OrderBy(e => e.Name, NameComparer);
    }

    internal static bool TryFindByName(this IEnumerable<Table>? tables, string name, [NotNullWhen(true)] out Table? result)
    {
        if (tables is not null)
            foreach (Table t in tables)
                if (NameComparer.Equals(t.Name, name))
                {
                    result = t;
                    return true;
                }
        result = null;
        return false;
    }

    internal static bool TryFindByName(this IEnumerable<Element>? elements, string name, [NotNullWhen(true)] out Element? result)
    {
        if (elements is not null)
            foreach (Element e in elements)
                if (NameComparer.Equals(e.Name, name))
                {
                    result = e;
                    return true;
                }
        result = null;
        return false;
    }

    internal static Table? FindByName(this IEnumerable<Table>? tables, string name) => tables?.FirstOrDefault(t => NameComparer.Equals(t.Name, name));

    internal static bool HasTable(this IEnumerable<Table>? tables, string name) => tables?.Any(t => NameComparer.Equals(t.Name, name)) ?? false;

    internal static Element? FindByName(this IEnumerable<Element>? elements, string name) => elements?.FirstOrDefault(e => NameComparer.Equals(e.Name, name));

    internal static bool HasElement(this IEnumerable<Table>? elements, string name) => elements?.Any(e => NameComparer.Equals(e.Name, name)) ?? false;

    internal static async Task<IEnumerable<Table>> LoadAllReferencedAsync(this TypingsDbContext dbContext, IEnumerable<Table> tables, CancellationToken cancellationToken)
    {
        List<Table> toValidate = tables.ToList();
        int startCount = toValidate.Count;
        for (int index = 0; index < toValidate.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Table currentTable = toValidate[index];
            var entry = dbContext.Tables.Entry(currentTable);
            var name = currentTable.SuperClassName;
            if (name is not null && !toValidate.HasTable(name))
            {
                var superClass = await entry.GetReferencedEntityAsync(t => t.SuperClass, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (superClass is not null)
                    toValidate.Add(superClass);
            }
            foreach (Element element in await entry.GetRelatedCollectionAsync(t => t.Elements, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if ((name = element.RefTableName) is not null && !toValidate.HasTable(name))
                {
                    var refTable = await element.GetReferencedEntityAsync(dbContext.Elements, e => e.Reference, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (refTable is not null)
                        toValidate.Add(refTable);
                }
            }
        }
        return (startCount > toValidate.Count) ? toValidate.OrderBy(t => t.Name, NameComparer) : tables;
    }

    internal static async Task<IEnumerable<ElementInheritance>> GetElementsAsync(this EntityEntry<Table> entity, CancellationToken cancellationToken)
    {
        var elements = await entity.GetRelatedCollectionAsync(t => t.Elements, cancellationToken);
        EntityEntry<Table>? superClass = await entity.GetReferencedEntryAsync(t => t.SuperClass, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (superClass is null)
            return elements.Select(e => new ElementInheritance(e, null));
        var super = await GetAllElementInheritancesAsync(superClass, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return elements.Select(e =>
        {
            var n = e.Name;
            return new ElementInheritance(e, super.FirstOrDefault(s => NameComparer.Equals(s.Element.Name, n))?.Element);
        }).Where(i =>
        {
            var s = i.Super;
            return s is null || !i.Element.IsIdenticalTo(s);
        }).OrderBy(i => i.Element.Name, NameComparer);
    }

    public static bool ExtendsBaseRecord(this IEnumerable<Element> elements) => elements is not null && elements.Any(e => e.Name == JSON_KEY_SYS_ID && e.TypeName.NoCaseEquals(TYPE_NAME_GUID)) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_BY && e.TypeName.NoCaseEquals(TYPE_NAME_string)) && elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_ON && e.TypeName.NoCaseEquals(TYPE_NAME_glide_date_time)) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_MOD_COUNT && e.TypeName.NoCaseEquals(TYPE_NAME_integer)) && elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_BY && e.TypeName.NoCaseEquals(TYPE_NAME_string)) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_ON && e.TypeName.NoCaseEquals(TYPE_NAME_glide_date_time));

    public static bool ExtendsBaseRecord(this IEnumerable<ElementRecord> elements) => elements is not null && elements.Any(e => e.Name == JSON_KEY_SYS_ID && TYPE_NAME_GUID.NoCaseEquals(e.Type?.Value)) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_BY && TYPE_NAME_string.NoCaseEquals(e.Type?.Value)) && elements.Any(e => e.Name == JSON_KEY_SYS_CREATED_ON && TYPE_NAME_glide_date_time.NoCaseEquals(e.Type?.Value)) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_MOD_COUNT && TYPE_NAME_integer.NoCaseEquals(e.Type?.Value)) && elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_BY && TYPE_NAME_string.NoCaseEquals(e.Type?.Value)) &&
        elements.Any(e => e.Name == JSON_KEY_SYS_UPDATED_ON && TYPE_NAME_glide_date_time.NoCaseEquals(e.Type?.Value));

    public static bool IsGlobalScope(this Table table) => string.IsNullOrWhiteSpace(table.ScopeValue) || NameComparer.Equals(table.ScopeValue, GLOBAL_NAMESPACE);

    public static string GetGlideRecordTypeName(this Table table, string currentNamespace) => table.IsGlobalScope() ? $"{NS_NAME_GlideRecord}.{table.Name}" :
        NameComparer.Equals(table.ScopeValue, currentNamespace) ? $"{NS_NAME_record}.{table.Name}" : $"{table.ScopeValue}.{NS_NAME_record}.{table.Name}";

    public static string GetGlideElementTypeName(this Table table, string currentNamespace) => table.IsGlobalScope() ? $"{NS_NAME_GlideElement}.{table.Name}" :
        NameComparer.Equals(table.ScopeValue, currentNamespace) ? $"{NS_NAME_element}.{table.Name}" : $"{table.ScopeValue}.{NS_NAME_element}.{table.Name}";

    public static string GetFieldsTypeName(this Table table, string currentNamespace) => table.IsGlobalScope() ? $"{NS_NAME_tableFields}.{table.Name}" :
        NameComparer.Equals(table.ScopeValue, currentNamespace) ? $"{NS_NAME_fields}.{table.Name}" : $"{table.ScopeValue}.{NS_NAME_fields}.{table.Name}";

    public static string GetNamespace(this Table table) => string.IsNullOrWhiteSpace(table.ScopeValue) ? GLOBAL_NAMESPACE : table.ScopeValue;

    public static string GetGlideTableName(this Table table) => table.IsGlobalScope() ? table.Name : $"{table.ScopeValue}_{table.Name}";
}
