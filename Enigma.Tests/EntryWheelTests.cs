using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

public class EntryWheelTests
{
    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    private static IEnigmaMachineFactory Factory() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();

    private static KeySheet Sheet(string entryWheel) => new()
    {
        Reflector = "B", Rotors = "I II III", RingSettings = "AAA",
        Positions = "AAA", EntryWheel = entryWheel
    };

    private static string Encipher(IEnigmaMachine machine, string text) =>
        string.Concat(machine.Translate(text.Select(CharacterMap.GetIndex))
            .Select(CharacterMap.GetCharacter));

    [Fact]
    public void TheStandardWheelIsWiredStraightThrough()
    {
        Assert.All(
            Enumerable.Range(0, 26),
            letter =>
            {
                Assert.Equal(letter, EntryWheel.Standard.ToContact(letter));
                Assert.Equal(letter, EntryWheel.Standard.ToLamp(letter));
            });
    }

    [Fact]
    public void TheStandardWheelLeavesTheKnownVectorAlone()
    {
        Assert.Equal("BDZGO", Encipher(Factory().Create(Sheet("Standard")), "AAAAA"));
    }

    [Fact]
    public void AMachineWithNoEntryWheelNamedUsesTheStandardOne()
    {
        var sheet = Sheet(string.Empty);

        Assert.Equal("BDZGO", Encipher(Factory().Create(sheet), "AAAAA"));
    }

    [Fact]
    public void TheKeyboardWheelWiresTheFirstKeyToTheFirstContact()
    {
        // "QWERTZU..." read left to right: Q is wired to contact A, W to B.
        Assert.Equal(0, EntryWheel.Qwertz.ToContact(CharacterMap.GetIndex('Q')));
        Assert.Equal(1, EntryWheel.Qwertz.ToContact(CharacterMap.GetIndex('W')));
        Assert.Equal(CharacterMap.GetIndex('Q'), EntryWheel.Qwertz.ToLamp(0));
    }

    [Fact]
    public void EveryEntryWheelIsItsOwnInverseBetweenKeyAndContact()
    {
        Assert.All(
            Enumerable.Range(0, 26),
            letter => Assert.Equal(letter, EntryWheel.Qwertz.ToLamp(EntryWheel.Qwertz.ToContact(letter))));
    }

    [Fact]
    public void AKeyboardWiredWheelChangesTheCipher()
    {
        var standard = Encipher(Factory().Create(Sheet("Standard")), "AAAAA");
        var qwertz = Encipher(Factory().Create(Sheet("QWERTZ")), "AAAAA");

        Assert.NotEqual(standard, qwertz);
    }

    [Fact]
    public void AKeyboardWiredMachineIsStillReciprocal()
    {
        var cipher = Encipher(Factory().Create(Sheet("QWERTZ")), "ATTACKATDAWN");

        Assert.Equal("ATTACKATDAWN", Encipher(Factory().Create(Sheet("QWERTZ")), cipher));
    }

    [Fact]
    public void AKeyboardWiredMachineStillNeverEnciphersALetterToItself()
    {
        var message = string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 4));
        var cipher = Encipher(Factory().Create(Sheet("QWERTZ")), message);

        Assert.All(message.Zip(cipher), pair => Assert.NotEqual(pair.First, pair.Second));
    }

    [Fact]
    public void AnUnknownEntryWheelIsReported()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Factory().Create(Sheet("Typewriter")));

        // Part names are normalised before lookup, so the message echoes the key
        // that was searched for.
        Assert.Contains("TYPEWRITER", exception.Message);
        Assert.Contains("QWERTZ", exception.Message);
    }
}
