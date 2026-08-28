namespace Enigma.Rotors;

/// <summary>Wheel II of the Swiss Air Force's Enigma K, rewired by the Swiss, with a single notch on the letter ring.</summary>
public class RotorSwissKII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("ZOUESYDKFWPCIQXHMVBLGNJRAT");

    private static readonly int[] Turnover = WiringTable.Notches("E");

    public override string Name => "SK-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
