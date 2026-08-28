namespace Enigma.Rotors;

/// <summary>Wheel II of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("UPHZLWEQMTDJXCAKSOIGVBYFNR");

    private static readonly int[] Turnover = WiringTable.Notches("WZFLR");

    public override string Name => "T-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
