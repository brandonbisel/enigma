namespace Enigma;

/// <summary>
/// The entry wheel (Eintrittswalze), the fixed stator between the plugboard and
/// the first rotor. It decides which rotor contact each key is wired to.
///
/// On the Wehrmacht and naval machines it is wired straight through in alphabet
/// order, so it changes nothing; the commercial and railway machines wire it in
/// keyboard order instead, which is why their traffic cannot be read on a service
/// machine even with the same wheels.
/// </summary>
public interface IEntryWheel
{
    string Name { get; }

    /// <summary>The rotor contact a key is wired to, on the way in.</summary>
    int ToContact(int key);

    /// <summary>The lamp a rotor contact is wired to, on the way back out.</summary>
    int ToLamp(int contact);
}
