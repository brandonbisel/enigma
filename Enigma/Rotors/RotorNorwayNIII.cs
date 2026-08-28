namespace Enigma.Rotors;

/// <summary>
/// Wheel III of the Norenigma, the Enigma I machines the Norwegian police
/// security service kept after the war and rewired. The wheels are new; the notch
/// is where the service wheel of the same number had it, because the machine was
/// otherwise left alone.
/// </summary>
public class RotorNorwayNIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("JWFMHNBPUSDYTIXVZGRQLAOEKC");

    private static readonly int[] Turnover = WiringTable.Notches("V");

    public override string Name => "N-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
