namespace Enigma.Reflectors;

/// <summary>
/// The Enigma T's reflector. Settable, and nothing drives it — though its
/// operators were told to advance it by hand after every fifth group, which is a
/// procedure rather than a mechanism.
/// </summary>
public class ReflectorTirpitz : RotatingReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("GEKPBTAUMOCNILJDXZYFHWVQSR");

    public override string Name => "T";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
