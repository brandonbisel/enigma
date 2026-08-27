using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

public class ServiceCollectionExtensionsTests
{
    private static ServiceProvider BuildProvider() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider();

    [Theory]
    [InlineData("I")]
    [InlineData("II")]
    [InlineData("III")]
    [InlineData("IV")]
    [InlineData("V")]
    [InlineData("VI")]
    [InlineData("VII")]
    [InlineData("VIII")]
    public void EveryRotorResolvesUnderTheNameOnAKeySheet(string name)
    {
        using var provider = BuildProvider();

        var rotor = provider.GetRequiredKeyedService<IRotor>(name);

        Assert.Equal(name, rotor.Name);
    }

    [Theory]
    [InlineData("B")]
    [InlineData("C")]
    public void EveryReflectorResolvesUnderItsName(string name)
    {
        using var provider = BuildProvider();

        Assert.Equal(name, provider.GetRequiredKeyedService<IReflector>(name).Name);
    }

    [Fact]
    public void RotorsAreTransient_SoTwoMachinesNeverShareAPosition()
    {
        using var provider = BuildProvider();

        var first = provider.GetRequiredKeyedService<IRotor>("I");
        var second = provider.GetRequiredKeyedService<IRotor>("I");

        Assert.NotSame(first, second);

        first.Step();
        Assert.NotEqual(first.Position, second.Position);
    }

    [Fact]
    public void PlugBoardsAreTransient_SoPatchingDoesNotLeakBetweenMachines()
    {
        using var provider = BuildProvider();

        var first = provider.GetRequiredService<IPlugBoard>();
        var second = provider.GetRequiredService<IPlugBoard>();

        Assert.NotSame(first, second);

        first.Connect(0, 25);
        Assert.False(second.IsConnected(0, 25));
    }

    [Fact]
    public void ReflectorsAreSingletons()
    {
        using var provider = BuildProvider();

        Assert.Same(
            provider.GetRequiredKeyedService<IReflector>("B"),
            provider.GetRequiredKeyedService<IReflector>("B"));
    }

    [Fact]
    public void TheAdvertisedPartsAreExactlyThePartsRegistered()
    {
        // MachineParts drives the error messages, so a part listed there but never
        // registered would name a rotor nobody can actually use, and the reverse
        // would hide one that works.
        var services = new ServiceCollection().AddEnigmaServices();

        var rotors = services
            .Where(service => service.ServiceType == typeof(IRotor))
            .Select(service => service.ServiceKey!.ToString()!)
            .ToHashSet();

        var reflectors = services
            .Where(service => service.ServiceType == typeof(IReflector))
            .Select(service => service.ServiceKey!.ToString()!)
            .ToHashSet();

        Assert.Equal(MachineParts.RotorNames.ToHashSet(), rotors);
        Assert.Equal(MachineParts.ReflectorNames.ToHashSet(), reflectors);
    }

    [Fact]
    public void EveryAdvertisedRotorResolves()
    {
        using var provider = BuildProvider();

        Assert.All(
            MachineParts.RotorNames,
            name => Assert.NotNull(provider.GetKeyedService<IRotor>(name)));
    }

    [Fact]
    public void CharacterMapResolves()
    {
        using var provider = BuildProvider();

        Assert.Equal(26, provider.GetRequiredService<ICharacterMap>().Count);
    }
}
