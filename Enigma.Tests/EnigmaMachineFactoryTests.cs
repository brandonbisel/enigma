using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

public class EnigmaMachineFactoryTests
{
    private static IEnigmaMachineFactory BuildFactory() =>
        new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();

    private static KeySheet DefaultKeySheet() => new()
    {
        Name = "Test",
        Reflector = "B",
        Rotors = "I II III",
        RingSettings = "AAA",
        Positions = "AAA"
    };

    [Fact]
    public void CreateBuildsTheMachineDescribedByTheKeySheet()
    {
        var machine = BuildFactory().Create(DefaultKeySheet());

        Assert.Equal(["I", "II", "III"], machine.Rotors.Select(rotor => rotor.Name));
        Assert.Equal("B", machine.Reflector.Name);
    }

    [Fact]
    public void CreateAppliesGrundstellungAndRingstellungToEachRotor()
    {
        var keySheet = DefaultKeySheet();
        keySheet.Positions = "EAA";      // E is the fifth letter, so index 4
        keySheet.RingSettings = "HAA";   // H is the eighth, so index 7

        var left = BuildFactory().Create(keySheet).Rotors.First();

        Assert.Equal(4, left.Position);
        Assert.Equal(7, left.RingSetting);
    }

    [Fact]
    public void CreateAcceptsTheNumbersPrintedOnAKeySheet()
    {
        var keySheet = DefaultKeySheet();
        keySheet.RingSettings = "01 08 26";

        var rotors = BuildFactory().Create(keySheet).Rotors.ToList();

        Assert.Equal([0, 7, 25], rotors.Select(rotor => rotor.RingSetting));
    }

    [Fact]
    public void CreatePatchesThePlugBoard()
    {
        var keySheet = DefaultKeySheet();
        keySheet.Plugboard = "AZ BC";

        var machine = BuildFactory().Create(keySheet);

        Assert.True(machine.PlugBoard.IsConnected(0, 25));
        Assert.True(machine.PlugBoard.IsConnected(1, 2));
    }

    [Fact]
    public void CreateProducesIndependentMachines()
    {
        var factory = BuildFactory();

        var first = factory.Create(DefaultKeySheet());
        var second = factory.Create(DefaultKeySheet());

        first.Translate(0);

        Assert.NotEqual(first.Rotors.Last().Position, second.Rotors.Last().Position);
    }

    [Fact]
    public void CreateRejectsAnUnknownRotorName()
    {
        var keySheet = DefaultKeySheet();
        keySheet.Rotors = "XI II III";

        var exception = Assert.Throws<ArgumentException>(() => BuildFactory().Create(keySheet));

        Assert.Contains("XI", exception.Message);
        Assert.Contains("VIII", exception.Message);   // it lists what is available
    }

    [Fact]
    public void CreateRejectsAnUnknownReflectorName()
    {
        var keySheet = DefaultKeySheet();
        keySheet.Reflector = "D";

        var exception = Assert.Throws<ArgumentException>(() => BuildFactory().Create(keySheet));

        Assert.Contains("'D'", exception.Message);
        Assert.Contains("B-THIN", exception.Message);
    }

    [Fact]
    public void ARotorNameMayContainAHyphen()
    {
        // "K-I" is one rotor, not a "K" and an "I". The hyphen separates plugboard
        // pairs, but never rotor names. Parsed directly, since the library ships
        // no hyphenated wheel of its own.
        var keySheet = DefaultKeySheet();
        keySheet.Rotors = "K-I K-II K-III";

        Assert.Equal(
            ["K-I", "K-II", "K-III"],
            keySheet.Wheels().Select(wheel => wheel.Name));
    }

    [Fact]
    public void PlugboardPairsMayBeSeparatedByHyphens()
    {
        var keySheet = DefaultKeySheet();
        keySheet.Plugboard = "AZ-BC";

        var machine = BuildFactory().Create(keySheet);

        Assert.True(machine.PlugBoard.IsConnected(0, 25));
        Assert.True(machine.PlugBoard.IsConnected(1, 2));
    }

    [Fact]
    public void AMachineBuiltFromAKeySheetMatchesTheKnownVector()
    {
        var machine = BuildFactory().Create(DefaultKeySheet());
        var characterMap = new DefaultCharacterMap();

        var input = "AAAAA".Select(characterMap.GetIndex);
        var output = string.Concat(machine.Translate(input).Select(characterMap.GetCharacter));

        Assert.Equal("BDZGO", output);
    }
}
