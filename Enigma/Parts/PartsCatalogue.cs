using Enigma.Models;
using Microsoft.Extensions.Logging;

namespace Enigma.Parts;

/// <summary>
/// Parts defined in a file, laid over the ones the library ships with. A defined
/// part with the same name as a built-in one takes its place, which is what makes
/// it possible to model a machine whose wheels happen to share our names. The
/// substitution is reported, because ciphertext produced that way cannot be
/// reproduced without the same file.
/// </summary>
public class PartsCatalogue : IPartsCatalogue
{
    private readonly IPartsCatalogue _builtIn;
    private readonly Dictionary<string, RotorDefinition> _rotors;
    private readonly Dictionary<string, ReflectorDefinition> _reflectors;

    public PartsCatalogue(IPartsCatalogue builtIn, PartsFile parts, ILogger<PartsCatalogue>? logger = null)
    {
        _builtIn = builtIn;

        _rotors = parts.Rotors.ToDictionary(
            rotor => Normalise(rotor.Name), StringComparer.OrdinalIgnoreCase);

        _reflectors = parts.Reflectors.ToDictionary(
            reflector => Normalise(reflector.Name), StringComparer.OrdinalIgnoreCase);

        // Build every definition once, so a bad wiring is reported when the file is
        // read rather than part way through enciphering a message.
        foreach (var name in _rotors.Keys)
        {
            CreateRotor(name);
        }

        foreach (var name in _reflectors.Keys)
        {
            GetReflector(name);
        }

        foreach (var shadowed in _rotors.Keys.Intersect(builtIn.RotorNames, StringComparer.OrdinalIgnoreCase))
        {
            logger?.LogWarning("Rotor {Name} is defined in the parts file and replaces the built-in one", shadowed);
        }

        foreach (var shadowed in _reflectors.Keys.Intersect(builtIn.ReflectorNames, StringComparer.OrdinalIgnoreCase))
        {
            logger?.LogWarning("Reflector {Name} is defined in the parts file and replaces the built-in one", shadowed);
        }
    }

    public IReadOnlyList<string> RotorNames =>
        _rotors.Keys.Union(_builtIn.RotorNames, StringComparer.OrdinalIgnoreCase).Order().ToList();

    public IReadOnlyList<string> ReflectorNames =>
        _reflectors.Keys.Union(_builtIn.ReflectorNames, StringComparer.OrdinalIgnoreCase).Order().ToList();

    public IRotor CreateRotor(string name)
    {
        if (_rotors.TryGetValue(Normalise(name), out var definition))
        {
            return definition.Thin ? new DefinedThinRotor(definition) : new DefinedRotor(definition);
        }

        // Reported here rather than by the inner catalogue, which cannot know what
        // the file added and would list only half of what is available.
        if (!_builtIn.RotorNames.Contains(Normalise(name), StringComparer.OrdinalIgnoreCase))
        {
            throw UnknownRotor(name, RotorNames);
        }

        return _builtIn.CreateRotor(name);
    }

    public IReflector GetReflector(string name)
    {
        if (_reflectors.TryGetValue(Normalise(name), out var definition))
        {
            return new DefinedReflector(definition);
        }

        if (!_builtIn.ReflectorNames.Contains(Normalise(name), StringComparer.OrdinalIgnoreCase))
        {
            throw UnknownReflector(name, ReflectorNames);
        }

        return _builtIn.GetReflector(name);
    }

    private static string Normalise(string name) => (name ?? string.Empty).Trim().ToUpperInvariant();

    internal static ArgumentException UnknownRotor(string name, IEnumerable<string> known) =>
        new($"Unknown rotor '{name}'. Known rotors: {string.Join(", ", known)}.");

    internal static ArgumentException UnknownReflector(string name, IEnumerable<string> known) =>
        new($"Unknown reflector '{name}'. Known reflectors: {string.Join(", ", known)}.");
}
