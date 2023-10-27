using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class InvalidResponseTypeException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public JsonNode? Result { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        logger.LogInvalidResponseType(RequestUri, Result);
        IsLogged = true;
    }

    public InvalidResponseTypeException() => (RequestUri, Result) = (EmptyURI, null);

    public InvalidResponseTypeException(string? message) : base(message) => (RequestUri, Result) = (EmptyURI, null);

    public InvalidResponseTypeException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Result) = (EmptyURI, null);

    public InvalidResponseTypeException(Uri requestUri, JsonNode? result) => (RequestUri, Result) = (requestUri, result);

    public InvalidResponseTypeException(Uri requestUri, JsonNode? result, string? message) : base(message) => (RequestUri, Result) = (requestUri, result);

    public InvalidResponseTypeException(Uri requestUri, JsonNode? result, Exception? innerException) : this(requestUri, result, null, innerException) { }

    public InvalidResponseTypeException(Uri requestUri, JsonNode? result, string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Result) = (requestUri, result);

    protected InvalidResponseTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? value = info.GetString(nameof(Result));
        if (string.IsNullOrWhiteSpace(value))
            Result = null;
        else
            try { Result = JsonNode.Parse(value); }
            //codeql[cs/catch-of-all-exceptions] No need to record exception.
            catch { Result = null; }
        RequestUri = string.IsNullOrEmpty(value = info.GetString(nameof(RequestUri))) ? EmptyURI : Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ? uri : new Uri(value, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(Result), Result?.ToJsonString());
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
