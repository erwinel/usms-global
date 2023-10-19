// using static SnTsTypeGenerator.Models.EntityAccessors;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models;

public static class EntityAccessors
{
    internal static void SetOptionalNavForeignKey<T>(object syncRoot, string? value, ref string? fk, ref T? nav, Func<T, string> getPrimaryKey)
        where T : class
    {
        lock (syncRoot)
        {
            if (value is null)
            {
                if (fk is not null)
                {
                    fk = null;
                    nav = null;
                }
            }
            else if (fk is null || !NameComparer.Equals(value, fk))
            {
                if (nav is null)
                    fk = value;
                else if (NameComparer.Equals(value, getPrimaryKey(nav)))
                    fk = null;
                else
                    nav = null;
            }
        }
    }

    internal static void SetOptionalNonEmptyNavForeignKey<T>(object syncRoot, string? value, ref string? fk, ref T? nav, Func<T, string> getPrimaryKey)
        where T : class
    {
        lock (syncRoot)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (fk is not null)
                {
                    fk = null;
                    nav = null;
                }
            }
            else if (fk is null || !NameComparer.Equals(value, fk))
            {

                if (nav is null)
                    fk = value;
                else if (NameComparer.Equals(value, getPrimaryKey(nav)))
                    fk = null;
                else
                    nav = null;
            }
        }
    }

    internal static void SetRequiredNonEmptyNavForeignKey<T>(object syncRoot, string? value, ref string fk, ref T? nav, Func<T, string> getPrimaryKey)
        where T : class
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
        lock (syncRoot)
        {
            if (nav is not null)
            {
                if (NameComparer.Equals(value, getPrimaryKey(nav)))
                    return;
                nav = null;
            }
            fk = value;
        }
    }

    internal static void SetRequiredNavProperty<T>(object syncRoot, T? value, ref string fk, ref T? nav, Func<T, string> getPrimaryKey)
    {
        lock (syncRoot)
        {
            if (value is null)
            {
                if (nav is null)
                    return;
                fk = getPrimaryKey(nav);
            }
            else
            {
                if (nav is not null && ReferenceEquals(nav, value))
                    return;
                fk = string.Empty;
            }
            nav = value;
        }
    }

    internal static void SetOptionalNavProperty<T>(object syncRoot, T? value, ref string? fk, ref T? nav)
    {
        lock (syncRoot)
        {
            if (nav is null)
            {
                if (value is null)
                    return;
                fk = null;
            }
            else if (value is not null && ReferenceEquals(nav, value))
                return;
            nav = value;
        }
    }
}
