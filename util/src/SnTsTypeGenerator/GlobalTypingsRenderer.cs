using System.CodeDom.Compiler;
using static SnTsTypeGenerator.SnApiConstants;

namespace SnTsTypeGenerator;

public sealed class GlobalTypingsRenderer : TypingsRenderer
{
    protected override string CurrentScope => DEFAULT_NAMESPACE;

    public GlobalTypingsRenderer(IndentedTextWriter writer, TypingsDbContext dbContext) : base(writer, dbContext) => writer.Indent = 0;

    protected override Task WriteStartRecordsNamespace() => Writer.WriteLineAsync($"declare namespace {NS_NAME_GlideRecord} {{");

    protected override Task WriteStartElementsNamespace() => Writer.WriteLineAsync($"declare namespace {NS_NAME_GlideElement} {{");

    protected override Task WriteStartFieldsNamespace() => Writer.WriteLineAsync($"declare namespace {NS_NAME_tableFields} {{");
}
