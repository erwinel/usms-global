using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;

namespace SnTsTypeGenerator.Services;

[Serializable]
internal class LogOutputFileAccessException : Exception, ILogTrackable
{
    public string Path { get; private set; }

    public bool IsLogged { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        if (string.IsNullOrWhiteSpace(Message))
            logger.LogOutputFileAccessError(Path, InnerException ?? this);
        else
            logger.LogOutputFileAccessError(Path, Message, InnerException ?? this);
        IsLogged = true;
    }

    public LogOutputFileAccessException() => Path = string.Empty;

    public LogOutputFileAccessException(string? message) : base(message) => Path = string.Empty;

    public LogOutputFileAccessException(string? message, Exception? innerException) : base(message, innerException) => Path = string.Empty;

    public LogOutputFileAccessException(string message, Exception innerException, string path) : base(message, innerException) => Path = path;

    public LogOutputFileAccessException(Exception innerException, string path) : base(null, innerException) => Path = path;

    protected LogOutputFileAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        Path = info.GetString(nameof(Path)) ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Path), Path);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}