
namespace SnTsTypeGenerator;

public class CommandSettings
{
    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? Dbfile { get; set; }

    /// <summary>
    /// Database table names to generate typings for.
    /// </summary>
    public List<string>? Table { get; set; }

    /// <summary>
    /// Login user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURI { get; set; }
}