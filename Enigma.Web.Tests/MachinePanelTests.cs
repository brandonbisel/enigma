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
/// The panel is a view of a machine. It reports which key went down and shows the
/// lamp it is given; it works nothing out for itself, so it cannot tell a different
/// story from the message it belongs to.
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
    public void TheLampItIsGivenIsTheLampThatLights()
    {
        var panel = Render(Open("default"), lit: 'Q');

        Assert.Equal("Q", Lit(panel));
    }

    [Fact]
    public void NoLampIsAlightWhenThereIsNoneToShow()
    {
        Assert.Null(Lit(Render(Open("default"))));
    }

    [Fact]
    public void PressingAKeyReportsIt()
    {
        var keyed = new List<char>();
        var panel = Render(Open("default"), keyed: keyed.Add);

        Key(panel, 'A').MouseDown();

        Assert.Equal(['A'], keyed);
    }

    [Fact]
    public void ReleasingAKeyIsReported()
    {
        var released = 0;
        var panel = Render(Open("default"), released: () => released++);

        Key(panel, 'A').MouseDown();
        Key(panel, 'A').MouseUp();

        Assert.Equal(1, released);
    }

    [Fact]
    public void ReleasingWithNothingHeldReportsNothing()
    {
        var released = 0;
        var panel = Render(Open("default"), released: () => released++);

        Key(panel, 'A').MouseUp();

        Assert.Equal(0, released);
    }

    [Fact]
    public void HoldingAKeyDownReportsItOnlyOnce()
    {
        // The real keyboard locks out a second key while one is down, and letting
        // two through would step the wheels twice.
        var keyed = new List<char>();
        var panel = Render(Open("default"), keyed: keyed.Add);

        Key(panel, 'A').MouseDown();
        Key(panel, 'A').MouseDown();
        Key(panel, 'B').MouseDown();

        Assert.Equal(['A'], keyed);
    }

    [Fact]
    public void AnAutorepeatIsNeverAFreshPress()
    {
        // Held keys autorepeat in a browser. A repeat is not a press, whatever the
        // panel currently believes is held — asserted on its own first, because the
        // one-key-at-a-time lockout would otherwise hide this entirely.
        var keyed = new List<char>();
        var panel = Render(Open("default"), keyed: keyed.Add);
        var element = panel.Find("[data-testid=panel]");

        element.KeyDown(new KeyboardEventArgs { Key = "a", Repeat = true });

        Assert.Empty(keyed);

        element.KeyDown(new KeyboardEventArgs { Key = "a" });
        element.KeyDown(new KeyboardEventArgs { Key = "a", Repeat = true });

        Assert.Equal(['a'], keyed);
    }

    [Fact]
    public void APhysicalKeyIsReportedLikeAClick()
    {
        var keyed = new List<char>();
        var panel = Render(Open("default"), keyed: keyed.Add);

        panel.Find("[data-testid=panel]").KeyDown(new KeyboardEventArgs { Key = "Q" });

        Assert.Equal(['Q'], keyed);
    }

    [Fact]
    public void AKeyWithNoSingleCharacterIsNotAKey()
    {
        var keyed = new List<char>();
        var panel = Render(Open("default"), keyed: keyed.Add);

        panel.Find("[data-testid=panel]").KeyDown(new KeyboardEventArgs { Key = "Shift" });

        Assert.Empty(keyed);
    }

    [Fact]
    public void TheWindowsShowWhereTheWheelsStand()
    {
        var session = Open("default");
        var panel = Render(session);

        Assert.Equal("AAA", Windows(panel));

        session.Press('A');
        panel.Render();

        Assert.Equal("AAB", Windows(panel));
    }

    [Fact]
    public void AMachineWhoseReflectorTurnsShowsItsWindow()
    {
        // A Zählwerk machine's UKW is part of the setting, so the operator sees it.
        Assert.Equal(4, Render(Open(Zaehlwerk)).FindAll("[data-testid=window]").Count);
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

    private IRenderedComponent<MachinePanel> Render(
        EnigmaSession session,
        char? lit = null,
        Action<char>? keyed = null,
        Action? released = null) =>
        Render<MachinePanel>(parameters => parameters
            .Add(panel => panel.Session, session)
            .Add(panel => panel.Lit, lit)
            .Add(panel => panel.OnKeyed, key => keyed?.Invoke(key))
            .Add(panel => panel.OnReleased, () => released?.Invoke()));

    private static IElement Key(IRenderedComponent<MachinePanel> panel, char letter) =>
        panel.Find($"[data-testid=key][data-letter={letter}]");

    private static string? Lit(IRenderedComponent<MachinePanel> panel) =>
        panel.FindAll("[data-testid=lamp].lamp-lit") is [var lamp] ? lamp.TextContent : null;

    private static string Windows(IRenderedComponent<MachinePanel> panel) =>
        string.Concat(panel.FindAll("[data-testid=window] .window-letter")
            .Select(window => window.TextContent));

    private static EnigmaSession Open(string name)
    {
        var catalogue = new KeySheetCatalogue();

        Assert.True(catalogue.TryGet(name, out var sheet));

        return Open(sheet);
    }

    private static EnigmaSession Open(KeySheet sheet)
    {
        var result = EnigmaSession.Open(Factory(), sheet);

        Assert.True(result.Succeeded, result.Error);

        return result.Session!;
    }

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
