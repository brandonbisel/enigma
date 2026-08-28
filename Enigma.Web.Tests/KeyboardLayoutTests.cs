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
    public void AnAlphabetOfAnotherShapeIsLaidOutInItsOwnOrder()
    {
        // The QWERTZ arrangement is only known for the twenty six letter machines.
        // A numeric machine such as an Enigma Z has its own keyboard.
        var digits = new CharacterMap("Digits", "0123456789");

        Assert.Equal(["012345678", "9"], KeyboardLayout.Rows(digits));
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
