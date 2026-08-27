namespace Enigma;

/// <summary>
/// What kind of machine is being built: how its wheels are driven, and which
/// arrangements of parts could actually be assembled.
///
/// These are properties of a model rather than of Enigmas in general — a naval M4
/// takes a fourth wheel that an Enigma I cannot, and a Zählwerk machine has no
/// plugboard at all — so they live here instead of in the factory.
/// </summary>
public interface IMachineLayout
{
    string Name { get; }

    IStepping Drive { get; }

    /// <summary>The entry wheel used when a key sheet does not name one.</summary>
    string DefaultEntryWheel { get; }

    /// <summary>False for machines built without a Steckerbrett.</summary>
    bool AllowsPlugBoard { get; }

    void Validate(IReadOnlyList<IRotor> rotors, IReflector reflector, IEntryWheel entryWheel);
}
