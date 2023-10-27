using Microsoft.Extensions.Logging;

namespace SnTsTypeGenerator;

public interface ILogTrackable
{
    bool IsLogged { get; }
    void Log(ILogger logger);
}
