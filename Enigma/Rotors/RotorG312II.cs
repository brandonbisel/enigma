namespace Enigma.Rotors;

/// <summary>Wheel II of the Abwehr G-312 (Bletchley Park), with its 15 notches.</summary>
public class RotorG312II : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("HQZGPJTMOBLNCIFDYAWVEUSRKX");

    // The notch positions are identical on every surviving Zählwerk machine,
    // whatever its wiring and whoever the customer was.
    private static readonly int[] Turnover = WiringTable.Notches("STVYZACDFGHKMNQ");

    public override string Name => "G312-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
