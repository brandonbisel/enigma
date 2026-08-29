namespace Enigma.App;

/// <summary>
/// Which Doppelbuchstabentauschtafel is in force, and whether it is one that
/// survives.
///
/// An operator did not choose. He took the Kennziffer column his cipher net was
/// allotted, read down the Tauschtafelplan to the day of the month, and used the
/// table it named. Naming a table outright is the concession to anyone working
/// without a calendar, and the calendar wins whenever it is given.
///
/// A set can legitimately call for a table that does not survive — "Quelle" is eight
/// tables of nine. That is an answer rather than a fault, and both front ends have to
/// be able to say it, which is why it is decided here and not in either of them.
/// </summary>
public static class BigramTableChoice
{
    /// <param name="set">Which booklet is in use.</param>
    /// <param name="kennziffer">The Tauschtafelplan column, or zero to take the letter as given.</param>
    /// <param name="monatstag">The day of the month, when a Kennziffer is given.</param>
    /// <param name="tafel">The table to use when no Kennziffer is.</param>
    public static TableChoice From(BigramTableSet set, int kennziffer, int monatstag, char tafel)
    {
        ArgumentNullException.ThrowIfNull(set);

        var letter = char.ToUpperInvariant(
            kennziffer > 0 ? set.Plan.Tafel(kennziffer, monatstag) : tafel);

        return new TableChoice(
            set,
            letter,
            set.Tables.TryGetValue(letter, out var table) ? table : null);
    }
}

/// <summary>The table a day or a choice comes to, and whether it is published.</summary>
/// <param name="Set">The booklet it was looked up in.</param>
/// <param name="Letter">The table named, whether or not it survives.</param>
/// <param name="Table">The table itself, or null if that letter is not published.</param>
public sealed record TableChoice(BigramTableSet Set, char Letter, BigramTable? Table)
{
    public bool Found => Table is not null;

    /// <summary>Why there is no table, in words fit to show an operator.</summary>
    ///
    /// <remarks>
    /// The count comes from the set's own Tauschtafelplan rather than a constant. Sets
    /// were not all the same size — Quelle and Meer run to nine tables, Flußlauf to
    /// fifteen — so a number written in here would be right for some sets and a lie
    /// for others.
    /// </remarks>
    public string Missing =>
        $"Tafel {Letter} of \"{Set.Name}\" is not published. Its Tauschtafelplan names " +
        $"{Set.Plan.Tafeln.Count} tables, {Set.Plan.TafelRange}, and the scan that " +
        $"survives holds {Set.Tables.Count} of them.";
}
