namespace Enigma.Rotors;

/// <summary>Wheel I of the A28 (A-865) / G31 commercial, with its 17 notches.</summary>
public class RotorGI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("LPGSZMHAEOQKVXRFYBUTNICJDW");

    // The notch positions are identical on every surviving Zählwerk machine,
    // whatever its wiring and whoever the customer was.
    private static readonly int[] Turnover = WiringTable.Notches("SUVWZABCEFGIKLOPQ");

    public override string Name => "G-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
