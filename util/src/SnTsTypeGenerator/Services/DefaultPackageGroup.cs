namespace SnTsTypeGenerator.Services;

public class DefaultPackageGroup
{
    public string Name { get; set; } = null!;
    
    public bool? IsBaseline { get; set; }
    
    public List<string>? Packages { get; set; }
}