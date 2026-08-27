namespace Enigma.Rotors;

public class RotorVI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("JPGVOUMFYQBENHZRDKASXLICTW");

    private static readonly int[] Turnover = [12, 25]; // M and Z

    public override string Name => "VI";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
