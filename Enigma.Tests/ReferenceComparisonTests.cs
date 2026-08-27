using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Tests.Reference;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

public class ReferenceComparisonTests
{
    private static readonly string[] RotorNames =
        ["I", "II", "III", "IV", "V", "VI", "VII", "VIII"];

    private static readonly string[] ReflectorNames = ["B", "C"];

    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    private static IEnigmaMachineFactory BuildFactory() =>
        new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();

    // The oracle is only worth comparing against once it reproduces the published
    // vectors itself, so these three anchor it before the differential tests run.

    [Fact]
    public void Reference_ReproducesTheFiveCharacterKnownVector()
    {
        var reference = new ReferenceEnigma(["I", "II", "III"], "B", [0, 0, 0], [0, 0, 0]);

        Assert.Equal("BDZGO", reference.Encrypt(new string('A', 5)));
    }

    [Fact]
    public void Reference_ReproducesTheTwentySixCharacterKnownVector()
    {
        var reference = new ReferenceEnigma(["I", "II", "III"], "B", [0, 0, 0], [0, 0, 0]);

        Assert.Equal("BDZGOWCXLTKSBTMCDLPBMUQOFX", reference.Encrypt(new string('A', 26)));
    }

    [Fact]
    public void Reference_AgreesWithTheImplementationAcrossTheDoubleStep()
    {
        var reference = new ReferenceEnigma(["I", "II", "III"], "B", [0, 0, 0], [0, 0, 0]);
        var machine = BuildFactory().Create(Settings(["I", "II", "III"], "B", [0, 0, 0], [0, 0, 0]));

        var expected = reference.Encrypt(new string('A', 150));

        Assert.Equal(expected, Encrypt(machine, new string('A', 150)));
        Assert.Equal([1, 6, 20], reference.Positions);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Implementation_AgreesWithTheReferenceAcrossRandomConfigurations(int seed)
    {
        // A fixed seed per case keeps a failure reproducible while still covering
        // rotor choice and order, ring settings, start positions, plugboards,
        // both reflectors, and message lengths that cross several turnovers.
        var random = new Random(seed);
        var factory = BuildFactory();

        for (var iteration = 0; iteration < 50; iteration++)
        {
            var rotors = RotorNames.OrderBy(_ => random.Next()).Take(3).ToArray();
            var reflector = ReflectorNames[random.Next(ReflectorNames.Length)];
            var positions = Enumerable.Range(0, 3).Select(_ => random.Next(26)).ToArray();
            var rings = Enumerable.Range(0, 3).Select(_ => random.Next(26)).ToArray();
            var plugs = RandomPlugs(random);
            var message = RandomMessage(random);

            var reference = new ReferenceEnigma(rotors, reflector, positions, rings, plugs);
            var settings = Settings(rotors, reflector, positions, rings, plugs);
            var machine = factory.Create(settings);

            var expected = reference.Encrypt(message);
            var actual = Encrypt(machine, message);

            Assert.True(
                expected == actual,
                $"""
                 Diverged from the reference implementation.
                   rotors    {string.Join(' ', rotors)}
                   reflector {reflector}
                   positions {string.Join(' ', positions)}
                   rings     {string.Join(' ', rings)}
                   plugs     {string.Join(' ', plugs.Select(pair => $"{pair.Item1}-{pair.Item2}"))}
                   message   {message}
                   expected  {expected}
                   actual    {actual}
                 """);

            Assert.Equal(reference.Positions, machine.Rotors.Select(rotor => rotor.Position));
        }
    }

    private static List<(int, int)> RandomPlugs(Random random)
    {
        var letters = Enumerable.Range(0, 26).OrderBy(_ => random.Next()).ToList();
        var count = random.Next(0, 11);

        return Enumerable.Range(0, count)
            .Select(i => (letters[i * 2], letters[i * 2 + 1]))
            .ToList();
    }

    private static string RandomMessage(Random random)
    {
        var length = random.Next(1, 300);

        return string.Concat(
            Enumerable.Range(0, length).Select(_ => (char)('A' + random.Next(26))));
    }

    private static KeySheet Settings(
        IReadOnlyList<string> rotors,
        string reflector,
        IReadOnlyList<int> positions,
        IReadOnlyList<int> rings,
        IEnumerable<(int, int)>? plugs = null) => new()
    {
        Name = "Differential",
        Reflector = reflector,
        Rotors = string.Join(' ', rotors),
        Positions = string.Concat(positions.Select(position => (char)('A' + position))),
        RingSettings = string.Concat(rings.Select(ring => (char)('A' + ring))),
        Plugboard = string.Join(' ', (plugs ?? [])
            .Select(pair => $"{(char)('A' + pair.Item1)}{(char)('A' + pair.Item2)}"))
    };

    private static string Encrypt(IEnigmaMachine machine, string text)
    {
        var input = text.Select(CharacterMap.GetIndex);

        return string.Concat(machine.Translate(input).Select(CharacterMap.GetCharacter));
    }
}
