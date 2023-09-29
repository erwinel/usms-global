using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Constants;

namespace SnTsTypeGenerator;

public static class LoggerMessages
{
    #region Critical DbfileValidation Error (0x0001)
    
    /// <summary>
    /// Numerical event code for database file validation error.
    /// </summary>
    public const int EVENT_ID_CriticalDbfileValidationError = 0x0001;
    
    /// <summary>
    /// Event ID for database file validation error.
    /// </summary>
    public static readonly EventId CriticalDbfileValidationError = new(EVENT_ID_CriticalDbfileValidationError, nameof(CriticalDbfileValidationError));
    
    private static readonly Action<ILogger, string, Exception?> _criticalDbfileValidationError = LoggerMessage.Define<string>(LogLevel.Critical, CriticalDbfileValidationError,
        "Unexpected error validating DB file path \"{DbFile}\".");
    
    /// <summary>
    /// Logs a database validation error event (DbfileValidation) with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbfileValidationError(this ILogger logger, string dbFile, Exception error) => _criticalDbfileValidationError(logger, dbFile, error);
    
    #endregion

    #region Critical DbfileAccess Error (0x0002)
    
    /// <summary>
    /// Numerical event code for file access error.
    /// </summary>
    public const int EVENT_ID_CriticalDbfileAccessError = 0x0002;
    
    /// <summary>
    /// Event ID for database file access error.
    /// </summary>
    public static readonly EventId CriticalDbfileAccessError = new(EVENT_ID_CriticalDbfileAccessError, nameof(CriticalDbfileAccessError));
    
    private static readonly Action<ILogger, string, Exception?> _criticalDbfileAccessError = LoggerMessage.Define<string>(LogLevel.Critical, CriticalDbfileAccessError,
        "Unable to create DB file \"{Dbfile}\".");
    
    /// <summary>
    /// Logs a database file access error event (DbfileAccess) with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbfileAccessError(this ILogger logger, string dbFile, Exception error) => _criticalDbfileAccessError(logger, dbFile, error);
    
    #endregion
    
    #region Critical DbInitializationFailure Error (0x0003)
    
    /// <summary>
    /// Numerical event code for database initialization error.
    /// </summary>
    public const int EVENT_ID_CriticalDbInitializationFailure = 0x0003;
    
    /// <summary>
    /// Event ID for database initialization error.
    /// </summary>
    public static readonly EventId CriticalDbInitializationFailure = new(EVENT_ID_CriticalDbInitializationFailure, nameof(CriticalDbInitializationFailure));
    
    private static readonly Action<ILogger, string, Type, string, Exception> _criticalDbInitializationFailure = LoggerMessage.Define<string, Type, string>(LogLevel.Critical, CriticalDbInitializationFailure,
        "Unexpected error while executing DB initialization query {QueryString} for {Type} in {DbPath}.");
    
    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="querystring">The query string that failed.</param>
    /// <param name="type">The DB entity object type.</param>
    /// <param name="dbpath">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbInitializationFailure(this ILogger logger, string querystring, Type type, string dbpath, Exception error) => _criticalDbInitializationFailure(logger, querystring, type, dbpath, error);
    
    #endregion
    
    #region Critical UserNameNotProvided Error (0x0004)
    
    /// <summary>
    /// Numerical event code for missing user name.
    /// </summary>
    public const int EVENT_ID_CriticalUserNameNotProvided = 0x0004;
    
    /// <summary>
    /// Event ID for missing user name.
    /// </summary>
    public static readonly EventId CriticalUserNameNotProvided = new(EVENT_ID_CriticalUserNameNotProvided, nameof(CriticalUserNameNotProvided));
    
    private static readonly Action<ILogger, Exception?> _criticalUserNameNotProvided = LoggerMessage.Define(LogLevel.Critical, CriticalUserNameNotProvided,
        "User name was not provided.");
    
    /// <summary>
    /// Logs a missing user name event (UserNameNotProvided) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalUserNameNotProvided(this ILogger logger) => _criticalUserNameNotProvided(logger, null);
    
    #endregion

    #region Critical PasswordNotProvided Error (0x0005)
    
    /// <summary>
    /// Numerical event code for missing password.
    /// </summary>
    public const int EVENT_ID_CriticalPasswordNotProvided = 0x0005;
    
    /// <summary>
    /// Event ID for missing password.
    /// </summary>
    public static readonly EventId CriticalPasswordNotProvided = new(EVENT_ID_CriticalPasswordNotProvided, nameof(CriticalPasswordNotProvided));
    
    private static readonly Action<ILogger, Exception?> _criticalPasswordNotProvided = LoggerMessage.Define(LogLevel.Critical, CriticalPasswordNotProvided,
        "Password was not provided.");
    
    /// <summary>
    /// Logs a missing password event (PasswordNotProvided) with event code 0x0005.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalPasswordNotProvided(this ILogger logger) => _criticalPasswordNotProvided(logger, null);
    
    #endregion

    #region Critical RemoteInstanceUriNotProvided Error (0x0006)
    
    /// <summary>
    /// Numerical event code for missing remote URL.
    /// </summary>
    public const int EVENT_ID_CriticalRemoteInstanceUriNotProvided = 0x0006;
    
    /// <summary>
    /// Event ID for missing remote URL.
    /// </summary>
    public static readonly EventId CriticalRemoteInstanceUriNotProvided = new(EVENT_ID_CriticalRemoteInstanceUriNotProvided, nameof(CriticalRemoteInstanceUriNotProvided));
    
    private static readonly Action<ILogger, Exception?> _criticalRemoteInstanceUriNotProvided = LoggerMessage.Define(LogLevel.Critical, CriticalRemoteInstanceUriNotProvided,
        "The remote ServiceNow instance URI was not provided.");
    
    /// <summary>
    /// Logs a missing remote URL event (RemoteInstanceUriNotProvided) with event code 0x0006.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalRemoteInstanceUriNotProvided(this ILogger logger) => _criticalRemoteInstanceUriNotProvided(logger, null);
    
    #endregion
    
    #region Critical InvalidRemoteInstanceUri Error (0x0007)
    
    /// <summary>
    /// Numerical event code for invalid remote URL.
    /// </summary>
    public const int EVENT_ID_CriticalInvalidRemoteInstanceUri = 0x0007;
    
    /// <summary>
    /// Event ID for invalid remote URL.
    /// </summary>
    public static readonly EventId CriticalInvalidRemoteInstanceUri = new(EVENT_ID_CriticalInvalidRemoteInstanceUri, nameof(CriticalInvalidRemoteInstanceUri));
    
    private static readonly Action<ILogger, Exception?> _criticalInvalidRemoteInstanceUri = LoggerMessage.Define(LogLevel.Critical, CriticalInvalidRemoteInstanceUri,
        "The remote ServiceNow instance URI was not an absolute URI with the http or https scheme.");
    
    /// <summary>
    /// Logs an invalid remote URL event (InvalidRemoteInstanceUri) with event code 0x0007.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogCriticalInvalidRemoteInstanceUri(this ILogger logger) => _criticalInvalidRemoteInstanceUri(logger, null);
    
    #endregion
    
    #region NoTableNamesSpecified Warning (0x0008)
    
    /// <summary>
    /// Numerical event code for no table names provided.
    /// </summary>
    public const int EVENT_ID_NoTableNamesSpecifiedWarning = 0x0008;
    
    /// <summary>
    /// Event ID for no table names provided.
    /// </summary>
    public static readonly EventId NoTableNamesSpecifiedWarning = new(EVENT_ID_NoTableNamesSpecifiedWarning, nameof(NoTableNamesSpecifiedWarning));
    
    private static readonly Action<ILogger, Exception?> _noTableNamesSpecifiedWarning = LoggerMessage.Define(LogLevel.Warning, NoTableNamesSpecifiedWarning,
        "No table names were specified; nothing to do.");
    
    /// <summary>
    /// Logs a no table names provided event (NoTableNamesSpecified) with event code 0x0008.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNoTableNamesSpecifiedWarning(this ILogger logger) => _noTableNamesSpecifiedWarning(logger, null);
    
    #endregion
    
    #region Critical GlobalAndScopedSwitchesBothSet Error (0x0009)
    
    /// <summary>
    /// Numerical event code for scope switch syntax error.
    /// </summary>
    public const int EVENT_ID_CriticalGlobalAndScopedSwitchesBothSet = 0x0009;
    
    /// <summary>
    /// Event ID for scope switch syntax error.
    /// </summary>
    public static readonly EventId CriticalGlobalAndScopedSwitchesBothSet = new(EVENT_ID_CriticalGlobalAndScopedSwitchesBothSet, nameof(CriticalGlobalAndScopedSwitchesBothSet));
    
    private static readonly Action<ILogger, Exception?> _criticalGlobalAndScopedSwitchesBothSet = LoggerMessage.Define(LogLevel.Critical, CriticalGlobalAndScopedSwitchesBothSet,
        $"The {nameof(AppSettings.Global)} ({AppSettings.SHORTHAND_g}) and {nameof(AppSettings.Scoped)} ({AppSettings.SHORTHAND_s}) options cannot be specified at the same time.");
    
    
    /// <summary>
    /// Logs a scope switch syntax error event (GlobalAndScopedSwitchesBothSet) with event code 0x0009.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalGlobalAndScopedSwitchesBothSet(this ILogger logger) => _criticalGlobalAndScopedSwitchesBothSet(logger, null);
    
    #endregion

    #region Critical OutputFileAlreadyExists Error (0x0010)
    
    /// <summary>
    /// Numerical event code for an already-existing output file.
    /// </summary>
    public const int EVENT_ID_OutputFileAlreadyExists = 0x0010;
    
    /// <summary>
    /// Event ID for an already-existing output file.
    /// </summary>
    public static readonly EventId OutputFileAlreadyExists = new(EVENT_ID_OutputFileAlreadyExists, nameof(OutputFileAlreadyExists));
    
    private static readonly Action<ILogger, string, Exception?> _outputFileAlreadyExists = LoggerMessage.Define<string>(LogLevel.Critical, OutputFileAlreadyExists,
        "File {Path}");
    
    /// <summary>
    /// Logs an already-existing output file event (OutputFileAlreadyExists) with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAlreadyExists(this ILogger logger, string path) => _outputFileAlreadyExists(logger, path, null);
    
    #endregion

    #region Critical OutputFileAccess Error (0x0011)
    
    /// <summary>
    /// Numerical event code for output file access error.
    /// </summary>
    public const int EVENT_ID_OutputFileAccessError = 0x0011;
    
    /// <summary>
    /// Event ID for output file access error.
    /// </summary>
    public static readonly EventId OutputFileAccessError = new(EVENT_ID_OutputFileAccessError, nameof(OutputFileAccessError));
    
    private static readonly Action<ILogger, string, Exception?> _outputFileAccessError1 = LoggerMessage.Define<string>(LogLevel.Critical, OutputFileAccessError,
        "Error accessing output file {Path}.");
    
    private static readonly Action<ILogger, string, string, Exception?> _outputFileAccessError2 = LoggerMessage.Define<string, string>(LogLevel.Critical, OutputFileAccessError,
        "Error accessing output file {Path}: {Message}");

    /// <summary>
    /// Logs an output file access error event (OutputFileAccessError) with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAccessError(this ILogger logger, string path, Exception? error = null) => _outputFileAccessError1(logger, path, error);
    
    /// <summary>
    /// Logs an output file access error event (OutputFileAccessError) with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="message">The message describing the error.</param>
    public static void LogOutputFileAccessError(this ILogger logger, string path, string message) => _outputFileAccessError2(logger, path, message, null);
    
    #endregion

    #region HttpRequestFailed Error (0x0012)
    
    /// <summary>
    /// Numerical event code for HTTP request failure.
    /// </summary>
    public const int EVENT_ID_HttpRequestFailed = 0x0012;
    
    /// <summary>
    /// Event ID for HTTP request failure.
    /// </summary>
    public static readonly EventId HttpRequestFailed = new(EVENT_ID_HttpRequestFailed, nameof(HttpRequestFailed));
    
    private static readonly Action<ILogger, Uri, Exception?> _httpRequestFailed1 = LoggerMessage.Define<Uri>(LogLevel.Error, HttpRequestFailed,
        "Remote request {URI} failed.");
    
    private static readonly Action<ILogger, Uri, string, Exception?> _httpRequestFailed2 = LoggerMessage.Define<Uri, string>(LogLevel.Error, HttpRequestFailed,
        "Remote request {URI} failed: {Message}");
    
    private static readonly Action<ILogger, Uri, int, string, Exception?> _httpRequestFailed3 = LoggerMessage.Define<Uri, int, string>(LogLevel.Error, HttpRequestFailed,
        "Remote request {URI} failed with error code {ErrorCode} ({Description}).");
    private static readonly Action<ILogger, Uri, int, string, string, Exception?> _httpRequestFailed4 = LoggerMessage.Define<Uri, int, string, string>(LogLevel.Error, HttpRequestFailed,
    "Remote request {URI} failed with error code {ErrorCode} ({Description}): {Message}");
    
    /// <summary>
    /// Logs an HTTP request failure event (HttpRequestFailed) with event code 0x0012.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI that failed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogHttpRequestFailed(this ILogger logger, Uri uri, HttpRequestException? error)
    {
        if (error is null)
            _httpRequestFailed1(logger, uri, error);
        else if (error.StatusCode.HasValue)
        {
            if (string.IsNullOrWhiteSpace(error.Message))
                _httpRequestFailed3(logger, uri, (int)error.StatusCode.Value, error.StatusCode.Value.ToDisplayName(), error);
            else
                _httpRequestFailed4(logger, uri, (int)error.StatusCode.Value, error.StatusCode.Value.ToDisplayName(), error.Message, error);
        }
        else if (string.IsNullOrWhiteSpace(error.Message))
            _httpRequestFailed1(logger, uri, error);
        else
            _httpRequestFailed2(logger, uri, error.Message, error);
    }
    
    #endregion

    #region GetResponseContentFailed Error (0x0013)
    
    /// <summary>
    /// Numerical event code for HTTP response parsing error.
    /// </summary>
    public const int EVENT_ID_GetResponseContentFailed = 0x0013;
    
    /// <summary>
    /// Event ID for HTTP response parsing error.
    /// </summary>
    public static readonly EventId GetResponseContentFailed = new(EVENT_ID_GetResponseContentFailed, nameof(GetResponseContentFailed));
    
    private static readonly Action<ILogger, Uri, Exception?> _getResponseContentFailed = LoggerMessage.Define<Uri>(LogLevel.Error, GetResponseContentFailed,
        "Failed to get text-based content from remote URI {URI}");
    
    /// <summary>
    /// Logs a HTTP response parsing error event (GetResponseContentFailed) with event code 0x0013.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request uri.</param>
    /// <param name="error">The optional exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogGetResponseContentFailed(this ILogger logger, Uri uri, Exception? error = null) => _getResponseContentFailed(logger, uri, error);
    
    #endregion

    #region JsonCouldNotBeParsed Error (0x0014)
    
    /// <summary>
    /// Numerical event code for JSON parsing error.
    /// </summary>
    public const int EVENT_ID_JsonCouldNotBeParsed = 0x0014;
    
    /// <summary>
    /// Event ID for JSON parsing error.
    /// </summary>
    public static readonly EventId JsonCouldNotBeParsed = new(EVENT_ID_JsonCouldNotBeParsed, nameof(JsonCouldNotBeParsed));
    
    private static readonly Action<ILogger, Uri, string, Exception?> _jsonCouldNotBeParsed = LoggerMessage.Define<Uri, string>(LogLevel.Error, JsonCouldNotBeParsed,
        "Unable to parse response from {URI}; Content: {Content}");
    
    /// <summary>
    /// Logs a JSON parsing error event (JsonCouldNotBeParsed) with event code 0x0014.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="content">The content that could not be parsed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogJsonCouldNotBeParsed(this ILogger logger, Uri uri, string content, JsonException? error) => _jsonCouldNotBeParsed(logger, uri, content, error);
    
    #endregion

    #region InvalidHttpResponse Error (0x0015)
    
    /// <summary>
    /// Numerical event code for invalid HTTP response.
    /// </summary>
    public const int EVENT_ID_InvalidHttpResponse = 0x0015;
    
    /// <summary>
    /// Event ID for invalid HTTP response.
    /// </summary>
    public static readonly EventId InvalidHttpResponse = new(EVENT_ID_InvalidHttpResponse, nameof(InvalidHttpResponse));
    
    private static readonly Action<ILogger, Uri, string, Exception?> _invalidHttpResponse = LoggerMessage.Define<Uri, string>(LogLevel.Error, InvalidHttpResponse,
        "Response from {URI} did not match the expected type; Content: {Content}");
    
    /// <summary>
    /// Logs an invalid HTTP response event (InvalidHttpResponse) with event code 0x0015.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="content">The response text.</param>
    /// <param name="error">The exception that caused the event</param>
    public static void LogInvalidHttpResponse(this ILogger logger, Uri uri, string content) => _invalidHttpResponse(logger, uri, content, null);

    #endregion

    #region ExecuteMethod Scope
    
    private static readonly Func<ILogger, string, IDisposable?> _executeMethodScope = LoggerMessage.DefineScope<string>("Execute method {MethodName}()");

    /// <summary>
    /// Formats the ExecuteMethod message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginExecuteMethodScope(this ILogger logger, string methodName) => _executeMethodScope(logger, methodName);
    
    private static readonly Func<ILogger, string, string, object?, IDisposable?> _executeMethodScope1 = LoggerMessage.DefineScope<string, string, object?>("Execute method {MethodName}({ParamName}: {ParamValue})");
    
    /// <summary>
    /// Formats the ExecuteMethod1 message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="paramName">The name of the method parameter.</param>
    /// <param name="paramValue">The value of the method parameter.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginExecuteMethodScope(this ILogger logger, string methodName, string paramName, object? paramValue) => _executeMethodScope1(logger, methodName, paramName, paramValue);
    
    #endregion

    #region ValidatingEntity Trace (0x0016)
    
    /// <summary>
    // Numerical event code for ValidatingEntity.
    /// </summary>
    public const int EVENT_ID_ValidatingEntity = 0x0016;
    
    /// <summary>
    // Event ID for ValidatingEntity.
    /// </summary>
    public static readonly EventId ValidatingEntity = new(EVENT_ID_ValidatingEntity, nameof(ValidatingEntity));
    
    private static readonly Action<ILogger, EntityState, string, IValidatableObject, Exception?> _validatingEntity = LoggerMessage.Define<EntityState, string, IValidatableObject>(LogLevel.Trace, ValidatingEntity,
        "Validating {State} {Name} {Entity}");
    
    /// <summary>
    /// Logs a ValidatingEntity event with event code 0x0016.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state while being validated.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidatingEntity(this ILogger logger, EntityState state, IEntityType  metadata, IValidatableObject entity)
    {
        string displayName = metadata.DisplayName()?.Trim()!;
        if (string.IsNullOrEmpty(displayName) && string.IsNullOrEmpty(displayName = metadata.Name?.Trim()!))
        {
            Type t = entity.GetType();
            if (string.IsNullOrWhiteSpace(displayName = t.FullName!))
                displayName = t.Name;
        }
        _validatingEntity(logger, state, displayName, entity, null);
    }
    
    #endregion

    #region EntityValidationFailure Error (0x0017)
    
    /// <summary>
    // Numerical event code for EntityValidationFailure.
    /// </summary>
    public const int EVENT_ID_EntityValidationFailure = 0x0017;
    
    /// <summary>
    // Event ID for EntityValidationFailure.
    /// </summary>
    public static readonly EventId EntityValidationFailure = new(EVENT_ID_EntityValidationFailure, nameof(EntityValidationFailure));
    
    private static readonly Action<ILogger, string, string, IValidatableObject, ValidationException> _entityValidationFailure1 = LoggerMessage.Define<string, string, IValidatableObject>(LogLevel.Error, EntityValidationFailure,
        "Error Validating {Name} ({ValidationMessage}) {Entity}");
    
    private static readonly Action<ILogger, string, string, string, IValidatableObject, ValidationException> _entityValidationFailure2 = LoggerMessage.Define<string, string, string, IValidatableObject>(LogLevel.Error, EntityValidationFailure,
        "Error Validating {Name} [{Properties}] ({ValidationMessage}) {Entity}");
    
    /// <summary>
    /// Logs an EntityValidationFailure event with event code 0x0017.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogEntityValidationFailure(this ILogger logger, IEntityType metadata, IValidatableObject entity, ValidationException error) // => _entityValidationFailure(logger, metadata, entity, error);
    {
        IEnumerable<string> memberNames = error.ValidationResult.MemberNames.Where(n => !string.IsNullOrWhiteSpace(n));
        string name = metadata.DisplayName()?.Trim()!;
        if (name.Length == 0)
        {
            Type t = entity.GetType();
            if (string.IsNullOrWhiteSpace(name = t.FullName!))
                name = t.Name;
        }
        string message = error.ValidationResult.ErrorMessage?.Trim()!;
        if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(message = error.Message?.Trim()!))
            _entityValidationFailure1(logger, name, memberNames.Any() ? $"Validation error on {string.Join(", ", memberNames)}" : "Validation Failure", entity, error);
        else if (memberNames.Any())
            _entityValidationFailure1(logger, name, message, entity, error);
        else
            _entityValidationFailure2(logger, name, string.Join(", ", memberNames), message, entity, error);
    }
    
    #endregion

    #region ValidationCompleted Trace (0x0018)
    
    /// <summary>
    // Numerical event code for ValidationCompleted.
    /// </summary>
    public const int EVENT_ID_ValidationCompleted = 0x0018;
    
    /// <summary>
    // Event ID for ValidationCompleted.
    /// </summary>
    public static readonly EventId ValidationCompleted = new(EVENT_ID_ValidationCompleted, nameof(ValidationCompleted));
    
    private static readonly Action<ILogger, EntityState, string, IValidatableObject, Exception?> _validationCompleted = LoggerMessage.Define<EntityState, string, IValidatableObject>(LogLevel.Trace, ValidationCompleted,
        "Validation for {State} {Name} {Entity}");
    
    /// <summary>
    /// Logs a ValidationCompleted event with event code 0x0018.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state during validation.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidationCompleted(this ILogger logger, EntityState state, IEntityType metadata, IValidatableObject entity)
    {
        string name = metadata.DisplayName()?.Trim()!;
        if (name.Length == 0)
        {
            Type t = entity.GetType();
            if (string.IsNullOrWhiteSpace(name = t.FullName!))
                name = t.Name;
        }
        _validationCompleted(logger, state, name, entity, null);
    }
    
    #endregion

    #region DbSaveChangesCompleted Trace (0x0019)
    
    /// <summary>
    // Numerical event code for DbSaveChangesCompleted.
    /// </summary>
    public const int EVENT_ID_DbSaveChangesCompletedTrace = 0x0019;
    
    /// <summary>
    // Event ID for DbSaveChangesCompleted.
    /// </summary>
    public static readonly EventId DbSaveChangesCompletedTrace = new(EVENT_ID_DbSaveChangesCompletedTrace, nameof(DbSaveChangesCompletedTrace));
    
    private static readonly Action<ILogger, string, int, Exception?> _dbSaveChangesCompletedTrace = LoggerMessage.Define<string, int>(LogLevel.Trace, DbSaveChangesCompletedTrace,
        "Message {MethodSignature} {ReturnValue}");
    
    /// <summary>
    /// Logs an DbSaveChangesCompleted event with event code 0x0019.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodSignature">The first event parameter.</param>
    /// <param name="returnValue">The second event parameter.</param>
    public static void LogDbSaveChangesCompletedTrace(this ILogger logger, bool isAsync, bool? acceptAllChangesOnSuccess, int returnValue) => _dbSaveChangesCompletedTrace(logger, isAsync ?
        (acceptAllChangesOnSuccess.HasValue ? $"SaveChangesAsync({acceptAllChangesOnSuccess.Value})" : "SaveChangesAsync()") :
        acceptAllChangesOnSuccess.HasValue ? $"SaveChanges({acceptAllChangesOnSuccess.Value})" : "SaveChanges()", returnValue, null);
    
    #endregion

    
    #region InvalidResponseType Error (0x0020)
    
    /// <summary>
    // Numerical event code for InvalidResponseType.
    /// </summary>
    public const int EVENT_ID_InvalidResponseType = 0x0020;
    
    /// <summary>
    // Event ID for InvalidResponseType.
    /// </summary>
    public static readonly EventId InvalidResponseType = new(EVENT_ID_InvalidResponseType, nameof(InvalidResponseType));
    
    private static readonly Action<ILogger, Uri, Exception?> _invalidResponseType1 = LoggerMessage.Define<Uri>(LogLevel.Error, InvalidResponseType,
        "Response from {URI} retuned null.");
    
    private static readonly Action<ILogger, Uri, string, string, Exception?> _invalidResponseType2 = LoggerMessage.Define<Uri, string, string>(LogLevel.Error, InvalidResponseType,
        "Response from {URI} retuned unexpected type {Type}. Actual Result: {Result}");
    
    /// <summary>
    /// Logs an InvalidResponseType event with event code 0x0020.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="type">The unexpected type.</param>
    /// <param name="response">The actual response.</param>
    public static void LogInvalidResponseType(this ILogger logger, Uri uri, JsonNode? response)
    {
        if (response is null)
            _invalidResponseType1(logger, uri, null);
        else
            _invalidResponseType2(logger, uri, response.GetType().Name, response.ToJsonString(), null);
    }

    #endregion

    #region ResponseResultPropertyNotFound Error (0x0021)

    /// <summary>
    // Numerical event code for ResponseResultPropertyNotFound.
    /// </summary>
    public const int EVENT_ID_ResponseResultPropertyNotFoundError = 0x0021;
    
    /// <summary>
    // Event ID for ResponseResultPropertyNotFound.
    /// </summary>
    public static readonly EventId ResponseResultPropertyNotFoundError = new(EVENT_ID_ResponseResultPropertyNotFoundError, nameof(ResponseResultPropertyNotFoundError));
    
    private static readonly Action<ILogger, Uri, string, Exception?> _responseResultPropertyNotFoundError = LoggerMessage.Define<Uri, string>(LogLevel.Error, ResponseResultPropertyNotFoundError,
        $"Response from  {{URI}} did not contain a property named \"{JSON_KEY_RESULT}\". Actual Response: {{Response}}");
    
    /// <summary>
    /// Logs an ResponseResultPropertyNotFound event with event code 0x0021.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    public static void LogResponseResultPropertyNotFound(this ILogger logger, Uri uri, JsonObject response) => _responseResultPropertyNotFoundError(logger, uri, response.ToJsonString(), null);

    #endregion

    #region NoResultsFromQuery Error (0x0022)
    
    /// <summary>
    // Numerical event code for NoResultsFromQuery.
    /// </summary>
    public const int EVENT_ID_NoResultsFromQuery = 0x0022;
    
    /// <summary>
    // Event ID for NoResultsFromQuery.
    /// </summary>
    public static readonly EventId NoResultsFromQuery = new(EVENT_ID_NoResultsFromQuery, nameof(NoResultsFromQuery));
    
    private static readonly Action<ILogger, Uri, string, Exception?> _noResultsFromQuery = LoggerMessage.Define<Uri, string>(LogLevel.Error, NoResultsFromQuery,
        "Response from {URI} returned no results. Actual response: {Response}");
    
    /// <summary>
    /// Logs an NoResultsFromQuery event with event code 0x0022.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    public static void LogNoResultsFromQuery(this ILogger logger, Uri uri, JsonObject response) => _noResultsFromQuery(logger, uri, response.ToJsonString(), null);
    
    #endregion

    #region MultipleResponseItems Warning (0x0023)
    
    /// <summary>
    // Numerical event code for MultipleResponseItems.
    /// </summary>
    public const int EVENT_ID_MultipleResponseItems = 0x0023;
    
    /// <summary>
    // Event ID for MultipleResponseItems.
    /// </summary>
    public static readonly EventId MultipleResponseItems = new(EVENT_ID_MultipleResponseItems, nameof(MultipleResponseItems));
    
    private static readonly Action<ILogger, Uri, int, string, Exception?> _multipleResponseItems = LoggerMessage.Define<Uri, int, string>(LogLevel.Warning, MultipleResponseItems,
        "Response from  returned  additional values. Actual response: {URI} {AdditionalCount} {Response}");
    
    /// <summary>
    /// Logs an MultipleResponseItems event with event code 0x0023.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="additionalCount">The number of additional elements.</param>
    /// <param name="response">The actual response.</param>
    public static void LogMultipleResponseItems(this ILogger logger, Uri uri, int additionalCount, JsonObject response) => _multipleResponseItems(logger, uri, additionalCount, response.ToJsonString(), null);
    
    #endregion

    #region InvalidResultElementType Error (0x0024)
    
    /// <summary>
    // Numerical event code for InvalidResultElementType.
    /// </summary>
    public const int EVENT_ID_InvalidResultElementType = 0x0024;
    
    /// <summary>
    // Event ID for InvalidResultElementType.
    /// </summary>
    public static readonly EventId InvalidResultElementType = new(EVENT_ID_InvalidResultElementType, nameof(InvalidResultElementType));
    
    private static readonly Action<ILogger, Uri, string, int, string, Exception?> _invalidResultElementType = LoggerMessage.Define<Uri, string, int, string>(LogLevel.Error, InvalidResultElementType,
        "Response from {URI} had an unexpected type {Type} at index {Index}. Actual element: {JSON}");

    /// <summary>
    /// Logs an InvalidResultElementType event with event code 0x0024.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="type">The unexpected type.</param>
    /// <param name="index">The element index.</param>
    /// <param name="response">The actual element.</param>
    public static void LogInvalidResultElementType(this ILogger logger, Uri uri, JsonNode? element, int index)
    {
        if (element is null)
            _invalidResultElementType(logger, uri, "null", index, "", null);
        else
            _invalidResultElementType(logger, uri, element.GetType().Name, index, element.ToJsonString(), null);
    }

    #endregion

    #region ExpectedPropertyNotFound Error (0x0025)
    
    /// <summary>
    // Numerical event code for ExpectedPropertyNotFound.
    /// </summary>
    public const int EVENT_ID_ExpectedPropertyNotFoundError = 0x0025;
    
    /// <summary>
    // Event ID for ExpectedPropertyNotFound.
    /// </summary>
    public static readonly EventId ExpectedPropertyNotFoundError = new(EVENT_ID_ExpectedPropertyNotFoundError, nameof(ExpectedPropertyNotFoundError));
    
    private static readonly Action<ILogger, Uri, string, int, string, Exception?> _expectedPropertyNotFoundError1 = LoggerMessage.Define<Uri, string, int, string>(LogLevel.Error, ExpectedPropertyNotFoundError,
        "Response from {URI} is missing property {PropertyName} at index {Index}. Actual response: {Response}");
    
    private static readonly Action<ILogger, Uri, string, string, Exception?> _expectedPropertyNotFoundError2 = LoggerMessage.Define<Uri, string, string>(LogLevel.Error, ExpectedPropertyNotFoundError,
        "Response from {URI} is missing property {PropertyName}. Actual response: {Response}");
    
    /// <summary>
    /// Logs an ExpectedPropertyNotFound event with event code 0x0025.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="propertyname">The name of the missing field.</param>
    /// <param name="index">The index of the result item.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogExpectedPropertyNotFoundError(this ILogger logger, Uri uri, string propertyname, int index, JsonObject element) => _expectedPropertyNotFoundError1(logger, uri, propertyname, index, element.ToJsonString(), null);
    
    /// <summary>
    /// Logs an ExpectedPropertyNotFound event with event code 0x0025.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="propertyname">The name of the missing field.</param>
    /// <param name="response">The actual response.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogExpectedPropertyNotFoundError(this ILogger logger, Uri uri, string propertyname, JsonObject element) => _expectedPropertyNotFoundError2(logger, uri, propertyname, element.ToJsonString(), null);
    
    #endregion
}