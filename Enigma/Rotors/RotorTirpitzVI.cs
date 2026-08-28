namespace Enigma.Rotors;

/// <summary>Wheel VI of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzVI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("XFUZGALVHCNYSEWQTDMRBKPIOJ");

    private static readonly int[] Turnover = WiringTable.Notches("XEIMQ");

    public override string Name => "T-VI";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
