using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The thin rotors are half width and only fit in the space a thin reflector
/// frees, so a machine either has three full wheels and a full reflector, or four
/// wheels with a thin one leftmost and a thin reflector. Nothing else was buildable.
/// </summary>
public class MachineFitmentTests
{
    private static IEnigmaMachineFactory Factory() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();

    private static KeySheet Sheet(string reflector, string rotors, string wheels) => new()
    {
        Reflector = reflector, Rotors = rotors, RingSettings = wheels, Positions = wheels
    };

    [Fact]
    public void AThinRotorIsRefusedInAThreeWheelMachine()
    {
        Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("B", "I II Beta", "AAA")));
    }

    [Fact]
    public void AFourthRotorIsRefusedWithAFullWidthReflector()
    {
        Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("B", "Beta II IV I", "AAAA")));
    }

    [Fact]
    public void AThinReflectorIsRefusedWithoutAFourthRotor()
    {
        Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("B-Thin", "I II III", "AAA")));
    }

    [Fact]
    public void TheThinRotorMustSitLeftmost()
    {
        Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("B-Thin", "I Beta II III", "AAAA")));
    }

    [Fact]
    public void TwoThinRotorsAreRefused()
    {
        Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("B-Thin", "Beta Gamma II III", "AAAA")));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    public void OnlyThreeOrFourWheelsWereEverBuilt(int count)
    {
        var rotors = string.Join(' ', new[] { "I", "II", "III", "IV", "V" }.Take(count));

        Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("B", rotors, new string('A', count))));
    }

    [Fact]
    public void AThinRotorRefusesToStepEvenWhenStepIsCalledOnItDirectly()
    {
        // No ratchet, so nothing turns it, whoever asks.
        var beta = new Rotors.RotorBeta();

        Assert.Equal(0, beta.Step());
        Assert.Equal(0, beta.Position);
    }

    [Fact]
    public void AThinRotorCanStillBeSetByHand()
    {
        // An operator set the fourth wheel before closing the lid; only the pawl
        // is defeated by the missing ratchet.
        var beta = new Rotors.RotorBeta();

        beta.SetPosition(17);

        Assert.Equal(17, beta.Position);
    }

    [Fact]
    public void AThinRotorHasNoNotchSoItNeverDrivesItsNeighbour()
    {
        var beta = new Rotors.RotorBeta();

        Assert.Empty(beta.GetTurnoverPositions());

        for (var position = 0; position < 26; position++)
        {
            beta.SetPosition(position);
            Assert.False(beta.IsTurnoverPosition());
        }
    }

    [Fact]
    public void AThinRotorIsNeverDrivenEvenIfItIsPutInADrivenSlot()
    {
        // Built directly, bypassing the fitment rules: the mechanism itself must
        // still refuse to turn a wheel that has no ratchet.
        var characterMap = new DefaultCharacterMap();
        var beta = new Rotors.RotorBeta();

        var machine = new EnigmaMachine(
            new PlugBoard(characterMap),
            [new Rotors.RotorI(), new Rotors.RotorII(), beta],
            new Reflectors.ReflectorB());

        for (var i = 0; i < 100; i++)
        {
            machine.Translate(0);
        }

        Assert.Equal(0, beta.Position);
    }

    [Fact]
    public void AnM4WithItsThinRotorAtAIsTheSameMachineAsAnM3()
    {
        // This equivalence is what let an M4 exchange traffic with an M3.
        var m4 = Factory().Create(Sheet("B-Thin", "Beta I II III", "AAAA"));
        var m3 = Factory().Create(Sheet("B", "I II III", "AAA"));

        Assert.Equal(
            m3.Translate(Enumerable.Repeat(0, 60)),
            m4.Translate(Enumerable.Repeat(0, 60)));
    }
}
