namespace Enigma.Rotors;

public class RotorGamma : ThinRotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("FSOKANUERHMBTIYCWLQPZXVGJD");

    public override string Name => "Gamma";
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
