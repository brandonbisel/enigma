namespace Enigma.Reflectors;

/// <summary>Thin reflector UKW-B ("Bruno"), paired with a fourth rotor in the M4.</summary>
public class ReflectorBThin : ReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("ENKQAUYWJICOPBLMDXZVFTHRGS");

    public override string Name => "B-Thin";
    public override bool IsThin => true;
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
