using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class ResponseResultPropertyNotFoundException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public JsonObject Result { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        logger.LogResponseResultPropertyNotFound(RequestUri, Result, InnerException ?? this);
        IsLogged = true;
    }

    public ResponseResultPropertyNotFoundException() => (RequestUri, Result) = (EmptyURI, new JsonObject());

    public ResponseResultPropertyNotFoundException(string? message) : base(message) => (RequestUri, Result) = (EmptyURI, new JsonObject());

    public ResponseResultPropertyNotFoundException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Result) = (EmptyURI, new JsonObject());

    public ResponseResultPropertyNotFoundException(Uri requestUri, JsonObject result) => (RequestUri, Result) = (requestUri, result);

    public ResponseResultPropertyNotFoundException(Uri requestUri, JsonObject result, string? message) : base(message) => (RequestUri, Result) = (requestUri, result);

    public ResponseResultPropertyNotFoundException(Uri requestUri, JsonObject result, Exception? innerException) : this(requestUri, result, null, innerException) { }

    public ResponseResultPropertyNotFoundException(Uri requestUri, JsonObject result, string? message, Exception? innerException) : base(message, innerException) =>
        (RequestUri, Result) = (requestUri, result);

    protected ResponseResultPropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? value = info.GetString(nameof(Result));
        if (string.IsNullOrWhiteSpace(value))
            Result = new();
        else
            try { Result = (JsonObject.Parse(value) as JsonObject) ?? new(); }
            //codeql[cs/catch-of-all-exceptions] No need to record exception.
            catch { Result = new(); }
        RequestUri = string.IsNullOrEmpty(value = info.GetString(nameof(RequestUri))) ? EmptyURI : Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ? uri : new Uri(value, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(Result), Result.ToJsonString());
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}