using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;

namespace SnTsTypeGenerator;

[Serializable]
public class TrackedException : Exception, ILogTrackable
{
    public bool IsLogged => true;
    public TrackedException() { }
    public TrackedException(string message) : base(message) { }
    public TrackedException(string message, Exception inner) : base(message, inner) { }
    public TrackedException(Exception inner) : base(inner?.Message, inner) { }
    protected TrackedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public void Log(ILogger logger) { }
}
