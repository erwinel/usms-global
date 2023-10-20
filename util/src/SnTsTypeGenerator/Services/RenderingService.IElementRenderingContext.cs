namespace SnTsTypeGenerator.Services;

public partial class RenderingService
{
    interface IElementRenderingContext
    {
        int IndentLevel { get; }
        string? Scope { get; }
        string? Package { get; }
        bool IsExplicitScalarType(string? typeName);
        string GetElementName(string? typeName);
    }
}
