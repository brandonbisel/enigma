namespace Enigma;

public static class WiringTable
{
    // Wiring is conventionally written as the sequence of letters each contact
    // maps to, in contact order: "EKMF..." means A->E, B->K, C->M, D->F.
    public static IDictionary<int, int> FromString(string wiring)
    {
        var table = new Dictionary<int, int>(wiring.Length);

        for (var i = 0; i < wiring.Length; i++)
        {
            table[i] = wiring[i] - 'A';
        }

        if (table.Values.Distinct().Count() != wiring.Length)
        {
            throw new ArgumentException($"Wiring '{wiring}' does not map each contact to a distinct output.", nameof(wiring));
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
