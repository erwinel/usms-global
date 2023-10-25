using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator.Services;

[Serializable]
public class DbFileDirectoryNotFoundException : Exception, ILogTrackable
{
    public bool IsLogged { get; private set; }

    public string DbPath { get; private set; }

    public void Log(ILogger logger, bool force = false)
    {
        if (IsLogged && !force)
            return;
        logger.LogDbFileDirectoryNotFound(DbPath);
    }

    public DbFileDirectoryNotFoundException() => DbPath = string.Empty;

    public DbFileDirectoryNotFoundException(string? dbPath, string? message = null) : base(message) => DbPath = dbPath ?? string.Empty;

    public DbFileDirectoryNotFoundException(string? dbPath, string? message, Exception? innerException) : base(message, innerException) => DbPath = dbPath ?? string.Empty;

    protected DbFileDirectoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        DbPath = info.GetString(nameof(DbPath)) ?? string.Empty;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(DbPath), DbPath);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}