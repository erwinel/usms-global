namespace SnTsTypeGenerator.Rendering;

public static class TextWriterExtensionMethods
{
    public static async Task WriteLinesAsync(this TextWriter writer, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        foreach (string l in lines)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            await writer.WriteLineAsync(l);
        }
    }

    public static async Task WriteJsDocAsync(this TextWriter writer, IEnumerable<string> lines, CancellationToken cancellationToken) => await writer.WriteLinesAsync(lines.ToJsDocLines(), cancellationToken);

    public static async Task WriteJsDocAsync(this TextWriter writer, CancellationToken cancellationToken, params string[] lines) => await writer.WriteJsDocAsync(lines, cancellationToken);
}
