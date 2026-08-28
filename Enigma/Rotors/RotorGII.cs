namespace Enigma.Rotors;

/// <summary>Wheel II of the A28 (A-865) / G31 commercial, with its 15 notches.</summary>
public class RotorGII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString(CommercialWiring.Second);

    // The notch positions are identical on every surviving Zählwerk machine,
    // whatever its wiring and whoever the customer was.
    private static readonly int[] Turnover = WiringTable.Notches("STVYZACDFGHKMNQ");

    public override string Name => "G-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
