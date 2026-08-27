namespace Enigma.Reflectors;

public class ReflectorC : ReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromString("FVPJIAOYEDRZXWGCTKUQSBNMHL");

    public override string Name => "C";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
