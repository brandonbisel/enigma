namespace Enigma.Reflectors;

/// <summary>Thin reflector UKW-C ("Caesar"), paired with a fourth rotor in the M4.</summary>
public class ReflectorCThin : ReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("RDOBJNTKVEHMLFCWZAXGYIPSUQ");

    public override string Name => "C-Thin";
    public override bool IsThin => true;
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
