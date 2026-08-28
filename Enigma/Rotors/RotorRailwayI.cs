namespace Enigma.Rotors;

/// <summary>Wheel I of the Railway Enigma (Rocket), as wired in machine K438, with a single notch on the letter ring.</summary>
public class RotorRailwayI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("EVLPKUDJHTGSZFRABWYICOXNMQ");

    private static readonly int[] Turnover = WiringTable.Notches("Y");

    public override string Name => "R-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
