namespace Enigma.Rotors;

/// <summary>
/// Wheel I of the Norenigma, the Enigma I machines the Norwegian police
/// security service kept after the war and rewired. The wheels are new; the notch
/// is where the service wheel of the same number had it, because the machine was
/// otherwise left alone.
/// </summary>
public class RotorNorwayNI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("WTOKASUYVRBXJHQCPZEFMDINLG");

    private static readonly int[] Turnover = WiringTable.Notches("Q");

    public override string Name => "N-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
