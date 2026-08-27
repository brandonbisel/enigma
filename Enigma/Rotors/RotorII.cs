namespace Enigma.Rotors;

public class RotorII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("AJDKSIRUXBLHWTMCQGZNPYFVOE");

    private static readonly int[] Turnover = [4]; // E

    public override string Name => "II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
