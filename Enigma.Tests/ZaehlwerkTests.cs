using Enigma.Extensions.DependencyInjection;
using Enigma.Machines;
using Enigma.Models;
using Enigma.Reflectors;
using Enigma.Rotors;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The Zählwerk Enigma G31. Its wheels are turned by cogs rather than pawls, so
/// there is no double step; its reflector is both set and driven; and it has no
/// plugboard at all. See the README for where the wirings and the drive come from.
/// </summary>
public class ZaehlwerkTests
{
    private static readonly ICharacterMap Alphabet = new DefaultCharacterMap();

    private static IEnigmaMachineFactory Factory() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();

    private static KeySheet Sheet(
        string positions = "AAA",
        string rings = "AAA",
        string ukwPosition = "A",
        string ukwRing = "A") => new()
    {
        Model = "G-31",
        Reflector = "G",
        Rotors = "G-I G-II G-III",
        RingSettings = rings,
        Positions = positions,
        ReflectorPosition = ukwPosition,
        ReflectorRingSetting = ukwRing
    };

    private static string Encipher(IEnigmaMachine machine, string text) =>
        string.Concat(machine.Translate(text.Select(Alphabet.GetIndex)).Select(Alphabet.GetCharacter));

    // Produced by an independent implementation of the drive, transcribed from the
    // reference simulator's semantics rather than from this library's code.
    [Fact]
    public void MatchesTheReferenceOnASixtyCharacterRun()
    {
        const string expected =
            "HKUGGVKFIWIZVBGSBIUXOPBBONTLCQTKUZOBFJKNVBWSMDTPWWFVKGFULNKI";

        Assert.Equal(expected, Encipher(Factory().Create(Sheet()), new string('A', 60)));
    }

    [Fact]
    public void MatchesTheReferenceWithEveryWheelAndTheReflectorSet()
    {
        var sheet = Sheet(positions: "ABC", rings: "DEF", ukwPosition: "G", ukwRing: "H");

        Assert.Equal("KVRSYBVMSPLS", Encipher(Factory().Create(sheet), "ATTACKATDAWN"));
    }

    [Fact]
    public void TheMiddleWheelAdvancesOnlyOnACarry_NeverTwice()
    {
        // The defining difference from a pawl machine. The fast wheel here has
        // eleven notches, so the middle wheel advances eleven times in a
        // revolution — and never twice in successive keypresses.
        var machine = Factory().Create(Sheet());
        var wheels = machine.Rotors.ToList();
        var middle = wheels[1];
        var fast = wheels[2];

        var doubleSteps = 0;
        var advances = 0;
        var previouslyAdvanced = false;

        for (var i = 0; i < 26; i++)
        {
            var before = middle.Position;
            var fastWasOnNotch = fast.IsTurnoverPosition();

            machine.Translate(0);

            var advanced = middle.Position != before;

            if (advanced)
            {
                advances++;
                if (previouslyAdvanced && !fastWasOnNotch)
                {
                    doubleSteps++;
                }
            }

            // The middle wheel moves only when the fast wheel was on a notch.
            Assert.Equal(fastWasOnNotch, advanced);
            previouslyAdvanced = advanced;
        }

        Assert.Equal(11, advances);
        Assert.Equal(0, doubleSteps);
    }

    [Fact]
    public void TheReflectorIsDrivenDuringEncipherment()
    {
        // "The UKW can be moved by wheel 3" — the leftmost wheel ends the carry chain.
        var machine = Factory().Create(Sheet());
        var reflector = Assert.IsAssignableFrom<IRotatingReflector>(machine.Reflector);

        var start = reflector.Position;
        machine.Translate(Enumerable.Repeat(0, 500).ToList());

        Assert.NotEqual(start, reflector.Position);
    }

    [Fact]
    public void AServiceMachineNeverMovesItsReflector()
    {
        var machine = Factory().Create(new KeySheet
        {
            Reflector = "B", Rotors = "I II III", RingSettings = "AAA", Positions = "AAA"
        });

        Assert.IsNotAssignableFrom<IRotatingReflector>(machine.Reflector);
    }

    [Fact]
    public void SettingTheReflectorChangesTheCipher()
    {
        var atA = Encipher(Factory().Create(Sheet()), "AAAAA");
        var atB = Encipher(Factory().Create(Sheet(ukwPosition: "B")), "AAAAA");

        Assert.NotEqual(atA, atB);
    }

    [Fact]
    public void ItIsStillReciprocalAndNeverEnciphersALetterToItself()
    {
        var message = string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 8));

        var cipher = Encipher(Factory().Create(Sheet(positions: "QNX", rings: "BUL")), message);
        var back = Encipher(Factory().Create(Sheet(positions: "QNX", rings: "BUL")), cipher);

        Assert.Equal(message, back);
        Assert.All(message.Zip(cipher), pair => Assert.NotEqual(pair.First, pair.Second));
    }

    [Fact]
    public void ItUsesTheKeyboardWiredEntryWheelWithoutBeingTold()
    {
        // A key sheet that names no entry wheel gets the one its model used, so
        // saying nothing must equal saying QWERTZ, and must differ from Standard.
        Assert.Equal(
            Encipher(Factory().Create(Sheet()), "AAAAA"),
            Encipher(Factory().Create(WithEntryWheel(Sheet(), "QWERTZ")), "AAAAA"));
        Assert.NotEqual(
            Encipher(Factory().Create(Sheet()), "AAAAA"),
            Encipher(Factory().Create(WithEntryWheel(Sheet(), "Standard")), "AAAAA"));
    }

    private static KeySheet WithEntryWheel(KeySheet sheet, string entryWheel)
    {
        sheet.EntryWheel = entryWheel;
        return sheet;
    }

    [Fact]
    public void AZaehlwerkMachineHasNoPlugboard()
    {
        var sheet = Sheet();
        sheet.Plugboard = "AV BS";

        var exception = Assert.Throws<ArgumentException>(() => Factory().Create(sheet));

        Assert.Contains("no plugboard", exception.Message);
    }

    [Fact]
    public void AFixedReflectorIsRefused()
    {
        var sheet = Sheet();
        sheet.Reflector = "B";

        Assert.Throws<ArgumentException>(() => Factory().Create(sheet));
    }

    [Fact]
    public void AFourthWheelIsRefused()
    {
        var sheet = Sheet();
        sheet.Rotors = "G-I G-II G-III G-I";
        sheet.Positions = "AAAA";
        sheet.RingSettings = "AAAA";

        Assert.Throws<ArgumentException>(() => Factory().Create(sheet));
    }

    [Theory]
    [InlineData("G-I", 17)]
    [InlineData("G-II", 15)]
    [InlineData("G-III", 11)]
    public void TheWheelsCarryTheirDocumentedNotchCounts(string name, int expected)
    {
        IRotor rotor = name switch
        {
            "G-I" => new RotorGI(),
            "G-II" => new RotorGII(),
            _ => new RotorGIII()
        };

        Assert.Equal(expected, rotor.GetTurnoverPositions().Distinct().Count());
    }

    [Fact]
    public void TheNotchCountsAreRelativelyPrimeToTheAlphabet()
    {
        // Which is why they were chosen: it stretches the machine's period.
        foreach (var notches in new[] { 17, 15, 11 })
        {
            Assert.Equal(1, GreatestCommonDivisor(notches, 26));
        }
    }

    [Fact]
    public void TheAbwehrWheelsAreWiredDifferentlyFromTheCommercialOnes()
    {
        var commercial = Encipher(Factory().Create(Sheet()), "AAAAA");

        var abwehr = Sheet();
        abwehr.Rotors = "G312-I G312-II G312-III";
        abwehr.Reflector = "G312";

        Assert.NotEqual(commercial, Encipher(Factory().Create(abwehr), "AAAAA"));
    }

    [Fact]
    public void TheDriveIsTheGearedOne()
    {
        var services = new ServiceCollection().AddEnigmaServices().BuildServiceProvider();
        var layout = services.GetRequiredService<IPartsCatalogue>().GetLayout("G-31");

        Assert.IsType<GearDrive>(layout.Drive);
        Assert.False(layout.AllowsPlugBoard);
        Assert.Equal("QWERTZ", layout.DefaultEntryWheel);
    }

    private static int GreatestCommonDivisor(int a, int b) => b == 0 ? a : GreatestCommonDivisor(b, a % b);
}
