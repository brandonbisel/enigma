namespace Enigma.Reflectors;

/// <summary>
/// The Railway Enigma's reflector, from machine K438. It is set to a position and
/// stays there: Friedman's report of a moving UKW on this machine was mistaken.
/// </summary>
public class ReflectorRailway : RotatingReflectorBase
{
    private static readonly IDictionary<int, int> ReflectorWiring =
        WiringTable.FromReflectorString("DNSAJQIPGEXRWBVHFLCZYOMKUT");

    public override string Name => "R";
    protected override IDictionary<int, int> Wiring => ReflectorWiring;
}
