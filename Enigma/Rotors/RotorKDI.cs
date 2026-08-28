namespace Enigma.Rotors;

/// <summary>
/// Wheel I of the Enigma KD, a commercial K fitted with a rewirable UKW-D.
/// All three wheels carry the same nine notches.
/// </summary>
public class RotorKDI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("VEZIOJCXKYDUNTWAPLQGBHSFMR");

    private static readonly int[] Turnover = WiringTable.Notches("SUYAEHLNQ");

    public override string Name => "KD-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
