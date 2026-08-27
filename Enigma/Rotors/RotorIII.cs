namespace Enigma.Rotors;

public class RotorIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("BDFHJLCPRTXVZNYEIWGAKMUSQO");

    private static readonly int[] Turnover = [21]; // V

    public override string Name => "III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
