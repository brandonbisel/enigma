namespace Enigma.Reflectors;

/// <summary>The reflector of the commercial A28/G31. It is set like a wheel, and driven like one.</summary>
public class ReflectorG : RotatingReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("IMETCGFRAYSQBZXWLHKDVUPOJN");

    public override string Name => "G";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
