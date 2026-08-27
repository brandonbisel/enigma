using System.Text;

namespace Enigma;

/// <summary>
/// The conventions a signaller followed to get a message onto a machine that has
/// only twenty six letters, no space bar and no digits.
/// </summary>
public static class MessageText
{
    private static readonly string[] Numerals =
        ["NULL", "EINS", "ZWO", "DREI", "VIER", "FUENF", "SECHS", "SIEBEN", "ACHT", "NEUN"];

    /// <summary>
    /// Turns ordinary German text into the letters that could actually be keyed:
    /// umlauts expanded, the sharp s doubled, digits spelled out, spaces written
    /// as X, and anything else dropped.
    ///
    /// Radio operators also wrote "ch" as Q, which is why the intercepts read
    /// BEOBAQTET and RIQTUNG. That is not applied here: it is a habit rather than
    /// a rule, it was not universal, and it would quietly rewrite any word
    /// containing those two letters.
    /// </summary>
    public static string Prepare(string text)
    {
        var prepared = new StringBuilder();

        foreach (var character in text ?? string.Empty)
        {
            prepared.Append(Translate(character));
        }

        return prepared.ToString();
    }

    /// <summary>
    /// Breaks a message into the groups it was transmitted in, five letters at a
    /// time by convention, so that a miscount shows up at the receiving end.
    /// </summary>
    public static string InGroups(string text, int size = 5)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        var letters = (text ?? string.Empty).Where(char.IsLetter).ToArray();
        var groups = new List<string>();

        for (var i = 0; i < letters.Length; i += size)
        {
            groups.Add(new string(letters, i, Math.Min(size, letters.Length - i)));
        }

        return string.Join(' ', groups);
    }

    private static string Translate(char character) => char.ToUpperInvariant(character) switch
    {
        'Ä' => "AE",
        'Ö' => "OE",
        'Ü' => "UE",
        'ß' or 'ẞ' => "SS",
        ' ' or '\t' => "X",
        >= 'A' and <= 'Z' => char.ToUpperInvariant(character).ToString(),
        >= '0' and <= '9' => Numerals[character - '0'],
        _ => string.Empty
    };
}
