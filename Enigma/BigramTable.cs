namespace Enigma;

/// <summary>
/// A Doppelbuchstabentauschtafel: the double-letter conversion table the
/// Kriegsmarine laid over its message indicators, so that what a listener heard bore
/// no relation to the trigrams the operator had chosen.
///
/// The table is reciprocal. If AK is written as BD then BD is written as AK, which
/// is what lets the receiving station use the same table without reversing it — and
/// which means a table is an involution on pairs of letters, exactly as a reflector
/// is on single ones.
///
/// A set ran to nine tables and a calendar said which to use. None ships here: the
/// recovered tables survive as photographs of the originals rather than as anything
/// machine-readable, and six hundred and seventy-six entries transcribed by eye is
/// precisely the sort of cipher data this library does not invent. Supply one and it
/// will be used.
/// </summary>
public sealed class BigramTable
{
    private readonly Dictionary<string, string> _table;

    private BigramTable(Dictionary<string, string> table, ICharacterMap alphabet)
    {
        _table = table;
        Alphabet = alphabet;
    }

    public ICharacterMap Alphabet { get; }

    /// <summary>How many bigrams the table gives a substitution for.</summary>
    public int Count => _table.Count;

    /// <summary>
    /// Whether every bigram in the alphabet is covered, as a real table's would be.
    /// A partial table is useful for study and for tests, but no station could send
    /// with one.
    /// </summary>
    public bool IsComplete => Count == Alphabet.Count * Alphabet.Count;

    /// <summary>
    /// Reads a table written as substitutions, "AK=BD HQ=BJ", separated by
    /// whitespace or commas. Each entry implies its own reverse, so a table may be
    /// written once through; writing both ways is allowed as long as they agree.
    /// </summary>
    public static BigramTable Parse(string entries, ICharacterMap? characterMap = null)
    {
        var alphabet = characterMap ?? CharacterMap.Latin;
        var table = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var entry in Split(entries))
        {
            var sides = entry.Split(['=', ':'], StringSplitOptions.TrimEntries);

            if (sides.Length != 2)
            {
                throw new FormatException(
                    $"Bigram entry '{entry}' is not a substitution such as \"AK=BD\".");
            }

            Add(table, alphabet, Bigram(sides[0], alphabet), Bigram(sides[1], alphabet));
        }

        return new BigramTable(table, alphabet);
    }

    /// <summary>
    /// Substitutes one bigram. The same call serves both stations, the table being
    /// its own inverse.
    /// </summary>
    public string Substitute(string bigram)
    {
        var normalised = Bigram(bigram, Alphabet);

        return _table.TryGetValue(normalised, out var substituted)
            ? substituted
            : throw new ArgumentException(
                $"The table has no entry for the bigram '{normalised}'.", nameof(bigram));
    }

    private static void Add(
        Dictionary<string, string> table, ICharacterMap alphabet, string from, string to)
    {
        // Reciprocity is what makes one table serve both ends, so it is built in
        // rather than trusted to whoever wrote the file out.
        Assign(table, from, to);
        Assign(table, to, from);
    }

    private static void Assign(Dictionary<string, string> table, string from, string to)
    {
        if (table.TryGetValue(from, out var existing) && existing != to)
        {
            throw new FormatException(
                $"Bigram '{from}' is given as both '{existing}' and '{to}'.");
        }

        table[from] = to;
    }

    private static string Bigram(string value, ICharacterMap alphabet)
    {
        var trimmed = (value ?? string.Empty).Trim().ToUpperInvariant();

        if (trimmed.Length != 2 || trimmed.Any(character => alphabet.GetIndex(character) < 0))
        {
            throw new FormatException(
                $"'{value}' is not a pair of characters from the {alphabet.Name} alphabet.");
        }

        return trimmed;
    }

    private static IEnumerable<string> Split(string? value) =>
        (value ?? string.Empty).Split(
            [' ', '\t', '\r', '\n', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
