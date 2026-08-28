namespace Enigma.Rotors;

/// <summary>
/// Wheel III of the commercial Enigma K (A27). The same wiring as the Enigma D,
/// but the notch is on the letter ring as it is on every later machine, and each
/// wheel carries at its own letter.
/// </summary>
public class RotorKIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString(CommercialWiring.Third);

    private static readonly int[] Turnover = WiringTable.Notches("N");

    public override string Name => "K-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
