namespace Enigma.Tests;

public class MessageTextTests
{
    [Fact]
    public void SpacesBecomeX()
    {
        Assert.Equal("ATTACKXATXDAWN", MessageText.Prepare("attack at dawn"));
    }

    [Fact]
    public void UmlautsAndTheSharpSAreExpanded()
    {
        Assert.Equal("GROESSEXUEBERXAENDERUNG", MessageText.Prepare("größe über änderung"));
    }

    [Fact]
    public void DigitsAreSpelledOut()
    {
        Assert.Equal("EINSACHTDREINULL", MessageText.Prepare("1830"));
    }

    [Fact]
    public void PunctuationIsDropped()
    {
        // Only spaces become X. Punctuation is dropped outright, so the trailing
        // full stop leaves nothing behind, and the two spaces around the dash
        // leave two.
        Assert.Equal("HALTXXSTOP", MessageText.Prepare("Halt! - stop."));
    }

    [Fact]
    public void PreparedTextIsAlwaysKeyable()
    {
        var prepared = MessageText.Prepare("Angriff um 06:30 Uhr — Größe: 12 Männer!");

        Assert.All(prepared, character => Assert.InRange(character, 'A', 'Z'));
    }

    [Fact]
    public void MessagesAreSentInFiveLetterGroups()
    {
        Assert.Equal("BDZGO WCXLT KSBTM CD", MessageText.InGroups("BDZGOWCXLTKSBTMCD"));
    }

    [Fact]
    public void TheGroupSizeCanBeChanged()
    {
        Assert.Equal("BDZG OWCX LT", MessageText.InGroups("BDZGOWCXLT", 4));
    }

    [Fact]
    public void GroupingIgnoresWhitespaceAlreadyPresent()
    {
        Assert.Equal("BDZGO WCXLT", MessageText.InGroups("BDZGO WCXLT"));
    }

    [Fact]
    public void AnEmptyMessageGroupsToNothing()
    {
        Assert.Equal(string.Empty, MessageText.InGroups(string.Empty));
    }

    [Fact]
    public void AGroupSizeMustBePositive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MessageText.InGroups("ABC", 0));
    }
}
