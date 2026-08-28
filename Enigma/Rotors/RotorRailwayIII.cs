namespace Enigma.Rotors;

/// <summary>Wheel III of the Railway Enigma (Rocket), as wired in machine K438, with a single notch on the letter ring.</summary>
public class RotorRailwayIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("JHDBSKYPZNMVXURECLIGQOAWTF");

    private static readonly int[] Turnover = WiringTable.Notches("N");

    public override string Name => "R-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
