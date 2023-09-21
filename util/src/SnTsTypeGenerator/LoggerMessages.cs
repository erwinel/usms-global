using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator;

public static class LoggerMessages
{
    #region Critical DbfileValidation Error (0x0001)
    
    public const int EVENT_ID_CriticalDbfileValidationError = 0x0001;
    public static readonly EventId CriticalDbfileValidationError = new(EVENT_ID_CriticalDbfileValidationError, nameof(CriticalDbfileValidationError));
    private static readonly Action<ILogger, string, Exception?> _criticalDbfileValidationError = LoggerMessage.Define<string>(LogLevel.Critical, CriticalDbfileValidationError,
        "Unexpected error validating DB file path \"{DbFile}\".");
    /// <summary>
    /// Logs a DbfileValidation event with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbfileValidationError(this ILogger logger, string dbFile, Exception error) => _criticalDbfileValidationError(logger, dbFile, error);
    
    #endregion
    #region Critical DbfileAccess Error (0x0002)
    
    public const int EVENT_ID_CriticalDbfileAccessError = 0x0002;
    public static readonly EventId CriticalDbfileAccessError = new(EVENT_ID_CriticalDbfileAccessError, nameof(CriticalDbfileAccessError));
    private static readonly Action<ILogger, string, Exception?> _criticalDbfileAccessError = LoggerMessage.Define<string>(LogLevel.Critical, CriticalDbfileAccessError,
        "Unable to create DB file \"{Dbfile}\".");
    /// <summary>
    /// Logs a DbfileAccess event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbfileAccessError(this ILogger logger, string dbFile, Exception error) => _criticalDbfileAccessError(logger, dbFile, error);
    
    #endregion
    
    #region Critical DbInitializationFailure Error (0x0003)
    
    public const int EVENT_ID_CriticalDbInitializationFailureError = 0x0003;
    public static readonly EventId CriticalDbInitializationFailureError = new(EVENT_ID_CriticalDbInitializationFailureError, nameof(CriticalDbInitializationFailureError));
    private static readonly Action<ILogger, string, Type, string, Exception> _criticalDbInitializationFailureError = LoggerMessage.Define<string, Type, string>(LogLevel.Critical, CriticalDbInitializationFailureError,
        "Unexpected error while executing DB initialization query {QueryString} for {Type} in {DbPath}.");
    /// <summary>
    /// Logs a DbInitializationFailure event with event code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="querystring">The query string that failed.</param>
    /// <param name="type">The DB entity object type.</param>
    /// <param name="dbpath">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbInitializationFailureError(this ILogger logger, string querystring, Type type, string dbpath, Exception error) => _criticalDbInitializationFailureError(logger, querystring, type, dbpath, error);
    
    #endregion
    
    #region Critical UserNameNotProvided Error (0x0004)
    
    public const int EVENT_ID_CriticalUserNameNotProvidedError = 0x0004;
    public static readonly EventId CriticalUserNameNotProvidedError = new(EVENT_ID_CriticalUserNameNotProvidedError, nameof(CriticalUserNameNotProvidedError));
    private static readonly Action<ILogger, Exception?> _criticalUserNameNotProvidedError = LoggerMessage.Define(LogLevel.Critical, CriticalUserNameNotProvidedError,
        "User name was not provided.");
    /// <summary>
    /// Logs a UserNameNotProvided event with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalUserNameNotProvidedError(this ILogger logger) => _criticalUserNameNotProvidedError(logger, null);
    
    #endregion

    #region Critical PasswordNotProvided Error (0x0005)
    
    public const int EVENT_ID_CriticalPasswordNotProvidedError = 0x0005;
    public static readonly EventId CriticalPasswordNotProvidedError = new(EVENT_ID_CriticalPasswordNotProvidedError, nameof(CriticalPasswordNotProvidedError));
    private static readonly Action<ILogger, Exception?> _criticalPasswordNotProvidedError = LoggerMessage.Define(LogLevel.Critical, CriticalPasswordNotProvidedError,
        "Password was not provided.");
    /// <summary>
    /// Logs a PasswordNotProvided event with event code 0x0005.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalPasswordNotProvidedError(this ILogger logger) => _criticalPasswordNotProvidedError(logger, null);
    
    #endregion

    #region Critical RemoteInstanceUriNotProvided Error (0x0006)
    
    public const int EVENT_ID_CriticalRemoteInstanceUriNotProvidedError = 0x0006;
    public static readonly EventId CriticalRemoteInstanceUriNotProvidedError = new(EVENT_ID_CriticalRemoteInstanceUriNotProvidedError, nameof(CriticalRemoteInstanceUriNotProvidedError));
    private static readonly Action<ILogger, Exception?> _criticalRemoteInstanceUriNotProvidedError = LoggerMessage.Define(LogLevel.Critical, CriticalRemoteInstanceUriNotProvidedError,
        "The remote ServiceNow instance URI was not provided.");
    /// <summary>
    /// Logs a RemoteInstanceUriNotProvided event with event code 0x0006.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalRemoteInstanceUriNotProvidedError(this ILogger logger) => _criticalRemoteInstanceUriNotProvidedError(logger, null);
    
    #endregion
    #region Critical InvalidRemoteInstanceUri Error (0x0007)
    
    public const int EVENT_ID_CriticalInvalidRemoteInstanceUriError = 0x0007;
    public static readonly EventId CriticalInvalidRemoteInstanceUriError = new(EVENT_ID_CriticalInvalidRemoteInstanceUriError, nameof(CriticalInvalidRemoteInstanceUriError));
    private static readonly Action<ILogger, Exception?> _criticalInvalidRemoteInstanceUriError = LoggerMessage.Define(LogLevel.Critical, CriticalInvalidRemoteInstanceUriError,
        "The remote ServiceNow instance URI was not an absolute URI with the http or https scheme.");
    /// <summary>
    /// Logs an InvalidRemoteInstanceUri event with event code 0x0007.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalInvalidRemoteInstanceUriError(this ILogger logger) => _criticalInvalidRemoteInstanceUriError(logger, null);
    
    #endregion
    #region NoTableNamesSpecified Warning (0x0008)
    
    public const int EVENT_ID_NoTableNamesSpecifiedWarning = 0x0008;
    public static readonly EventId NoTableNamesSpecifiedWarning = new(EVENT_ID_NoTableNamesSpecifiedWarning, nameof(NoTableNamesSpecifiedWarning));
    private static readonly Action<ILogger, Exception?> _noTableNamesSpecifiedWarning = LoggerMessage.Define(LogLevel.Warning, NoTableNamesSpecifiedWarning,
        "No table names were specified; nothing to do.");
    /// <summary>
    /// Logs a NoTableNamesSpecified event with event code 0x0008.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNoTableNamesSpecifiedWarning(this ILogger logger) => _noTableNamesSpecifiedWarning(logger, null);
    
    #endregion
    
    #region Critical GlobalAndScopedSwitchesBothSet Error (0x0009)
    
    public const int EVENT_ID_CriticalGlobalAndScopedSwitchesBothSetError = 0x0009;
    public static readonly EventId CriticalGlobalAndScopedSwitchesBothSetError = new(EVENT_ID_CriticalGlobalAndScopedSwitchesBothSetError, nameof(CriticalGlobalAndScopedSwitchesBothSetError));
    private static readonly Action<ILogger, Exception?> _criticalGlobalAndScopedSwitchesBothSetError = LoggerMessage.Define(LogLevel.Critical, CriticalGlobalAndScopedSwitchesBothSetError,
        $"The {nameof(AppSettings.Global)} ({AppSettings.SHORTHAND_g}) and {nameof(AppSettings.Scoped)} ({AppSettings.SHORTHAND_s}) options cannot be specified at the same time.");
    
    /// <summary>
    /// Logs an GlobalAndScopedSwitchesBothSet event with event code 0x0009.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalGlobalAndScopedSwitchesBothSetError(this ILogger logger) => _criticalGlobalAndScopedSwitchesBothSetError(logger, null);
    
    #endregion

    #region Critical OutputFileAlreadyExists Error (0x0010)
    
    public const int EVENT_ID_CriticalOutputFileAlreadyExistsError = 0x0010;
    public static readonly EventId CriticalOutputFileAlreadyExistsError = new(EVENT_ID_CriticalOutputFileAlreadyExistsError, nameof(CriticalOutputFileAlreadyExistsError));
    private static readonly Action<ILogger, string, Exception?> _criticalOutputFileAlreadyExistsError = LoggerMessage.Define<string>(LogLevel.Critical, CriticalOutputFileAlreadyExistsError,
        "File {Path}");
    /// <summary>
    /// Logs an OutputFileAlreadyExists event with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalOutputFileAlreadyExistsError(this ILogger logger, string path) => _criticalOutputFileAlreadyExistsError(logger, path, null);
    
    #endregion

    #region Critical OutputFileAccessError Error (0x0011)
    
    public const int EVENT_ID_CriticalOutputFileAccessErrorError = 0x0011;
    public static readonly EventId CriticalOutputFileAccessErrorError = new(EVENT_ID_CriticalOutputFileAccessErrorError, nameof(CriticalOutputFileAccessErrorError));
    private static readonly Action<ILogger, string, Exception?> _criticalOutputFileAccessErrorError1 = LoggerMessage.Define<string>(LogLevel.Critical, CriticalOutputFileAccessErrorError,
        "Error accessing output file {Path}.");
    private static readonly Action<ILogger, string, string, Exception?> _criticalOutputFileAccessErrorError2 = LoggerMessage.Define<string, string>(LogLevel.Critical, CriticalOutputFileAccessErrorError,
        "Error accessing output file {Path}: {Message}");

    /// <summary>
    /// Logs an OutputFileAccessError event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalOutputFileAccessErrorError(this ILogger logger, string path, Exception? error = null) => _criticalOutputFileAccessErrorError1(logger, path, error);
    
    /// <summary>
    /// Logs an OutputFileAccessError event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="message">The message describing the error.</param>
    public static void LogCriticalOutputFileAccessErrorError(this ILogger logger, string path, string message) => _criticalOutputFileAccessErrorError2(logger, path, message, null);
    
    #endregion

    #region HttpRequestFailed Error (0x0012)
    
    public const int EVENT_ID_HttpRequestFailedError = 0x0012;
    public static readonly EventId HttpRequestFailedError = new(EVENT_ID_HttpRequestFailedError, nameof(HttpRequestFailedError));
    private static readonly Action<ILogger, Uri, Exception?> _httpRequestFailedError = LoggerMessage.Define<Uri>(LogLevel.Error, HttpRequestFailedError,
        "Remote request failed ({URI}).");
    /// <summary>
    /// Logs an HttpRequestFailed event with event code 0x0012.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI that failed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogHttpRequestFailedError(this ILogger logger, Uri uri, HttpRequestException error) => _httpRequestFailedError(logger, uri, error);
    
    #endregion

    #region GetResponseContentFailed Error (0x0013)
    
    public const int EVENT_ID_GetResponseContentFailedError = 0x0013;
    public static readonly EventId GetResponseContentFailedError = new(EVENT_ID_GetResponseContentFailedError, nameof(GetResponseContentFailedError));
    private static readonly Action<ILogger, Uri, Exception?> _getResponseContentFailedError = LoggerMessage.Define<Uri>(LogLevel.Error, GetResponseContentFailedError,
        "Failed to get text-based content from remote URI {URI}");
    /// <summary>
    /// Logs a GetResponseContentFailed event with event code 0x0013.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request uri.</param>
    /// <param name="error">The optional exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogGetResponseContentFailedError(this ILogger logger, Uri uri, Exception? error = null) => _getResponseContentFailedError(logger, uri, error);
    
    #endregion

    #region JsonCouldNotBeParsed Error (0x0014)
    
    public const int EVENT_ID_JsonCouldNotBeParsedError = 0x0014;
    public static readonly EventId JsonCouldNotBeParsedError = new(EVENT_ID_JsonCouldNotBeParsedError, nameof(JsonCouldNotBeParsedError));
    private static readonly Action<ILogger, Uri, string, Exception?> _jsonCouldNotBeParsedError = LoggerMessage.Define<Uri, string>(LogLevel.Error, JsonCouldNotBeParsedError,
        "Unable to parse response from {URI}; Content: {Content}");
    /// <summary>
    /// Logs a JsonCouldNotBeParsed event with event code 0x0014.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="content">The content that could not be parsed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogJsonCouldNotBeParsedError(this ILogger logger, Uri uri, string content, JsonException error) => _jsonCouldNotBeParsedError(logger, uri, content, error);
    
    #endregion

    #region InvalidHttpResponse Error (0x0015)
    
    public const int EVENT_ID_InvalidHttpResponseError = 0x0015;
    public static readonly EventId InvalidHttpResponseError = new(EVENT_ID_InvalidHttpResponseError, nameof(InvalidHttpResponseError));
    private static readonly Action<ILogger, Uri, string, Exception?> _invalidHttpResponseError = LoggerMessage.Define<Uri, string>(LogLevel.Error, InvalidHttpResponseError,
        "Response from {URI} did not match the expected type; Content: {Content}");
    /// <summary>
    /// Logs an InvalidHttpResponse event with event code 0x0015.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="content">The response text.</param>
    /// <param name="error">The exception that caused the event</param>
    public static void LogInvalidHttpResponseError(this ILogger logger, Uri uri, string content) => _invalidHttpResponseError(logger, uri, content, null);
    
    #endregion
}