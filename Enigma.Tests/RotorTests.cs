using Enigma.Rotors;

namespace Enigma.Tests;

public class RotorTests
{
    private const int AlphabetSize = 26;

    public static TheoryData<IRotor> AllRotors() =>
    [
        new RotorI(), new RotorII(), new RotorIII(), new RotorIV(),
        new RotorV(), new RotorVI(), new RotorVII(), new RotorVIII()
    ];

    [Theory]
    [MemberData(nameof(AllRotors))]
    public void Translate_StaysInRange_ForEveryPositionAndInput(IRotor rotor)
    {
        for (var position = 0; position < AlphabetSize; position++)
        {
            rotor.SetPosition(position);

            for (var input = 0; input < AlphabetSize; input++)
            {
                Assert.InRange(rotor.Translate(input), 0, AlphabetSize - 1);
                Assert.InRange(rotor.TranslateReverse(input), 0, AlphabetSize - 1);
            }
        }
    }

    [Theory]
    [MemberData(nameof(AllRotors))]
    public void TranslateReverse_UndoesTranslate_ForEveryPositionRingAndInput(IRotor rotor)
    {
        for (var ring = 0; ring < AlphabetSize; ring++)
        {
            rotor.SetRingSetting(ring);

            for (var position = 0; position < AlphabetSize; position++)
            {
                rotor.SetPosition(position);

                for (var input = 0; input < AlphabetSize; input++)
                {
                    Assert.Equal(input, rotor.TranslateReverse(rotor.Translate(input)));
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(AllRotors))]
    public void Translate_IsInjective_AtEveryPosition(IRotor rotor)
    {
        for (var position = 0; position < AlphabetSize; position++)
        {
            rotor.SetPosition(position);

            var outputs = Enumerable.Range(0, AlphabetSize).Select(rotor.Translate).ToList();

            Assert.Equal(AlphabetSize, outputs.Distinct().Count());
        }
    }

    [Fact]
    public void Translate_RollsOverRatherThanReturningANegativeValue()
    {
        // Position 1 pushes low outputs below zero before the wrap: rotor I maps
        // contact 1 to J (9), so the raw subtraction is 9 - 1 with no wrap, while
        // contact 20 maps to A (0) and lands at -1 before rolling over to 25.
        var rotor = new RotorI();
        rotor.SetPosition(1);

        Assert.All(
            Enumerable.Range(0, AlphabetSize),
            input => Assert.InRange(rotor.Translate(input), 0, AlphabetSize - 1));
    }

    [Fact]
    public void Step_WrapsBackToZeroAfterAFullRevolution()
    {
        var rotor = new RotorI();

        for (var i = 0; i < AlphabetSize; i++)
        {
            rotor.Step();
        }

        Assert.Equal(0, rotor.Position);
    }

    [Fact]
    public void IsTurnoverPosition_StillMatchesAfterAFullRevolution()
    {
        var rotor = new RotorI();
        rotor.SetPosition(16); // Q

        Assert.True(rotor.IsTurnoverPosition());

        for (var i = 0; i < AlphabetSize; i++)
        {
            rotor.Step();
        }

        Assert.True(rotor.IsTurnoverPosition());
    }

    [Fact]
    public void SetPosition_NormalisesValuesOutsideTheAlphabet()
    {
        var rotor = new RotorI();

        rotor.SetPosition(-1);
        Assert.Equal(25, rotor.Position);

        rotor.SetPosition(26);
        Assert.Equal(0, rotor.Position);
    }

    [Fact]
    public void RingSetting_CancelsOutAnEqualPosition()
    {
        // A rotor at position P with ring R behaves like one at position P - R
        // with no ring offset, so equal values leave the wiring where it started.
        var reference = new RotorI();
        var offset = new RotorI();

        offset.SetRingSetting(5);
        offset.SetPosition(5);

        for (var input = 0; input < AlphabetSize; input++)
        {
            Assert.Equal(reference.Translate(input), offset.Translate(input));
        }
    }

    [Fact]
    public void RingSetting_ShiftsTheWiringWhenItDoesNotCancel()
    {
        var reference = new RotorI();
        var ringed = new RotorI();

        ringed.SetRingSetting(3);

        var reference0 = reference.Translate(0);
        var ringed0 = ringed.Translate(0);

        Assert.NotEqual(reference0, ringed0);
    }

    private static IRotor CreateRotor(string name) => name switch
    {
        "I" => new RotorI(),
        "II" => new RotorII(),
        "III" => new RotorIII(),
        "IV" => new RotorIV(),
        "V" => new RotorV(),
        "VI" => new RotorVI(),
        "VII" => new RotorVII(),
        "VIII" => new RotorVIII(),
        _ => throw new ArgumentOutOfRangeException(nameof(name))
    };

    [Theory]
    [InlineData("I", 16)]
    [InlineData("II", 4)]
    [InlineData("III", 21)]
    [InlineData("IV", 9)]
    [InlineData("V", 25)]
    public void SingleNotchRotors_TurnOverAtTheirDocumentedLetter(string name, int expected)
    {
        Assert.Equal([expected], CreateRotor(name).GetTurnoverPositions());
    }

    [Theory]
    [InlineData("VI")]
    [InlineData("VII")]
    [InlineData("VIII")]
    public void NavalRotors_TurnOverAtBothMAndZ(string name)
    {
        Assert.Equal([12, 25], CreateRotor(name).GetTurnoverPositions().Order());
    }
}
