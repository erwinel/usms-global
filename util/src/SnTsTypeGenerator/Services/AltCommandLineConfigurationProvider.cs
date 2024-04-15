
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;

namespace SnTsTypeGenerator.Services;

public class AltCommandLineConfigurationProvider : ConfigurationProvider
{
    private readonly Dictionary<string, string> _valueSwitchMappings = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _booleanwitchMappings = new(StringComparer.OrdinalIgnoreCase);

    public AltCommandLineConfigurationProvider(ImmutableArray<string> args, IDictionary<string, string> booleanwitchMappings, IDictionary<string, string>? switchMappings = null)
    {
        Args = args;
        if (booleanwitchMappings is null)
            throw new ArgumentNullException(nameof(booleanwitchMappings));
        foreach (var mapping in booleanwitchMappings)
        {
            // Only keys start with "--" or "-" are acceptable
            if (!mapping.Key.StartsWith("-") && !mapping.Key.StartsWith("--"))
                throw new ArgumentException("Invalid switch mappings key", nameof(switchMappings));

            if (_booleanwitchMappings.ContainsKey(mapping.Key))
                throw new ArgumentException("Duplicate switch mappings key", nameof(switchMappings));

            _booleanwitchMappings.Add(mapping.Key, mapping.Value);
        }

        if (switchMappings is not null)
            foreach (var mapping in switchMappings)
            {
                // Only keys start with "--" or "-" are acceptable
                if (!mapping.Key.StartsWith("-") && !mapping.Key.StartsWith("--"))
                    throw new ArgumentException("Invalid switch mappings key", nameof(switchMappings));

                if (_booleanwitchMappings.ContainsKey(mapping.Key) || _valueSwitchMappings.ContainsKey(mapping.Key))
                    throw new ArgumentException("Duplicate switch mappings key", nameof(switchMappings));

                _valueSwitchMappings.Add(mapping.Key, mapping.Value);
            }
    }

    /// <summary>
    /// The command line arguments.
    /// </summary>
    protected ImmutableArray<string> Args { get; private set; }

    /// <summary>
    /// Loads the configuration data from the command line args.
    /// </summary>
    public override void Load()
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var enumerator = Args.GetEnumerator();
        string value;
        string? key;
        if (_valueSwitchMappings is null || _valueSwitchMappings.Count == 0)
            while (enumerator.MoveNext())
            {
                var currentArg = enumerator.Current;
                var keyStartIndex = 0;

                if (currentArg.StartsWith("--"))
                    keyStartIndex = 2;
                else if (currentArg.StartsWith("-"))
                    keyStartIndex = 1;
                else if (currentArg.StartsWith("/"))
                {
                    // "/SomeSwitch" is equivalent to "--SomeSwitch" when interpreting switch mappings
                    // So we do a conversion to simplify later processing
                    currentArg = $"--{currentArg[1..]}";
                    keyStartIndex = 2;
                }

                var separator = currentArg.IndexOf('=');

                if (separator < 0)
                {
                    // If there is neither equal sign nor prefix in current arugment, it is an invalid format
                    if (keyStartIndex == 0)
                        throw new FormatException($"{enumerator.Current} is an invalid argument.");
                    if (_booleanwitchMappings.TryGetValue(currentArg, out key))
                        value = "true";
                    else
                    {
                        if (keyStartIndex == 1)
                            throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                        // Otherwise, use the switch name directly as a key
                        key = currentArg[keyStartIndex..];
                        var previousKey = enumerator.Current;
                        if (!enumerator.MoveNext())
                            throw new FormatException($"{previousKey} is missing a value");

                        value = enumerator.Current;
                    }
                }
                else
                {
                    if (keyStartIndex == 1)
                        throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                    key = currentArg[keyStartIndex..separator];
                    value = currentArg[(separator + 1)..];
                }

                // Override value when key is duplicated. So we always have the last argument win.
                data[key] = value;
            }
        else
            while (enumerator.MoveNext())
            {
                var currentArg = enumerator.Current;
                var keyStartIndex = 0;

                if (currentArg.StartsWith("--"))
                    keyStartIndex = 2;
                else if (currentArg.StartsWith("-"))
                    keyStartIndex = 1;
                else if (currentArg.StartsWith("/"))
                {
                    // "/SomeSwitch" is equivalent to "--SomeSwitch" when interpreting switch mappings
                    // So we do a conversion to simplify later processing
                    currentArg = string.Format("--{0}", currentArg[1..]);
                    keyStartIndex = 2;
                }

                var separator = currentArg.IndexOf('=');

                if (separator < 0)
                {
                    // If there is neither equal sign nor prefix in current arugment, it is an invalid format
                    if (keyStartIndex == 0)
                        throw new FormatException($"{enumerator.Current} is an invalid argument.");
                    if (_booleanwitchMappings.TryGetValue(currentArg, out key))
                        value = "true";
                    else
                    {
                        // If the switch is a key in given switch mappings, interpret it
                        if (!_valueSwitchMappings.TryGetValue(currentArg, out key))
                        {
                            // If the switch starts with a single "-" and it isn't in given mappings , it is an invalid usage
                            if (keyStartIndex == 1)
                                throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                            // Otherwise, use the switch name directly as a key
                            key = currentArg[keyStartIndex..];
                        }

                        var previousKey = enumerator.Current;
                        if (!enumerator.MoveNext())
                            throw new FormatException($"{previousKey} is missing a value");

                        value = enumerator.Current;
                    }
                }
                else
                {
                    var keySegment = currentArg[..separator];
                    if (!_valueSwitchMappings.TryGetValue(keySegment, out key))
                    {
                        if (keyStartIndex == 1)
                            throw new FormatException($"Shortcut {enumerator.Current} not defined.");
                        key = currentArg[keyStartIndex..separator];
                    }
                    value = currentArg[(separator + 1)..];
                }

                // Override value when key is duplicated. So we always have the last argument win.
                data[key] = value;
            }

        Data = data;
    }
}
