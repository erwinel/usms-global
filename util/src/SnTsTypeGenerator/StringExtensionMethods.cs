using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator;

public static partial class StringExtensionMethods
{
    internal static bool NoCaseEquals(this string? x, string? y) => string.IsNullOrWhiteSpace(x) ? string.IsNullOrWhiteSpace(y) : NameComparer.Equals(x, y);

    public static string ToDisplayName(this HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.Continue => "client can continue with its request",
        HttpStatusCode.SwitchingProtocols => "Switching Protocols",
        HttpStatusCode.EarlyHints => "Early Hints",
        HttpStatusCode.NonAuthoritativeInformation => "Non Authoritative Information",
        HttpStatusCode.NoContent => "No Content",
        HttpStatusCode.ResetContent => "Reset Content",
        HttpStatusCode.PartialContent => "Partial Content",
        HttpStatusCode.MultiStatus => "Multi Status",
        HttpStatusCode.AlreadyReported => "Already Reported",
        HttpStatusCode.IMUsed => "IM Used",
        HttpStatusCode.RedirectMethod => "Redirect Method",
        HttpStatusCode.NotModified => "Not Modified",
        HttpStatusCode.UseProxy => "Use Proxy",
        HttpStatusCode.RedirectKeepVerb => "Redirect Keep Verb",
        HttpStatusCode.PermanentRedirect => "Permanent Redirect",
        HttpStatusCode.BadRequest => "Bad Request",
        HttpStatusCode.PaymentRequired => "Payment Required",
        HttpStatusCode.NotFound => "Not Found",
        HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
        HttpStatusCode.NotAcceptable => "Not Acceptable",
        HttpStatusCode.ProxyAuthenticationRequired => "Proxy Authentication Required",
        HttpStatusCode.RequestTimeout => "Request Timeout",
        HttpStatusCode.LengthRequired => "Length Required",
        HttpStatusCode.PreconditionFailed => "Precondition Failed",
        HttpStatusCode.RequestEntityTooLarge => "Request Entity Too Large",
        HttpStatusCode.RequestUriTooLong => "Request URI Too Long",
        HttpStatusCode.UnsupportedMediaType => "Unsupported Media Type",
        HttpStatusCode.RequestedRangeNotSatisfiable => "Requested Range Not Satisfiable",
        HttpStatusCode.ExpectationFailed => "Expectation Failed",
        HttpStatusCode.MisdirectedRequest => "Misdirected Request",
        HttpStatusCode.UnprocessableEntity => "Unprocessable Entity",
        HttpStatusCode.FailedDependency => "Failed Dependency",
        HttpStatusCode.UpgradeRequired => "Upgrade Required",
        HttpStatusCode.PreconditionRequired => "Precondition Required",
        HttpStatusCode.TooManyRequests => "Too Many Requests",
        HttpStatusCode.RequestHeaderFieldsTooLarge => "Request Header Fields Too Large",
        HttpStatusCode.UnavailableForLegalReasons => "Unavailable For Legal Reasons",
        HttpStatusCode.InternalServerError => "Internal Server Error",
        HttpStatusCode.NotImplemented => "Not Implemented",
        HttpStatusCode.BadGateway => "Bad Gateway",
        HttpStatusCode.ServiceUnavailable => "Service Unavailable",
        HttpStatusCode.GatewayTimeout => "Gateway Timeout",
        HttpStatusCode.HttpVersionNotSupported => "Http Version Not Supported",
        HttpStatusCode.VariantAlsoNegotiates => "Variant Also Negotiates",
        HttpStatusCode.InsufficientStorage => "Insufficient Storage",
        HttpStatusCode.LoopDetected => "Loop Detected",
        HttpStatusCode.NotExtended => "Not Extended",
        HttpStatusCode.NetworkAuthenticationRequired => "Network Authentication Required",
        _ => statusCode.ToString("F"),
    };

    public static string? ToDescription(this HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.Continue => "client can continue with its request",
        HttpStatusCode.SwitchingProtocols => "Protocol version or protocol is being changed.",
        HttpStatusCode.Processing => "Server has accepted the complete request but hasn't completed it yet.",
        HttpStatusCode.EarlyHints => "Server is likely to send a final response with the header fields included in the informational response.",
        HttpStatusCode.OK => "Request succeeded.",
        HttpStatusCode.Created => "New resource created before response was sent.",
        HttpStatusCode.Accepted => "Request accepted for further processing.",
        HttpStatusCode.NonAuthoritativeInformation => "Returned meta information is from a cached copy.",
        HttpStatusCode.NoContent => "Request has been successfully processed; Response is intentionally blank.",
        HttpStatusCode.ResetContent => "Client should reset (not reload) the current resource.",
        HttpStatusCode.PartialContent => "Response is partial.",
        HttpStatusCode.MultiStatus => "Multiple status codes for single response from WebDAV operation.",
        HttpStatusCode.AlreadyReported => "Members of WebDAV binding previously enumerated in preceding multistatus response not included.",
        HttpStatusCode.IMUsed => "Response represents result of one or more instance-manipulations applied to current instance.",
        HttpStatusCode.Ambiguous => "Requested information has multiple representations.",
        HttpStatusCode.Moved => "Requested information moved to URI specified in Location header.",
        HttpStatusCode.Found or HttpStatusCode.RedirectKeepVerb or HttpStatusCode.PermanentRedirect => "Requested information is located at URI specified in Location header.",
        HttpStatusCode.RedirectMethod => "Redirect to the specified in Location header.",
        HttpStatusCode.NotModified => "Cached copy is up to date.",
        HttpStatusCode.UseProxy => "Use proxy server at URI specified in Location header.",
        HttpStatusCode.Unused => "Extension to HTTP/1.1 specification not fully specified.",
        HttpStatusCode.BadRequest => "Request not understood.",
        HttpStatusCode.Unauthorized => "Requested resource requires authentication.",
        HttpStatusCode.Forbidden => "Request fulfillment refused.",
        HttpStatusCode.NotFound => "Requested resource does not exist.",
        HttpStatusCode.MethodNotAllowed => "Request method not allowed on requested resource.",
        HttpStatusCode.NotAcceptable => "Accept headers not available as representation of resource.",
        HttpStatusCode.ProxyAuthenticationRequired => "Requested proxy requires authentication.",
        HttpStatusCode.RequestTimeout => "Request not sent within expected timeframe.",
        HttpStatusCode.Conflict => "Request not carried out due to conflict.",
        HttpStatusCode.Gone => "Requested resource no longer available.",
        HttpStatusCode.LengthRequired => "Content-length header missing.",
        HttpStatusCode.RequestedRangeNotSatisfiable => "Range of requested data cannot be returned.",
        HttpStatusCode.ExpectationFailed => "Expect header not be met by server.",
        HttpStatusCode.MisdirectedRequest => "Request directed at server that is not able to produce a response.",
        HttpStatusCode.UnprocessableEntity => "Well-formed request unable to be followed due to semantic errors.",
        HttpStatusCode.Locked => "Source or destination resource is locked.",
        HttpStatusCode.FailedDependency => "Method couldn't be performed on resource due to dependency upon another action.",
        HttpStatusCode.UpgradeRequired => "Client should switch to a different protocol.",
        HttpStatusCode.PreconditionRequired => "Request must be conditional.",
        HttpStatusCode.NotImplemented => "Requested function not supported.",
        HttpStatusCode.BadGateway => "Proxy server received a bad response from another proxy or the origin server.",
        HttpStatusCode.ServiceUnavailable => "Server temporarily unavailable.",
        HttpStatusCode.GatewayTimeout => "Proxy server timed out while waiting for a response.",
        HttpStatusCode.VariantAlsoNegotiates => "Chosen variant resource not a proper endpoint in negotiation process because is configured to engage in transparent content negotiation.",
        HttpStatusCode.InsufficientStorage => "Server is unable to store the representation needed to complete the request.",
        HttpStatusCode.LoopDetected => "Operation terminated due to infinite loop.",
        HttpStatusCode.NotExtended => "Further request extensions required for fulfillment.",
        HttpStatusCode.NetworkAuthenticationRequired => "Authentication required for network access.",
        _ => null,
    };

    [GeneratedRegex(@" \s+|(?! )\s+", RegexOptions.Compiled)]
    private static partial Regex GetAbnormalWsRegexRegex();
    public static readonly Regex AbnormalWsRegex = GetAbnormalWsRegexRegex();

    public static string AsWhitespaceNormalized(this string? text) => text is null || (text = text.Trim()).Length == 0 ? string.Empty : AbnormalWsRegex.Replace(text, " ");

    public static string AsNonEmpty(this string? value, Func<string> getDefaultValue) => string.IsNullOrWhiteSpace(value) ? getDefaultValue() : value;

    public static string AsNonEmpty(this string? value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;

    public static string? NullIfWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static readonly ImmutableArray<string> JSDOC_START = new string[] { "/**" }.ToImmutableArray();

    private static readonly ImmutableArray<string> JSDOC_END = new string[] { " */" }.ToImmutableArray();

    [GeneratedRegex(@"\r\n?|\n", RegexOptions.Compiled)]
    private static partial Regex GetLineBreakRegex();
    public static readonly Regex LineBreakRegex = GetLineBreakRegex();

    public static string[] SplitLines(this string? lines) => string.IsNullOrEmpty(lines) ? new string[] { "" } : LineBreakRegex.Split(lines);

    public static IEnumerable<string> ToJsDocLines(this IEnumerable<string> lines) => JSDOC_START.Concat(lines.Select(l => string.IsNullOrWhiteSpace(l) ? " *" : $" * {l}")).Concat(JSDOC_END);

    [GeneratedRegex(@"^[a-z_][a-z\d_]*$", RegexOptions.Compiled)]
    private static partial Regex GetScopeNameRegex();
    public static readonly Regex ScopeNameRegex = GetScopeNameRegex();

    public static bool IsValidScopeName([NotNullWhen(true)] this string? value) => !string.IsNullOrEmpty(value) && ScopeNameRegex.IsMatch(value);
}
