using Enigma.Models;

namespace Enigma.Parts;

internal sealed class DefinedRotor : RotorBase
{
    private readonly IDictionary<int, int> _wiring;
    private readonly int[] _notches;

    public DefinedRotor(RotorDefinition definition)
    {
        Name = definition.Name;
        _wiring = WiringTable.FromString(definition.Wiring);
        _notches = ParseNotches(definition.Notches, definition.Name);
    }

    public override string Name { get; }
    protected override IEnumerable<int> TurnoverPositions => _notches;
    protected override IDictionary<int, int> Wiring => _wiring;

    internal static int[] ParseNotches(string notches, string name) =>
        (notches ?? string.Empty)
            .Where(letter => !char.IsWhiteSpace(letter))
            .Select(letter => char.ToUpperInvariant(letter) is >= 'A' and <= 'Z'
                ? char.ToUpperInvariant(letter) - 'A'
                : throw new ArgumentException(
                    $"Rotor '{name}' has a notch '{letter}' that is not a letter."))
            .ToArray();
}

internal sealed class DefinedThinRotor : ThinRotorBase
{
    private readonly IDictionary<int, int> _wiring;

    public DefinedThinRotor(RotorDefinition definition)
    {
        Name = definition.Name;
        _wiring = WiringTable.FromString(definition.Wiring);

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

    public DefinedReflector(ReflectorDefinition definition)
    {
        Name = definition.Name;
        IsThin = definition.Thin;
        _wiring = WiringTable.FromReflectorString(definition.Wiring);
    }

    public override string Name { get; }
    public override bool IsThin { get; }
    protected override IDictionary<int, int> Wiring => _wiring;
}
