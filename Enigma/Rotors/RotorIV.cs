namespace Enigma.Rotors;

public class RotorIV : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("ESOVPZJAYQUIRHXLNFTGKDCMWB");

    private static readonly int[] Turnover = [9]; // J

    public override string Name => "IV";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
