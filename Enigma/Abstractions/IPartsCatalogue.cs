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
    IReadOnlyList<string> CharacterMapNames { get; }
    IReadOnlyList<string> LayoutNames { get; }

    /// <summary>
    /// A fresh rotor: rotors carry a position, so every machine needs its own.
    /// </summary>
    IRotor CreateRotor(string name, ICharacterMap? characterMap = null);

    IReflector GetReflector(string name, ICharacterMap? characterMap = null);

    IEntryWheel GetEntryWheel(string name, ICharacterMap? characterMap = null);

    ICharacterMap GetCharacterMap(string name);

    IMachineLayout GetLayout(string name);
}
