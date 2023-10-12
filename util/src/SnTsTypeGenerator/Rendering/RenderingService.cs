using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnTsTypeGenerator.Models;
using static SnTsTypeGenerator.SnApiConstants;
using static SnTsTypeGenerator.CmdLineConstants;

namespace SnTsTypeGenerator.Rendering;

public class RenderingService
{
    private ILogger<RenderingService> _logger;
    private readonly IServiceScope _scope;
    private readonly FileInfo? _outputFile;
    private readonly bool _forceOverwrite;
    private readonly bool _isScoped;

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _outputFile is not null;

    internal async Task RenderAsync(IEnumerable<TableInfo> toRender, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_outputFile is null)
            return;
        using var dbContext = _scope.ServiceProvider.GetRequiredService<TypingsDbContext>();
        StreamWriter streamWriter;
        try
        {
            streamWriter = new(_outputFile.FullName, new FileStreamOptions()
            {
                Access = FileAccess.Write,
                Mode = _forceOverwrite ? FileMode.Create : FileMode.CreateNew
            });
        }
        catch (Exception error)
        {
            _logger.LogOutputFileAccessError(_outputFile.FullName, error);
            return;
        }
        try
        {
            using (streamWriter)
            {
                using IndentedTextWriter writer = new(streamWriter, "    ");

                var nsGrouped = toRender.GroupBy(t => t.GetNamespace()).OrderBy(g => g.Key).ToArray();
                var gns = nsGrouped.FirstOrDefault(n => n.Key == DEFAULT_NAMESPACE);
                if (gns is not null)
                {
                    await new GlobalTypingsRenderer(writer, dbContext).WriteAsync(gns.OrderBy(t => t.Name).Select(t => dbContext.Tables.Entry(t)).ToArray(), cancellationToken);
                    if ((nsGrouped = nsGrouped.Where(n => n.Key != DEFAULT_NAMESPACE).ToArray()).Length > 0)
                        await writer.WriteLineAsync();
                }

                foreach (var nsg in nsGrouped.OrderBy(n => n.Key))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    await new ScopedTypingsRenderer(nsg.Key, writer, dbContext).WriteAsync(nsg.OrderBy(t => t.Name).Select(t => dbContext.Tables.Entry(t)).ToArray(), cancellationToken);
                }
                try
                {
                    await writer.FlushAsync();
                    await streamWriter.FlushAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogOutputFileAccessError(_outputFile.FullName, exception);
                }
            }
        }
        catch (Exception error) { _logger.LogUnexpecteException(error); }
    }

    public RenderingService(ILogger<RenderingService> logger, IServiceProvider services, IOptions<AppSettings> appSettingsOptions)
    {
        _logger = logger;
        _scope = services.CreateScope();
        AppSettings appSettings = appSettingsOptions.Value;
        if (string.IsNullOrWhiteSpace(appSettings.Mode))
        {
            _isScoped = false;
            _logger.LogDefaultRenderMode(_isScoped);
        }
        else
        {
            switch (appSettings.Mode.Trim().ToLower())
            {
                case MODE_SCOPED:
                case MODE_SCOPED_ABBR:
                    _isScoped = true;
                    break;
                case MODE_GLOBAL:
                case MODE_GLOBAL_ABBR:
                    _isScoped = false;
                    break;
                default:
                    _logger.LogInvalidModeOption(appSettings.Mode);
                    return;
            }
            _logger.LogRenderModeSettingValue(_isScoped);
        }

        _forceOverwrite = appSettings.Force ?? false;
        string outputFileName = appSettings.Output!;
        if (string.IsNullOrEmpty(outputFileName))
            outputFileName = DEFAULT_OUTPUT_FILENAME;
        try
        {
            FileInfo outputFile = new(outputFileName);
            outputFileName = outputFile.FullName;
            if (outputFile.Exists)
            {
                if (!_forceOverwrite)
                {
                    _logger.LogOutputFileAlreadyExists(outputFile.FullName);
                    return;
                }
            }
            else if (!(outputFile.Directory?.Exists ?? false))
            {
                _logger.LogOutputFileAccessError(outputFile.FullName, "Parent subdirectory does not exist");
                return;
            }
            _logger.LogUsingOutputFile(outputFileName, _forceOverwrite);
            _outputFile = outputFile;
        }
        catch (Exception exception)
        {
            _logger.LogOutputFileAccessError(outputFileName, exception);
            return;
        }
    }
}