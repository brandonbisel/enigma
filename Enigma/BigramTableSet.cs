namespace Enigma;

/// <summary>
/// A set of Doppelbuchstabentauschtafeln and the calendar issued with it.
///
/// The two travel together and are useless apart. A table without a plan is a
/// substitution table with nothing to say which day it belongs to; a plan without
/// its tables names letters that cannot be supplied. Pairing them here is what lets
/// a front end offer "which set are you on" as one choice rather than two that can
/// be mismatched.
/// </summary>
public sealed class BigramTableSet
{
    private BigramTableSet(
        string name,
        string serial,
        IReadOnlyDictionary<char, BigramTable> tables,
        Tauschtafelplan plan)
    {
        Name = name;
        Serial = serial;
        Tables = tables;
        Plan = plan;
    }

    /// <summary>The Kennwort the booklet was known by.</summary>
    public string Name { get; }

    /// <summary>The Prüf-Nummer of the copy transcribed.</summary>
    public string Serial { get; }

    public IReadOnlyDictionary<char, BigramTable> Tables { get; }

    public Tauschtafelplan Plan { get; }

    /// <summary>
    /// Whether every table the calendar can name is actually here. A set that is not
    /// complete still works for the days it covers, and says so for the days it does
    /// not — which is the honest behaviour, but worth being able to ask about.
    /// </summary>
    public bool IsComplete =>
        Enumerable
            .Range(1, Plan.Columns)
            .SelectMany(_ => Enumerable.Range(1, 31), (kennziffer, day) => (kennziffer, day))
            .All(cell => Tables.ContainsKey(Plan.Tafel(cell.kennziffer, cell.day)));

    /// <summary>
    /// "Quelle", booklet Prüf-Nr. 2499. Eight of its nine tables: the scan stops at
    /// Tafel H, and only the front of its plan was photographed, so this set covers
    /// six Kennziffer columns rather than twelve and has days it cannot serve. It is
    /// the set U-534 was using, which is why it is the default.
    /// </summary>
    public static BigramTableSet Quelle { get; } = new(
        "Quelle", "Prüf-Nr. 2499", BigramTables.Quelle, Tauschtafelplan.BrunoQuelle);

    /// <summary>
    /// "Meer", booklet Prüf-Nr. 3733. All nine tables and both sides of its plan, so
    /// every day of every column leads to a table that is here.
    /// </summary>
    public static BigramTableSet Meer { get; } = new(
        "Meer", "Prüf-Nr. 3733", BigramTables.Meer, Tauschtafelplan.BrunoMeer);

    /// <summary>The sets that ship, by name.</summary>
    public static IReadOnlyDictionary<string, BigramTableSet> All { get; } =
        new Dictionary<string, BigramTableSet>(StringComparer.OrdinalIgnoreCase)
        {
            ["Quelle"] = Quelle,
            ["Meer"] = Meer,
        };

    public override string ToString() => Name;
}
