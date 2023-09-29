using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

[Serializable]
internal class InvalidResponseTypeException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public JsonNode? Result { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogInvalidResponseType(RequestUri, Result);
        IsLogged = true;
    }

    public InvalidResponseTypeException() => (RequestUri, Result) = (EmptyURI, null);

    public InvalidResponseTypeException(string? message) : base(message) => (RequestUri, Result) = (EmptyURI, null);

    public InvalidResponseTypeException(Uri requestUri, JsonObject result, string? message = null, Exception? innerException = null) : base(message, innerException) => (RequestUri, Result) = (requestUri, result);

    public InvalidResponseTypeException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Result) = (EmptyURI, null);

    protected InvalidResponseTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? value = info.GetString(nameof(Result));
        if (string.IsNullOrWhiteSpace(value))
            Result = null;
        else
            try { Result = JsonObject.Parse(value); }
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
