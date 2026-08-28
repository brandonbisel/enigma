using AngleSharp.Dom;
using Bunit;
using Enigma;
using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Web.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Web.Tests;

/// <summary>
/// The panel is a view of a machine, so what it owes is that what it shows is what
/// the machine did — the same lamp, the same window, at the same instant.
/// </summary>
public class MachinePanelTests : BunitContext
{
    [Fact]
    public void TheKeyboardIsBuiltFromTheAlphabetRatherThanAConstant()
    {
        var panel = Render(Open("default"));

        Assert.Equal(26, panel.FindAll("[data-testid=key]").Count);
        Assert.Equal(26, panel.FindAll("[data-testid=lamp]").Count);
    }

    [Fact]
    public void PressingAKeyLightsTheLampTheMachineReturns()
    {
        var expected = Session("default").Press('A');
        var panel = Render(Open("default"));

        Key(panel, 'A').MouseDown();

        Assert.Equal(expected.ToString(), Lit(panel));
    }

    [Fact]
    public void NoLampIsAlightBeforeAKeyIsPressed()
    {
        Assert.Null(Lit(Render(Open("default"))));
    }

    [Fact]
    public void ReleasingTheKeyPutsTheLampOut()
    {
        var panel = Render(Open("default"));

        Key(panel, 'A').MouseDown();
        Key(panel, 'A').MouseUp();

        Assert.Null(Lit(panel));
    }

    [Fact]
    public void TheWheelsTurnBeforeTheLampLights()
    {
        // The window a keypress shows is the one the operator read as the lamp lit,
        // which is after the wheels moved. Showing the earlier one would teach the
        // machine backwards.
        var panel = Render(Open("default"));

        Assert.Equal("AAA", Windows(panel));

        Key(panel, 'A').MouseDown();

        Assert.Equal("AAB", Windows(panel));
    }

    [Fact]
    public void HoldingAKeyDownEnciphersOnlyOnce()
    {
        // The real keyboard locks out a second key while one is down, and a held
        // key does not step the wheels again however long it is held.
        var panel = Render(Open("default"));

        Key(panel, 'A').MouseDown();
        Key(panel, 'A').MouseDown();
        Key(panel, 'B').MouseDown();

        Assert.Equal("AAB", Windows(panel));
        Assert.Equal(1, Tape(panel).Length);
    }

    [Fact]
    public void TheTapeCollectsWhatWasEnciphered()
    {
        var expected = Session("default").Type("ATTACK");
        var panel = Render(Open("default"));

        foreach (var key in "ATTACK")
        {
            Key(panel, key).MouseDown();
            Key(panel, key).MouseUp();
        }

        Assert.Equal(expected, Tape(panel));
    }

    [Fact]
    public void PressingTheLitLetterGivesTheOriginalBack()
    {
        // Reciprocity, as an operator would check it: the property the machine was
        // built around and the first thing anyone tries.
        var panel = Render(Open("default"));

        Key(panel, 'A').MouseDown();

        var lamp = Lit(panel)![0];

        Key(panel, 'A').MouseUp();
        panel.Find("[data-testid=reset]").Click();

        Key(panel, lamp).MouseDown();

        Assert.Equal("A", Lit(panel));
    }

    [Fact]
    public void AKeyTheMachineHasNotGotDoesNothing()
    {
        var panel = Render(Open("default"));

        panel.Find("[data-testid=panel]").KeyDown(new KeyboardEventArgs
        {
            Key = "7"
        });

        Assert.Null(Lit(panel));
        Assert.Equal("AAA", Windows(panel));
        Assert.Empty(Tape(panel));
    }

    [Fact]
    public void AnAutorepeatIsNeverAFreshPress()
    {
        // Held keys autorepeat in a browser. A repeat is not a press, whatever the
        // panel currently believes is held -- asserted on its own first, because
        // the one-key-at-a-time lockout would otherwise hide this entirely.
        var panel = Render(Open("default"));
        var element = panel.Find("[data-testid=panel]");

        element.KeyDown(new KeyboardEventArgs { Key = "a", Repeat = true });

        Assert.Null(Lit(panel));
        Assert.Equal("AAA", Windows(panel));
        Assert.Empty(Tape(panel));

        element.KeyDown(new KeyboardEventArgs { Key = "a" });
        element.KeyDown(new KeyboardEventArgs { Key = "a", Repeat = true });

        Assert.Equal("AAB", Windows(panel));
        Assert.Equal(1, Tape(panel).Length);
    }

    [Fact]
    public void ResettingReturnsTheWheelsAndClearsTheTape()
    {
        var panel = Render(Open("default"));

        foreach (var key in "ATTACK")
        {
            Key(panel, key).MouseDown();
            Key(panel, key).MouseUp();
        }

        panel.Find("[data-testid=reset]").Click();

        Assert.Equal("AAA", Windows(panel));
        Assert.Empty(Tape(panel));
    }

    [Fact]
    public void AMachineWhoseReflectorTurnsShowsItsWindow()
    {
        // A Zählwerk machine's UKW is part of the setting, so the operator can see
        // it. Four windows, not three.
        var panel = Render(Open(Zaehlwerk));

        Assert.Equal(4, panel.FindAll("[data-testid=window]").Count);
    }

    [Fact]
    public void AMachineWhoseReflectorIsFixedShowsOnlyTheWheels()
    {
        Assert.Equal(3, Render(Open("default")).FindAll("[data-testid=window]").Count);
    }

    [Fact]
    public void TheFourthWheelOfAnM4IsShown()
    {
        Assert.Equal(4, Render(Open("u264")).FindAll("[data-testid=window]").Count);
    }

    private IRenderedComponent<MachinePanel> Render(EnigmaSession session) =>
        Render<MachinePanel>(parameters => parameters.Add(panel => panel.Session, session));

    private static IElement Key(IRenderedComponent<MachinePanel> panel, char letter) =>
        panel.Find($"[data-testid=key][data-letter={letter}]");

    private static string? Lit(IRenderedComponent<MachinePanel> panel) =>
        panel.FindAll("[data-testid=lamp].lamp-lit") is [var lamp] ? lamp.TextContent : null;

    private static string Windows(IRenderedComponent<MachinePanel> panel) =>
        string.Concat(panel.FindAll("[data-testid=window] .window-letter")
            .Select(window => window.TextContent));

    private static string Tape(IRenderedComponent<MachinePanel> panel) =>
        panel.Find("[data-testid=tape]").TextContent;

    private static EnigmaSession Open(string name)
    {
        var catalogue = new KeySheetCatalogue([new("g31", Zaehlwerk)]);

        Assert.True(catalogue.TryGet(name, out var sheet));

        return Open(sheet);
    }

    private static EnigmaSession Open(KeySheet sheet)
    {
        var result = EnigmaSession.Open(Factory(), sheet);

        Assert.True(result.Succeeded, result.Error);

        return result.Session!;
    }

    private static EnigmaSession Session(string name) => Open(name);

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

    private static IEnigmaMachineFactory Factory() =>
        new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();
}
