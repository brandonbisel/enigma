namespace Enigma.Analysis.Tests;

public class IndexOfCoincidenceTests
{
    private static readonly IScore Measure = new IndexOfCoincidence();

    private static int[] Contacts(string text) =>
        text.Select(character => character - 'A').ToArray();

    [Fact]
    public void GermanStandsWellAboveTextWithNoStructure()
    {
        var german = Measure.Of(Contacts(Machinery.German));

        // A flat alphabet repeated has every letter equally often, which is the
        // rate a text with no structure tends to: one chance in twenty six.
        var flat = Measure.Of(Contacts(
            string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 16))));

        Assert.InRange(flat, 0.036, 0.040);
        Assert.True(german > flat * 1.3, $"German scored {german:F5} against {flat:F5}.");
    }

    [Fact]
    public void RelabellingTheAlphabetChangesNothing()
    {
        // This is the whole reason the measure is worth anything against a machine
        // with a plugboard: it counts how often two characters agree, and renaming
        // the characters moves every count to a different letter without changing
        // one of them. What it does not survive is the board being *inside* the
        // rotors rather than outside, which is the case that actually arises.
        var plain = Contacts(Machinery.German);
        var relabelled = plain.Select(contact => (contact * 7 + 3) % 26).ToArray();

        Assert.Equal(Measure.Of(plain), Measure.Of(relabelled), 12);
    }

    [Fact]
    public void OneCharacterCannotAgreeWithAnything()
    {
        Assert.Equal(0, Measure.Of([4]));
        Assert.Equal(0, Measure.Of([]));
    }

    [Fact]
    public void EveryCharacterTheSameIsTotalAgreement()
    {
        Assert.Equal(1, Measure.Of(Contacts("AAAAAAAAAA")));
    }

    [Fact]
    public void ItCountsInTheAlphabetItWasGiven()
    {
        // The numbers-only Enigma Z has ten contacts, so its flat rate is one in ten
        // rather than one in twenty six. A measure that assumed 26 would read every
        // Z30 decipherment as extraordinarily structured.
        var digits = new IndexOfCoincidence(CharacterMap.Digits);
        var flat = digits.Of(Enumerable.Range(0, 200).Select(i => i % 10).ToArray());

        Assert.InRange(flat, 0.09, 0.11);
    }
}
