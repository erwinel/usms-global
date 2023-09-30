using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

[Serializable]
internal class InvalidResultElementType : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public JsonObject Element { get; }

    public int Index { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogInvalidResultElementType(RequestUri, Element, Index);
        IsLogged = true;
    }

    public InvalidResultElementType() => (RequestUri, Element, Index) = (EmptyURI, new JsonObject(), -1);

    public InvalidResultElementType(string? message) : base(message) => (RequestUri, Element, Index) = (EmptyURI, new JsonObject(), -1);

    public InvalidResultElementType(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Element, Index) = (EmptyURI, new JsonObject(), -1);

    public InvalidResultElementType(Uri requestUri, JsonObject element, int index) => (RequestUri, Element, Index) = (requestUri, element, index);

    public InvalidResultElementType(Uri requestUri, JsonObject element, int index, string? message) : base(message) => (RequestUri, Element, Index) = (requestUri, element, index);

    public InvalidResultElementType(Uri requestUri, JsonObject element, int index, Exception? innerException) : this(requestUri, element, index, null, innerException) { }

    public InvalidResultElementType(Uri requestUri, JsonObject element, int index, string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Element, Index) = (requestUri, element, index);

    protected InvalidResultElementType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? value = info.GetString(nameof(Element));
        if (string.IsNullOrWhiteSpace(value))
            Element = new();
        else
            try { Element = (JsonNode.Parse(value) as JsonObject) ?? new(); }
            catch { Element = new(); }
        Index = info.GetInt32(nameof(Index));
        RequestUri = string.IsNullOrEmpty(value = info.GetString(nameof(RequestUri))) ? EmptyURI : Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ? uri : new Uri(value, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(Index), Index);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}