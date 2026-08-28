namespace Enigma.Rotors;

/// <summary>Wheel II of the Railway Enigma (Rocket), as wired in machine K438, with a single notch on the letter ring.</summary>
public class RotorRailwayII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("HXMQKGJTSCZFLBERNAWYIDOVPU");

    private static readonly int[] Turnover = WiringTable.Notches("E");

    public override string Name => "R-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
