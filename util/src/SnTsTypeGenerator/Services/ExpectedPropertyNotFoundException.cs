using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class ExpectedPropertyNotFoundException : Exception, ILogTrackable
{
    public Uri RequestUri { get; }

    public JsonObject Element { get; }

    public string PropertyName { get; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogExpectedPropertyNotFound(RequestUri, PropertyName, Element);
        IsLogged = true;
    }

    public ExpectedPropertyNotFoundException() => (RequestUri, Element, PropertyName) = (EmptyURI, new JsonObject(), string.Empty);

    public ExpectedPropertyNotFoundException(string? message) : base(message) => (RequestUri, Element, PropertyName) = (EmptyURI, new JsonObject(), string.Empty);

    public ExpectedPropertyNotFoundException(string? message, Exception? innerException) : base(message, innerException) => (RequestUri, Element, PropertyName) = (EmptyURI, new JsonObject(), string.Empty);

    public ExpectedPropertyNotFoundException(Uri requestUri, JsonObject element, string propertyName) => (RequestUri, Element, PropertyName) = (requestUri, element, propertyName);

    public ExpectedPropertyNotFoundException(Uri requestUri, JsonObject element, string propertyName, string? message) : base(message) => (RequestUri, Element, PropertyName) = (requestUri, element, propertyName);

    public ExpectedPropertyNotFoundException(Uri requestUri, JsonObject element, string propertyName, Exception? innerException) : this(requestUri, element, propertyName, null, innerException) { }

    public ExpectedPropertyNotFoundException(Uri requestUri, JsonObject element, string propertyName, string? message, Exception? innerException) : base(message, innerException) =>
        (RequestUri, Element, PropertyName) = (requestUri, element, propertyName);

    protected ExpectedPropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        string? value = info.GetString(nameof(Element));
        if (string.IsNullOrWhiteSpace(value))
            Element = new();
        else
            try { Element = (JsonNode.Parse(value) as JsonObject) ?? new(); }
            //codeql[cs/catch-of-all-exceptions] No need to record exception.
            catch { Element = new(); }
        PropertyName = info.GetString(nameof(PropertyName)) ?? string.Empty;
        RequestUri = string.IsNullOrEmpty(value = info.GetString(nameof(RequestUri))) ? EmptyURI : Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ? uri : new Uri(value, UriKind.Relative);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestUri), RequestUri.OriginalString);
        info.AddValue(nameof(PropertyName), PropertyName);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
