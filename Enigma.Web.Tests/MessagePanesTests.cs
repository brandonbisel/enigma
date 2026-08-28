using Bunit;
using Enigma.Web.Components;

namespace Enigma.Web.Tests;

public class MessagePanesTests : BunitContext
{
    [Fact]
    public void TheMessageAndTheCipherAreShown()
    {
        var panes = Render(plaintext: "ATTACK", ciphertext: "BZHGNO");

        Assert.Equal("ATTACK", panes.Find("[data-testid=plaintext]").GetAttribute("value"));
        Assert.Equal("BZHGNO", panes.Find("[data-testid=ciphertext]").TextContent);
    }

    [Fact]
    public void WritingTheMessageIsReported()
    {
        string? written = null;
        var panes = Render(plaintext: string.Empty, onPlaintext: text => written = text);

        panes.Find("[data-testid=plaintext]").Change("ATTACK AT DAWN");

        Assert.Equal("ATTACK AT DAWN", written);
    }

    [Fact]
    public void ClearingTheMessageIsAnEmptyMessage()
    {
        // There is no separate reset: a machine with nothing typed on it is a
        // machine at its start.
        string? written = null;
        var panes = Render(plaintext: "ATTACK", onPlaintext: text => written = text);

        panes.Find("[data-testid=clear]").Click();

        Assert.Equal(string.Empty, written);
    }

    [Fact]
    public void FittingTheTextToTheKeyboardIsReported()
    {
        bool? prepare = null;
        var panes = Render(onPrepare: value => prepare = value);

        panes.Find("[data-testid=prepare]").Change(true);

        Assert.True(prepare);
    }

    [Fact]
    public void TurningOnGroupsAsksForTheConventionalFive()
    {
        int? groups = null;
        var panes = Render(onGroups: value => groups = value);

        panes.Find("[data-testid=grouped]").Change(true);

        Assert.Equal(5, groups);
    }

    [Fact]
    public void TurningOffGroupsAsksForNone()
    {
        int? groups = 5;
        var panes = Render(groups: 5, onGroups: value => groups = value);

        panes.Find("[data-testid=grouped]").Change(false);

        Assert.Null(groups);
    }

    [Fact]
    public void TheGroupSizeIsOfferedOnlyWhenGroupingIsOn()
    {
        Assert.Empty(Render().FindAll("[data-testid=group-size]"));
        Assert.NotEmpty(Render(groups: 5).FindAll("[data-testid=group-size]"));
    }

    [Fact]
    public void ChangingTheGroupSizeIsReported()
    {
        int? groups = null;
        var panes = Render(groups: 5, onGroups: value => groups = value);

        panes.Find("[data-testid=group-size]").Change("4");

        Assert.Equal(4, groups);
    }

    [Fact]
    public void AGroupSizeThatIsNotANumberIsIgnored()
    {
        // InGroups refuses a size below one, and a control should not be able to
        // ask it for something it refuses.
        int? groups = 5;
        var panes = Render(groups: 5, onGroups: value => groups = value);

        panes.Find("[data-testid=group-size]").Change("nonsense");
        panes.Find("[data-testid=group-size]").Change("0");

        Assert.Equal(5, groups);
    }

    private IRenderedComponent<MessagePanes> Render(
        string plaintext = "",
        string ciphertext = "",
        bool prepare = false,
        int? groups = null,
        Action<string>? onPlaintext = null,
        Action<bool>? onPrepare = null,
        Action<int?>? onGroups = null) =>
        Render<MessagePanes>(parameters => parameters
            .Add(panes => panes.Plaintext, plaintext)
            .Add(panes => panes.Ciphertext, ciphertext)
            .Add(panes => panes.Prepare, prepare)
            .Add(panes => panes.Groups, groups)
            .Add(panes => panes.OnPlaintext, text => onPlaintext?.Invoke(text))
            .Add(panes => panes.OnPrepare, value => onPrepare?.Invoke(value))
            .Add(panes => panes.OnGroups, value => onGroups?.Invoke(value)));
}
