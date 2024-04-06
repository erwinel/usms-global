using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Services.SnApiConstants;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public class RemoteUriService
{
    /// <summary>
    /// Gets the base URL of the remote ServiceNow instance.
    /// </summary>
    internal Uri BaseURL { get; }

    /// <summary>
    /// Gets the previous remote ServiceNow instance host FQDN.
    /// </summary>
    internal string OldFqdn { get; }

    /// <summary>
    /// Gets the current remote ServiceNow instance host FQDN.
    /// </summary>
    internal string Fqdn { get; }

    /// <summary>
    /// Gets the original, user-provided URL of the remote ServiceNow instance.
    /// </summary>
    internal string OriginalUrl { get; }
    
    /// <summary>
    /// Indicates whether the remote instance is a Personal Developer Instance.
    /// </summary>
    internal bool IsPdi { get; }

    /// <summary>
    /// The new descriptive name to use for the remote instance or null to use default or existing.
    /// </summary>
    internal string? Label { get; }

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful { get; }

    internal Uri GetUrl(string path, string? query = null)
    {
        if (InitSuccessful)
            return (string.IsNullOrWhiteSpace(query) ? new UriBuilder(BaseURL) { Path = path } : new UriBuilder(BaseURL) { Path = path, Query = query }).Uri;
        return new Uri(string.IsNullOrWhiteSpace(query) ? path : (query[0] == '?') ? path + query : $"{path}?{query}", UriKind.Relative);
    }

    public RemoteUriService(IOptions<AppSettings> appSettingsOptions, ILogger<RemoteUriService> logger)
    {
        AppSettings appSettings = appSettingsOptions.Value;
        OriginalUrl = appSettings.RemoteURL ?? string.Empty;
        IsPdi = appSettings.IsPdi ?? false;
        Label = appSettings.RemoteLabel.NullIfWhiteSpace();
        if (!appSettings.ShowHelp())
        {
            if (string.IsNullOrWhiteSpace(appSettings.RemoteURL))
                logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.RemoteURL), SHORTHAND_r);
            else if (Uri.TryCreate(OriginalUrl, UriKind.Absolute, out Uri? uri))
            {
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                {
                    BaseURL = new UriBuilder(uri) { Fragment = null, Query = null, Path = "/" }.Uri;
                    Fqdn = BaseURL.Host.ToLower();
                    
                    var uriString = appSettings.ExistingURL;
                    if (string.IsNullOrWhiteSpace(uriString))
                    {
                        OldFqdn = Fqdn;
                        InitSuccessful = true;
                        return;
                    }
                    if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                    {
                        if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                        {
                            InitSuccessful = true;
                            OldFqdn = uri.Host.ToLower();
                            return;
                        }
                        logger.LogInvalidRemoteInstanceUrl(uri);
                    }
                    else
                        logger.LogInvalidRemoteInstanceUrl(Uri.TryCreate(uriString, UriKind.Relative, out uri) ? uri : new Uri(Uri.EscapeDataString(uriString), UriKind.Relative));
                    InitSuccessful = false;
                    OldFqdn = Fqdn;
                    return;
                }
                logger.LogInvalidRemoteInstanceUrl(uri);
            }
            else
                logger.LogInvalidRemoteInstanceUrl(Uri.TryCreate(OriginalUrl, UriKind.Relative, out uri) ? uri : new Uri(Uri.EscapeDataString(OriginalUrl), UriKind.Relative));
        }
        InitSuccessful = false;
        BaseURL = EmptyURI;
        OldFqdn = Fqdn = string.Empty;
    }
}