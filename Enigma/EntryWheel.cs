namespace Enigma;

/// <summary>
/// An entry wheel described by its keyboard order: the letter at position i is the
/// key wired to contact i. That is the form the wiring is always published in, so
/// the standard service wheel reads "ABCDEFGHIJKLMNOPQRSTUVWXYZ" and the
/// commercial one "QWERTZUIOASDFGHJKPYXCVBNML", the keyboard read left to right.
/// </summary>
public class EntryWheel : IEntryWheel
{
    private readonly int[] _toLamp;
    private readonly int[] _toContact;

    public EntryWheel(string name, string keyboard, ICharacterMap? characterMap = null)
    {
        Name = name;

        var wiring = WiringTable.FromString(keyboard, characterMap);

        _toLamp = new int[wiring.Count];
        _toContact = new int[wiring.Count];

        foreach (var (contact, key) in wiring)
        {
            _toLamp[contact] = key;
            _toContact[key] = contact;
        }
    }

    public string Name { get; }

    public int Contacts => _toContact.Length;

    public int ToContact(int key) => _toContact[key];

    public int ToLamp(int contact) => _toLamp[contact];

    /// <summary>Wired in alphabet order, as every service Enigma was.</summary>
    public static EntryWheel Standard { get; } = StraightThrough(CharacterMap.Latin);

    /// <summary>
    /// A wheel wired straight through in whatever alphabet the machine works in.
    /// Since it is the identity it exists for any alphabet, unlike the keyboard
    /// wheels, which are a fixed twenty six key layout.
    /// </summary>
    public static EntryWheel StraightThrough(ICharacterMap characterMap) =>
        new("Standard",
            string.Concat(Enumerable.Range(0, characterMap.Count).Select(characterMap.GetCharacter)),
            characterMap);

    /// <summary>Wired in keyboard order, as the commercial and railway machines were.</summary>
    public static EntryWheel Qwertz { get; } = new("QWERTZ", "QWERTZUIOASDFGHJKPYXCVBNML");
}
