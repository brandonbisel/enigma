namespace Enigma.Reflectors;

/// <summary>The Norenigma's reflector, rewired along with its wheels.</summary>
public class ReflectorNorway : ReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("MOWJYPUXNDSRAIBFVLKZGQCHET");

    public override string Name => "N";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
