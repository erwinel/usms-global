namespace SnTsTypeGenerator.Services;

public partial class RenderingService
{
    interface IElementRenderingContext
    {
        int IndentLevel { get; }
        string? Scope { get; }
        string? Package { get; }
        bool IsExplicitScalarType(string typeName);
        string GetElementName(string typeName);
    }
}
/*
Are there any plans of what we could do if the next
What would stop places like Fulton County from having "130% voter turnout", and dems win anyway? I think we need to be prepared for that. We can't rely on senators save us from that.
*/