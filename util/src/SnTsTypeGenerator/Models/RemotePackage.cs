using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models;

/// <summary>
/// Deserialized "Package" (<see cref="TABLE_NAME_SYS_PACKAGE" />) record from a ServiceNow instance.
/// </summary>
/// <param name="ID">The value of the <c><see cref="JSON_KEY_SOURCE" />.value</c> property.</param>
/// <param name="Name">The value of the <c><see cref="JSON_KEY_NAME" />.value</c> property.</param>
/// <param name="Version">The value of the <c><see cref="JSON_KEY_VERSION" />.value</c> property.</param>
/// <param name="SysID">The value of the <c><see cref="JSON_KEY_SYS_ID" />.value</c> property.</param>
/// <param name="Licensable">The value of the <c><see cref="JSON_KEY_LICENSABLE" />.value</c> property.</param>
/// <param name="SubscriptionRequirement">The value of the <c><see cref="JSON_KEY_ENFORCE_LICENSE" />.value</c> property.</param>
/// <param name="Active">The value of the <c><see cref="JSON_KEY_ACTIVE" />.value</c> property.</param>
public record RemotePackage(string ID, string Name, string Version, string SysID, bool Licensable, string SubscriptionRequirement, bool Active)
{
    internal static RemoteApplication? ApplicationFromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysScope)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysScope.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysScope);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysScope);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysScope);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysScope);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysScope, 0);
            sysScope = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysScope = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysScope);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysScope);
        }
        if (!sysScope.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysScope, JSON_KEY_SYS_ID);
        if (!sysScope.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
            throw new ExpectedPropertyNotFoundException(requestUri, sysScope, JSON_KEY_SCOPE);
        return new RemoteApplication(Name: sysScope.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            Value: value,
            ID: sysScope.GetFieldAsNonEmpty(JSON_KEY_SOURCE),
            Version: sysScope.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            ShortDescription: sysScope.GetFieldAsNonEmpty(JSON_KEY_SHORT_DESCRIPTION),
            SysID: sys_id,
            Licensable: sysScope.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            SubscriptionRequirement: sysScope.GetFieldAsString(JSON_KEY_ENFORCE_LICENSE),
            Vendor: sysScope.GetFieldAsString(JSON_KEY_VENDOR),
            VendorPrefix: sysScope.GetFieldAsString(JSON_KEY_VENDOR_PREFIX),
            Private: sysScope.GetFieldAsBoolean(JSON_KEY_PRIVATE),
            Active: sysScope.GetFieldAsBoolean(JSON_KEY_ACTIVE));
    }
    
    internal static RemoteSysPlugin? PluginFromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysPlugin)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysPlugin.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysPlugin);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysPlugin);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysPlugin);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysPlugin);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysPlugin, 0);
            sysPlugin = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysPlugin = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysPlugin);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysPlugin);
        }
        if (!sysPlugin.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysPlugin, JSON_KEY_SYS_ID);
        if (!sysPlugin.TryGetFieldAsNonEmpty(JSON_KEY_SOURCE, out string? source))
            throw new ExpectedPropertyNotFoundException(requestUri, sysPlugin, JSON_KEY_SOURCE);
        return new RemoteSysPlugin(ID: source,
            Name: sysPlugin.GetFieldAsNonEmpty(JSON_KEY_NAME, source),
            Version: sysPlugin.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            SysID: sys_id,
            Parent: sysPlugin.GetFieldAsNonEmpty(JSON_KEY_PARENT),
            Licensable: sysPlugin.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            SubscriptionRequirement: sysPlugin.GetFieldAsString(JSON_KEY_ENFORCE_LICENSE),
            Vendor: sysPlugin.GetFieldAsString(JSON_KEY_VENDOR),
            VendorPrefix: sysPlugin.GetFieldAsString(JSON_KEY_VENDOR_PREFIX),
            InstallDate: sysPlugin.GetFieldAsDateTimeOrNull(JSON_KEY_INSTALL_DATE),
            Private: sysPlugin.GetFieldAsBoolean(JSON_KEY_PRIVATE),
            Active: sysPlugin.GetFieldAsBoolean(JSON_KEY_ACTIVE));
    }
    
    internal static RemotePackage? PackageFromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysPackage)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysPackage.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysPackage);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysPackage);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysPackage);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysPackage);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysPackage, 0);
            sysPackage = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysPackage = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysPackage);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysPackage);
        }
        if (!sysPackage.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysPackage, JSON_KEY_SYS_ID);
        if (!sysPackage.TryGetFieldAsNonEmpty(JSON_KEY_SOURCE, out string? source))
            throw new ExpectedPropertyNotFoundException(requestUri, sysPackage, JSON_KEY_SOURCE);
        return new RemotePackage(ID: source,
            Name: sysPackage.GetFieldAsNonEmpty(JSON_KEY_NAME, source),
            Version: sysPackage.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            SysID: sys_id,
            Licensable: sysPackage.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            SubscriptionRequirement: sysPackage.GetFieldAsString(JSON_KEY_ENFORCE_LICENSE),
            Active: sysPackage.GetFieldAsBoolean(JSON_KEY_ACTIVE));
    }
    
    internal static RemoteCustomApplication? CustomApplicationFromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysApp)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysApp.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysApp);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysApp);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysApp);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysApp);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysApp, 0);
            sysApp = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysApp = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysApp);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysApp);
        }
        if (!sysApp.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysApp, JSON_KEY_SYS_ID);
        if (!sysApp.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
            throw new ExpectedPropertyNotFoundException(requestUri, sysApp, JSON_KEY_NAME);
        return new RemoteCustomApplication(Name: sysApp.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            Value: value,
            ID: sysApp.GetFieldAsNonEmpty(JSON_KEY_SOURCE),
            Version: sysApp.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            ShortDescription: sysApp.GetFieldAsNonEmpty(JSON_KEY_SHORT_DESCRIPTION),
            SysID: sys_id,
            Licensable: sysApp.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            SubscriptionRequirement: sysApp.GetFieldAsString(JSON_KEY_ENFORCE_LICENSE),
            Vendor: sysApp.GetFieldAsString(JSON_KEY_VENDOR),
            VendorPrefix: sysApp.GetFieldAsString(JSON_KEY_VENDOR_PREFIX),
            StoreURL: sysApp.GetFieldAsString(JSON_KEY_STORE_URL),
            StoreCorrelationID: sysApp.GetFieldAsString(JSON_KEY_STORE_CORRELATION_ID),
            Code: sysApp.GetFieldAsString(JSON_KEY_SYS_CODE),
            Private: sysApp.GetFieldAsBoolean(JSON_KEY_PRIVATE),
            InstalledViaDependency: sysApp.GetFieldAsBoolean(JSON_KEY_INSTALLED_AS_DEPENDENCY),
            Active: sysApp.GetFieldAsBoolean(JSON_KEY_ACTIVE),
            Dependencies: sysApp.GetFieldAsStringArray(JSON_KEY_DEPENDENCIES));
    }

    internal static RemoteStoreApp? StoreAppFromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysStoreApp)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysStoreApp.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysStoreApp);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysStoreApp);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysStoreApp);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysStoreApp);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysStoreApp, 0);
            sysStoreApp = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysStoreApp = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysStoreApp);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysStoreApp);
        }
        if (!sysStoreApp.TryGetFieldAsNonEmpty(JSON_KEY_SYS_ID, out string? sys_id))
            throw new ExpectedPropertyNotFoundException(requestUri, sysStoreApp, JSON_KEY_SYS_ID);
        if (!sysStoreApp.TryGetFieldAsNonEmpty(JSON_KEY_SCOPE, out string? value))
            throw new ExpectedPropertyNotFoundException(requestUri, sysStoreApp, JSON_KEY_NAME);
        return new RemoteStoreApp(Name: sysStoreApp.GetFieldAsNonEmpty(JSON_KEY_NAME, value),
            Value: value,
            ID: sysStoreApp.GetFieldAsNonEmpty(JSON_KEY_SOURCE),
            Version: sysStoreApp.GetFieldAsNonEmpty(JSON_KEY_VERSION),
            ShortDescription: sysStoreApp.GetFieldAsNonEmpty(JSON_KEY_SHORT_DESCRIPTION),
            SysID: sys_id,
            Licensable: sysStoreApp.GetFieldAsBoolean(JSON_KEY_LICENSABLE),
            SubscriptionRequirement: sysStoreApp.GetFieldAsString(JSON_KEY_ENFORCE_LICENSE),
            Vendor: sysStoreApp.GetFieldAsString(JSON_KEY_VENDOR),
            VendorPrefix: sysStoreApp.GetFieldAsString(JSON_KEY_VENDOR_PREFIX),
            Code: sysStoreApp.GetFieldAsString(JSON_KEY_SYS_CODE),
            InstallDate: sysStoreApp.GetFieldAsDateTimeOrNull(JSON_KEY_INSTALL_DATE),
            IsStoreApp: sysStoreApp.GetFieldAsBoolean(JSON_KEY_IS_STORE_APP),
            Private: sysStoreApp.GetFieldAsBoolean(JSON_KEY_PRIVATE),
            Active: sysStoreApp.GetFieldAsBoolean(JSON_KEY_ACTIVE),
            Dependencies: sysStoreApp.GetFieldAsStringArray(JSON_KEY_DEPENDENCIES));
    }

}

public record RemotePackageDependency(RemoteRef Package, RemoteRef Dependency, string? MinVersion)
{
    internal static RemotePackageDependency? FromJson(Uri requestUri, JsonNode? jsonNode, ILogger logger, bool expectArray)
    {
        if (jsonNode is not JsonObject sysPackage)
            throw new InvalidHttpResponseException(requestUri, jsonNode?.ToJsonString());
        if (!sysPackage.TryGetPropertyValue(JSON_KEY_RESULT, out jsonNode))
            throw new ResponseResultPropertyNotFoundException(requestUri, sysPackage);
        if (expectArray)
        {
            if (jsonNode is not JsonArray arr)
                throw new InvalidResponseTypeException(requestUri, sysPackage);
            int length = arr.Count;
            if (length == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysPackage);
                return null;
            }
            if (length > 1)
                logger.LogMultipleResponseItems(requestUri, length - 1, sysPackage);

            if ((jsonNode = arr[0]) is not JsonObject)
                throw new InvalidResultElementTypeException(requestUri, sysPackage, 0);
            sysPackage = (JsonObject)jsonNode;
        }
        else if (jsonNode is JsonObject)
            sysPackage = (JsonObject)jsonNode;
        else
        {
            if (jsonNode is JsonArray arr && arr.Count == 0)
            {
                logger.LogNoResultsFromQuery(requestUri, sysPackage);
                return null;
            }
            throw new InvalidResponseTypeException(requestUri, sysPackage);
        }
        var package = RemoteRef.FromProperty(sysPackage, JSON_KEY_SYS_PACKAGE) ?? throw new ExpectedPropertyNotFoundException(requestUri, sysPackage, JSON_KEY_SYS_PACKAGE);
        var dependency = RemoteRef.FromProperty(sysPackage, JSON_KEY_DEPENDENCY) ?? throw new ExpectedPropertyNotFoundException(requestUri, sysPackage, JSON_KEY_DEPENDENCY);
        return new RemotePackageDependency(Package: package, Dependency: dependency, MinVersion: sysPackage.GetFieldAsString(JSON_KEY_MIN_VERSION));
    }
    
}