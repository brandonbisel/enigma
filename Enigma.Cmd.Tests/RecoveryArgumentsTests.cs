using Enigma.Cmd;

namespace Enigma.Cmd.Tests;

/// <summary>
/// What the command line will and will not accept for a recovery run.
///
/// Most of these are refusals, for the same reason the naval ones are: a search
/// argument that is half-obeyed spends minutes producing a confident answer to a
/// question nobody asked.
/// </summary>
public class RecoveryArgumentsTests
{
    private static readonly string[] SheetWheels = ["II", "IV", "V"];

    private static RecoveryArguments Read(
        bool recover = true,
        string? wheels = null,
        int? fitted = null,
        string? reflectors = null,
        int? candidates = null,
        IReadOnlyList<string>? sheetWheels = null,
        string sheetReflector = "B") =>
        RecoveryArguments.Read(
            recover, wheels, fitted, reflectors, candidates, sheetWheels ?? SheetWheels, sheetReflector);

    [Fact]
    public void WithoutRecoverNothingIsSearched()
    {
        var read = RecoveryArguments.Read(false, null, null, null, null, SheetWheels, "B");

        Assert.False(read.Wanted);
        Assert.Null(read.Error);
    }

    [Fact]
    public void SearchArgumentsWithoutRecoverAreRefusedRatherThanIgnored()
    {
        // Silently ignoring these would encipher the message instead of attacking it,
        // which looks like success and is not.
        var read = RecoveryArguments.Read(false, "I II III", null, null, null, SheetWheels, "B");

        Assert.False(read.Wanted);
        Assert.NotNull(read.Error);
    }

    [Fact]
    public void WhatIsNotGivenComesOffTheKeySheet()
    {
        var read = Read();

        Assert.True(read.Wanted);
        Assert.Equal(SheetWheels, read.Box);
        Assert.Equal(3, read.Fitted);
        Assert.Equal(["B"], read.Reflectors);
        Assert.Equal(5, read.Candidates);
    }

    [Fact]
    public void TheBoxIsSplitOnSpacesAndCommasButNotHyphens()
    {
        // Wheel names carry hyphens -- "K-I", "D-III" -- exactly as in a rotor order,
        // and splitting on one would turn a wheel into two that do not exist.
        var read = Read(wheels: "K-I, K-II K-III");

        Assert.Equal(["K-I", "K-II", "K-III"], read.Box);
    }

    [Fact]
    public void WheelNamesAreTakenHoweverTheyAreCased()
    {
        Assert.Equal(["I", "VI", "VIII"], Read(wheels: "i vi viii").Box);
    }

    [Fact]
    public void ABoxSmallerThanTheSheetsOwnWheelCountIsRefused()
    {
        // The sheet names three wheels, so three are fitted unless --fitted says
        // otherwise, and two wheels cannot fill three slots.
        var read = Read(wheels: "I VI");

        Assert.False(read.Wanted);
        Assert.Contains("two places at once", read.Error);
    }

    [Fact]
    public void SeveralReflectorsCanBeTried()
    {
        Assert.Equal(["B", "C"], Read(reflectors: "B C").Reflectors);
    }

    [Fact]
    public void MoreWheelsFittedThanTheBoxHoldsIsRefused()
    {
        var read = Read(wheels: "I II", fitted: 3);

        Assert.False(read.Wanted);
        Assert.Contains("two places at once", read.Error);
    }

    [Fact]
    public void AnEmptyBoxWithNothingOnTheSheetIsRefused()
    {
        var read = Read(sheetWheels: []);

        Assert.False(read.Wanted);
        Assert.Contains("--wheels", read.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FewerThanOneCandidateIsRefused(int candidates)
    {
        var read = Read(candidates: candidates);

        Assert.False(read.Wanted);
        Assert.NotNull(read.Error);
    }

    [Fact]
    public void FewerThanOneWheelFittedIsRefused()
    {
        var read = Read(fitted: 0);

        Assert.False(read.Wanted);
        Assert.NotNull(read.Error);
    }

    [Fact]
    public void AFourWheelSheetIsSearchedAsAFourWheelMachine()
    {
        var read = Read(
            wheels: "BETA GAMMA I II III IV",
            sheetWheels: ["BETA", "II", "IV", "I"],
            sheetReflector: "B-THIN");

        Assert.Equal(4, read.Fitted);
        Assert.Equal(["B-THIN"], read.Reflectors);
    }
}
