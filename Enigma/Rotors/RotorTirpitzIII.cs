namespace Enigma.Rotors;

/// <summary>Wheel III of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("QUDLYRFEKONVZAXWHMGPJBSICT");

    private static readonly int[] Turnover = WiringTable.Notches("WZEKQ");

    public override string Name => "T-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
