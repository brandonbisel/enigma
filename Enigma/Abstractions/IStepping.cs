namespace Enigma;

/// <summary>
/// How the wheels are advanced on a keypress. The service Enigmas used pawls and
/// ratchets, which produce the double step; the Zählwerk machines used cogwheels,
/// which do not.
/// </summary>
public interface IStepping
{
    string Name { get; }

    /// <summary>Advances the wheels, and the reflector if it is one that moves.</summary>
    void Advance(IReadOnlyList<IRotor> rotors, IReflector reflector);
}
