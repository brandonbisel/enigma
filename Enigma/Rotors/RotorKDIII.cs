namespace Enigma.Rotors;

/// <summary>
/// Wheel III of the Enigma KD, a commercial K fitted with a rewirable UKW-D.
/// All three wheels carry the same nine notches.
/// </summary>
public class RotorKDIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("NWLHXGRBYOJSAZDVTPKFQMEUIC");

    private static readonly int[] Turnover = WiringTable.Notches("SUYAEHLNQ");

    public override string Name => "KD-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
