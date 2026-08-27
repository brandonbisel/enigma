using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Parts;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The alphabet is the single authority for how many contacts a machine has. Every
/// service Enigma used twenty six capital letters, but nothing in the mechanism
/// requires that.
/// </summary>
public class CharacterSetTests
{
    // A ten character machine: small enough to reason about, even so a reflector
    // can be wired in pairs.
    private static readonly PartsFile TenLetterMachine = new()
    {
        CharacterMaps = [new CharacterMapDefinition { Name = "Ten", Characters = "ABCDEFGHIJ" }],
        Rotors =
        [
            new RotorDefinition { Name = "T-I", Wiring = "BDFHJACEGI", Notches = "E" },
            new RotorDefinition { Name = "T-II", Wiring = "CEGIABDFHJ", Notches = "C" },
            new RotorDefinition { Name = "T-III", Wiring = "DAFBGCHEIJ", Notches = "H" }
        ],
        Reflectors = [new ReflectorDefinition { Name = "T-UKW", Wiring = "FGHIJABCDE" }]
    };

    private static IEnigmaMachineFactory Factory(PartsFile? parts = null)
    {
        var services = new ServiceCollection().AddEnigmaServices();

        if (parts is not null)
        {
            services.AddSingleton<IPartsCatalogue>(provider => new PartsCatalogue(
                ActivatorUtilities.CreateInstance<BuiltInPartsCatalogue>(provider), parts));
        }

        return services.BuildServiceProvider().GetRequiredService<IEnigmaMachineFactory>();
    }

    private static KeySheet TenLetterSheet() => new()
    {
        CharacterMap = "Ten",
        Reflector = "T-UKW",
        Rotors = "T-I T-II T-III",
        RingSettings = "AAA",
        Positions = "AAA"
    };

    private static string Run(IEnigmaMachine machine, ICharacterMap alphabet, string text) =>
        string.Concat(machine.Translate(text.Select(alphabet.GetIndex)).Select(alphabet.GetCharacter));

    [Fact]
    public void AMachineCanWorkInAnAlphabetThatIsNotTwentySix()
    {
        var alphabet = new CharacterMap("Ten", "ABCDEFGHIJ");
        var factory = Factory(TenLetterMachine);

        // Every character has to be in the alphabet: "HELLO" would not be, since
        // this machine has no O.
        var cipher = Run(factory.Create(TenLetterSheet()), alphabet, "HEIIA");

        Assert.Equal(5, cipher.Length);
        Assert.All(cipher, character => Assert.InRange(character, 'A', 'J'));
    }

    [Fact]
    public void ASmallerAlphabetIsStillReciprocal()
    {
        var alphabet = new CharacterMap("Ten", "ABCDEFGHIJ");
        var factory = Factory(TenLetterMachine);

        const string message = "ABCDEFGHIJABCDEFGHIJ";
        var cipher = Run(factory.Create(TenLetterSheet()), alphabet, message);

        Assert.Equal(message, Run(factory.Create(TenLetterSheet()), alphabet, cipher));
    }

    [Fact]
    public void ASmallerAlphabetStillNeverEnciphersACharacterToItself()
    {
        var alphabet = new CharacterMap("Ten", "ABCDEFGHIJ");
        var factory = Factory(TenLetterMachine);

        var message = string.Concat(Enumerable.Repeat("ABCDEFGHIJ", 5));
        var cipher = Run(factory.Create(TenLetterSheet()), alphabet, message);

        Assert.All(message.Zip(cipher), pair => Assert.NotEqual(pair.First, pair.Second));
    }

    [Fact]
    public void WheelSettingsAreNumberedWithinTheAlphabet()
    {
        var sheet = TenLetterSheet();
        sheet.RingSettings = "01 05 10";

        var rotors = Factory(TenLetterMachine).Create(sheet).Rotors.ToList();

        Assert.Equal([0, 4, 9], rotors.Select(rotor => rotor.RingSetting));
    }

    [Fact]
    public void AWheelSettingOutsideTheAlphabetIsRefused()
    {
        var sheet = TenLetterSheet();
        sheet.RingSettings = "01 05 26";

        var exception = Assert.Throws<FormatException>(() => Factory(TenLetterMachine).Create(sheet));

        Assert.Contains("01 to 10", exception.Message);
    }

    [Fact]
    public void APartBuiltForADifferentAlphabetIsRefused()
    {
        // Rotor I is a twenty six contact wheel; this machine has ten.
        var sheet = TenLetterSheet();
        sheet.Rotors = "I T-II T-III";

        var exception = Assert.Throws<ArgumentException>(() => Factory(TenLetterMachine).Create(sheet));

        Assert.Contains("26 contacts", exception.Message);
        Assert.Contains("10 characters", exception.Message);
    }

    [Fact]
    public void AWiringOfTheWrongLengthIsRefused()
    {
        var alphabet = new CharacterMap("Ten", "ABCDEFGHIJ");

        var exception = Assert.Throws<ArgumentException>(
            () => WiringTable.FromString("EKMFLGDQVZNTOWYHXUSPAIBRCJ", alphabet));

        Assert.Contains("26 contacts", exception.Message);
    }

    [Fact]
    public void AnOddAlphabetCannotCarryAReflector()
    {
        var alphabet = new CharacterMap("Five", "ABCDE");

        var exception = Assert.Throws<ArgumentException>(
            () => WiringTable.FromReflectorString("BADEC", alphabet));

        Assert.Contains("cannot be wired in pairs", exception.Message);
    }

    [Fact]
    public void AnAlphabetNeedNotBeLetters()
    {
        var alphabet = new CharacterMap("Digits", "0123456789");

        Assert.Equal(0, alphabet.GetIndex('0'));
        Assert.Equal(9, alphabet.GetIndex('9'));
        Assert.Equal(-1, alphabet.GetIndex('A'));
        Assert.Equal('7', alphabet.GetCharacter(7));
    }

    [Fact]
    public void AnAlphabetMayNotRepeatACharacter()
    {
        Assert.Throws<ArgumentException>(() => new CharacterMap("Bad", "AABCDE"));
    }

    [Fact]
    public void AnUnknownAlphabetIsReported()
    {
        var sheet = TenLetterSheet();
        sheet.CharacterMap = "Cyrillic";

        var exception = Assert.Throws<ArgumentException>(() => Factory(TenLetterMachine).Create(sheet));

        Assert.Contains("CYRILLIC", exception.Message);
        Assert.Contains("TEN", exception.Message);
    }

    [Fact]
    public void TheDefaultAlphabetIsStillTheLatinOne()
    {
        var machine = Factory().Create(new KeySheet
        {
            Reflector = "B", Rotors = "I II III", RingSettings = "AAA", Positions = "AAA"
        });

        Assert.Equal("BDZGO", Run(machine, CharacterMap.Latin, "AAAAA"));
    }
}
