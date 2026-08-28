using Enigma;
using Enigma.Web;

namespace Enigma.Web.Tests;

public class KeyboardLayoutTests
{
    [Fact]
    public void TheLatinAlphabetIsLaidOutAsAnEnigmaKeyboard()
    {
        Assert.Equal(["QWERTZUIO", "ASDFGHJK", "PYXCVBNML"], KeyboardLayout.Rows(CharacterMap.Latin));
    }

    [Fact]
    public void EveryLetterAppearsOnTheKeyboardExactlyOnce()
    {
        // Derived rather than asserted: a keyboard missing a letter would be a
        // machine with a key the operator could never press.
        var keys = string.Concat(KeyboardLayout.Rows(CharacterMap.Latin));

        Assert.Equal(26, keys.Length);
        Assert.Equal(26, keys.Distinct().Count());
        Assert.All("ABCDEFGHIJKLMNOPQRSTUVWXYZ", letter => Assert.Contains(letter, keys));
    }

    [Fact]
    public void TheTenDigitsAreOneRowOfFigures()
    {
        // The Enigma Z's keyboard is a single row reading 1 to 0, not two rows
        // reading 0 to 9. Where a key sits is not where a contact sits.
        Assert.Equal(["1234567890"], KeyboardLayout.Rows(CharacterMap.Digits));
    }

    [Fact]
    public void EveryFigureAppearsOnTheKeyboardExactlyOnce()
    {
        var keys = string.Concat(KeyboardLayout.Rows(CharacterMap.Digits));

        Assert.Equal(10, keys.Length);
        Assert.Equal(10, keys.Distinct().Count());
    }

    [Fact]
    public void AnAlphabetOfAnotherShapeIsLaidOutInItsOwnOrder()
    {
        // The known arrangements are the twenty six letter keyboard and the ten
        // figure one. Anything else is laid out in the order its alphabet runs.
        var short_ = new CharacterMap("Short", "ABCDEFGHIJKL");

        Assert.Equal(["ABCDEFGHI", "JKL"], KeyboardLayout.Rows(short_));
    }

    [Fact]
    public void AnAlphabetOfTwentySixCharactersThatIsNotTheLatinOneKeepsItsOwnOrder()
    {
        // Same size, different letters: the keyboard arrangement is a fact about
        // the Latin machines, not about any alphabet that happens to be as long.
        var shifted = new CharacterMap("Shifted", "BCDEFGHIJKLMNOPQRSTUVWXYZA");

        Assert.Equal(["BCDEFGHIJ", "KLMNOPQRS", "TUVWXYZA"], KeyboardLayout.Rows(shifted));
    }
}
