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

    [Fact]
    public void ChangingASettingKeysTheMachineAgain()
    {
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=positions]").Change("XYZ");

        Assert.Equal("XYZ", Windows(page));
    }

    [Fact]
    public void ChangingASettingStartsTheMessageAgain()
    {
        // A machine keyed afresh has typed nothing, and the tape belongs to the
        // machine that was replaced.
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=key][data-letter=A]").MouseDown();
        page.Find("[data-testid=key][data-letter=A]").MouseUp();

        Assert.NotEmpty(page.Find("[data-testid=tape]").TextContent);

        page.Find("[data-testid=positions]").Change("XYZ");

        Assert.Empty(page.Find("[data-testid=tape]").TextContent);
    }

    [Fact]
    public void ASettingTheMachineCannotBeBuiltFromLeavesTheLastWorkingOneInPlace()
    {
        // Mid-way through setting up is not the same as holding a broken machine.
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=rings]").Change("NOTASETTING");

        Assert.NotEmpty(page.FindAll("[data-testid=error]"));
        Assert.NotEmpty(page.FindAll("[data-testid=panel]"));
        Assert.Equal("AAA", Windows(page));
    }

    [Fact]
    public void TheErrorClearsOnceTheSettingsWorkAgain()
    {
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=rings]").Change("NOTASETTING");
        page.Find("[data-testid=rings]").Change("BUL");

        Assert.Empty(page.FindAll("[data-testid=error]"));
    }

    [Fact]
    public void AMachineWithNoPlugboardIsOfferedNone()
    {
        // A Zählwerk Enigma has no Steckerbrett, so the page shows none rather than
        // showing one whose cables are refused.
        var page = Render(new KeySheetCatalogue([new("g31", Zaehlwerk)]));

        Assert.NotEmpty(page.FindAll("[data-testid=stecker]"));

        page.Find("[data-testid=sheet]").Change("g31");

        Assert.Empty(page.FindAll("[data-testid=stecker]"));
    }

    [Fact]
    public void CablingTwoLettersPatchesTheMachine()
    {
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=jack][data-letter=A]").Click();
        page.Find("[data-testid=jack][data-letter=V]").Click();

        Assert.Equal("AV", page.Find("[data-testid=cables]").TextContent.Trim());
        Assert.Contains("jack-cabled", page.Find("[data-testid=jack][data-letter=A]").ClassName);
    }

    [Fact]
    public void PullingACableOutUnpatchesTheMachine()
    {
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=sheet]").Change("barbarossa");

        Assert.Contains("AV", page.Find("[data-testid=cables]").TextContent);

        page.Find("[data-testid=jack][data-letter=A]").Click();

        Assert.DoesNotContain("AV", page.Find("[data-testid=cables]").TextContent);
    }

    [Fact]
    public void CablingChangesTheCipher()
    {
        // The point of the board: the same key gives a different lamp.
        var page = Render(new KeySheetCatalogue());

        page.Find("[data-testid=key][data-letter=A]").MouseDown();

        var plain = page.Find("[data-testid=lamp].lamp-lit").TextContent;

        page.Find("[data-testid=key][data-letter=A]").MouseUp();
        page.Find("[data-testid=jack][data-letter=A]").Click();
        page.Find("[data-testid=jack][data-letter=V]").Click();
        page.Find("[data-testid=key][data-letter=A]").MouseDown();

        Assert.NotEqual(plain, page.Find("[data-testid=lamp].lamp-lit").TextContent);
    }

    private static string Windows(IRenderedComponent<Home> page) =>
        string.Concat(page.FindAll("[data-testid=window] .window-letter")
            .Select(window => window.TextContent));

    private static readonly KeySheet Zaehlwerk = new()
    {
        Name = "Zählwerk G-31",
        Model = "G-31",
        Reflector = "G",
        Rotors = "G-I G-II G-III",
        RingSettings = "AAA",
        Positions = "AAA",
        ReflectorPosition = "A",
        ReflectorRingSetting = "A"
    };

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
