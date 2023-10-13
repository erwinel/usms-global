using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class InvalidHttpResponseException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public JsonNode? Response { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogInvalidHttpResponse(RequestUri, Response);
        IsLogged = true;
    }

    public InvalidHttpResponseException() => (RequestUri, Response) = (EmptyURI, null);

    public InvalidHttpResponseException(string? message) : base(message) => (RequestUri, Response) = (EmptyURI, null);

    public InvalidHttpResponseException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Response) = (EmptyURI, null);

    internal InvalidHttpResponseException(Uri requestUri, JsonNode? response) => (RequestUri, Response) = (requestUri, response);

    internal InvalidHttpResponseException(Uri requestUri, JsonNode? response, string? message) : base(message) => (RequestUri, Response) = (requestUri, response);

    internal InvalidHttpResponseException(Uri requestUri, JsonNode? response, JsonException? innerException) : this(requestUri, response, null, innerException) { }

    internal InvalidHttpResponseException(Uri requestUri, JsonNode? response, string? message, JsonException? innerException) : base(message, innerException) => (RequestUri, Response) = (requestUri, response);

    protected InvalidHttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? value = info.GetString(nameof(Response));
        if (string.IsNullOrWhiteSpace(value))
            Response = null;
        else
            try { Response = JsonNode.Parse(value); }
            catch { Response = null; }
        RequestUri = string.IsNullOrEmpty(value = info.GetString(nameof(RequestUri))) ? EmptyURI : Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ? uri : new Uri(value, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(Response), Response?.ToJsonString());
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
