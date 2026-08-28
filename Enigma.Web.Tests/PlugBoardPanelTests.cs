using AngleSharp.Dom;
using Bunit;
using Enigma;
using Enigma.Web.Components;

namespace Enigma.Web.Tests;

public class PlugBoardPanelTests : BunitContext
{
    [Fact]
    public void EveryLetterHasAJack()
    {
        Assert.Equal(26, Render([]).FindAll("[data-testid=jack]").Count);
    }

    [Fact]
    public void TwoClicksRunACable()
    {
        (char, char)? patched = null;
        var board = Render([], patch: cable => patched = cable);

        Jack(board, 'A').Click();
        Jack(board, 'V').Click();

        Assert.Equal(('A', 'V'), patched);
    }

    [Fact]
    public void OneClickTakesUpAPlugWithoutCablingAnything()
    {
        var patched = false;
        var board = Render([], patch: _ => patched = true);

        Jack(board, 'A').Click();

        Assert.False(patched);
        Assert.Contains("jack-held", Jack(board, 'A').ClassName);
    }

    [Fact]
    public void ClickingTheSameJackTwicePutsThePlugBack()
    {
        var patched = false;
        var board = Render([], patch: _ => patched = true);

        Jack(board, 'A').Click();
        Jack(board, 'A').Click();

        Assert.False(patched);
        Assert.DoesNotContain("jack-held", Jack(board, 'A').ClassName);
    }

    [Fact]
    public void AJackWithACableIsShownAsCabled()
    {
        var board = Render([('A', 'V')]);

        Assert.Contains("jack-cabled", Jack(board, 'A').ClassName);
        Assert.Contains("jack-cabled", Jack(board, 'V').ClassName);
        Assert.DoesNotContain("jack-cabled", Jack(board, 'B').ClassName);
    }

    [Fact]
    public void ClickingACabledJackPullsTheCableOut()
    {
        (char, char)? unpatched = null;
        var board = Render([('A', 'V')], unpatch: cable => unpatched = cable);

        Jack(board, 'V').Click();

        Assert.Equal(('V', 'A'), unpatched);
    }

    [Fact]
    public void APlugInTheWayComesOutBeforeAnythingElseGoesIn()
    {
        // A jack takes one plug, so clicking a cabled letter can only mean removing
        // the cable that is there -- never running a second one to it.
        var patched = false;
        (char, char)? unpatched = null;
        var board = Render([('A', 'V')], patch: _ => patched = true, unpatch: cable => unpatched = cable);

        Jack(board, 'B').Click();
        Jack(board, 'A').Click();

        Assert.False(patched);
        Assert.NotNull(unpatched);
    }

    [Fact]
    public void TheCablesAreListed()
    {
        Assert.Equal("AV BS", Render([('A', 'V'), ('B', 'S')]).Find("[data-testid=cables]").TextContent.Trim());
    }

    [Fact]
    public void AnEmptyBoardSaysSo()
    {
        Assert.Contains("No cables", Render([]).Find("[data-testid=cables]").TextContent);
    }

    private IRenderedComponent<PlugBoardPanel> Render(
        (char From, char To)[] cables,
        Action<(char, char)>? patch = null,
        Action<(char, char)>? unpatch = null) =>
        Render<PlugBoardPanel>(parameters => parameters
            .Add(board => board.Alphabet, CharacterMap.Latin)
            .Add(board => board.Cables, cables)
            .Add(board => board.OnPatch, cable => patch?.Invoke(cable))
            .Add(board => board.OnUnpatch, cable => unpatch?.Invoke(cable)));

    private static IElement Jack(IRenderedComponent<PlugBoardPanel> board, char letter) =>
        board.Find($"[data-testid=jack][data-letter={letter}]");
}
