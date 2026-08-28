namespace Enigma.Rotors;

/// <summary>
/// Wheel II of the commercial Enigma K (A27). The same wiring as the Enigma D,
/// but the notch is on the letter ring as it is on every later machine, and each
/// wheel carries at its own letter.
/// </summary>
public class RotorKII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString(CommercialWiring.Second);

    private static readonly int[] Turnover = WiringTable.Notches("E");

    public override string Name => "K-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
