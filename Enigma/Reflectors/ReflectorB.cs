namespace Enigma.Reflectors;

public class ReflectorB : ReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("YRUHQSLDPXNGOKMIEBFZCWVJAT");

    public override string Name => "B";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
