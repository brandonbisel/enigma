using Enigma.Cmd;

namespace Enigma.Cmd.Tests;

/// <summary>
/// What the command line will and will not accept for the naval procedure.
///
/// Most of these are refusals, which is the point. Every one of them is a
/// combination that could otherwise be half-obeyed — a padding letter quietly
/// ignored, a table quietly substituted — and half-obeying a cipher setting
/// produces plausible nonsense rather than an error.
/// </summary>
public class NavalArgumentsTests
{
    private static NavalArguments Read(
        string? setName = null,
        string? tafel = null,
        int? kennziffer = null,
        int? monatstag = null,
        string? kenngruppen = null,
        string? fillers = null,
        string? messageKey = null,
        string? indicator = null,
        bool doubled = false) =>
        NavalArguments.Read(
            setName, tafel, kennziffer, monatstag, kenngruppen, fillers, messageKey, indicator, doubled);

    // -- not the naval procedure at all -----------------------------------------

    [Fact]
    public void NothingNavalOnTheLineIsNotTheNavalProcedure()
    {
        var read = Read(messageKey: "RTZ");

        Assert.False(read.Failed);
        Assert.False(read.Wanted);
        Assert.Null(read.Table);
    }

    [Fact]
    public void TheArmyProcedureIsLeftAlone()
    {
        // --doubled and --indicator together are the Army's business, and the
        // naval reader must not object to them.
        Assert.False(Read(indicator: "VZAVZA", doubled: true).Failed);
    }

    // -- choosing the table -----------------------------------------------------

    [Fact]
    public void ATafelNamesATable()
    {
        var read = Read(tafel: "A", indicator: "FNHCGVET");

        Assert.False(read.Failed);
        Assert.Same(BigramTables.QuelleA, read.Table);

        // Nothing to report: the operator chose it himself.
        Assert.Null(read.Note);
    }

    [Fact]
    public void ALowerCaseTafelIsTheSameTable()
    {
        Assert.Same(BigramTables.QuelleA, Read(tafel: "a", indicator: "FNHCGVET").Table);
    }

    [Fact]
    public void AKennzifferAndADayReadTheTableOffTheCalendar()
    {
        // 1 May 1945: the day U-534 sent on Tafel A.
        var read = Read(kennziffer: 6, monatstag: 1, indicator: "FNHCGVET");

        Assert.False(read.Failed);
        Assert.Same(BigramTables.QuelleA, read.Table);
        Assert.Contains("Tafel A", read.Note);
    }

    [Fact]
    public void TheCalendarSaysSoOutLoud()
    {
        // The operator did not pick this table, so he is told which one he got.
        var note = Read(kennziffer: 6, monatstag: 1, indicator: "FNHCGVET").Note;

        Assert.Contains("Kennziffer 6", note);
        Assert.Contains("Monatstag 1", note);
    }

    [Fact]
    public void ADayThatCallsForTheTableNobodyHasIsRefused()
    {
        Assert.Contains("Tafel J", Read(kennziffer: 2, monatstag: 3, indicator: "FNHCGVET").Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void AKennzifferOutsideThePlanIsRefused(int kennziffer)
    {
        Assert.Contains("--kennziffer", Read(kennziffer: kennziffer, monatstag: 1).Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void ADayOutsideTheMonthIsRefused(int monatstag)
    {
        Assert.Contains("--monatstag", Read(kennziffer: 1, monatstag: monatstag).Error);
    }

    [Fact]
    public void AKennzifferWithoutADayIsRefused()
    {
        // Defaulting the day would silently pick a table off the wrong row.
        Assert.Contains("--monatstag", Read(kennziffer: 6, indicator: "FNHCGVET").Error);
    }

    [Fact]
    public void ATafelAndAKennzifferTogetherAreRefused()
    {
        var read = Read(tafel: "A", kennziffer: 6, monatstag: 1, indicator: "FNHCGVET");

        Assert.Contains("not both", read.Error);
    }

    [Fact]
    public void ATafelThatIsNotOneLetterIsRefused()
    {
        Assert.Contains("--tafel", Read(tafel: "AB", indicator: "FNHCGVET").Error);
    }

    // -- what to do with it -----------------------------------------------------

    [Fact]
    public void KenngruppenAreSplitIntoTwoTrigrams()
    {
        var read = Read(tafel: "A", kenngruppen: "DUZ YMU");

        Assert.False(read.Failed);
        Assert.Equal("DUZ", read.KeyGroup);
        Assert.Equal("YMU", read.MessageGroup);
    }

    [Theory]
    [InlineData("DUZYMU")]
    [InlineData("DUZ YMU")]
    [InlineData("duz ymu")]
    [InlineData("DUZ-YMU")]
    public void TheTrigramsMayBeWrittenHowever(string written)
    {
        var read = Read(tafel: "A", kenngruppen: written);

        Assert.Equal("DUZ", read.KeyGroup);
        Assert.Equal("YMU", read.MessageGroup);
    }

    [Theory]
    [InlineData("DUZ")]
    [InlineData("DUZYM")]
    [InlineData("DUZYMUX")]
    public void KenngruppenThatAreNotSixLettersAreRefused(string written)
    {
        Assert.Contains("six letters", Read(tafel: "A", kenngruppen: written).Error);
    }

    [Fact]
    public void KenngruppenWithoutATableAreRefused()
    {
        // The procedure cannot run without one, so this would fail later anyway.
        Assert.Contains("needs a table", Read(kenngruppen: "DUZYMU").Error);
    }

    [Fact]
    public void ATableWithNothingToWorkOnIsRefused()
    {
        Assert.Contains("nothing to work with it", Read(tafel: "A").Error);
    }

    [Theory]
    [InlineData("RTZ", null)]
    [InlineData(null, "FNHCGVET")]
    public void SendingAndReceivingAtOnceIsRefused(string? messageKey, string? indicator)
    {
        var read = Read(
            tafel: "A", kenngruppen: "DUZYMU", messageKey: messageKey, indicator: indicator);

        Assert.Contains("not both", read.Error);
    }

    // -- the padding ------------------------------------------------------------

    [Fact]
    public void FillersDefaultToX()
    {
        var read = Read(tafel: "A", kenngruppen: "DUZYMU");

        Assert.Equal('X', read.FirstFiller);
        Assert.Equal('X', read.LastFiller);
    }

    [Fact]
    public void FillersAreTakenInOrder()
    {
        // K pads the front and Z the back, which is what makes U-534's indicator
        // come out as it was sent rather than merely round-trip.
        var read = Read(tafel: "A", kenngruppen: "DUZYMU", fillers: "kz");

        Assert.Equal('K', read.FirstFiller);
        Assert.Equal('Z', read.LastFiller);
    }

    [Theory]
    [InlineData("K")]
    [InlineData("KZX")]
    public void FillersThatAreNotTwoLettersAreRefused(string written)
    {
        Assert.Contains("--fillers", Read(tafel: "A", kenngruppen: "DUZYMU", fillers: written).Error);
    }

    [Fact]
    public void FillersWithoutTheNavalProcedureAreRefused()
    {
        // Rather than accepting a setting that would do nothing.
        Assert.Contains("--fillers", Read(messageKey: "RTZ", fillers: "KZ").Error);
    }

    // -- combinations that belong to the other service --------------------------

    [Fact]
    public void DoublingWithTheNavalProcedureIsRefused()
    {
        var read = Read(tafel: "A", indicator: "FNHCGVET", doubled: true);

        Assert.Contains("never sent a key twice", read.Error);
    }

    [Fact]
    public void ARefusalCarriesNoSettings()
    {
        // A half-built set of options is worse than none: nothing downstream
        // should be able to use one.
        var read = Read(tafel: "A", kenngruppen: "DUZ");

        Assert.True(read.Failed);
        Assert.False(read.Wanted);
        Assert.Null(read.Table);
        Assert.Null(read.KeyGroup);
    }

    // -- more than one set -------------------------------------------------------

    [Fact]
    public void TheSetDefaultsToQuelle()
    {
        // The set U-534 was on, and the one the pinned message needs.
        Assert.Same(BigramTables.QuelleA, Read(tafel: "A", indicator: "FNHCGVET").Table);
    }

    [Fact]
    public void AnotherSetGivesAnotherTable()
    {
        Assert.Same(
            BigramTables.MeerA, Read(setName: "Meer", tafel: "A", indicator: "FNHCGVET").Table);
    }

    [Fact]
    public void TheSetNameIsNotCaseSensitive()
    {
        Assert.Same(
            BigramTables.MeerA, Read(setName: "meer", tafel: "A", indicator: "FNHCGVET").Table);
    }

    [Fact]
    public void AnUnknownSetIsRefusedAndListsTheKnownOnes()
    {
        // "Aegir" is a Kriegsmarine cipher-net name, not one of the booklets here.
        var read = Read(setName: "Aegir", tafel: "A", indicator: "FNHCGVET");

        Assert.Contains("Unknown set", read.Error);
        Assert.Contains("Quelle", read.Error);
        Assert.Contains("Meer", read.Error);
        Assert.Contains("Flusslauf", read.Error);
    }

    [Fact]
    public void TheFlusslaufSetIsOnOffer()
    {
        var read = Read(setName: "Flusslauf", tafel: "A", indicator: "FNHCGVET");

        Assert.False(read.Failed);
        Assert.Same(BigramTables.FlusslaufA, read.Table);
    }

    [Fact]
    public void AFlusslaufDayWithoutItsTableIsRefusedRatherThanSubstituted()
    {
        // Seven of its fifteen tables are not transcribed yet. A day that wants one
        // has to say so: quietly handing back Tafel A would decrypt to nonsense.
        var read = Read(setName: "Flusslauf", kennziffer: 1, monatstag: 2, indicator: "FNHCGVET");

        Assert.Contains("Tafel L", read.Error);
        Assert.Contains("15 tables, A to P without I", read.Error);
    }

    [Fact]
    public void NamingASetIsEnoughToMeanTheNavalProcedure()
    {
        // Without a --tafel or --kennziffer, --set alone still says which procedure
        // is meant, so the refusal is about the missing work and not the set.
        Assert.Contains("nothing to work with it", Read(setName: "Meer").Error);
    }

    [Fact]
    public void MeersCalendarReachesColumnsQuellesDoesNot()
    {
        // Quelle has six Kennziffer columns photographed, Meer twelve.
        Assert.Contains("--kennziffer", Read(kennziffer: 12, monatstag: 1, indicator: "X").Error);

        Assert.False(
            Read(setName: "Meer", kennziffer: 12, monatstag: 1, indicator: "FNHCGVET").Failed);
    }

    [Fact]
    public void MeerHasNoDayThatCannotBeServed()
    {
        // Every column and day of the complete set resolves; Quelle's Tafel J days
        // do not. This is the difference the set choice exists to expose.
        Assert.All(
            from kennziffer in Enumerable.Range(1, BigramTableSet.Meer.Plan.Columns)
            from day in Enumerable.Range(1, 31)
            select Read(
                setName: "Meer", kennziffer: kennziffer, monatstag: day, indicator: "FNHCGVET"),
            read => Assert.False(read.Failed));
    }
}
