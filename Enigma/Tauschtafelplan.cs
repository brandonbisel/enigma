namespace Enigma;

/// <summary>
/// The Tauschtafelplan: the calendar issued with a set of
/// Doppelbuchstabentauschtafeln saying which of its lettered tables applies on which
/// day of the month.
///
/// Without it the tables are only substitution tables. The plan is what turns them
/// into a key: the operator took his Kennziffer column from the Zuteilungsliste,
/// according to which cipher net he was on, read down to the day of the month, and
/// found the letter of the table to use. The Kennziffer is therefore an input here
/// rather than something this class can derive.
/// </summary>
public sealed class Tauschtafelplan
{
    private readonly char[,] _printed;
    private readonly char[,] _effective;

    private Tauschtafelplan(string name, char[,] printed, char[,] effective)
    {
        Name = name;
        _printed = printed;
        _effective = effective;
    }

    public string Name { get; }

    /// <summary>How many Kennziffer columns this plan carries.</summary>
    public int Columns => _printed.GetLength(1);

    /// <summary>
    /// The table letter in force, with the sheet's pen corrections applied. This is
    /// what an operator holding the corrected sheet would have used.
    /// </summary>
    public char Tafel(int kennziffer, int dayOfMonth) =>
        Cell(_effective, kennziffer, dayOfMonth);

    /// <summary>
    /// The table letter as originally printed, before the corrections. Kept separate
    /// because the two layers are different evidence: the print is the issued
    /// edition, the pen is one station's amendment to it.
    /// </summary>
    public char AsPrinted(int kennziffer, int dayOfMonth) =>
        Cell(_printed, kennziffer, dayOfMonth);

    private char Cell(char[,] grid, int kennziffer, int dayOfMonth)
    {
        if (kennziffer < 1 || kennziffer > Columns)
        {
            throw new ArgumentOutOfRangeException(
                nameof(kennziffer), kennziffer,
                $"'{Name}' carries Kennziffer columns 1 to {Columns}.");
        }

        if (dayOfMonth < 1 || dayOfMonth > grid.GetLength(0))
        {
            throw new ArgumentOutOfRangeException(
                nameof(dayOfMonth), dayOfMonth, "A Monatstag runs from 1 to 31.");
        }

        return grid[dayOfMonth - 1, kennziffer - 1];
    }

    /// <summary>
    /// Tauschtafelplan "Bruno" for the set Kennwort "Quelle", Prüfnr. 1772a,
    /// transcribed from the photograph of the original published by Michael
    /// Hörenberg at enigma.hoerenberg.com.
    ///
    /// Two caveats travel with it. Only the six Kennziffer columns printed on the
    /// front are here; the sheet is footed "Fortsetzung Rückseite!" and the reverse
    /// is not in the photograph. And the copy photographed is Prüfnr. 1772a where
    /// the tables this library ships are from booklet 2499 — the same edition and so
    /// the same printed plan, but a different physical copy, and the pen corrections
    /// belong to whoever held 1772.
    ///
    /// Those corrections strike out every printed C and H in columns one, four and
    /// six and write a replacement over each, consistently per column. After them
    /// those three columns use only A B D E F G J. C and H are gone from them
    /// entirely, while the columns left uncorrected still contain both.
    ///
    /// It is checkable against real traffic at one point, and it holds: U-534's
    /// message P1030690 of 1 May 1945 was sent on Tafel A, and column six — the one
    /// pencilled "Mai 45" — reads A on the first of the month.
    /// </summary>
    public static Tauschtafelplan BrunoQuelle { get; } = Parse("Bruno / Quelle", Printed, Corrections);

    /// <summary>
    /// Tauschtafelplan "Bruno" for the set Kennwort "Meer", Prüfnr. 3733a, from the
    /// Crypto Museum's scan of that booklet.
    ///
    /// A better sheet than <see cref="BrunoQuelle"/> in three ways. It is clean print
    /// with no pen corrections, so there is only one layer to read. Both sides are
    /// reproduced, which is what shows that a full plan carries **twelve** Kennziffer
    /// columns — the Quelle photograph stops at six and is footed "Fortsetzung
    /// Rückseite!", and this is what was on the other side of it.
    ///
    /// And it is the only cipher data here that has been checked against a second
    /// physical copy. A calendar has no involution to catch a mistyped cell, so this
    /// grid was read a second time from the Crypto Museum's scan of Prüf-Nr. 4002 — a
    /// different booklet of the same edition, separately photographed — and all 372
    /// cells agreed. That verifies the transcription rather than the edition: an error
    /// in the printing itself would appear in both copies, and would anyway be what
    /// the operators worked from.
    /// </summary>
    public static Tauschtafelplan BrunoMeer { get; } =
        Parse("Bruno / Meer", MeerPrinted, string.Empty);

    private static Tauschtafelplan Parse(string name, string printed, string corrections)
    {
        var rows = printed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var grid = new char[rows.Length, rows[0].Length];

        for (var day = 0; day < rows.Length; day++)
        {
            if (rows[day].Length != rows[0].Length)
            {
                throw new FormatException(
                    $"Monatstag {day + 1} has {rows[day].Length} columns, not {rows[0].Length}.");
            }

            for (var column = 0; column < rows[day].Length; column++)
            {
                grid[day, column] = rows[day][column];
            }
        }

        var amended = (char[,])grid.Clone();

        // Each correction says what was printed as well as what was written over it,
        // so a slip in either transcription fails here rather than silently standing.
        foreach (var correction in corrections.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = correction.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var day = int.Parse(parts[0]);
            var kennziffer = int.Parse(parts[1]);

            if (grid[day - 1, kennziffer - 1] != parts[2][0])
            {
                throw new FormatException(
                    $"Monatstag {day}, Kennziffer {kennziffer} is printed " +
                    $"{grid[day - 1, kennziffer - 1]}, but the correction says {parts[2]}.");
            }

            amended[day - 1, kennziffer - 1] = parts[3][0];
        }

        return new Tauschtafelplan(name, grid, amended);
    }

    // Rows are Monatstag 1 to 31, characters are Kennziffer one to six.
    private const string Printed =
        "JDBCFA AGDJCF GJEBAC DEAHFB BCFGDA EFGBHD HDBFEJ CAHGBD FHCEGA HEJABF " +
        "CJDHAB DAECHF JGAEBH GCHDAF AEBJFG EBFHAJ BJCAFH FDGBJC BAFJED EHGBDA " +
        "JFEGAH FCABGD HGJDAE AEBCHG DACHEJ GFHABE CDFEHA ABEDJH FGDJEB HFABJD " +
        "BHFGAC";

    // Monatstag, Kennziffer, the letter printed, the letter written over it.
    private const string Corrections =
        "1 4 C A, 3 6 C G, 4 4 H D, 7 1 H G, 8 1 C E, 10 1 H G, 11 1 C E, " +
        "11 4 H D, 12 4 C A, 13 6 H B, 16 4 H D, 17 6 H B, 18 6 C G, 21 6 H B, " +
        "23 1 H G, 24 4 C A, 25 4 H D, 27 1 C E, 28 6 H B, 30 1 H G, 31 6 C G";

    // Rows are Monatstag 1 to 31, characters are Kennziffer one to twelve.
    private const string MeerPrinted =
        "ADCGEHGAEHJD EBFDAGHGAFCE GDHJEBCDHBGJ JADCBEFJCDEA BCGEJADFHJBG " +
        "DHJGFCBHDEFC HCBEDFECFGDH CAEBGJHJBCGF FJADCEGCJEAD CGEFHBJFACEG " +
        "FBJHAGADGHJC DGBAJCDHEACF GBFCDHABGDHJ ECAJBDGEHFCB BEGDHAEGBJFD " +
        "HDCGFBJAEBDH AJHBGECFDGEA JFDECAHDAFJE CHAJEGBHJCAF AFGCBDFACHGJ " +
        "BJDFGHAEFDBC GDFHACFBAGHE JEHADFJGCBAH DBCFEJBJHEDA FAEDHBGCEJHB " +
        "HEJBFDCDJAEG EHBJCFEBFHJD HGFCJAHGDBCE BEAHFCDJBFGA FCHABJBCGAFH " +
        "AFBGDEEFADBC";
}
