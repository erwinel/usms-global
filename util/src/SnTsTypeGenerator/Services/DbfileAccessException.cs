using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator.Services;

[Serializable]
public class DbfileAccessException : Exception, ILogTrackable
{
    public bool IsLogged { get; private set; }

    public string DbFilePath { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        if (InnerException is System.Security.SecurityException || InnerException is UnauthorizedAccessException)
            logger.LogDbFileAccessError(DbFilePath, InnerException);
        else if (InnerException is NotSupportedException notSupportedException)
            logger.LogDbfilePathInvalid(DbFilePath, notSupportedException);
        else if (InnerException is PathTooLongException pathTooLongException)
            logger.LogDbfilePathTooLong(DbFilePath, pathTooLongException);
        else
            logger.LogDbfileValidationError(DbFilePath, InnerException ?? this);
        IsLogged = true;
    }

    public DbfileAccessException() => DbFilePath = string.Empty;

    public DbfileAccessException(string? message) : base(message) => DbFilePath = string.Empty;

    public DbfileAccessException(string? message, Exception? innerException) : base(message, innerException) => DbFilePath = string.Empty;

    public DbfileAccessException(Exception innerException, string dbFilePath) : this(innerException.Message, innerException, dbFilePath) { }

    public DbfileAccessException(string? message, Exception innerException, string dbFilePath) : base(message, innerException)
    {
        DbFilePath = dbFilePath;
    }

    protected DbfileAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        DbFilePath = info.GetString(nameof(DbFilePath)) ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(DbFilePath), DbFilePath);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
