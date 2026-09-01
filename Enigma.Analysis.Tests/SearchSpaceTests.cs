using Enigma.Models;

namespace Enigma.Analysis.Tests;

public class SearchSpaceTests
{
    private static readonly KeySheet Service = new() { Model = "Service", Reflector = "B" };

    [Fact]
    public void EveryOrderingOfTheBoxIsAnArrangement()
    {
        var space = SearchSpace.Of(Machinery.Parts, Service, ["I", "II", "III", "IV", "V"], 3);

        // Five wheels taken three at a time, in order: 5 x 4 x 3.
        Assert.Equal(60, space.Arrangements.Count);
        Assert.Equal(60L * 26 * 26 * 26, space.Settings);
    }

    [Fact]
    public void AWheelCannotBeInTwoPlacesAtOnce()
    {
        var space = SearchSpace.Of(Machinery.Parts, Service, ["I", "II", "III"], 3);

        Assert.All(space.Arrangements, arrangement =>
            Assert.Equal(3, arrangement.Wheels.Distinct().Count()));
    }

    [Fact]
    public void TheMachinesOwnFitmentRulesDecideWhatIsSearched()
    {
        // A thin wheel only fits leftmost, and only beside a thin reflector. The
        // search does not restate that -- it asks the layout, which is what keeps the
        // rule in one place.
        var naval = new KeySheet { Model = "Service", Reflector = "B-Thin" };
        var space = SearchSpace.Of(
            Machinery.Parts, naval, ["BETA", "GAMMA", "I", "II", "III"], 4);

        Assert.NotEmpty(space.Arrangements);
        Assert.All(space.Arrangements, arrangement =>
        {
            Assert.Contains(arrangement.Wheels[0], new[] { "BETA", "GAMMA" });
            Assert.DoesNotContain(arrangement.Wheels[1], new[] { "BETA", "GAMMA" });
        });
    }

    [Fact]
    public void ABoxThatCouldNotBuildTheMachineIsRefused()
    {
        // Four full width wheels behind a thin reflector is not a machine anyone
        // could assemble, so there is nothing to search.
        var naval = new KeySheet { Model = "Service", Reflector = "B-Thin" };

        Assert.Throws<ArgumentException>(() =>
            SearchSpace.Of(Machinery.Parts, naval, ["I", "II", "III", "IV"], 4));
    }

    [Fact]
    public void MoreWheelsFittedThanTheBoxHoldsIsRefused()
    {
        Assert.Throws<ArgumentException>(() =>
            SearchSpace.Of(Machinery.Parts, Service, ["I", "II"], 3));
    }

    [Fact]
    public void SeveralReflectorsMultiplyTheArrangements()
    {
        var space = SearchSpace.Of(
            Machinery.Parts, Service, ["I", "II", "III"], 3, ["B", "C"]);

        Assert.Equal(12, space.Arrangements.Count);
    }

    [Fact]
    public void TheSheetItWritesCarriesNoCables()
    {
        // The wheels are recovered through the cables rather than after them, so the
        // machine a search runs has none in -- including an Uhr, which is a board.
        var known = new KeySheet
        {
            Model = "Service", Reflector = "B", Plugboard = "AV BS CG", Uhr = "06"
        };

        var space = SearchSpace.Of(Machinery.Parts, known, ["I", "II", "III"], 3);
        var sheet = space.Sheet(space.Arrangements[0], "AAA", "AAA");

        Assert.Empty(sheet.Plugboard);
        Assert.Empty(sheet.Uhr);
    }

    [Fact]
    public void ItReadsCiphertextInTheMachinesOwnAlphabet()
    {
        var space = SearchSpace.Of(Machinery.Parts, Service, ["I", "II", "III"], 3);

        Assert.Equal([0, 1, 25], space.Read("ab z!"));
    }
}
