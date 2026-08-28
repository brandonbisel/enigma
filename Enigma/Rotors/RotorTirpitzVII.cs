namespace Enigma.Rotors;

/// <summary>Wheel VII of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzVII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("BJVFTXPLNAYOZIKWGDQERUCHSM");

    private static readonly int[] Turnover = WiringTable.Notches("YCFKR");

    public override string Name => "T-VII";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
