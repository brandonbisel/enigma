using Enigma.Reflectors;

namespace Enigma.Tests;

public class WiringValidationTests
{
    [Theory]
    [InlineData("ekmflgdqvzntowyhxuspaibrcj")]   // lower case would index outside the alphabet
    [InlineData("EKMFLGDQVZNTOWYHXUSPAIBRC1")]   // a digit
    [InlineData("EKMF LGDQ")]                    // spaces
    public void WiringMustBeCapitalLetters(string wiring)
    {
        Assert.Throws<ArgumentException>(() => WiringTable.FromString(wiring));
    }

    [Fact]
    public void WiringMustBeAPermutation()
    {
        Assert.Throws<ArgumentException>(
            () => WiringTable.FromString("ABCDEFGHIJKLMNOPQRSTUVWXYA"));
    }

    [Fact]
    public void ReflectorWiringMayNotConnectALetterToItself()
    {
        // A is left wired to A, which would let a letter encipher to itself.
        Assert.Throws<ArgumentException>(
            () => WiringTable.FromReflectorString("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
    }

    [Fact]
    public void ReflectorWiringMustBePaired()
    {
        // A three-cycle rather than pairs: the reflector would not be its own inverse.
        Assert.Throws<ArgumentException>(
            () => WiringTable.FromReflectorString("BCAEFDHIGKLJNOMQRPTUSWXVZY"));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("C")]
    [InlineData("B-Thin")]
    [InlineData("C-Thin")]
    public void EveryPackagedReflectorIsPairedWithNoFixedPoint(string name)
    {
        IReflector reflector = name switch
        {
            "A" => new ReflectorA(),
            "B" => new ReflectorB(),
            "C" => new ReflectorC(),
            "B-Thin" => new ReflectorBThin(),
            _ => new ReflectorCThin()
        };

        for (var contact = 0; contact < 26; contact++)
        {
            var output = reflector.Translate(contact);

            Assert.NotEqual(contact, output);
            Assert.Equal(contact, reflector.Translate(output));
        }
    }
}
