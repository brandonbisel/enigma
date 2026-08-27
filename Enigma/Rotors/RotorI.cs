namespace Enigma.Rotors;

public class RotorI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("EKMFLGDQVZNTOWYHXUSPAIBRCJ");

    private static readonly int[] Turnover = [16]; // Q

    public override string Name => "I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
