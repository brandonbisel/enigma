namespace Enigma.Rotors;

/// <summary>Wheel I of the Swiss Air Force's Enigma K, rewired by the Swiss, with a single notch on the letter ring.</summary>
public class RotorSwissKI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("PEZUOHXSCVFMTBGLRINQJWAYDK");

    private static readonly int[] Turnover = WiringTable.Notches("Y");

    public override string Name => "SK-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
