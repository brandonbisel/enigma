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

    private static EnigmaSettings DefaultSettings() => new()
    {
        Name = "Test",
        Reflector = "B",
        Rotors =
        [
            new RotorSettings { Name = "I" },
            new RotorSettings { Name = "II" },
            new RotorSettings { Name = "III" }
        ]
    };

    [Fact]
    public void CreateBuildsTheMachineDescribedBySettings()
    {
        var machine = BuildFactory().Create(DefaultSettings());

        Assert.Equal(["I", "II", "III"], machine.Rotors.Select(rotor => rotor.Name));
        Assert.Equal("B", machine.Reflector.Name);
    }

    [Fact]
    public void CreateAppliesPositionAndRingSettingToEachRotor()
    {
        var settings = DefaultSettings();
        settings.Rotors[0].Position = 4;
        settings.Rotors[0].RingSetting = 7;

        var machine = BuildFactory().Create(settings);
        var left = machine.Rotors.First();

        Assert.Equal(4, left.Position);
        Assert.Equal(7, left.RingSetting);
    }

    [Fact]
    public void CreatePatchesThePlugBoard()
    {
        var settings = DefaultSettings();
        settings.Plugboard = new Dictionary<int, int> { [0] = 25 };

        var machine = BuildFactory().Create(settings);

        Assert.True(machine.PlugBoard.IsConnected(0, 25));
    }

    [Fact]
    public void CreateProducesIndependentMachines()
    {
        var factory = BuildFactory();

        var first = factory.Create(DefaultSettings());
        var second = factory.Create(DefaultSettings());

        first.Translate(0);

        Assert.NotEqual(
            first.Rotors.Last().Position,
            second.Rotors.Last().Position);
    }

    [Fact]
    public void CreateRejectsAnUnknownRotorName()
    {
        var settings = DefaultSettings();
        settings.Rotors[0].Name = "XI";

        Assert.Throws<InvalidOperationException>(() => BuildFactory().Create(settings));
    }

    [Fact]
    public void CreateRejectsAnUnknownReflectorName()
    {
        var settings = DefaultSettings();
        settings.Reflector = "D";

        Assert.Throws<InvalidOperationException>(() => BuildFactory().Create(settings));
    }

    [Fact]
    public void AMachineBuiltFromSettingsMatchesTheKnownVector()
    {
        var machine = BuildFactory().Create(DefaultSettings());
        var characterMap = new DefaultCharacterMap();

        var input = "AAAAA".Select(characterMap.GetIndex);
        var output = string.Concat(machine.Translate(input).Select(characterMap.GetCharacter));

        Assert.Equal("BDZGO", output);
    }
}
