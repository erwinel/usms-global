using System.Net;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

[Serializable]
internal class RequestFailedException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public HttpStatusCode? StatusCode => (InnerException as HttpRequestException)?.StatusCode;

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogHttpRequestFailed(RequestUri, InnerException as HttpRequestException);
        IsLogged = true;
    }

    public RequestFailedException() => RequestUri = EmptyURI;

    public RequestFailedException(string? message) : base(message) => RequestUri = EmptyURI;

    internal RequestFailedException(Uri requestUri, HttpRequestException? innerException) : this(requestUri, null, innerException) { }

    internal RequestFailedException(Uri requestUri, string? message, HttpRequestException? innerException) : base(message, innerException) => RequestUri = requestUri;

    protected RequestFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? uriString = info.GetString(nameof(RequestUri));
        RequestUri = string.IsNullOrEmpty(uriString) ? EmptyURI : Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri) ? uri : new Uri(uriString, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}