using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class ResponseParsingException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    private string ResponseBody { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        logger.LogJsonCouldNotBeParsed(RequestUri, ResponseBody, InnerException ?? this);
        IsLogged = true;
    }

    public ResponseParsingException() => (RequestUri, ResponseBody) = (EmptyURI, string.Empty);

    public ResponseParsingException(string? message) : base(message) => (RequestUri, ResponseBody) = (EmptyURI, string.Empty);

    public ResponseParsingException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, ResponseBody) = (EmptyURI, string.Empty);

    internal ResponseParsingException(Uri requestUri, string responseBody) => (RequestUri, ResponseBody) = (requestUri, responseBody);

    internal ResponseParsingException(Uri requestUri, string responseBody, string? message) : base(message) => (RequestUri, ResponseBody) = (requestUri, responseBody);

    internal ResponseParsingException(Uri requestUri, string responseBody, JsonException? innerException) : this(requestUri, responseBody, null, innerException) { }

    internal ResponseParsingException(Uri requestUri, string responseBody, string? message, JsonException? innerException) : base(message, innerException) => (RequestUri, ResponseBody) = (requestUri, responseBody);

    protected ResponseParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        ResponseBody = info.GetString(nameof(ResponseBody)) ?? string.Empty;
        string? uriString = info.GetString(nameof(RequestUri));
        RequestUri = string.IsNullOrEmpty(uriString) ? EmptyURI : Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri) ? uri : new Uri(uriString, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(ResponseBody), ResponseBody);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
