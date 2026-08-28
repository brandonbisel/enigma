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

    private static readonly string[] Qwertz = ["QWERTZUIO", "ASDFGHJK", "PYXCVBNML"];

    /// <summary>The widest a row gets before the keys are wrapped onto the next one.</summary>
    private const int RowWidth = 9;

    /// <summary>
    /// The rows of the panel, each a run of characters from the given alphabet.
    /// </summary>
    public static IReadOnlyList<string> Rows(ICharacterMap alphabet)
    {
        ArgumentNullException.ThrowIfNull(alphabet);

        return IsLatin(alphabet) ? Qwertz : Wrapped(alphabet);
    }

    private static bool IsLatin(ICharacterMap alphabet) =>
        alphabet.Count == Latin.Length &&
        Latin.Select((letter, index) => alphabet.GetCharacter(index) == letter).All(same => same);

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
