using Enigma.App;

namespace Enigma.Cmd;

/// <summary>
/// The naval side of the command line, read and checked before anything is built.
///
/// A table is what marks the procedure as naval: the Navy's indicator cannot be
/// worked without one and the Army's never wants one, so the operator never has to
/// say which service he means. Everything else follows from that, including the
/// refusals — most of the ways of getting this wrong are combinations that would
/// otherwise be silently half-obeyed.
/// </summary>
public sealed record NavalArguments
{
    /// <summary>The table in force, or null when this is not the naval procedure.</summary>
    public BigramTable? Table { get; private init; }

    public string? KeyGroup { get; private init; }

    public string? MessageGroup { get; private init; }

    public char FirstFiller { get; private init; } = 'X';

    public char LastFiller { get; private init; } = 'X';

    /// <summary>
    /// Something the operator should be told but which is not the message — what the
    /// calendar said, when it was the calendar that chose.
    /// </summary>
    public string? Note { get; private init; }

    /// <summary>Why the command line will not do, in words fit to print.</summary>
    public string? Error { get; private init; }

    public bool Failed => Error is not null;

    /// <summary>Whether the naval procedure was asked for at all.</summary>
    public bool Wanted => Table is not null;

    public static NavalArguments Read(
        string? tafel,
        int? kennziffer,
        int? monatstag,
        string? kenngruppen,
        string? fillers,
        string? messageKey,
        string? indicator,
        bool doubled)
    {
        var columns = Tauschtafelplan.BrunoQuelle.Columns;
        var wanted = tafel is not null || kennziffer is not null;

        if (tafel is not null && kennziffer is not null)
        {
            return Refused("Give either --tafel to name a table or --kennziffer to look one up, not both.");
        }

        if (kennziffer is { } column && (column < 1 || column > columns))
        {
            return Refused(
                $"--kennziffer is a column of the Tauschtafelplan, 1 to {columns}.");
        }

        if (kennziffer is not null && monatstag is null)
        {
            return Refused(
                "--kennziffer names a column; --monatstag is needed to say which day to read it at.");
        }

        if (kennziffer is not null && monatstag is < 1 or > 31)
        {
            return Refused("--monatstag is a day of the month, 1 to 31.");
        }

        if (kenngruppen is not null && !wanted)
        {
            return Refused(
                "--kenngruppen is the naval procedure, which needs a table: give --tafel or --kennziffer.");
        }

        if (kenngruppen is not null && (messageKey is not null || indicator is not null))
        {
            return Refused(
                "Give --kenngruppen to send the naval way or --indicator to read one, not both.");
        }

        if (wanted && kenngruppen is null && indicator is null)
        {
            return Refused(
                "A table was given but nothing to work with it: add --kenngruppen to send or --indicator to receive.");
        }

        if (wanted && doubled)
        {
            return Refused(
                "--doubled is the Army procedure before 1938; the naval one never sent a key twice.");
        }

        if (fillers is not null && !wanted)
        {
            return Refused("--fillers only means anything with the naval procedure.");
        }

        if (!wanted)
        {
            return new NavalArguments();
        }

        if (tafel is not null && Letters(tafel).Length != 1)
        {
            return Refused($"--tafel is one letter, but '{tafel}' is not.");
        }

        var chosen = BigramTableChoice.From(
            kennziffer ?? 0, monatstag ?? 1, tafel is { Length: > 0 } named ? Letters(named)[0] : 'A');

        if (!chosen.Found)
        {
            return Refused(chosen.Missing);
        }

        var padding = "XX";

        if (fillers is not null)
        {
            if (Letters(fillers) is not { Length: 2 } pair)
            {
                return Refused($"--fillers is two padding letters, but '{fillers}' is not.");
            }

            padding = pair;
        }

        string? keyGroup = null;
        string? messageGroup = null;

        if (kenngruppen is not null)
        {
            if (Letters(kenngruppen) is not { Length: 6 } trigrams)
            {
                return Refused(
                    "--kenngruppen is two trigrams, six letters: the Schlüsselkenngruppe then the Verfahrenkenngruppe.");
            }

            (keyGroup, messageGroup) = (trigrams[..3], trigrams[3..]);
        }

        return new NavalArguments
        {
            Table = chosen.Table,
            KeyGroup = keyGroup,
            MessageGroup = messageGroup,
            FirstFiller = padding[0],
            LastFiller = padding[1],

            // Worth saying only when the operator did not pick the table himself.
            Note = kennziffer is null
                ? null
                : $"Tauschtafelplan Bruno, Kennziffer {kennziffer}, Monatstag {monatstag}: " +
                  $"Tafel {chosen.Letter}"
        };
    }

    private static NavalArguments Refused(string error) => new() { Error = error };

    // Operators wrote indicators in groups; the machine only cares about the letters.
    private static string Letters(string value) =>
        new(value.Where(char.IsLetter).Select(char.ToUpperInvariant).ToArray());
}
