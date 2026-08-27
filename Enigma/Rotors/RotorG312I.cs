namespace Enigma.Rotors;

/// <summary>Wheel I of the Abwehr G-312 (Bletchley Park), with its 17 notches.</summary>
public class RotorG312I : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("DMTWSILRUYQNKFEJCAZBPGXOHV");

    // The notch positions are identical on every surviving Zählwerk machine,
    // whatever its wiring and whoever the customer was.
    private static readonly int[] Turnover = WiringTable.Notches("SUVWZABCEFGIKLOPQ");

    public override string Name => "G312-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
