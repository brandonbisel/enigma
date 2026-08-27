namespace Enigma.Rotors;

public class RotorVII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("NZJHGRCXMYSWBOUFAIVLPEKQDT");

    private static readonly int[] Turnover = [12, 25]; // M and Z

    public override string Name => "VII";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
