namespace Enigma.Rotors;

public class RotorBeta : ThinRotorBase
{
    private static readonly IDictionary<int, int> RotorWiring =
        WiringTable.FromString("LEYJVCNIXWPBQMDRTAKZGFUHOS");

    public override string Name => "Beta";
    protected override IDictionary<int, int> Wiring => RotorWiring;
}
