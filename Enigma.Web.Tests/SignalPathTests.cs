using Bunit;
using Enigma;
using Enigma.Reflectors;
using Enigma.Rotors;
using Enigma.Web.Components;

namespace Enigma.Web.Tests;

/// <summary>
/// The view of the signal path shows the machine's own account of a keypress. It
/// must not retell it: every letter on screen comes from the trace.
/// </summary>
public class SignalPathTests : BunitContext
{
    [Fact]
    public void NothingIsShownUntilFollowingIsAskedFor()
    {
        var view = Render(watching: false);

        Assert.NotEmpty(view.FindAll("[data-testid=signal-off]"));
        Assert.Empty(view.FindAll("[data-testid=steps]"));
    }

    [Fact]
    public void FollowingWithNothingKeyedYetSaysSo()
    {
        var view = Render(watching: true);

        Assert.NotEmpty(view.FindAll("[data-testid=signal-idle]"));
        Assert.Empty(view.FindAll("[data-testid=steps]"));
    }

    [Fact]
    public void EveryComponentTheCurrentPassedThroughIsListed()
    {
        var trace = Keyed('A');
        var view = Render(watching: true, trace: trace);

        Assert.Equal(trace.Steps.Count, view.FindAll("[data-testid=step]").Count);
        Assert.Equal(
            trace.Steps.Select(step => step.Component),
            view.FindAll(".step-part").Select(part => part.TextContent));
    }

    [Fact]
    public void EachStepShowsTheContactsItJoined()
    {
        var trace = Keyed('A');
        var view = Render(watching: true, trace: trace);

        Assert.Equal(
            trace.Steps.Select(step => $"{Letter(step.Input)} → {Letter(step.Output)}"),
            view.FindAll(".step-flow").Select(flow => flow.TextContent.Trim()));
    }

    [Fact]
    public void TheKeyTheLampAndTheWindowAreShown()
    {
        var trace = Keyed('A');
        var view = Render(watching: true, trace: trace);
        var ends = view.Find("[data-testid=signal-ends]").TextContent;

        Assert.Contains(Letter(trace.Input).ToString(), ends);
        Assert.Contains(Letter(trace.Output).ToString(), ends);
        Assert.Contains(string.Concat(trace.Window.Select(Letter)), ends);
    }

    [Fact]
    public void TheReflectorIsMarkedAsTheTurningPoint()
    {
        var view = Render(watching: true, trace: Keyed('A'));

        var turn = Assert.Single(view.FindAll("[data-testid=step].step-reflector"));

        Assert.Equal("B", turn.QuerySelector(".step-part")!.TextContent);
    }

    [Fact]
    public void ThePathIsSplitAtTheReflector()
    {
        // Everything before the reflector is the way in and everything after it the
        // way out, and the two legs are the same length because the current retraces
        // its steps. Note the last of them is the plugboard, which carries no
        // apostrophe: a board of cables is its own inverse, so the trace names the
        // same board the same way both times.
        var view = Render(watching: true, trace: Keyed('A'));
        var steps = view.FindAll("[data-testid=step]").Select(step => step.ClassName ?? "").ToList();
        var turn = steps.FindIndex(step => step.Contains("step-reflector"));

        Assert.All(steps.Take(turn), step => Assert.DoesNotContain("step-back", step));
        Assert.All(steps.Skip(turn + 1), step => Assert.Contains("step-back", step));
        Assert.Equal(turn, steps.Count - turn - 1);
    }

    [Fact]
    public void TheTurnIsFoundOnAFourWheelMachineToo()
    {
        var view = Render(watching: true, trace: Keyed('A', thin: true));

        var turn = Assert.Single(view.FindAll("[data-testid=step].step-reflector"));

        Assert.Equal("B-Thin", turn.QuerySelector(".step-part")!.TextContent);
    }

    [Fact]
    public void AskingToFollowIsReported()
    {
        bool? watching = null;
        var view = Render(watching: false, onWatching: value => watching = value);

        view.Find("[data-testid=watching]").Change(true);

        Assert.True(watching);
    }

    [Fact]
    public void AskingToStopIsReported()
    {
        bool? watching = null;
        var view = Render(watching: true, onWatching: value => watching = value);

        view.Find("[data-testid=watching]").Change(false);

        Assert.False(watching);
    }

    private IRenderedComponent<SignalPath> Render(
        bool watching, TranslationTrace? trace = null, Action<bool>? onWatching = null) =>
        Render<SignalPath>(parameters => parameters
            .Add(view => view.Alphabet, CharacterMap.Latin)
            .Add(view => view.Trace, trace)
            .Add(view => view.Watching, watching)
            .Add(view => view.OnWatching, value => onWatching?.Invoke(value)));

    private static char Letter(int contact) => CharacterMap.Latin.GetCharacter(contact);

    private static TranslationTrace Keyed(char key, bool thin = false)
    {
        var machine = new EnigmaMachine(
            new PlugBoard(CharacterMap.Latin),
            thin
                ? [new RotorBeta(), new RotorI(), new RotorII(), new RotorIII()]
                : [new RotorI(), new RotorII(), new RotorIII()],
            thin ? new ReflectorBThin() : new ReflectorB(),
            characterMap: CharacterMap.Latin);

        TranslationTrace? trace = null;

        machine.Translated += captured => trace = captured;
        machine.Translate(CharacterMap.Latin.GetIndex(key));

        return trace!;
    }
}
