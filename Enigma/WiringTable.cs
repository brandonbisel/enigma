namespace Enigma;

public static class WiringTable
{
    /// <summary>
    /// Wiring is conventionally written as the sequence of letters each contact
    /// maps to, in contact order: "EKMF..." means A to E, B to K, C to M, D to F.
    /// </summary>
    public static IDictionary<int, int> FromString(string wiring)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(wiring);

        var table = new Dictionary<int, int>(wiring.Length);

        for (var i = 0; i < wiring.Length; i++)
        {
            var letter = wiring[i];

            if (letter is < 'A' or > 'Z')
            {
                throw new ArgumentException(
                    $"Wiring '{wiring}' contains '{letter}'. Wiring is written in capital letters A to Z.",
                    nameof(wiring));
            }

            table[i] = letter - 'A';
        }

        // Every contact must reach exactly one other, or the rotor could not be
        // run backwards and letters would be lost.
        if (table.Values.Distinct().Count() != wiring.Length ||
            table.Values.Any(contact => contact >= wiring.Length))
        {
            throw new ArgumentException(
                $"Wiring '{wiring}' is not a permutation: it must use each of its {wiring.Length} contacts exactly once.",
                nameof(wiring));
        }

        return table;
    }

    /// <summary>
    /// A reflector is wired in pairs, so its table must be its own inverse and may
    /// never connect a letter to itself. Both properties are what make the machine
    /// reciprocal, and the second is the flaw that made it attackable.
    /// </summary>
    public static IDictionary<int, int> FromReflectorString(string wiring)
    {
        var table = FromString(wiring);

        foreach (var (contact, output) in table)
        {
            if (contact == output)
            {
                throw new ArgumentException(
                    $"Reflector wiring '{wiring}' connects contact {contact} to itself.",
                    nameof(wiring));
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
