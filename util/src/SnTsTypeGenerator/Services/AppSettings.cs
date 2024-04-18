
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public class AppSettings
{
    /// <summary>
    /// The remote ServiceNow instance URI.
    /// </summary>
    public string? RemoteURL { get; set; }

    /// <summary>
    /// Comma-separated database table names to generate typings for.
    /// </summary>
    public string? Table { get; set; }

    /// <summary>
    /// Database table names to generate typings for.
    /// </summary>
    public List<string>? Tables { get; set; }

    /// <summary>
    /// Indicates whether the typings are generated for globally-scoped scripting.
    /// </summary>
    public bool? GlobalScope { get; set; }

    /// <summary>
    /// Specifies the relative or absolute path of the database file.
    /// </summary>
    /// <remarks>If this path is not absolute, it will be resolved relative to the <see cref="Microsoft.Extensions.Hosting.ContentRootPath" />.
    /// The default value of this setting is defined in the <see cref="DEFAULT_DbFile" /> constant.</remarks>
    public string? DbFile { get; set; }

    /// <summary>
    /// Emits typings for base types.
    /// </summary>
    public bool? EmitBaseTypes { get; set; }

    /// <summary>
    /// Emits referenced types as well.
    /// </summary>
    public bool? IncludeReferenced { get; set; }

    /// <summary>
    /// The name of the package group for all new packages that could not be automatically grouped.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_default_pkg"/> switch.</remarks>
    public string? DefaultPackageGroup { get; set; }

    /// <summary>
    /// All newly found packages that are active and not licensable on the remote instance are to be considered baseline.
    /// </summary>
    public bool? BaselineInit { get; set; }

    /// <summary>
    /// Login user name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password credential.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the client ID in the remote ServiceNow instance's Application Registry.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_c"/> switch.</remarks>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret in the remote ServiceNow instance's Application Registry.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// The output file name.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_o"/> switch.</remarks>
    public string? Output { get; set; }

    /// <summary>
    /// Force overwrite of output file.
    /// </summary>
    public bool? ForceOverwrite { get; set; }

    /// <summary>
    /// Indicates whether to modify the target remote source.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_m"/> switch.
    /// <para>Only the <see cref="RemoteURL"/>, <see cref="NewURL"/>, <see cref="IsPdi"/>, and <see cref="RemoteLabel"/> options are used when this is <see langword="true"/>.</para></remarks>
    public bool? ModifySource { get; set; }

    /// <summary>
    /// Used with the <see cref="ModifySource"/> option to change the remote ServiceNow instance URI from the value of this option to <see cref="RemoteURL"/>.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_existing_url"/> switch.</remarks>
    public string? ExistingURL { get; set; }

    /// <summary>
    /// Indicates whether the remote instance is a Personal Developer Instance.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_pdi"/> switch.</remarks>
    public bool? IsPdi { get; set; }

    /// <summary>
    /// The descriptive name to use for the remote instance.
    /// </summary>
    /// <remarks>This can be specified using the <see cref="SHORTHAND_remote_label"/> switch.</remarks>
    public string? RemoteLabel { get; set; }

    /// <summary>
    /// Gets package groups contained in <see cref="TypingsDbContext.PackageGroups"/> table and the <see cref="DefaultPackageGroups"/> setting.
    /// </summary>
    public bool? GetPackageGroups { get; set; }

    /// <summary>
    /// Gets remote sources contained in <see cref="TypingsDbContext.SourceInstances"/> table.
    /// </summary>
    public bool? GetRemoteSources { get; set; }

    /// <summary>
    /// Gets the glide type mappings to refer to when adding new rows to the <see cref="Models.GlideType"/> table.
    /// </summary>
    public List<JsClassMapping>? JsClassMappings { get; set; }

    /// <summary>
    /// Gets the glide type mappings to refer to when adding new rows to the <see cref="Models.GlideType"/> table.
    /// </summary>
    public List<KnownGlideType>? KnownGlideTypes { get; set; }

    /// <summary>
    /// Gets the glide type mappings to refer to when adding new rows to the <see cref="Models.GlideType"/> table.
    /// </summary>
    public List<DefaultPackageGroup>? DefaultPackageGroups { get; set; }

    public bool? Help { get; set; }

    public bool ShowHelp() => Help ?? false;

    private static readonly Dictionary<string, string> _valueSwitchMappings = new()
    {
        { $"-{SHORTHAND_d}", $"{nameof(SnTsTypeGenerator)}:{nameof(DbFile)}" },
        { $"--{SHORTHAND_db_file}", $"{nameof(SnTsTypeGenerator)}:{nameof(DbFile)}" },
        { $"-{SHORTHAND_t}", $"{nameof(SnTsTypeGenerator)}:{nameof(Table)}" },
        { $"--{nameof(Table)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Table)}" },
        { $"--{SHORTHAND_default_pkg}", $"{nameof(SnTsTypeGenerator)}:{nameof(DefaultPackageGroup)}" },
        { $"-{SHORTHAND_g}", $"{nameof(SnTsTypeGenerator)}:{nameof(GlobalScope)}" },
        { $"-{SHORTHAND_u}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"--{nameof(SHORTHAND_user_name)}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"--{nameof(UserName)}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"--{nameof(SHORTHAND_login)}", $"{nameof(SnTsTypeGenerator)}:{nameof(UserName)}" },
        { $"-{SHORTHAND_p}", $"{nameof(SnTsTypeGenerator)}:{nameof(Password)}" },
        { $"--{nameof(Password)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Password)}" },
        { $"-{SHORTHAND_c}", $"{nameof(SnTsTypeGenerator)}:{nameof(ClientId)}" },
        { $"--{nameof(SHORTHAND_client_id)}", $"{nameof(SnTsTypeGenerator)}:{nameof(ClientId)}" },
        { $"-{SHORTHAND_x}", $"{nameof(SnTsTypeGenerator)}:{nameof(ClientSecret)}" },
        { $"--{nameof(SHORTHAND_client_secret)}", $"{nameof(SnTsTypeGenerator)}:{nameof(ClientSecret)}" },
        { $"-{SHORTHAND_r}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"--{nameof(SHORTHAND_remote_url)}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteURL)}" },
        { $"-{SHORTHAND_o}", $"{nameof(SnTsTypeGenerator)}:{nameof(Output)}" },
        { $"--{nameof(Output)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Output)}" }
    };

    private static readonly Dictionary<string, string> _booleanSwitchMappings = new()
    {
        { $"-{SHORTHAND_b}", $"{nameof(SnTsTypeGenerator)}:{nameof(EmitBaseTypes)}" },
        { $"--{SHORTHAND_emit_base_types}", $"{nameof(SnTsTypeGenerator)}:{nameof(EmitBaseTypes)}" },
        { $"-{SHORTHAND_i}", $"{nameof(SnTsTypeGenerator)}:{nameof(IncludeReferenced)}" },
        { $"--{SHORTHAND_include_referenced}", $"{nameof(SnTsTypeGenerator)}:{nameof(IncludeReferenced)}" },
        { $"-{SHORTHAND_n}", $"{nameof(SnTsTypeGenerator)}:{nameof(BaselineInit)}" },
        { $"--{SHORTHAND_baseline_init}", $"{nameof(SnTsTypeGenerator)}:{nameof(BaselineInit)}" },
        { $"-{SHORTHAND_m}", $"{nameof(SnTsTypeGenerator)}:{nameof(ModifySource)}" },
        { $"--{SHORTHAND_modify_source}", $"{nameof(SnTsTypeGenerator)}:{nameof(ModifySource)}" },
        { $"--{SHORTHAND_existing_url}", $"{nameof(SnTsTypeGenerator)}:{nameof(ExistingURL)}" },
        { $"--{SHORTHAND_pdi}", $"{nameof(SnTsTypeGenerator)}:{nameof(IsPdi)}" },
        { $"--{SHORTHAND_remote_label}", $"{nameof(SnTsTypeGenerator)}:{nameof(RemoteLabel)}" },
        { $"-{SHORTHAND_f}", $"{nameof(SnTsTypeGenerator)}:{nameof(ForceOverwrite)}" },
        { $"--{SHORTHAND_force}", $"{nameof(SnTsTypeGenerator)}:{nameof(ForceOverwrite)}" },
        { $"--{SHORTHAND_get_package_groups}", $"{nameof(SnTsTypeGenerator)}:{nameof(GetPackageGroups)}" },
        { $"--{SHORTHAND_get_remote_sources}", $"{nameof(SnTsTypeGenerator)}:{nameof(GetRemoteSources)}" },
        { $"-{SHORTHAND_h}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { $"-{SHORTHAND__3F_}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" },
        { $"--{nameof(Help)}", $"{nameof(SnTsTypeGenerator)}:{nameof(Help)}" }
    };

    internal static void Configure(string[] args, IConfigurationBuilder builder)
    {
        builder.Add(new AltCommandLineConfigurationSource(args?.ToImmutableArray() ?? ImmutableArray<string>.Empty, _booleanSwitchMappings, _valueSwitchMappings));
    }
}
