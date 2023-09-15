
namespace SnTsTypeGenerator;

public class AppSettings
{
    /// <summary>
    /// Default name of database file.
    /// </summary>
    public const string DEFAULT_DbFile = $"Typings.db";

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />. The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile  { get; set; }

    /// <summary>
    /// comma-delimited list of table names.
    /// </summary>
    public string? Tables  { get; set; }

    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURI { get; set; }
}
