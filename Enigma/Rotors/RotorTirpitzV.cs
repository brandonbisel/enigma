namespace Enigma.Rotors;

/// <summary>Wheel V of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzV : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("UAXGISNJBVERDYLFZWTPCKOHMQ");

    private static readonly int[] Turnover = WiringTable.Notches("YCFKR");

    public override string Name => "T-V";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
