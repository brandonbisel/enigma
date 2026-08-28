namespace Enigma.Rotors;

/// <summary>Wheel VIII of the Enigma T (Tirpitz), built for traffic with the Japanese navy, with its five notches, which it shares with another wheel of the set.</summary>
public class RotorTirpitzVIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("YMTPNZHWKODAJXELUQVGCBISFR");

    private static readonly int[] Turnover = WiringTable.Notches("XEIMQ");

    public override string Name => "T-VIII";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
