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

    public EntryWheel(string name, string keyboard)
    {
        Name = name;

        var wiring = WiringTable.FromString(keyboard);

        _toLamp = new int[wiring.Count];
        _toContact = new int[wiring.Count];

        foreach (var (contact, key) in wiring)
        {
            _toLamp[contact] = key;
            _toContact[key] = contact;
        }
    }

    public string Name { get; }

    public int ToContact(int key) => _toContact[key];

    public int ToLamp(int contact) => _toLamp[contact];

    /// <summary>Wired in alphabet order, as every service Enigma was.</summary>
    public static EntryWheel Standard { get; } = new("Standard", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

    /// <summary>Wired in keyboard order, as the commercial and railway machines were.</summary>
    public static EntryWheel Qwertz { get; } = new("QWERTZ", "QWERTZUIOASDFGHJKPYXCVBNML");
}
