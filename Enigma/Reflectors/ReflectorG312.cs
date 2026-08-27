namespace Enigma.Reflectors;

/// <summary>The reflector of the Abwehr G-312. It is set like a wheel, and driven like one.</summary>
public class ReflectorG312 : RotatingReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("RULQMZJSYGOCETKWDAHNBXPVIF");

    public override string Name => "G312";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
