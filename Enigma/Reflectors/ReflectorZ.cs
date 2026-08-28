namespace Enigma.Reflectors;

/// <summary>
/// The reflector of the Enigma Z30. It is set like a wheel and driven like one:
/// a fourth pawl rides the leftmost wheel's notch ring and turns it.
/// </summary>
public class ReflectorZ : RotatingReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("2507918364", CharacterMap.Digits);

    public override string Name => "Z";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
