using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    public const int EVENT_ID_DbfileValidationError = 0x0001;

    /// <summary>
    /// Event ID for database file validation error.
    /// </summary>
    public static readonly EventId DbfileValidationError = new(EVENT_ID_DbfileValidationError, nameof(DbfileValidationError));

    private static readonly Action<ILogger, string, Exception?> _dbfileValidationError = LoggerMessage.Define<string>(LogLevel.Critical, DbfileValidationError,
        "Unexpected error validating DB file path \"{DbFile}\".");

    /// <summary>
    /// Logs a database validation error event (DbfileValidation) with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbfileValidationError(this ILogger logger, string dbFile, Exception error) => _dbfileValidationError(logger, dbFile, error);

    #endregion

    #region Critical DbFileDirectoryNotFOund Error (0x0002)

    /// <summary>
    // Numerical event code for DbFileDirectoryNotFOund.
    /// </summary>
    public const int EVENT_ID_DbFileDirectoryNotFound = 0x0002;

    /// <summary>
    // Event ID for DbFileDirectoryNotFOund.
    /// </summary>
    public static readonly EventId DbFileDirectoryNotFound = new(EVENT_ID_DbFileDirectoryNotFound, nameof(DbFileDirectoryNotFound));

    private static readonly Action<ILogger, string, Exception?> _dbFileDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, DbFileDirectoryNotFound,
        "Parent directory for database file {Path} does not exist.");

    /// <summary>
    /// Logs an DbFileDirectoryNotFOund event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The database file object.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogDbFileDirectoryNotFound(this ILogger logger, FileInfo dbFile, Exception? error = null) => _dbFileDirectoryNotFound(logger, dbFile.FullName, error);

    #endregion

    #region Critical DbfileAccess Error (0x0003)

    /// <summary>
    /// Numerical event code for file access error.
    /// </summary>
    public const int EVENT_ID_DbfileAccessError = 0x0003;

    /// <summary>
    /// Event ID for database file access error.
    /// </summary>
    public static readonly EventId DbfileAccessError = new(EVENT_ID_DbfileAccessError, nameof(DbfileAccessError));

    private static readonly Action<ILogger, string, Exception?> _dbfileAccessError = LoggerMessage.Define<string>(LogLevel.Critical, DbfileAccessError,
        "Unable to create DB file \"{Dbfile}\".");

    /// <summary>
    /// Logs a database file access error event (DbfileAccess) with event code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbfileAccessError(this ILogger logger, string dbFile, Exception error) => _dbfileAccessError(logger, dbFile, error);

    #endregion

    #region Critical DbInitializationFailure Error (0x0004)

    /// <summary>
    /// Numerical event code for database initialization error.
    /// </summary>
    public const int EVENT_ID_CriticalDbInitializationFailure = 0x0004;

    /// <summary>
    /// Event ID for database initialization error.
    /// </summary>
    public static readonly EventId CriticalDbInitializationFailure = new(EVENT_ID_CriticalDbInitializationFailure, nameof(CriticalDbInitializationFailure));

    private static readonly Action<ILogger, string, Type, string, Exception> _criticalDbInitializationFailure1 = LoggerMessage.Define<string, Type, string>(LogLevel.Critical, CriticalDbInitializationFailure,
        "Unexpected error while executing DB initialization query {QueryString} for {Type} in {DbPath}.");

    private static readonly Action<ILogger, string, Exception> _criticalDbInitializationFailure2 = LoggerMessage.Define<string>(LogLevel.Critical, CriticalDbInitializationFailure,
        "Unexpected error while executing DB initialization for {DbPath}.");

    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="querystring">The query string that failed.</param>
    /// <param name="type">The DB entity object type.</param>
    /// <param name="dbFile">The database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbInitializationFailure(this ILogger logger, string querystring, Type type, FileInfo dbFile, Exception error) =>
        _criticalDbInitializationFailure1(logger, querystring, type, dbFile.FullName, error);

    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogCriticalDbInitializationFailure(this ILogger logger, FileInfo dbFile, Exception error) => _criticalDbInitializationFailure2(logger, dbFile.FullName, error);

    #endregion

    #region Critical CriticalSettingValueNotProvided Error (0x0005)
    
    /// <summary>
    // Numerical event code for CriticalSettingValueNotProvided.
    /// </summary>
    public const int EVENT_ID_CriticalSettingValueNotProvided = 0x0005;
    
    /// <summary>
    // Event ID for CriticalSettingValueNotProvided.
    /// </summary>
    public static readonly EventId CriticalSettingValueNotProvided = new(EVENT_ID_CriticalSettingValueNotProvided, nameof(CriticalSettingValueNotProvided));
    
    private static readonly Action<ILogger, string, Exception?> _criticalSettingValueNotProvided1= LoggerMessage.Define<string>(LogLevel.Critical, CriticalSettingValueNotProvided,
        "{SettingName} setting is missing.");
    
    private static readonly Action<ILogger, string, string, Exception?> _criticalSettingValueNotProvided2 = LoggerMessage.Define<string, string>(LogLevel.Critical, CriticalSettingValueNotProvided,
        "{SettingName} ({CmdLineSwitch}) not provided.");
    
    /// <summary>
    /// Logs an CriticalSettingValueNotProvided event with event code 0x0005.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="settingName">The name of the setting.</param>
    /// <param name="cmdLineSwitch">The command-line switch</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalSettingValueNotProvided(this ILogger logger, string settingName, string? cmdLineSwitch = null)
    {
        if (string.IsNullOrWhiteSpace(cmdLineSwitch))
            _criticalSettingValueNotProvided1(logger, settingName, null);
        else
            _criticalSettingValueNotProvided2(logger, settingName, cmdLineSwitch, null);
    }
    
    #endregion

    #region Critical UserNameNotProvided Error (0x0005)

    /// <summary>
    /// Numerical event code for missing user name.
    /// </summary>
    public const int EVENT_ID_UserNameNotProvided = 0x0005;

    /// <summary>
    /// Event ID for missing user name.
    /// </summary>
    public static readonly EventId UserNameNotProvided = new(EVENT_ID_UserNameNotProvided, nameof(UserNameNotProvided));

    private static readonly Action<ILogger, Exception?> _userNameNotProvided = LoggerMessage.Define(LogLevel.Critical, UserNameNotProvided,
        "User name was not provided.");

    [Obsolete("Use LogCriticalSettingValueNotProvided")]
    public static void LogUserNameNotProvided(this ILogger logger) => _userNameNotProvided(logger, null);

    #endregion

    #region Critical PasswordNotProvided Error (0x0006)

    /// <summary>
    /// Numerical event code for missing password.
    /// </summary>
    public const int EVENT_ID_PasswordNotProvided = 0x0006;

    /// <summary>
    /// Event ID for missing password.
    /// </summary>
    public static readonly EventId PasswordNotProvided = new(EVENT_ID_PasswordNotProvided, nameof(PasswordNotProvided));

    private static readonly Action<ILogger, Exception?> _passwordNotProvided = LoggerMessage.Define(LogLevel.Critical, PasswordNotProvided,
        "Password was not provided.");

    [Obsolete("Use LogCriticalSettingValueNotProvided")]
    public static void LogPasswordNotProvided(this ILogger logger) => _passwordNotProvided(logger, null);

    #endregion

    #region Critical RemoteInstanceUriNotProvided Error (0x0007)

    /// <summary>
    /// Numerical event code for missing remote URL.
    /// </summary>
    public const int EVENT_ID_RemoteInstanceUriNotProvided = 0x0007;

    /// <summary>
    /// Event ID for missing remote URL.
    /// </summary>
    public static readonly EventId RemoteInstanceUriNotProvided = new(EVENT_ID_RemoteInstanceUriNotProvided, nameof(RemoteInstanceUriNotProvided));

    private static readonly Action<ILogger, Exception?> _remoteInstanceUriNotProvided = LoggerMessage.Define(LogLevel.Critical, RemoteInstanceUriNotProvided,
        "The remote ServiceNow instance URI was not provided.");

    [Obsolete("Use LogCriticalSettingValueNotProvided")]
    public static void LogRemoteInstanceUriNotProvided(this ILogger logger) => _remoteInstanceUriNotProvided(logger, null);

    #endregion

    #region Critical InvalidRemoteInstanceUri Error (0x0008)

    /// <summary>
    /// Numerical event code for invalid remote URL.
    /// </summary>
    public const int EVENT_ID_InvalidRemoteInstanceUri = 0x0008;

    /// <summary>
    /// Event ID for invalid remote URL.
    /// </summary>
    public static readonly EventId InvalidRemoteInstanceUri = new(EVENT_ID_InvalidRemoteInstanceUri, nameof(InvalidRemoteInstanceUri));

    private static readonly Action<ILogger, Exception?> _invalidRemoteInstanceUri = LoggerMessage.Define(LogLevel.Critical, InvalidRemoteInstanceUri,
        "The remote ServiceNow instance URI was not an absolute URI with the http or https scheme.");

    /// <summary>
    /// Logs an invalid remote URL event (InvalidRemoteInstanceUri) with event code 0x0008.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    public static void LogInvalidRemoteInstanceUri(this ILogger logger) => _invalidRemoteInstanceUri(logger, null);

    #endregion

    #region NoTableNamesSpecified Warning (0x0009)

    /// <summary>
    /// Numerical event code for no table names provided.
    /// </summary>
    public const int EVENT_ID_NoTableNamesSpecifiedWarning = 0x0009;

    /// <summary>
    /// Event ID for no table names provided.
    /// </summary>
    public static readonly EventId NoTableNamesSpecifiedWarning = new(EVENT_ID_NoTableNamesSpecifiedWarning, nameof(NoTableNamesSpecifiedWarning));

    private static readonly Action<ILogger, Exception?> _noTableNamesSpecifiedWarning = LoggerMessage.Define(LogLevel.Warning, NoTableNamesSpecifiedWarning,
        "No table names were specified; nothing to do.");

    /// <summary>
    /// Logs a no table names provided event (NoTableNamesSpecified) with event code 0x0009.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNoTableNamesSpecifiedWarning(this ILogger logger) => _noTableNamesSpecifiedWarning(logger, null);

    #endregion

    #region Critical GlobalAndScopedSwitchesBothSet Error (0x000a)

    /// <summary>
    /// Numerical event code for scope switch syntax error.
    /// </summary>
    public const int EVENT_ID_GlobalAndScopedSwitchesBothSet = 0x000a;

    /// <summary>
    /// Event ID for scope switch syntax error.
    /// </summary>
    public static readonly EventId GlobalAndScopedSwitchesBothSet = new(EVENT_ID_GlobalAndScopedSwitchesBothSet, nameof(GlobalAndScopedSwitchesBothSet));

    private static readonly Action<ILogger, Exception?> _globalAndScopedSwitchesBothSet = LoggerMessage.Define(LogLevel.Critical, GlobalAndScopedSwitchesBothSet,
        $"The {nameof(AppSettings.Global)} ({AppSettings.SHORTHAND_g}) and {nameof(AppSettings.Scoped)} ({AppSettings.SHORTHAND_s}) options cannot be specified at the same time.");


    /// <summary>
    /// Logs a scope switch syntax error event (GlobalAndScopedSwitchesBothSet) with event code 0x000a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogGlobalAndScopedSwitchesBothSet(this ILogger logger) => _globalAndScopedSwitchesBothSet(logger, null);

    #endregion

    #region Critical OutputFileAlreadyExists Error (0x000b)

    /// <summary>
    /// Numerical event code for an already-existing output file.
    /// </summary>
    public const int EVENT_ID_OutputFileAlreadyExists = 0x000b;

    /// <summary>
    /// Event ID for an already-existing output file.
    /// </summary>
    public static readonly EventId OutputFileAlreadyExists = new(EVENT_ID_OutputFileAlreadyExists, nameof(OutputFileAlreadyExists));

    private static readonly Action<ILogger, string, Exception?> _outputFileAlreadyExists = LoggerMessage.Define<string>(LogLevel.Critical, OutputFileAlreadyExists,
        "File {Path}");

    /// <summary>
    /// Logs an already-existing output file event (OutputFileAlreadyExists) with event code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAlreadyExists(this ILogger logger, string path) => _outputFileAlreadyExists(logger, path, null);

    #endregion

    #region Critical OutputFileAccess Error (0x000c)

    /// <summary>
    /// Numerical event code for output file access error.
    /// </summary>
    public const int EVENT_ID_OutputFileAccessError = 0x000c;

    /// <summary>
    /// Event ID for output file access error.
    /// </summary>
    public static readonly EventId OutputFileAccessError = new(EVENT_ID_OutputFileAccessError, nameof(OutputFileAccessError));

    private static readonly Action<ILogger, string, Exception?> _outputFileAccessError1 = LoggerMessage.Define<string>(LogLevel.Critical, OutputFileAccessError,
        "Error accessing output file {Path}.");

    private static readonly Action<ILogger, string, string, Exception?> _outputFileAccessError2 = LoggerMessage.Define<string, string>(LogLevel.Critical, OutputFileAccessError,
        "Error accessing output file {Path}: {Message}");

    /// <summary>
    /// Logs an output file access error event (OutputFileAccessError) with event code 0x000c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAccessError(this ILogger logger, string path, Exception? error = null) => _outputFileAccessError1(logger, path, error);

    /// <summary>
    /// Logs an output file access error event (OutputFileAccessError) with event code 0x000c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="message">The message describing the error.</param>
    public static void LogOutputFileAccessError(this ILogger logger, string path, string message) => _outputFileAccessError2(logger, path, message, null);

    #endregion

    #region HttpRequestFailed Error (0x000d)

    /// <summary>
    /// Numerical event code for HTTP request failure.
    /// </summary>
    public const int EVENT_ID_HttpRequestFailed = 0x000d;

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
    /// Logs an HTTP request failure event (HttpRequestFailed) with event code 0x000d.
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

    #region GetResponseContentFailed Error (0x000e)

    /// <summary>
    /// Numerical event code for HTTP response parsing error.
    /// </summary>
    public const int EVENT_ID_GetResponseContentFailed = 0x000e;

    /// <summary>
    /// Event ID for HTTP response parsing error.
    /// </summary>
    public static readonly EventId GetResponseContentFailed = new(EVENT_ID_GetResponseContentFailed, nameof(GetResponseContentFailed));

    private static readonly Action<ILogger, Uri, Exception?> _getResponseContentFailed = LoggerMessage.Define<Uri>(LogLevel.Error, GetResponseContentFailed,
        "Failed to get text-based content from remote URI {URI}");

    /// <summary>
    /// Logs a HTTP response parsing error event (GetResponseContentFailed) with event code 0x000e.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request uri.</param>
    /// <param name="error">The optional exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogGetResponseContentFailed(this ILogger logger, Uri uri, Exception? error = null) => _getResponseContentFailed(logger, uri, error);

    #endregion

    #region JsonCouldNotBeParsed Error (0x000f)

    /// <summary>
    /// Numerical event code for JSON parsing error.
    /// </summary>
    public const int EVENT_ID_JsonCouldNotBeParsed = 0x000f;

    /// <summary>
    /// Event ID for JSON parsing error.
    /// </summary>
    public static readonly EventId JsonCouldNotBeParsed = new(EVENT_ID_JsonCouldNotBeParsed, nameof(JsonCouldNotBeParsed));

    private static readonly Action<ILogger, Uri, string, Exception?> _jsonCouldNotBeParsed = LoggerMessage.Define<Uri, string>(LogLevel.Error, JsonCouldNotBeParsed,
        "Unable to parse response from {URI}; Content: {Content}");

    /// <summary>
    /// Logs a JSON parsing error event (JsonCouldNotBeParsed) with event code 0x000f.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="content">The content that could not be parsed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogJsonCouldNotBeParsed(this ILogger logger, Uri uri, string content, JsonException? error) => _jsonCouldNotBeParsed(logger, uri, content, error);

    #endregion

    #region InvalidHttpResponse Error (0x0010)

    /// <summary>
    /// Numerical event code for invalid HTTP response.
    /// </summary>
    public const int EVENT_ID_InvalidHttpResponse = 0x0010;

    /// <summary>
    /// Event ID for invalid HTTP response.
    /// </summary>
    public static readonly EventId InvalidHttpResponse = new(EVENT_ID_InvalidHttpResponse, nameof(InvalidHttpResponse));

    private static readonly Action<ILogger, Uri, string, Exception?> _invalidHttpResponse = LoggerMessage.Define<Uri, string>(LogLevel.Error, InvalidHttpResponse,
        "Response from {URI} did not match the expected type; Content: {Content}");

    /// <summary>
    /// Logs an invalid HTTP response event (InvalidHttpResponse) with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The response text.</param>
    /// <param name="error">The exception that caused the event</param>
    public static void LogInvalidHttpResponse(this ILogger logger, Uri uri, JsonNode? response) => _invalidHttpResponse(logger, uri, (response is null) ? "null" : response.ToJsonString(), null);

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

    #region ValidatingEntity Trace (0x0011)

    /// <summary>
    // Numerical event code for ValidatingEntity.
    /// </summary>
    public const int EVENT_ID_ValidatingEntity = 0x0011;

    /// <summary>
    // Event ID for ValidatingEntity.
    /// </summary>
    public static readonly EventId ValidatingEntity = new(EVENT_ID_ValidatingEntity, nameof(ValidatingEntity));

    private static readonly Action<ILogger, EntityState, string, IValidatableObject, Exception?> _validatingEntity = LoggerMessage.Define<EntityState, string, IValidatableObject>(LogLevel.Trace, ValidatingEntity,
        "Validating {State} {Name} {Entity}");

    /// <summary>
    /// Logs a ValidatingEntity event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state while being validated.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidatingEntity(this ILogger logger, EntityState state, IEntityType metadata, IValidatableObject entity)
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

    #region EntityValidationFailure Error (0x0012)

    /// <summary>
    // Numerical event code for EntityValidationFailure.
    /// </summary>
    public const int EVENT_ID_EntityValidationFailure = 0x0012;

    /// <summary>
    // Event ID for EntityValidationFailure.
    /// </summary>
    public static readonly EventId EntityValidationFailure = new(EVENT_ID_EntityValidationFailure, nameof(EntityValidationFailure));

    private static readonly Action<ILogger, string, string, IValidatableObject, ValidationException> _entityValidationFailure1 =
        LoggerMessage.Define<string, string, IValidatableObject>(LogLevel.Error, EntityValidationFailure, "Error Validating {Name} ({ValidationMessage}) {Entity}");

    private static readonly Action<ILogger, string, string, string, IValidatableObject, ValidationException> _entityValidationFailure2 =
        LoggerMessage.Define<string, string, string, IValidatableObject>(LogLevel.Error, EntityValidationFailure, "Error Validating {Name} [{Properties}] ({ValidationMessage}) {Entity}");

    /// <summary>
    /// Logs an EntityValidationFailure event with event code 0x0012.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogEntityValidationFailure(this ILogger logger, IEntityType metadata, IValidatableObject entity, ValidationException error)
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

    #region ValidationCompleted Trace (0x0013)

    /// <summary>
    // Numerical event code for ValidationCompleted.
    /// </summary>
    public const int EVENT_ID_ValidationCompleted = 0x0013;

    /// <summary>
    // Event ID for ValidationCompleted.
    /// </summary>
    public static readonly EventId ValidationCompleted = new(EVENT_ID_ValidationCompleted, nameof(ValidationCompleted));

    private static readonly Action<ILogger, EntityState, string, IValidatableObject, Exception?> _validationCompleted =
        LoggerMessage.Define<EntityState, string, IValidatableObject>(LogLevel.Trace, ValidationCompleted, "Validation for {State} {Name} {Entity}");

    /// <summary>
    /// Logs a ValidationCompleted event with event code 0x0013.
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

    #region DbSaveChangesCompleted Trace (0x0014)

    /// <summary>
    // Numerical event code for DbSaveChangesCompleted.
    /// </summary>
    public const int EVENT_ID_DbSaveChangesCompletedTrace = 0x0014;

    /// <summary>
    // Event ID for DbSaveChangesCompleted.
    /// </summary>
    public static readonly EventId DbSaveChangesCompletedTrace = new(EVENT_ID_DbSaveChangesCompletedTrace, nameof(DbSaveChangesCompletedTrace));

    private static readonly Action<ILogger, string, int, Exception?> _dbSaveChangesCompletedTrace = LoggerMessage.Define<string, int>(LogLevel.Trace, DbSaveChangesCompletedTrace,
        "Message {MethodSignature} {ReturnValue}");

    /// <summary>
    /// Logs an DbSaveChangesCompleted event with event code 0x0014.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodSignature">The first event parameter.</param>
    /// <param name="returnValue">The second event parameter.</param>
    public static void LogDbSaveChangesCompletedTrace(this ILogger logger, bool isAsync, bool? acceptAllChangesOnSuccess, int returnValue) => _dbSaveChangesCompletedTrace(logger, isAsync ?
        (acceptAllChangesOnSuccess.HasValue ? $"SaveChangesAsync({acceptAllChangesOnSuccess.Value})" : "SaveChangesAsync()") :
        acceptAllChangesOnSuccess.HasValue ? $"SaveChanges({acceptAllChangesOnSuccess.Value})" : "SaveChanges()", returnValue, null);

    #endregion


    #region InvalidResponseType Error (0x0015)

    /// <summary>
    // Numerical event code for InvalidResponseType.
    /// </summary>
    public const int EVENT_ID_InvalidResponseType = 0x0015;

    /// <summary>
    // Event ID for InvalidResponseType.
    /// </summary>
    public static readonly EventId InvalidResponseType = new(EVENT_ID_InvalidResponseType, nameof(InvalidResponseType));

    private static readonly Action<ILogger, Uri, Exception?> _invalidResponseType1 = LoggerMessage.Define<Uri>(LogLevel.Error, InvalidResponseType,
        "Response from {URI} retuned null.");

    private static readonly Action<ILogger, Uri, string, string, Exception?> _invalidResponseType2 = LoggerMessage.Define<Uri, string, string>(LogLevel.Error, InvalidResponseType,
        "Response from {URI} retuned unexpected type {Type}. Actual Result: {Result}");

    /// <summary>
    /// Logs an InvalidResponseType event with event code 0x0015.
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

    #region ResponseResultPropertyNotFound Error (0x0016)

    /// <summary>
    // Numerical event code for ResponseResultPropertyNotFound.
    /// </summary>
    public const int EVENT_ID_ResponseResultPropertyNotFound = 0x0016;

    /// <summary>
    // Event ID for ResponseResultPropertyNotFound.
    /// </summary>
    public static readonly EventId ResponseResultPropertyNotFound = new(EVENT_ID_ResponseResultPropertyNotFound, nameof(ResponseResultPropertyNotFound));

    private static readonly Action<ILogger, Uri, string, Exception?> _responseResultPropertyNotFound = LoggerMessage.Define<Uri, string>(LogLevel.Error, ResponseResultPropertyNotFound,
        $"Response from  {{URI}} did not contain a property named \"{JSON_KEY_RESULT}\". Actual Response: {{Response}}");

    /// <summary>
    /// Logs an ResponseResultPropertyNotFound event with event code 0x0016.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    public static void LogResponseResultPropertyNotFound(this ILogger logger, Uri uri, JsonObject response) => _responseResultPropertyNotFound(logger, uri, response.ToJsonString(), null);

    #endregion

    #region NoResultsFromQuery Error (0x0017)

    /// <summary>
    // Numerical event code for NoResultsFromQuery.
    /// </summary>
    public const int EVENT_ID_NoResultsFromQuery = 0x0017;

    /// <summary>
    // Event ID for NoResultsFromQuery.
    /// </summary>
    public static readonly EventId NoResultsFromQuery = new(EVENT_ID_NoResultsFromQuery, nameof(NoResultsFromQuery));

    private static readonly Action<ILogger, Uri, string, Exception?> _noResultsFromQuery = LoggerMessage.Define<Uri, string>(LogLevel.Error, NoResultsFromQuery,
        "Response from {URI} returned no results. Actual response: {Response}");

    /// <summary>
    /// Logs an NoResultsFromQuery event with event code 0x0017.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    public static void LogNoResultsFromQuery(this ILogger logger, Uri uri, JsonObject response) => _noResultsFromQuery(logger, uri, response.ToJsonString(), null);

    #endregion

    #region MultipleResponseItems Warning (0x0018)

    /// <summary>
    // Numerical event code for MultipleResponseItems.
    /// </summary>
    public const int EVENT_ID_MultipleResponseItems = 0x0018;

    /// <summary>
    // Event ID for MultipleResponseItems.
    /// </summary>
    public static readonly EventId MultipleResponseItems = new(EVENT_ID_MultipleResponseItems, nameof(MultipleResponseItems));

    private static readonly Action<ILogger, Uri, int, string, Exception?> _multipleResponseItems = LoggerMessage.Define<Uri, int, string>(LogLevel.Warning, MultipleResponseItems,
        "Response from  returned  additional values. Actual response: {URI} {AdditionalCount} {Response}");

    /// <summary>
    /// Logs an MultipleResponseItems event with event code 0x0018.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="additionalCount">The number of additional elements.</param>
    /// <param name="response">The actual response.</param>
    public static void LogMultipleResponseItems(this ILogger logger, Uri uri, int additionalCount, JsonObject response) => _multipleResponseItems(logger, uri, additionalCount, response.ToJsonString(), null);

    #endregion

    #region InvalidResultElementType Error (0x0019)

    /// <summary>
    // Numerical event code for InvalidResultElementType.
    /// </summary>
    public const int EVENT_ID_InvalidResultElementType = 0x0019;

    /// <summary>
    // Event ID for InvalidResultElementType.
    /// </summary>
    public static readonly EventId InvalidResultElementType = new(EVENT_ID_InvalidResultElementType, nameof(InvalidResultElementType));

    private static readonly Action<ILogger, Uri, string, int, string, Exception?> _invalidResultElementType = LoggerMessage.Define<Uri, string, int, string>(LogLevel.Error, InvalidResultElementType,
        "Response from {URI} had an unexpected type {Type} at index {Index}. Actual element: {JSON}");

    /// <summary>
    /// Logs an InvalidResultElementType event with event code 0x0019.
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

    #region ExpectedPropertyNotFound Error (0x001a)

    /// <summary>
    // Numerical event code for ExpectedPropertyNotFound.
    /// </summary>
    public const int EVENT_ID_ExpectedPropertyNotFound = 0x001a;

    /// <summary>
    // Event ID for ExpectedPropertyNotFound.
    /// </summary>
    public static readonly EventId ExpectedPropertyNotFound = new(EVENT_ID_ExpectedPropertyNotFound, nameof(ExpectedPropertyNotFound));

    private static readonly Action<ILogger, Uri, string, int, string, Exception?> _expectedPropertyNotFound1 = LoggerMessage.Define<Uri, string, int, string>(LogLevel.Error, ExpectedPropertyNotFound,
        "Response from {URI} is missing property {PropertyName} at index {Index}. Actual response: {Response}");

    private static readonly Action<ILogger, Uri, string, string, Exception?> _expectedPropertyNotFound2 = LoggerMessage.Define<Uri, string, string>(LogLevel.Error, ExpectedPropertyNotFound,
        "Response from {URI} is missing property {PropertyName}. Actual response: {Response}");

    /// <summary>
    /// Logs an ExpectedPropertyNotFound event with event code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="propertyname">The name of the missing field.</param>
    /// <param name="index">The index of the result item.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogExpectedPropertyNotFound(this ILogger logger, Uri uri, string propertyname, int index, JsonObject element) =>
        _expectedPropertyNotFound1(logger, uri, propertyname, index, element.ToJsonString(), null);

    /// <summary>
    /// Logs an ExpectedPropertyNotFound event with event code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="propertyname">The name of the missing field.</param>
    /// <param name="response">The actual response.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogExpectedPropertyNotFound(this ILogger logger, Uri uri, string propertyname, JsonObject element) => _expectedPropertyNotFound2(logger, uri, propertyname, element.ToJsonString(), null);

    #endregion

    #region APIRequestStart Trace (0x001b)

    /// <summary>
    // Numerical event code for APIRequestStart.
    /// </summary>
    public const int EVENT_ID_APIRequestStart = 0x001b;

    /// <summary>
    // Event ID for APIRequestStart.
    /// </summary>
    public static readonly EventId APIRequestStart = new(EVENT_ID_APIRequestStart, nameof(APIRequestStart));

    private static readonly Action<ILogger, Uri, Exception?> _apiRequestStart = LoggerMessage.Define<Uri>(LogLevel.Trace, APIRequestStart,
        "Sending API request to {URI}");

    /// <summary>
    /// Logs an APIRequestStart event with event code 0x001b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The API requst URL.</param>
    public static void LogAPIRequestStart(this ILogger logger, Uri uri) => _apiRequestStart(logger, uri, null);

    #endregion

    #region APIRequestCompleted Trace (0x001c)

    /// <summary>
    // Numerical event code for APIRequestCompleted.
    /// </summary>
    public const int EVENT_ID_APIRequestCompleted = 0x001c;

    /// <summary>
    // Event ID for APIRequestCompleted.
    /// </summary>
    public static readonly EventId APIRequestCompleted = new(EVENT_ID_APIRequestCompleted, nameof(APIRequestCompleted));

    private static readonly Action<ILogger, Uri, string, Exception?> _apirequestCompleted = LoggerMessage.Define<Uri, string>(LogLevel.Trace, APIRequestCompleted,
        "API request  returned {URL} {Result}");

    /// <summary>
    /// Logs an APIRequestCompleted event with event code 0x001c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The request URL.</param>
    /// <param name="result">The parsed API result.</param>
    public static void LogAPIRequestCompleted(this ILogger logger, Uri url, JsonNode? result) => _apirequestCompleted(logger, url, (result is null) ? "null" : result.ToJsonString(), null);

    #endregion

    #region GettingTableByNameFromRemote Trace (0x001d)

    /// <summary>
    // Numerical event code for GettingTableByNameFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingTableByNameFromRemote = 0x001d;

    /// <summary>
    // Event ID for GettingTableByNameFromRemote.
    /// </summary>
    public static readonly EventId GettingTableByNameFromRemote = new(EVENT_ID_GettingTableByNameFromRemote, nameof(GettingTableByNameFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingTableByNameFromRemote = LoggerMessage.Define<string>(LogLevel.Trace, GettingTableByNameFromRemote,
        "Getting table by name {Name} from remote instance.");

    /// <summary>
    /// Logs an GettingTableByNameFromRemote event with event code 0x001d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="name">The name of the table being looked up.</param>
    public static void LogGettingTableByNameFromRemote(this ILogger logger, string name) => _gettingTableByNameFromRemote(logger, name, null);

    #endregion

    #region GettingTableBySysIdFromRemote Trace (0x001e)

    /// <summary>
    // Numerical event code for GettingTableBySysIdFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingTableBySysIdFromRemot = 0x001e;

    /// <summary>
    // Event ID for GettingTableBySysIdFromRemote.
    /// </summary>
    public static readonly EventId GettingTableBySysIdFromRemote = new(EVENT_ID_GettingTableBySysIdFromRemot, nameof(GettingTableBySysIdFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingTableBySysIdFromRemote = LoggerMessage.Define<string>(LogLevel.Trace, GettingTableBySysIdFromRemote,
        "Getting table by Sys ID {SysID} from remote instance.");

    /// <summary>
    /// Logs an GettingTableBySysIdFromRemote event with event code 0x001e.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="sysID">The Sys ID of the table to look up.</param>
    public static void LogGettingTableBySysIdFromRemote(this ILogger logger, string sysID) => _gettingTableBySysIdFromRemote(logger, sysID, null);

    #endregion

    #region GettingElementsByTableNameFromRemote Trace (0x001f)

    /// <summary>
    // Numerical event code for GettingElementsByTableNameFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingElementsByTableNameFromRemote = 0x001f;

    /// <summary>
    // Event ID for GettingElementsByTableNameFromRemote.
    /// </summary>
    public static readonly EventId GettingElementsByTableNameFromRemote = new(EVENT_ID_GettingElementsByTableNameFromRemote, nameof(GettingElementsByTableNameFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingElementsByTableNameFromRemote = LoggerMessage.Define<string>(LogLevel.Trace, GettingElementsByTableNameFromRemote,
        "Getting elements from remote instance with table name {TableName}.");

    /// <summary>
    /// Logs an GettingElementsByTableNameFromRemote event with event code 0x001f.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table.</param>
    public static void LogGettingElementsByTableNameFromRemote(this ILogger logger, string tableName) => _gettingElementsByTableNameFromRemote(logger, tableName, null);

    #endregion

    #region GettingScopeByIdentifierFromRemote Trace (0x0020)

    /// <summary>
    // Numerical event code for GettingScopeByIdentifierFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingScopeByIdentifierFromRemote = 0x0020;

    /// <summary>
    // Event ID for GettingScopeByIdentifierFromRemote.
    /// </summary>
    public static readonly EventId GettingScopeByIdentifierFromRemote = new(EVENT_ID_GettingScopeByIdentifierFromRemote, nameof(GettingScopeByIdentifierFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingScopeByIdentifierFromRemote = LoggerMessage.Define<string>(LogLevel.Trace, GettingScopeByIdentifierFromRemote,
        "Getting scope by unique identifier {Identifer} from remote instance.");

    /// <summary>
    /// Logs an GettingScopeByIdentifierFromRemote event with event code 0x0020.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identifer">The unique identifier of the sys_scope.</param>
    public static void LogGettingScopeByIdentifierFromRemote(this ILogger logger, string identifer) => _gettingScopeByIdentifierFromRemote(logger, identifer, null);

    #endregion

    #region GettingTypeByNameFromRemote Trace (0x0021)

    /// <summary>
    // Numerical event code for GettingTypeByNameFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingTypeByNameFromRemoteTrace = 0x0021;

    /// <summary>
    // Event ID for GettingTypeByNameFromRemote.
    /// </summary>
    public static readonly EventId GettingTypeByNameFromRemoteTrace = new(EVENT_ID_GettingTypeByNameFromRemoteTrace, nameof(GettingTypeByNameFromRemoteTrace));

    private static readonly Action<ILogger, string, Exception?> _gettingTypeByNameFromRemoteTrace = LoggerMessage.Define<string>(LogLevel.Trace, GettingTypeByNameFromRemoteTrace,
        "Getting type by name {TypeName} from remote instance.");

    /// <summary>
    /// Logs an GettingTypeByNameFromRemote event with event code 0x0021.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="typeName">The name of the sys_glide_object.</param>
    public static void LogGettingTypeByNameFromRemoteTrace(this ILogger logger, string typeName) => _gettingTypeByNameFromRemoteTrace(logger, typeName, null);

    #endregion

    #region AddingTableToDb Trace (0x0022)

    /// <summary>
    // Numerical event code for AddingTableToDb.
    /// </summary>
    public const int EVENT_ID_AddingTableToDb = 0x0022;

    /// <summary>
    // Event ID for AddingTableToDb.
    /// </summary>
    public static readonly EventId AddingTableToDb = new(EVENT_ID_AddingTableToDb, nameof(AddingTableToDb));

    private static readonly Action<ILogger, string, Exception?> _AddingTableToDb = LoggerMessage.Define<string>(LogLevel.Trace, AddingTableToDb,
        "Adding table {TableName} to database.");

    /// <summary>
    /// Logs an AddingTableToDb event with event code 0x0022.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table being added.</param>
    public static void LogAddingTableToDb(this ILogger logger, string tableName) => _AddingTableToDb(logger, tableName, null);

    #endregion

    #region AddingElementsToDatabase Trace (0x0023)

    /// <summary>
    // Numerical event code for AddingElementsToDatabase.
    /// </summary>
    public const int EVENT_ID_AddingElementsToDatabase = 0x0023;

    /// <summary>
    // Event ID for AddingElementsToDatabase.
    /// </summary>
    public static readonly EventId AddingElementsToDatabase = new(EVENT_ID_AddingElementsToDatabase, nameof(AddingElementsToDatabase));

    private static readonly Action<ILogger, string, Exception?> _AddingElementsToDatabase = LoggerMessage.Define<string>(LogLevel.Trace, AddingElementsToDatabase,
        "Adding elements for table {TableName} to database.");

    /// <summary>
    /// Logs an AddingElementsToDatabase event with event code 0x0023.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table.</param>
    public static void LogAddingElementsToDatabase(this ILogger logger, string tableName) => _AddingElementsToDatabase(logger, tableName, null);

    #endregion

    #region NewTableSaveComplete Trace (0x0024)

    /// <summary>
    // Numerical event code for NewTableSaveComplete.
    /// </summary>
    public const int EVENT_ID_NewTableSaveCompleteTrace = 0x0024;

    /// <summary>
    // Event ID for NewTableSaveComplete.
    /// </summary>
    public static readonly EventId NewTableSaveCompleteTrace = new(EVENT_ID_NewTableSaveCompleteTrace, nameof(NewTableSaveCompleteTrace));

    private static readonly Action<ILogger, string, Exception?> _newTableSaveCompleteTrace = LoggerMessage.Define<string>(LogLevel.Trace, NewTableSaveCompleteTrace,
        "Table named {TableName} and related entites saved to database.");

    /// <summary>
    /// Logs an NewTableSaveComplete event with event code 0x0024.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table.</param>
    public static void LogNewTableSaveCompleteTrace(this ILogger logger, string tableName) => _newTableSaveCompleteTrace(logger, tableName, null);

    #endregion


    #region Critical UnexpecteException Error (0x00ff)

    /// <summary>
    // Numerical event code for UnexpecteException.
    /// </summary>
    public const int EVENT_ID_UnexpecteException = 0x00ff;

    /// <summary>
    // Event ID for UnexpecteException.
    /// </summary>
    public static readonly EventId UnexpecteException = new(EVENT_ID_UnexpecteException, nameof(UnexpecteException));

    private static readonly Action<ILogger, Exception?> _unexpecteException = LoggerMessage.Define(LogLevel.Critical, UnexpecteException,
        "An unexpected exception has occurred.");

    /// <summary>
    /// Logs an UnexpecteException event with event code 0x00ff.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogUnexpecteException(this ILogger logger, Exception error) => _unexpecteException(logger, error);

    #endregion
}