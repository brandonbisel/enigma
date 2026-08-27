namespace Enigma.Rotors;

/// <summary>Wheel III of the A28 (A-865) / G31 commercial, with its 11 notches.</summary>
public class RotorGIII : RotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("CJGDPSHKTURAWZXFMYNQOBVLIE");

    // The notch positions are identical on every surviving Zählwerk machine,
    // whatever its wiring and whoever the customer was.
    private static readonly int[] Turnover = WiringTable.Notches("UWXAEFHKMNR");

    public override string Name => "G-III";
    protected override IEnumerable<int> TurnoverPositions => Turnover;
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
