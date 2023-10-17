namespace SnTsTypeGenerator.Services;

public static class TextWriterExtensionMethods
{
    public static async Task WriteDeclareNamespace(this TextWriter writer, string name)
    {
        await writer.WriteAsync("declare namespace ");
        await writer.WriteAsync(name);
        await writer.WriteLineAsync(" {");
    }

    public static async Task WriteExportNamespace(this TextWriter writer, string name)
    {
        await writer.WriteAsync("export namespace ");
        await writer.WriteAsync(name);
        await writer.WriteLineAsync(" {");
    }
}
