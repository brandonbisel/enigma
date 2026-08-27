namespace Enigma.Rotors;

public class RotorVIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("FKQHTLXOCBJSPDZRAMEWNIUYGV");

    private static readonly int[] Turnover = [12, 25]; // M and Z

    public override string Name => "VIII";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
