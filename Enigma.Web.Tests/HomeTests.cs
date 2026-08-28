using Bunit;
using Enigma;
using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Web.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Web.Tests;

public class HomeTests : BunitContext
{
    [Fact]
    public void EveryKeySheetOnOfferIsListed()
    {
        var page = Render(new KeySheetCatalogue());

        Assert.Equal(KeySheets.All.Count, page.FindAll("[data-testid=sheet] option").Count);
    }

    [Fact]
    public void APanelIsShownForTheSheetInUse()
    {
        var page = Render(new KeySheetCatalogue());

        Assert.NotNull(page.Find("[data-testid=panel]"));
        Assert.Equal(26, page.FindAll("[data-testid=key]").Count);
    }

    [Fact]
    public void ChoosingAnotherSheetKeysAnotherMachine()
    {
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=sheet]").Change("u264");

        // The M4 has four wheels where the Enigma I has three.
        Assert.Equal(4, page.FindAll("[data-testid=window]").Count);
    }

    [Fact]
    public void ChoosingAnotherSheetStartsItsMachineAfresh()
    {
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=key][data-letter=A]").MouseDown();
        page.Find("[data-testid=sheet]").Change("barbarossa");

        // Barbarossa's Grundstellung, not a machine that has already been typed on.
        Assert.Equal("BLA", string.Concat(
            page.FindAll("[data-testid=window] .window-letter").Select(window => window.TextContent)));
        Assert.Empty(page.Find("[data-testid=tape]").TextContent);
    }

    [Fact]
    public void AKeySheetTheMachineCannotBeBuiltFromShowsAMessage()
    {
        // A front end that threw here would show a blank page and a console error.
        var page = Render(new KeySheetCatalogue([new("default", Broken)]));

        Assert.Contains("NOSUCHWHEEL", page.Find("[data-testid=error]").TextContent);
        Assert.Empty(page.FindAll("[data-testid=panel]"));
    }

    private IRenderedComponent<Home> Render(IKeySheetCatalogue catalogue)
    {
        Services.AddEnigmaServices();
        Services.AddSingleton(catalogue);

        return Render<Home>();
    }

    private static readonly KeySheet Broken = new()
    {
        Name = "Broken",
        Reflector = "B",
        Rotors = "I II NOSUCHWHEEL",
        RingSettings = "AAA",
        Positions = "AAA"
    };
}
