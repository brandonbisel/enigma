namespace Enigma.Rotors;

/// <summary>
/// Wheel I of the commercial Enigma K (A27). The same wiring as the Enigma D,
/// but the notch is on the letter ring as it is on every later machine, and each
/// wheel carries at its own letter.
/// </summary>
public class RotorKI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString(CommercialWiring.First);

    private static readonly int[] Turnover = WiringTable.Notches("Y");

    public override string Name => "K-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
