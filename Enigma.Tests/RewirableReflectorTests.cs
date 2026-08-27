using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Reflectors;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// UKW-D could be rewired in the field, which made the reflector part of the key.
/// Thirteen wires have to cover all twenty six contacts: any letter left unwired
/// would have nowhere to go.
/// </summary>
public class RewirableReflectorTests
{
    private const string Pairs = "AQ BG CK DI EL FX HZ MW NV OT PU RS JY";

    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    private static IEnigmaMachineFactory Factory() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();

    private static KeySheet Sheet(string pairs) => new()
    {
        Reflector = "D", ReflectorPairs = pairs,
        Rotors = "I II III", RingSettings = "AAA", Positions = "AAA"
    };

    private static string Encipher(IEnigmaMachine machine, string text) =>
        string.Concat(machine.Translate(text.Select(CharacterMap.GetIndex))
            .Select(CharacterMap.GetCharacter));

    [Fact]
    public void ThirteenPairsWireEveryContact()
    {
        var reflector = new RewirableReflector("D", Pairs);

        for (var contact = 0; contact < 26; contact++)
        {
            var output = reflector.Translate(contact);

            Assert.NotEqual(contact, output);
            Assert.Equal(contact, reflector.Translate(output));
        }
    }

    [Fact]
    public void FewerThanThirteenPairsIsRefused()
    {
        // Twelve pairs leave two letters unwired.
        var exception = Assert.Throws<ArgumentException>(
            () => new RewirableReflector("D", "AQ BG CK DI EL FX HZ MW NV OT PU RS"));

        Assert.Contains("13 pairs", exception.Message);
    }

    [Fact]
    public void AReusedLetterIsRefused()
    {
        Assert.Throws<ArgumentException>(
            () => new RewirableReflector("D", "AQ AG CK DI EL FX HZ MW NV OT PU RS JY"));
    }

    [Fact]
    public void ALetterWiredToItselfIsRefused()
    {
        Assert.Throws<ArgumentException>(
            () => new RewirableReflector("D", "AA BG CK DI EL FX HZ MW NV OT PU RS JY"));
    }

    [Fact]
    public void AMachineWithARewiredReflectorIsStillReciprocal()
    {
        var cipher = Encipher(Factory().Create(Sheet(Pairs)), "ATTACKATDAWN");

        Assert.Equal("ATTACKATDAWN", Encipher(Factory().Create(Sheet(Pairs)), cipher));
    }

    [Fact]
    public void ARewiredReflectorStillNeverEnciphersALetterToItself()
    {
        var message = string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 4));
        var cipher = Encipher(Factory().Create(Sheet(Pairs)), message);

        Assert.All(message.Zip(cipher), pair => Assert.NotEqual(pair.First, pair.Second));
    }

    [Fact]
    public void RewiringTheReflectorChangesTheCipher()
    {
        var one = Encipher(Factory().Create(Sheet(Pairs)), "AAAAA");
        var other = Encipher(
            Factory().Create(Sheet("AQ BG CK DI EL FX HZ MW NV OT PS RU JY")), "AAAAA");

        Assert.NotEqual(one, other);
    }

    [Fact]
    public void PairsMayBeWrittenWithHyphens()
    {
        var reflector = new RewirableReflector("D", "AQ-BG-CK-DI-EL-FX-HZ-MW-NV-OT-PU-RS-JY");

        Assert.Equal(CharacterMap.GetIndex('Q'), reflector.Translate(CharacterMap.GetIndex('A')));
    }
}
