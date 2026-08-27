namespace Enigma.Tests;

public class UhrSettingTests
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    [Fact]
    public void BothPlatesCoverTheAlphabetExactlyOnce()
    {
        // If a letter were missing an operator could not read it, and if one were
        // on two bands the setting would be ambiguous.
        foreach (var letter in Alphabet)
        {
            var group = $"{letter}{letter}";

            Assert.InRange(UhrSetting.Decode(group), 0, UhrSetting.Positions - 1);
        }
    }

    [Theory]
    [InlineData(0, "AA")]
    [InlineData(11, "GD")]
    [InlineData(39, "TX")]
    public void ASettingHasALetterGroup(int position, string expected)
    {
        Assert.Equal(expected, UhrSetting.Encode(position));
    }

    [Theory]
    [InlineData("AA", 0)]
    [InlineData("GD", 11)]
    [InlineData("TX", 39)]
    public void ALetterGroupHasASetting(string group, int expected)
    {
        Assert.Equal(expected, UhrSetting.Decode(group));
    }

    [Fact]
    public void EverySettingRoundTrips()
    {
        for (var position = 0; position < UhrSetting.Positions; position++)
        {
            Assert.Equal(position, UhrSetting.Decode(UhrSetting.Encode(position)));
        }
    }

    [Fact]
    public void TheBandsAreUnevenSoASettingHasSeveralGroups()
    {
        // Alphabet I band 1 is GHIJKLM and Alphabet II band 1 is DE, so setting 11
        // could be sent as any of fourteen pairs. An operator chose one.
        var groups = UhrSetting.AllGroupsFor(11).ToList();

        Assert.Equal(14, groups.Count);
        Assert.Contains("GD", groups);
        Assert.Contains("ME", groups);
        Assert.All(groups, group => Assert.Equal(11, UhrSetting.Decode(group)));
    }

    [Fact]
    public void EveryGroupOfEverySettingDecodesBack()
    {
        for (var position = 0; position < UhrSetting.Positions; position++)
        {
            Assert.All(
                UhrSetting.AllGroupsFor(position),
                group => Assert.Equal(position, UhrSetting.Decode(group)));
        }
    }

    [Fact]
    public void TheGroupsOfAllFortySettingsAccountForEveryPairOfLetters()
    {
        // Twenty six letters on each plate, so the forty settings between them must
        // name all 676 pairs exactly once.
        var all = Enumerable.Range(0, UhrSetting.Positions)
            .SelectMany(UhrSetting.AllGroupsFor)
            .ToList();

        Assert.Equal(26 * 26, all.Count);
        Assert.Equal(26 * 26, all.Distinct().Count());
    }

    [Fact]
    public void LowerCaseAndSpacingAreAccepted()
    {
        Assert.Equal(11, UhrSetting.Decode("g d"));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("ABC")]
    public void AGroupIsTwoLetters(string group)
    {
        Assert.Throws<ArgumentException>(() => UhrSetting.Decode(group));
    }

    [Fact]
    public void ACharacterThatIsNotOnThePlateIsRefused()
    {
        Assert.Throws<ArgumentException>(() => UhrSetting.Decode("A1"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(40)]
    public void OnlyTheFortyDialPositionsExist(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UhrSetting.Encode(position));
    }
}
