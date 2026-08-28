namespace Enigma.Rotors;

/// <summary>
/// Wheel IV of the Norenigma, the Enigma I machines the Norwegian police
/// security service kept after the war and rewired. The wheels are new; the notch
/// is where the service wheel of the same number had it, because the machine was
/// otherwise left alone.
/// </summary>
public class RotorNorwayNIV : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("FGZJMVXEPBWSHQTLIUDYKCNRAO");

    private static readonly int[] Turnover = WiringTable.Notches("J");

    public override string Name => "N-IV";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
