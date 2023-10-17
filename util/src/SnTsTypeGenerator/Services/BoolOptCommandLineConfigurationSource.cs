
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;

namespace SnTsTypeGenerator.Services;

public class BoolOptCommandLineConfigurationSource : IConfigurationSource
{
    public IDictionary<string, string>? ValueSwitchMappings { get; set; }
    
    public IDictionary<string, string> BooleanSwitchMappings { get; set; }

    public ImmutableArray<string> Args { get; set; }

    public BoolOptCommandLineConfigurationSource(ImmutableArray<string> args, IDictionary<string, string> booleanwitchMappings, IDictionary<string, string>? valueSwitchMappings = null) =>
        (Args, BooleanSwitchMappings, ValueSwitchMappings) = (args, booleanwitchMappings, valueSwitchMappings);

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new BoolOptCommandLineConfigurationProvider(Args, BooleanSwitchMappings, ValueSwitchMappings);
}
