namespace Enigma.Rotors;

/// <summary>Wheel I of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzI : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("KPTYUELOCVGRFQDANJMBSWHZXI");

    private static readonly int[] Turnover = WiringTable.Notches("WZEKQ");

    public override string Name => "T-I";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
