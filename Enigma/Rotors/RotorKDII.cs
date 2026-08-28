namespace Enigma.Rotors;

/// <summary>
/// Wheel II of the Enigma KD, a commercial K fitted with a rewirable UKW-D.
/// All three wheels carry the same nine notches.
/// </summary>
public class RotorKDII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("HGRBSJZETDLVPMQYCXAOKINFUW");

    private static readonly int[] Turnover = WiringTable.Notches("SUYAEHLNQ");

    public override string Name => "KD-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
