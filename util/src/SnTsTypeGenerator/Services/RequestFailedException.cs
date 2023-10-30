using System.Net;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class RequestFailedException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public string? Response { get; }

    public HttpStatusCode? StatusCode => (InnerException as HttpRequestException)?.StatusCode;

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        logger.LogHttpRequestFailed(RequestUri, InnerException ?? this);
        IsLogged = true;
    }

    public RequestFailedException() => RequestUri = EmptyURI;

    public RequestFailedException(string? message) : base(message) => RequestUri = EmptyURI;

    internal RequestFailedException(Uri requestUri) => RequestUri = requestUri;

    internal RequestFailedException(Uri requestUri, string? message, string? response = null) : base(message) => (RequestUri, Response) = (requestUri, response);

    internal RequestFailedException(Uri requestUri, HttpRequestException? innerException, string? response = null) : this(requestUri, null, innerException, response) { }

    internal RequestFailedException(Uri requestUri, string? message, HttpRequestException? innerException, string? response = null) : base(message, innerException) => (RequestUri, Response) = (requestUri, response);

    protected RequestFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        Response = info.GetString(nameof(Response));
        string? uriString = info.GetString(nameof(RequestUri));
        RequestUri = string.IsNullOrEmpty(uriString) ? EmptyURI : Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri) ? uri : new Uri(uriString, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(IsLogged), IsLogged);
        info.AddValue(nameof(Response), Response);
    }
}