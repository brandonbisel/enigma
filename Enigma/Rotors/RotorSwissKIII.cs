namespace Enigma.Rotors;

/// <summary>Wheel III of the Swiss Air Force's Enigma K, rewired by the Swiss, with a single notch on the letter ring.</summary>
public class RotorSwissKIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("EHRVXGAOBQUSIMZFLYNWKTPDJC");

    private static readonly int[] Turnover = WiringTable.Notches("N");

    public override string Name => "SK-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
