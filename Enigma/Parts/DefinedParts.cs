using Enigma.Models;

namespace Enigma.Parts;

internal sealed class DefinedRotor : RotorBase
{
    private readonly IDictionary<int, int> _wiring;
    private readonly int[] _notches;

    public DefinedRotor(RotorDefinition definition, ICharacterMap? characterMap = null)
    {
        var alphabet = characterMap ?? CharacterMap.Latin;

        Name = definition.Name;
        _wiring = WiringTable.FromString(definition.Wiring, alphabet);
        _notches = ParseNotches(definition.Notches, definition.Name, alphabet);
    }

    public override string Name { get; }
    protected override IEnumerable<int> TurnoverPositions => _notches;
    protected override IDictionary<int, int> Wiring => _wiring;

    internal static int[] ParseNotches(string notches, string name, ICharacterMap alphabet) =>
        (notches ?? string.Empty)
            .Where(character => !char.IsWhiteSpace(character))
            .Select(character => alphabet.GetIndex(character) is var index && index >= 0
                ? index
                : throw new ArgumentException(
                    $"Rotor '{name}' has a notch '{character}' that is not in the {alphabet.Name} alphabet."))
            .ToArray();
}

internal sealed class DefinedThinRotor : ThinRotorBase
{
    private readonly IDictionary<int, int> _wiring;

    public DefinedThinRotor(RotorDefinition definition, ICharacterMap? characterMap = null)
    {
        Name = definition.Name;
        _wiring = WiringTable.FromString(definition.Wiring, characterMap);

        if (!string.IsNullOrWhiteSpace(definition.Notches))
        {
            throw new ArgumentException(
                $"Thin rotor '{definition.Name}' cannot have notches: a thin wheel has none.");
        }
    }

    public override string Name { get; }
    protected override IDictionary<int, int> Wiring => _wiring;
}

internal sealed class DefinedReflector : ReflectorBase
{
    private readonly IDictionary<int, int> _wiring;

    public DefinedReflector(ReflectorDefinition definition, ICharacterMap? characterMap = null)
    {
        Name = definition.Name;
        IsThin = definition.Thin;
        _wiring = WiringTable.FromReflectorString(definition.Wiring, characterMap);
    }

    public override string Name { get; }
    public override bool IsThin { get; }
    protected override IDictionary<int, int> Wiring => _wiring;
}
