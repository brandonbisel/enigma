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
        var chosen = BigramTableChoice.From(kennziffer: 6, monatstag: 1, tafel: 'H');

        Assert.Equal('A', chosen.Letter);
        Assert.Same(BigramTables.QuelleA, chosen.Table);
    }

    [Fact]
    public void TheCalendarOutranksTheLetterGiven()
    {
        // An operator did not choose his table, so a Kennziffer has to win. The
        // 'H' above and here is deliberately not what the plan says.
        Assert.NotEqual('H', BigramTableChoice.From(6, 1, 'H').Letter);
    }

    [Fact]
    public void WithoutAKennzifferTheLetterGivenStands()
    {
        var chosen = BigramTableChoice.From(kennziffer: 0, monatstag: 1, tafel: 'D');

        Assert.Equal('D', chosen.Letter);
        Assert.Same(BigramTables.QuelleD, chosen.Table);
    }

    [Fact]
    public void ALowerCaseLetterIsTheSameTable()
    {
        Assert.Same(BigramTables.QuelleD, BigramTableChoice.From(0, 1, 'd').Table);
    }

    [Fact]
    public void ADayThatCallsForTheTableNobodyHasSaysWhichAndWhy()
    {
        // Kennziffer two on the third reads J, and Tafel J is in no surviving
        // source. Returning a substitute would decode to plausible nonsense.
        var chosen = BigramTableChoice.From(kennziffer: 2, monatstag: 3, tafel: 'A');

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
            select BigramTableChoice.From(kennziffer, day, 'A'),
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
            select BigramTableChoice.From(kennziffer, day, 'A'),
            chosen => Assert.True(chosen.Found || chosen.Letter == 'J'));
    }
}
