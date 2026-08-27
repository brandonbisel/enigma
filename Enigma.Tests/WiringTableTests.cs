namespace Enigma.Tests;

public class WiringTableTests
{
    [Fact]
    public void FromString_MapsEachContactToItsLetter()
    {
        var wiring = WiringTable.FromString("BDFHJLCPRTXVZNYEIWGAKMUSQO");

        Assert.Equal(1, wiring[0]);  // A -> B
        Assert.Equal(3, wiring[1]);  // B -> D
        Assert.Equal(14, wiring[25]); // Z -> O
    }

    [Fact]
    public void FromString_RejectsWiringThatIsNotAPermutation()
    {
        // Two contacts wired to A, which would leave a letter unreachable.
        var exception = Assert.Throws<ArgumentException>(
            () => WiringTable.FromString("ABCDEFGHIJKLMNOPQRSTUVWXYA"));

        Assert.Equal("wiring", exception.ParamName);
    }

    [Fact]
    public void Invert_ReversesEveryMapping()
    {
        var wiring = WiringTable.FromString("EKMFLGDQVZNTOWYHXUSPAIBRCJ");
        var inverse = WiringTable.Invert(wiring);

        Assert.Equal(wiring.Count, inverse.Count);
        Assert.All(wiring, pair => Assert.Equal(pair.Key, inverse[pair.Value]));
    }
}
