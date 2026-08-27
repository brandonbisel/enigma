namespace Enigma;

/// <summary>
/// The wheels and reflectors a machine can be built from, looked up by the name a
/// key sheet uses. Implementations may draw on the parts the library ships with,
/// on parts defined in a file, or on both.
/// </summary>
public interface IPartsCatalogue
{
    IReadOnlyList<string> RotorNames { get; }
    IReadOnlyList<string> ReflectorNames { get; }
    IReadOnlyList<string> EntryWheelNames { get; }

    /// <summary>
    /// A fresh rotor: rotors carry a position, so every machine needs its own.
    /// </summary>
    IRotor CreateRotor(string name);

    IReflector GetReflector(string name);

    IEntryWheel GetEntryWheel(string name);
}
