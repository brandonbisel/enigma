namespace Enigma.Rotors;

/// <summary>
/// Wheel V of the Norenigma, the Enigma I machines the Norwegian police
/// security service kept after the war and rewired. The wheels are new; the notch
/// is where the service wheel of the same number had it, because the machine was
/// otherwise left alone.
/// </summary>
public class RotorNorwayNV : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("HEJXQOTZBVFDASCILWPGYNMURK");

    private static readonly int[] Turnover = WiringTable.Notches("Z");

    public override string Name => "N-V";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
