namespace Enigma.Rotors;

/// <summary>Wheel III of the Abwehr G-312 (Bletchley Park), with its 11 notches.</summary>
public class RotorG312III : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("UQNTLSZFMREHDPXKIBVYGJCWOA");

    // The notch positions are identical on every surviving Zählwerk machine,
    // whatever its wiring and whoever the customer was.
    private static readonly int[] Turnover = WiringTable.Notches("UWXAEFHKMNR");

    public override string Name => "G312-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
