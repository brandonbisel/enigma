namespace Enigma.Web;

/// <summary>
/// How the keys and lamps are arranged on the panel. This is a fact about the
/// machine's woodwork rather than about its wiring: the rows below are the order
/// the letters sit on an Enigma keyboard, which is also — not by coincidence — the
/// order the commercial machines wired their entry wheel in.
///
/// The arrangement is only known for the twenty six letter machines. An alphabet
/// of any other shape, such as the ten digits of an Enigma Z, is laid out in its
/// own order instead, which is the arrangement those machines actually used.
/// </summary>
public static class KeyboardLayout
{
    private const string Latin = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";

    private static readonly string[] Qwertz = ["QWERTZUIO", "ASDFGHJK", "PYXCVBNML"];

    /// <summary>
    /// The Enigma Z's keyboard: a single row of ten figures, reading 1 to 0 rather
    /// than 0 to 9. Where a key sits is not the same question as where a contact
    /// sits, so this changes nothing about the cipher.
    /// </summary>
    private static readonly string[] Figures = ["1234567890"];

    /// <summary>The widest a row gets before the keys are wrapped onto the next one.</summary>
    private const int RowWidth = 9;

    /// <summary>
    /// The rows of the panel, each a run of characters from the given alphabet.
    /// </summary>
    public static IReadOnlyList<string> Rows(ICharacterMap alphabet)
    {
        ArgumentNullException.ThrowIfNull(alphabet);

        if (Is(alphabet, Latin))
        {
            return Qwertz;
        }

        return Is(alphabet, Digits) ? Figures : Wrapped(alphabet);
    }

    private static bool Is(ICharacterMap alphabet, string characters) =>
        alphabet.Count == characters.Length &&
        characters.Select((character, index) => alphabet.GetCharacter(index) == character).All(same => same);

    private static string[] Wrapped(ICharacterMap alphabet)
    {
        var characters = string.Concat(
            Enumerable.Range(0, alphabet.Count).Select(alphabet.GetCharacter));

        return Enumerable
            .Range(0, (characters.Length + RowWidth - 1) / RowWidth)
            .Select(row => characters.Substring(
                row * RowWidth, Math.Min(RowWidth, characters.Length - row * RowWidth)))
            .ToArray();
    }
}
