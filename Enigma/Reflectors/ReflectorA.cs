namespace Enigma.Reflectors;

public class ReflectorA : ReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("EJMZALYXVBWFCRQUONTSPIKHGD");

    public override string Name => "A";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
