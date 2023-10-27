using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator.Services;

[Serializable]
public class DbInitializationException : Exception, ILogTrackable
{
    public bool IsLogged { get; private set; }

    public int? ErrorCode { get; private set; }

    public string? SqlState { get; private set; }

    public string? DbPath { get; private set; }

    public string? CommandText { get; private set; }

    public CommandType? CommandType { get; private set; }

    public string? ConnectionString { get; private set; }

    public Type? EntityType { get; private set; }

    public void Log(ILogger logger)
    {
        if (IsLogged)
            return;
        if (string.IsNullOrWhiteSpace(ConnectionString))
            logger.LogDbInitializationFailure(EntityType, ErrorCode, SqlState, CommandType, CommandText, DbPath, InnerException ?? this);
        else
            logger.LogDbInitializationFailure(ConnectionString, ErrorCode, SqlState, CommandType, CommandText, InnerException ?? this);
        IsLogged = true;
    }

    public DbInitializationException() { }

    public DbInitializationException(string? message) : base(message) { }

    public DbInitializationException(string? message, Exception? innerException) : base(message, innerException)
    {
        if (innerException is DbException dbException)
        {
            ErrorCode = dbException.ErrorCode;
            SqlState = dbException.SqlState;
            if (dbException.BatchCommand is not null)
            {
                CommandText = dbException.BatchCommand.CommandText;
                CommandType = dbException.BatchCommand.CommandType;
            }
        }
    }

    public DbInitializationException(string dbFile, Type type, Exception innerException) : this(innerException.Message, dbFile, type, innerException) { }

    public DbInitializationException(string? message, string dbFile, Type type, Exception innerException) : this(message, innerException)
    {
        DbPath = dbFile;
        EntityType = type;
    }

    public DbInitializationException(Exception innerException, string connectionString) : this(innerException.Message, innerException, connectionString) { }

    public DbInitializationException(string? message, Exception innerException, string connectionString) : this(message, innerException)
    {
        ConnectionString = connectionString;
    }

    protected DbInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsLogged = info.GetBoolean(nameof(IsLogged));
        SqlState = info.GetString(nameof(SqlState));
        DbPath = info.GetString(nameof(DbPath));
        ConnectionString = info.GetString(nameof(ConnectionString));
        EntityType = info.GetValue(nameof(EntityType), typeof(Type)) as Type;
        CommandText = info.GetString(nameof(CommandText));
        string? value = info.GetString(nameof(CommandType));
        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, out CommandType ct))
            CommandType = ct;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(CommandType), CommandType.HasValue ? Enum.GetName(typeof(CommandType), CommandType.Value) : null);
        info.AddValue(nameof(CommandText), CommandText);
        info.AddValue(nameof(EntityType), EntityType);
        info.AddValue(nameof(ConnectionString), ConnectionString);
        info.AddValue(nameof(DbPath), DbPath);
        info.AddValue(nameof(SqlState), SqlState);
        info.AddValue(nameof(IsLogged), IsLogged);
    }
}
