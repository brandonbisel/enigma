namespace Enigma.Rotors;

/// <summary>
/// Wheel II of the Norenigma, the Enigma I machines the Norwegian police
/// security service kept after the war and rewired. The wheels are new; the notch
/// is where the service wheel of the same number had it, because the machine was
/// otherwise left alone.
/// </summary>
public class RotorNorwayNII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("GJLPUBSWEMCTQVHXAOFZDRKYNI");

    private static readonly int[] Turnover = WiringTable.Notches("E");

    public override string Name => "N-II";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
