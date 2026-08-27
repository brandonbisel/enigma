namespace Enigma;

public static class WiringTable
{
    /// <summary>
    /// Wiring is conventionally written as the sequence of characters each contact
    /// maps to, in contact order: "EKMF..." means A to E, B to K, C to M, D to F.
    /// The alphabet decides which characters are legal and how many contacts there
    /// are, so a wiring that is the wrong length for its machine is refused here
    /// rather than failing somewhere downstream.
    /// </summary>
    public static IDictionary<int, int> FromString(string wiring, ICharacterMap? characterMap = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(wiring);

        var alphabet = characterMap ?? CharacterMap.Latin;
        var table = new Dictionary<int, int>(wiring.Length);

        if (wiring.Length != alphabet.Count)
        {
            throw new ArgumentException(
                $"Wiring '{wiring}' has {wiring.Length} contacts, but the {alphabet.Name} alphabet has {alphabet.Count}.",
                nameof(wiring));
        }

        for (var i = 0; i < wiring.Length; i++)
        {
            var index = alphabet.GetIndex(wiring[i]);

            if (index < 0)
            {
                throw new ArgumentException(
                    $"Wiring '{wiring}' contains '{wiring[i]}', which is not in the {alphabet.Name} alphabet.",
                    nameof(wiring));
            }

            table[i] = index;
        }

        // Every contact must reach exactly one other, or the rotor could not be
        // run backwards and characters would be lost.
        if (table.Values.Distinct().Count() != wiring.Length)
        {
            throw new ArgumentException(
                $"Wiring '{wiring}' is not a permutation: it must use each of its {wiring.Length} contacts exactly once.",
                nameof(wiring));
        }

        return table;
    }

    /// <summary>
    /// A reflector is wired in pairs, so its table must be its own inverse and may
    /// never connect a contact to itself. Both properties are what make the machine
    /// reciprocal, and the second is the flaw that made it attackable. An alphabet
    /// with an odd number of characters cannot carry one at all.
    /// </summary>
    public static IDictionary<int, int> FromReflectorString(string wiring, ICharacterMap? characterMap = null)
    {
        var alphabet = characterMap ?? CharacterMap.Latin;

        if (alphabet.Count % 2 != 0)
        {
            throw new ArgumentException(
                $"The {alphabet.Name} alphabet has {alphabet.Count} characters, which cannot be wired in pairs.",
                nameof(characterMap));
        }

        var table = FromString(wiring, alphabet);

        foreach (var (contact, output) in table)
        {
            if (contact == output)
            {
                throw new ArgumentException(
                    $"Reflector wiring '{wiring}' connects contact {contact} to itself.", nameof(wiring));
            }

            if (table[output] != contact)
            {
                throw new ArgumentException(
                    $"Reflector wiring '{wiring}' is not paired: contact {contact} reaches {output}, " +
                    $"but {output} reaches {table[output]}.",
                    nameof(wiring));
            }
        }

        return table;
    }

    /// <summary>
    /// The turnover positions a wheel's notches sit at, given as the letters shown
    /// in the window, e.g. "Q" or the seventeen of a Zählwerk wheel.
    /// </summary>
    public static int[] Notches(string notches, ICharacterMap? characterMap = null)
    {
        var alphabet = characterMap ?? CharacterMap.Latin;

        return (notches ?? string.Empty)
            .Where(character => !char.IsWhiteSpace(character))
            .Select(character => alphabet.GetIndex(char.ToUpperInvariant(character)) is var index && index >= 0
                ? index
                : throw new ArgumentException(
                    $"Notch '{character}' is not in the {alphabet.Name} alphabet.", nameof(notches)))
            .ToArray();
    }

    public static IDictionary<int, int> Invert(IDictionary<int, int> wiring)
    {
        var table = new Dictionary<int, int>(wiring.Count);

        foreach (var (contact, output) in wiring)
        {
            table[output] = contact;
        }

        return table;
    }
}
