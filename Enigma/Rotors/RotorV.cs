namespace Enigma.Rotors;

public class RotorV : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("VZBRGITYUPSDNHLXAWMJQOFECK");

    private static readonly int[] Turnover = [25]; // Z

    public override string Name => "V";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
