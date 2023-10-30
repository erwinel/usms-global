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

    public string? Response { get; }

    public string? ContentType { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        if (string.IsNullOrEmpty(ContentType))
            logger.LogInvalidHttpResponse(RequestUri, Response, InnerException ?? this);
        else
            logger.LogInvalidHttpResponse(RequestUri, ContentType, Response, InnerException ?? this);
        IsLogged = true;
    }

    public InvalidHttpResponseException() => RequestUri = EmptyURI;


    public InvalidHttpResponseException(string? message) : base(message) => RequestUri = EmptyURI;

    public InvalidHttpResponseException(string? message, Exception? innerException) : base(message, innerException) => RequestUri = EmptyURI;

    internal InvalidHttpResponseException(Uri requestUri, string? response, string? contentType = null) => (RequestUri, Response, ContentType) = (requestUri, response, contentType);

    internal InvalidHttpResponseException(Uri requestUri, string? response, string? contentType, string? message) : base(message) => (RequestUri, Response, ContentType) = (requestUri, response, contentType);

    internal InvalidHttpResponseException(Uri requestUri, string? response, string? contentType, JsonException? innerException) : this(requestUri, response, contentType, null, innerException) { }

    internal InvalidHttpResponseException(Uri requestUri, string? response, JsonException? innerException) : this(requestUri, response, null, null, innerException) { }

    internal InvalidHttpResponseException(Uri requestUri, string? response, string? contentType, string? message, JsonException? innerException) : base(message, innerException) => (RequestUri, Response, ContentType) = (requestUri, response, contentType);

    protected InvalidHttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        Response = info.GetString(nameof(Response));
        ContentType = info.GetString(nameof(ContentType));
        string? value = info.GetString(nameof(RequestUri));
        RequestUri = string.IsNullOrEmpty(value) ? EmptyURI : Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ? uri : new Uri(value, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(Response), Response);
        info.AddValue(nameof(ContentType), ContentType);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
