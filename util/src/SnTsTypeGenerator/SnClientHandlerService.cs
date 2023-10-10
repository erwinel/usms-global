using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.SnApiConstants;
using static SnTsTypeGenerator.CmdLineConstants;

namespace SnTsTypeGenerator;

public sealed class SnClientHandlerService
{
    private readonly ILogger<SnClientHandlerService> _logger;

    private SnAccessToken? _token;

    /// <summary>
    /// Gets the base URL of the remote ServiceNow instance.
    /// </summary>
    internal Uri BaseURL { get; }

    public NetworkCredential ClientCredentials { get; }

    public NetworkCredential UserCredentials { get; }

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => BaseURL.IsAbsoluteUri && ClientCredentials is not null && UserCredentials is not null;

    internal HttpClientHandler CreateHttpClientHandler()
    {
        if (!InitSuccessful)
            throw new InvalidOperationException();
        return new() { Credentials = UserCredentials };
    }

    private async Task<JsonNode?> GetJsonResponseAsync(HttpClientHandler handler, HttpRequestMessage message, Uri requestUri, CancellationToken cancellationToken)
    {
        using HttpClient httpClient = new(handler);
        _logger.LogAPIRequestStart(requestUri);
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
        _logger.LogAPIRequestCompleted(requestUri, result);
        return result;
    }

    /// <summary>
    /// Gets a ServiceNow access token.
    /// </summary>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The access token.</returns>
    /// <seealso href="https://docs.servicenow.com/en-US/bundle/vancouver-platform-security/page/administer/security/reference/r_OAuthAPIResponseParameters.html"/>
    internal async Task<SnAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        SnAccessToken? token = _token;
        JsonNode? jsonNode;
        DateTime createdOn;
        Uri requestUri;
        if (token is null)
        {
            createdOn = DateTime.Now;
            using HttpClientHandler handler = CreateHttpClientHandler();
            using HttpClient httpClient = new(handler);
            requestUri = new UriBuilder(BaseURL) { Path = URI_PATH_AUTH_TOKEN }.Uri;
            HttpRequestMessage message = new(HttpMethod.Post, requestUri);
            message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            message.Headers.Add(HEADER_KEY_CLIENT_ID, ClientCredentials.UserName);
            message.Headers.Add(HEADER_KEY_CLIENT_SECRET, ClientCredentials.Password);
            message.Headers.Add(HEADER_KEY_GRANT_TYPE, HEADER_KEY_PASSWORD);
            message.Headers.Add(HEADER_KEY_USERNAME, UserCredentials.UserName);
            message.Headers.Add(HEADER_KEY_PASSWORD, UserCredentials.Password);
            jsonNode = await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
        }
        else
        {
            if (token.ExpiresOn < DateTime.Now)
                return token;
            createdOn = DateTime.Now;
            using HttpClientHandler handler = CreateHttpClientHandler();
            using HttpClient httpClient = new(handler);
            requestUri = new UriBuilder(BaseURL) { Path = URI_PATH_AUTH_TOKEN }.Uri;
            HttpRequestMessage message = new(HttpMethod.Post, requestUri);
            message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            message.Headers.Add(HEADER_KEY_CLIENT_ID, ClientCredentials.UserName);
            message.Headers.Add(HEADER_KEY_CLIENT_SECRET, ClientCredentials.Password);
            message.Headers.Add(HEADER_KEY_GRANT_TYPE, HEADER_KEY_REFRESH_TOKEN);
            message.Headers.Add(HEADER_KEY_REFRESH_TOKEN, token.RefreshToken);
            jsonNode = await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
        }
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidResponseTypeException(requestUri, jsonNode);
        if (resultObj.TryCoercePropertyAsNonEmpty(JSON_KEY_ACCESS_TOKEN, out string? access_token) && resultObj.TryCoercePropertyAsNonEmpty(JSON_KEY_REFRESH_TOKEN, out string? refresh_token) && resultObj.TryCoercePropertyAsInt("expires_in", out int expires_in))
        {
            _token = token = new(access_token, refresh_token, createdOn.AddSeconds(expires_in));
            return token;
        }
        throw new InvalidHttpResponseException(requestUri, resultObj);
    }

    private async Task<JsonNode?> GetJsonAsync(HttpClientHandler handler, Uri requestUri, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        configureHeaders?.Invoke(message.Headers);
        return await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
    }

    private async Task<JsonNode?> PostJsonAsync(HttpClientHandler handler, Uri requestUri, JsonNode? content, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClient httpClient = new(handler);
        HttpRequestMessage message = new(HttpMethod.Post, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        configureHeaders?.Invoke(message.Headers);
        if (content is not null)
            message.Content = JsonContent.Create(content, new MediaTypeHeaderValue(MediaTypeNames.Application.Json));
        return await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, string query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path, Query = query }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, string query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path, Query = query }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, string query, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path, Query = query }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, string query, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path, Query = query }.Uri;
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, null, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, JsonNode content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, content, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, null, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, JsonNode content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, content, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, null, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, JsonNode content, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, content, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, null, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, JsonNode content, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = new UriBuilder(BaseURL) { Path = path }.Uri;
        return (requestUri, await PostJsonAsync(handler, requestUri, content, configureHeaders, cancellationToken));
    }

    public SnClientHandlerService(IOptions<AppSettings> appSettingsOptions, ILogger<SnClientHandlerService> logger)
    {
        _logger = logger;
        AppSettings appSettings = appSettingsOptions.Value;
        var remoteUri = appSettings.RemoteURL;
        if (string.IsNullOrWhiteSpace(remoteUri))
        {
            _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.RemoteURL), SHORTHAND_r);
            BaseURL = EmptyURI;
            ClientCredentials = UserCredentials = null!;
            return;
        }
        if (Uri.TryCreate(remoteUri, UriKind.Absolute, out Uri? uri))
        {
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                BaseURL = EmptyURI;
                _logger.LogInvalidRemoteInstanceUrl(uri);
                ClientCredentials = UserCredentials = null!;
                return;
            }
            BaseURL = new UriBuilder(uri) { Fragment = null, Query = null, Path = "/" }.Uri;
            string? clientId = appSettings.ClientId;
            if (string.IsNullOrWhiteSpace(clientId))
            {
                Console.Write("Client ID: ");
                clientId = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.ClientId));
                    ClientCredentials = UserCredentials = null!;
                    return;
                }
            }
            string? clientSecret = appSettings.ClientSecret;
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                Console.Write("Client Secret: ");
                clientSecret = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.ClientSecret));
                    ClientCredentials = UserCredentials = null!;
                    return;
                }
            }
            ClientCredentials = new(clientId, clientSecret);
            string? userName = appSettings.UserName;
            if (string.IsNullOrWhiteSpace(userName))
            {
                Console.Write("User Name: ");
                userName = Console.ReadLine();
                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.UserName), SHORTHAND_u);
                    UserCredentials = null!;
                    return;
                }
            }
            string? password = appSettings.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.Write("Password: ");
                password = Console.ReadLine();
                if (string.IsNullOrEmpty(password))
                {
                    _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.Password), SHORTHAND_p);
                    UserCredentials = null!;
                    return;
                }
            }
            UserCredentials = new NetworkCredential(appSettings.UserName, password);
        }
        else
        {
            BaseURL = EmptyURI;
            _logger.LogInvalidRemoteInstanceUrl(Uri.TryCreate(remoteUri, UriKind.Relative, out uri) ? uri : new Uri(Uri.EscapeDataString(remoteUri), UriKind.Relative));
            ClientCredentials = UserCredentials = null!;
        }
    }
}
