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
    private readonly Dictionary<string, EntryWheelDefinition> _entryWheels;
    private readonly Dictionary<string, CharacterMapDefinition> _characterMaps;
    private readonly ICharacterMap _defaultAlphabet;

    public PartsCatalogue(IPartsCatalogue builtIn, PartsFile parts, ILogger<PartsCatalogue>? logger = null)
    {
        _builtIn = builtIn;

        _rotors = parts.Rotors.ToDictionary(
            rotor => Normalise(rotor.Name), StringComparer.OrdinalIgnoreCase);

        _reflectors = parts.Reflectors.ToDictionary(
            reflector => Normalise(reflector.Name), StringComparer.OrdinalIgnoreCase);

        _entryWheels = parts.EntryWheels.ToDictionary(
            wheel => Normalise(wheel.Name), StringComparer.OrdinalIgnoreCase);

        _characterMaps = parts.CharacterMaps.ToDictionary(
            map => Normalise(map.Name), StringComparer.OrdinalIgnoreCase);

        // A file usually describes one machine, so a part that does not name an
        // alphabet takes the file's own when there is exactly one to take.
        _defaultAlphabet = _characterMaps.Count == 1
            ? GetCharacterMap(_characterMaps.Keys.Single())
            : Enigma.CharacterMap.Latin;

        // Build every definition once, so a bad wiring is reported when the file is
        // read rather than part way through enciphering a message. Alphabets are
        // already resolved above, since the wheels are built against them.
        foreach (var name in _characterMaps.Keys)
        {
            GetCharacterMap(name);
        }

        foreach (var name in _rotors.Keys)
        {
            CreateRotor(name);
        }

        foreach (var name in _reflectors.Keys)
        {
            GetReflector(name);
        }

        foreach (var name in _entryWheels.Keys)
        {
            GetEntryWheel(name);
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

    // Layouts are not definable in a parts file: a machine's drive is code, not data.
    public IReadOnlyList<string> LayoutNames => _builtIn.LayoutNames;

    public IMachineLayout GetLayout(string name) => _builtIn.GetLayout(name);

    public IReadOnlyList<string> CharacterMapNames =>
        _characterMaps.Keys.Union(_builtIn.CharacterMapNames, StringComparer.OrdinalIgnoreCase).Order().ToList();

    public ICharacterMap GetCharacterMap(string name)
    {
        if (_characterMaps.TryGetValue(Normalise(name), out var definition))
        {
            return new CharacterMap(definition.Name, definition.Characters);
        }

        if (!_builtIn.CharacterMapNames.Contains(Normalise(name), StringComparer.OrdinalIgnoreCase))
        {
            throw UnknownCharacterMap(name, CharacterMapNames);
        }

        return _builtIn.GetCharacterMap(name);
    }

    public IReadOnlyList<string> EntryWheelNames =>
        _entryWheels.Keys.Union(_builtIn.EntryWheelNames, StringComparer.OrdinalIgnoreCase).Order().ToList();

    public IEntryWheel GetEntryWheel(string name, ICharacterMap? characterMap = null)
    {
        if (_entryWheels.TryGetValue(Normalise(name), out var definition))
        {
            return new EntryWheel(
                definition.Name, definition.Keyboard, AlphabetFor(definition.CharacterMap, characterMap));
        }

        if (!_builtIn.EntryWheelNames.Contains(Normalise(name), StringComparer.OrdinalIgnoreCase))
        {
            throw UnknownEntryWheel(name, EntryWheelNames);
        }

        return _builtIn.GetEntryWheel(name, characterMap);
    }

    public IRotor CreateRotor(string name, ICharacterMap? characterMap = null)
    {
        if (_rotors.TryGetValue(Normalise(name), out var definition))
        {
            var alphabet = AlphabetFor(definition.CharacterMap, characterMap);

            return definition.Thin
                ? new DefinedThinRotor(definition, alphabet)
                : new DefinedRotor(definition, alphabet);
        }

        // Reported here rather than by the inner catalogue, which cannot know what
        // the file added and would list only half of what is available.
        if (!_builtIn.RotorNames.Contains(Normalise(name), StringComparer.OrdinalIgnoreCase))
        {
            throw UnknownRotor(name, RotorNames);
        }

        return _builtIn.CreateRotor(name, characterMap);
    }

    public IReflector GetReflector(string name, ICharacterMap? characterMap = null)
    {
        if (_reflectors.TryGetValue(Normalise(name), out var definition))
        {
            return new DefinedReflector(definition, AlphabetFor(definition.CharacterMap, characterMap));
        }

        if (!_builtIn.ReflectorNames.Contains(Normalise(name), StringComparer.OrdinalIgnoreCase))
        {
            throw UnknownReflector(name, ReflectorNames);
        }

        return _builtIn.GetReflector(name, characterMap);
    }

    private ICharacterMap AlphabetFor(string declared, ICharacterMap? requested) =>
        !string.IsNullOrWhiteSpace(declared) ? GetCharacterMap(declared)
        : requested ?? _defaultAlphabet;

    private static string Normalise(string name) => (name ?? string.Empty).Trim().ToUpperInvariant();

    internal static ArgumentException UnknownRotor(string name, IEnumerable<string> known) =>
        new($"Unknown rotor '{name}'. Known rotors: {string.Join(", ", known)}.");

    internal static ArgumentException UnknownReflector(string name, IEnumerable<string> known) =>
        new($"Unknown reflector '{name}'. Known reflectors: {string.Join(", ", known)}.");

    internal static ArgumentException UnknownEntryWheel(string name, IEnumerable<string> known) =>
        new($"Unknown entry wheel '{name}'. Known entry wheels: {string.Join(", ", known)}.");

    internal static ArgumentException UnknownLayout(string name, IEnumerable<string> known) =>
        new($"Unknown machine model '{name}'. Known models: {string.Join(", ", known)}.");

    internal static ArgumentException UnknownCharacterMap(string name, IEnumerable<string> known) =>
        new($"Unknown character map '{name}'. Known character maps: {string.Join(", ", known)}.");
}
