using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The commercial pattern with other wheels: the Swiss Air Force's rewired K, the
/// Railway Enigma, and the Enigma T built for traffic with the Japanese navy.
///
/// Wirings and notches from the Crypto Museum, agreeing exactly with Daniel
/// Palloks' Universal Enigma; vectors taken from that simulator beforehand. The
/// Railway wheels are the original wiring found in machine K438, not Bletchley
/// Park's wartime reconstruction, which has the turnovers of wheels I and III
/// swapped through a misidentification.
/// </summary>
public class CommercialVariantTests
{
    [Theory]
    [InlineData("SK", "INRKJYXUKUTPIILMLQRGDOFUMBXRTZEGTKITYTPM")]
    [InlineData("R", "GUOJJUOCTUBSDTWKNYWVXWHPFXRKUNRTWRHJGURX")]
    [InlineData("T", "WLZNVCRJQPPGBDVNXGMGJGXCCIUWORLZCKUOUKIT")]
    public void MatchesTheReferenceFromTheStart(string machine, string expected)
    {
        Assert.Equal(expected, Encipher(Sheet(machine), new string('A', 40)));
    }

    [Theory]
    [InlineData("SK", "WXGROZWYZBUHC")]
    [InlineData("R", "QAAOHNDVTGXCN")]
    [InlineData("T", "VSVZDJYXUWYDT")]
    public void WithTheWheelsAndTheReflectorSetElsewhere(string machine, string expected)
    {
        var sheet = Sheet(machine);
        sheet.Positions = "QCW";
        sheet.ReflectorPosition = "F";

        Assert.Equal(expected, Encipher(sheet, "ATTACKATDAWNX"));
    }

    [Theory]
    [InlineData("SK", "KGFWXTZKMXRWCHXEVPFGQJRQXYGXWX")]
    [InlineData("R", "IQCHYFYXCGTNPFPPHSQPFCIKUYSEZW")]
    [InlineData("T", "PXKKKOFGNPWILUNJZOVPNVWUNLKELW")]
    public void WithRingSettings(string machine, string expected)
    {
        var sheet = Sheet(machine);
        sheet.RingSettings = "DGK";
        sheet.ReflectorRingSetting = "B";

        Assert.Equal(expected, Encipher(sheet, new string('A', 30)));
    }

    [Fact]
    public void TheEnigmaTCarriesEightWheelsToChooseThreeFrom()
    {
        var sheet = Sheet("T");
        sheet.Rotors = "T-VIII T-V T-II";
        sheet.Positions = "MCK";

        Assert.Equal("RYOOTVULTXLLLKDPZLDPGFWRHNIPTH", Encipher(sheet, new string('A', 30)));
    }

    [Fact]
    public void TheEnigmaTHasAStatorOfItsOwnAndNeedNotBeToldSo()
    {
        // Forgetting it would encipher perfectly well and wrongly, so the model
        // supplies it rather than leaving it to the key sheet.
        var sheet = Sheet("T");
        sheet.EntryWheel = string.Empty;

        Assert.Equal("WLZNVCRJQPPGBDVNXGMGJGXCCIUWORLZCKUOUKIT",
            Encipher(sheet, new string('A', 40)));
    }

    [Fact]
    public void TheOtherCommercialMachinesAreKeyboardWired()
    {
        Assert.Equal("QWERTZ", Open(Sheet("SK")).Machine.Layout.DefaultEntryWheel);
        Assert.Equal("QWERTZ", Open(Sheet("R")).Machine.Layout.DefaultEntryWheel);
        Assert.Equal("TIRPITZ", Open(Sheet("T")).Machine.Layout.DefaultEntryWheel);
    }

    [Fact]
    public void EveryEnigmaTWheelHasFiveNotches()
    {
        // Which is what stretches the machine's period; the service wheels have one.
        var parts = new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IPartsCatalogue>();

        Assert.All(
            new[] { "T-I", "T-II", "T-III", "T-IV", "T-V", "T-VI", "T-VII", "T-VIII" },
            name => Assert.Equal(5, parts.CreateRotor(name).GetTurnoverPositions().Count()));
    }

    [Theory]
    [InlineData("SK")]
    [InlineData("R")]
    [InlineData("T")]
    public void TheReflectorIsSetButNeverDriven(string machine)
    {
        var session = Open(Sheet(machine));
        var start = session.ReflectorPosition;

        session.Type(new string('A', 2000));

        Assert.Equal(start, session.ReflectorPosition);
    }

    [Theory]
    [InlineData("SK")]
    [InlineData("R")]
    [InlineData("T")]
    public void ThereIsNoPlugboard(string machine)
    {
        Assert.False(Open(Sheet(machine)).HasPlugBoard);
    }

    [Theory]
    [InlineData("SK")]
    [InlineData("R")]
    [InlineData("T")]
    public void ItIsStillReciprocalAndNeverEnciphersALetterToItself(string machine)
    {
        var plain = "ATTACKATDAWNXTHEWEATHERISFINEXX";

        Assert.Equal(plain, Encipher(Sheet(machine), Encipher(Sheet(machine), plain)));

        var session = Open(Sheet(machine));

        Assert.All(plain, letter => Assert.NotEqual(letter, session.Press(letter)));
    }

    private static KeySheet Sheet(string machine) => new()
    {
        Name = machine,
        Model = machine == "T" ? "Tirpitz" : "Commercial",
        Reflector = machine switch { "SK" => "G", "R" => "R", _ => "T" },
        Rotors = $"{machine}-III {machine}-II {machine}-I",
        RingSettings = "AAA",
        Positions = "AAA",
        ReflectorPosition = "A",
        ReflectorRingSetting = "A"
    };

    private static string Encipher(KeySheet sheet, string text) => Open(sheet).Type(text);

    private static EnigmaSession Open(KeySheet sheet)
    {
        var result = EnigmaSession.Open(Factory(), sheet);

        Assert.True(result.Succeeded, result.Error);

        return result.Session!;
    }

    private static IEnigmaMachineFactory Factory() =>
        new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();
}
