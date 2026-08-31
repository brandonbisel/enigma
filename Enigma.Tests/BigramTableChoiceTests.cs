using Enigma.App;

namespace Enigma.Tests;

/// <summary>
/// Which conversion table a day comes to. Both front ends ask this the same way, so
/// it is answered once, here.
/// </summary>
public class BigramTableChoiceTests
{
    [Fact]
    public void AKennzifferAndADayGiveTheTableTheCalendarNames()
    {
        // 1 May 1945, the column pencilled "Mai 45": the day U-534 sent on Tafel A.
        var chosen = BigramTableChoice.From(BigramTableSet.Quelle, kennziffer: 6, monatstag: 1, tafel: 'H');

        Assert.Equal('A', chosen.Letter);
        Assert.Same(BigramTables.QuelleA, chosen.Table);
    }

    [Fact]
    public void TheCalendarOutranksTheLetterGiven()
    {
        // An operator did not choose his table, so a Kennziffer has to win. The
        // 'H' above and here is deliberately not what the plan says.
        Assert.NotEqual('H', BigramTableChoice.From(BigramTableSet.Quelle, 6, 1, 'H').Letter);
    }

    [Fact]
    public void WithoutAKennzifferTheLetterGivenStands()
    {
        var chosen = BigramTableChoice.From(BigramTableSet.Quelle, kennziffer: 0, monatstag: 1, tafel: 'D');

        Assert.Equal('D', chosen.Letter);
        Assert.Same(BigramTables.QuelleD, chosen.Table);
    }

    [Fact]
    public void ALowerCaseLetterIsTheSameTable()
    {
        Assert.Same(BigramTables.QuelleD, BigramTableChoice.From(BigramTableSet.Quelle, 0, 1, 'd').Table);
    }

    [Fact]
    public void ADayThatCallsForTheTableNobodyHasSaysWhichAndWhy()
    {
        // Kennziffer two on the third reads J, and Tafel J is in no surviving
        // source. Returning a substitute would decode to plausible nonsense.
        var chosen = BigramTableChoice.From(BigramTableSet.Quelle, kennziffer: 2, monatstag: 3, tafel: 'A');

        Assert.Equal('J', chosen.Letter);
        Assert.False(chosen.Found);
        Assert.Null(chosen.Table);
        Assert.Contains("Tafel J", chosen.Missing);
    }

    [Fact]
    public void EveryDayOfEveryColumnComesToATableOrSaysItCannot()
    {
        // The plan names nine tables and eight are published, so every cell must
        // either resolve or explain itself. Neither an exception nor a silent
        // substitute is an answer.
        Assert.All(
            from kennziffer in Enumerable.Range(1, Tauschtafelplan.BrunoQuelle.Columns)
            from day in Enumerable.Range(1, 31)
            select BigramTableChoice.From(BigramTableSet.Quelle, kennziffer, day, 'A'),
            chosen =>
            {
                Assert.Contains(chosen.Letter, "ABCDEFGHJ");
                Assert.Equal(chosen.Letter != 'J', chosen.Found);
            });
    }

    [Fact]
    public void OnlyTafelJIsEverMissing()
    {
        // If another letter went missing the tables and the calendar would have
        // drifted apart, and this is where that shows.
        Assert.All(
            from kennziffer in Enumerable.Range(1, Tauschtafelplan.BrunoQuelle.Columns)
            from day in Enumerable.Range(1, 31)
            select BigramTableChoice.From(BigramTableSet.Quelle, kennziffer, day, 'A'),
            chosen => Assert.True(chosen.Found || chosen.Letter == 'J'));
    }

    // -- more than one set -------------------------------------------------------

    [Fact]
    public void TheSameLetterInAnotherSetIsAnotherTable()
    {
        Assert.NotSame(
            BigramTableChoice.From(BigramTableSet.Quelle, 0, 1, 'A').Table,
            BigramTableChoice.From(BigramTableSet.Meer, 0, 1, 'A').Table);
    }

    [Fact]
    public void EachSetIsReadAgainstItsOwnCalendar()
    {
        // The plans are different documents; the same column and day need not name
        // the same table.
        Assert.NotEqual(
            BigramTableChoice.From(BigramTableSet.Quelle, 2, 3, 'A').Letter,
            BigramTableChoice.From(BigramTableSet.Meer, 2, 3, 'A').Letter);
    }

    [Fact]
    public void MeerCanSupplyEveryDayAndQuelleCannot()
    {
        // The difference that matters between a complete set and an incomplete one.
        Assert.True(BigramTableSet.Meer.IsComplete);
        Assert.False(BigramTableSet.Quelle.IsComplete);
    }

    [Fact]
    public void AMissingTableNamesTheSetItIsMissingFrom()
    {
        var missing = BigramTableChoice.From(BigramTableSet.Quelle, 2, 3, 'A').Missing;

        Assert.Contains("Tafel J", missing);
        Assert.Contains("Quelle", missing);
    }

    [Fact]
    public void HowManyTablesASetShouldHaveComesFromItsPlan()
    {
        // Sets are not all one size: Quelle and Meer run to nine tables, Flußlauf to
        // fifteen. A count written into this message would be right for some sets and
        // wrong for others, so it is read off the calendar the set was issued with.
        var missing = BigramTableChoice.From(BigramTableSet.Quelle, 2, 3, 'A').Missing;

        Assert.Contains("9 tables, A to J without I", missing);
        Assert.Contains("holds 8 of them", missing);
    }

    [Fact]
    public void ASetThatNamesFifteenTablesSaysSo()
    {
        // The message reads its counts off the plan, so a fifteen-table set has to be
        // described as one — the phrasing that made Flußlauf's missing tables legible.
        Assert.Equal("A to P without I", Tauschtafelplan.BrunoFlusslauf.TafelRange);
        Assert.Equal(15, Tauschtafelplan.BrunoFlusslauf.Tafeln.Count);
    }
}
