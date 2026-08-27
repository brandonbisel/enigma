namespace Enigma;

/// <summary>
/// The two letter group that carried an Enigma Uhr's dial setting.
///
/// The setting is a number from 00 to 39, and it travelled as a pair of letters
/// read off the two plates inside the lid of the Uhr's box. "Alphabet I" divides
/// the twenty six letters into four bands giving the tens digit, and "Alphabet II"
/// into ten bands giving the units. Four times ten is forty, one for each position
/// of the dial.
///
/// The bands are uneven, so a digit has several letters that stand for it and an
/// operator could pick any of them. That means the encoding is one way only in the
/// strict sense: every group decodes to one setting, but a setting has several
/// groups that represent it. <see cref="Encode"/> returns the first of each band.
///
/// These are the letters stamped on the plate, so this is the Latin alphabet by
/// construction and does not vary with a machine's character map.
/// </summary>
public static class UhrSetting
{
    public const int Positions = 40;

    /// <summary>Alphabet I: the tens digit, 0 to 3.</summary>
    private static readonly string[] Tens =
        ["ABCDEF", "GHIJKLM", "NOPQRS", "TUVWXYZ"];

    /// <summary>Alphabet II: the units digit, 0 to 9.</summary>
    private static readonly string[] Units =
        ["ABC", "DE", "FGH", "IJ", "KLM", "NO", "PQR", "ST", "UVW", "XYZ"];

    /// <summary>The letter group that stands for a dial setting.</summary>
    public static string Encode(int position)
    {
        if (position is < 0 or >= Positions)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position), position,
                $"The Uhr dial has {Positions} positions, numbered 0 to {Positions - 1}.");
        }

        return $"{Tens[position / 10][0]}{Units[position % 10][0]}";
    }

    /// <summary>The dial setting a letter group stands for.</summary>
    public static int Decode(string group)
    {
        var letters = new string((group ?? string.Empty)
            .Where(character => !char.IsWhiteSpace(character))
            .Select(char.ToUpperInvariant)
            .ToArray());

        if (letters.Length != 2)
        {
            throw new ArgumentException(
                $"An Uhr setting is two letters, but '{group}' has {letters.Length}.", nameof(group));
        }

        var tens = Band(Tens, letters[0], "Alphabet I", group);
        var units = Band(Units, letters[1], "Alphabet II", group);

        return tens * 10 + units;
    }

    /// <summary>Every letter group that stands for a dial setting, an operator's choice.</summary>
    public static IEnumerable<string> AllGroupsFor(int position)
    {
        if (position is < 0 or >= Positions)
        {
            throw new ArgumentOutOfRangeException(
                nameof(position), position,
                $"The Uhr dial has {Positions} positions, numbered 0 to {Positions - 1}.");
        }

        return
            from tens in Tens[position / 10]
            from units in Units[position % 10]
            select $"{tens}{units}";
    }

    private static int Band(IReadOnlyList<string> bands, char letter, string plate, string? group)
    {
        for (var band = 0; band < bands.Count; band++)
        {
            if (bands[band].Contains(letter))
            {
                return band;
            }
        }

        throw new ArgumentException(
            $"'{letter}' in '{group}' is not on the Uhr's {plate} plate.", nameof(group));
    }
}
