using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

[Serializable]
internal class InvalidHttpResponseException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    private string ResponseBody { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogInvalidHttpResponse(RequestUri, ResponseBody);
        IsLogged = true;
    }

    public InvalidHttpResponseException() => (RequestUri, ResponseBody) = (EmptyURI, string.Empty);

    public InvalidHttpResponseException(string? message) : base(message) => (RequestUri, ResponseBody) = (EmptyURI, string.Empty);

    public InvalidHttpResponseException(Uri requestUri, string responseBody, Exception? innerException) : this(requestUri, responseBody, null, innerException) { }

    public InvalidHttpResponseException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, ResponseBody) = (EmptyURI, string.Empty);

    internal InvalidHttpResponseException(Uri requestUri, string responseBody, JsonException? innerException) : this(requestUri, responseBody, null, innerException) { }

    internal InvalidHttpResponseException(Uri requestUri, string responseBody, string? message = null, Exception? innerException = null) : base(message, innerException) => (RequestUri, ResponseBody) = (requestUri, responseBody);

    protected InvalidHttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
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
