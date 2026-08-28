using Enigma.App;
using Enigma.Models;

namespace Enigma.Tests;

public class KeySheetCatalogueTests
{
    [Fact]
    public void ThePackagedSheetsAreOnOfferByDefault()
    {
        var catalogue = new KeySheetCatalogue();

        Assert.Equal(KeySheets.All.Keys.Order(), catalogue.Names.Order());
        Assert.True(catalogue.TryGet("barbarossa", out _));
    }

    [Fact]
    public void NamesAreNotCaseSensitive()
    {
        // A key sheet is named by a person typing it, not by a machine.
        Assert.True(new KeySheetCatalogue().TryGet("BARBAROSSA", out _));
    }

    [Fact]
    public void AnUnknownNameIsReportedRatherThanThrown()
    {
        Assert.False(new KeySheetCatalogue().TryGet("no-such-sheet", out var sheet));
        Assert.Null(sheet);
    }

    [Fact]
    public void SuppliedSheetsAreAddedToThePackagedOnes()
    {
        var catalogue = new KeySheetCatalogue([new("mine", Sheet("Mine"))]);

        Assert.True(catalogue.TryGet("mine", out var mine));
        Assert.Equal("Mine", mine.Name);
        Assert.True(catalogue.TryGet("barbarossa", out _));
    }

    [Fact]
    public void ASuppliedSheetReplacesAPackagedOneOfTheSameName()
    {
        var catalogue = new KeySheetCatalogue([new("barbarossa", Sheet("Not the real one"))]);

        Assert.True(catalogue.TryGet("barbarossa", out var sheet));
        Assert.Equal("Not the real one", sheet.Name);
    }

    [Fact]
    public void ReplacingAPackagedSheetIsReported()
    {
        // The packaged sheets are each verified against a published message, so a
        // front end may want to say when one is no longer that sheet.
        var catalogue = new KeySheetCatalogue([new("barbarossa", Sheet("Mine")), new("mine", Sheet("Mine"))]);

        Assert.Equal(["barbarossa"], catalogue.Shadowed);
    }

    [Fact]
    public void NothingIsShadowedWhenNothingIsSupplied()
    {
        Assert.Empty(new KeySheetCatalogue().Shadowed);
    }

    private static KeySheet Sheet(string name) => new()
    {
        Name = name,
        Reflector = "B",
        Rotors = "I II III",
        RingSettings = "AAA",
        Positions = "AAA"
    };
}
