using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class GetResponseContentFailedException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        logger.LogGetResponseContentFailed(RequestUri);
        IsLogged = true;
    }

    public GetResponseContentFailedException() => RequestUri = EmptyURI;

    public GetResponseContentFailedException(string? message) : base(message) => RequestUri = EmptyURI;

    public GetResponseContentFailedException(string? message, Exception? innerException) : base(message, innerException) => RequestUri = EmptyURI;

    internal GetResponseContentFailedException(Uri requestUri) => RequestUri = requestUri;

    internal GetResponseContentFailedException(Uri requestUri, string? message) : base(message) => RequestUri = requestUri;

    internal GetResponseContentFailedException(Uri requestUri, Exception? innerException) : this(requestUri, null, innerException) { }

    internal GetResponseContentFailedException(Uri requestUri, string? message, Exception? innerException) : base(message, innerException) => RequestUri = requestUri;

    protected GetResponseContentFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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