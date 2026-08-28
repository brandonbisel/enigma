using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// Two machines that are other machines rewired.
///
/// The Norenigma is an Enigma I: the Norwegian police security service kept theirs
/// after the war and gave it new wheels and a new reflector, leaving the plugboard,
/// the straight-through stator and the pawls alone. So it is a service machine, and
/// uses the service layout.
///
/// The KD is a commercial K with a rewirable UKW-D in place of its reflector. That
/// reflector has no setting at all, which is the one thing separating it from the
/// other commercial machines.
///
/// Vectors from Daniel Palloks' Universal Enigma, taken beforehand.
/// </summary>
public class NorenigmaAndKdTests
{
    [Fact]
    public void TheNorenigmaMatchesTheReferenceFromTheStart()
    {
        Assert.Equal(
            "QWSCMIJHVVVLRHXIGXCWODDWUWZSJQWVFSKKXNQF",
            Encipher(Norenigma(), new string('A', 40)));
    }

    [Fact]
    public void TheNorenigmaWithTheWheelsSetElsewhere()
    {
        var sheet = Norenigma();
        sheet.Positions = "QCW";

        Assert.Equal("OOVGVWRUOLTGU", Encipher(sheet, "ATTACKATDAWNX"));
    }

    [Fact]
    public void TheNorenigmaWithRingSettings()
    {
        var sheet = Norenigma();
        sheet.RingSettings = "DGK";

        Assert.Equal("DYVRTFYIWXRPKFFDRQISOYYBQWWFJX", Encipher(sheet, new string('A', 30)));
    }

    [Fact]
    public void TheNorenigmaChoosesThreeWheelsOfFive()
    {
        var sheet = Norenigma();
        sheet.Rotors = "N-V N-IV N-II";
        sheet.Positions = "BLA";

        Assert.Equal("BQYPLYUMIWJOFKZFTUIIRDRSNMKJSS", Encipher(sheet, new string('A', 30)));
    }

    [Fact]
    public void TheNorenigmaIsAServiceMachineAndKeepsItsPlugboard()
    {
        // Everything but the wheels and the reflector was left as it was, so it has
        // a Steckerbrett where the commercial machines have none.
        var session = Open(Norenigma());

        Assert.Equal("Service", session.Model);
        Assert.True(session.HasPlugBoard);
        Assert.Null(session.ReflectorPosition);
    }

    [Fact]
    public void TheNorenigmaKeepsTheServiceTurnovers()
    {
        // New wiring, same notches: wheel I still carries at Q, as an Enigma I's does.
        var sheet = Norenigma();
        sheet.Rotors = "N-III N-II N-I";
        sheet.Positions = "AAQ";

        var session = Open(sheet);

        session.Press('A');

        Assert.Equal("ABR", session.WindowText);
    }

    [Fact]
    public void TheKdMatchesTheReferenceFromTheStart()
    {
        Assert.Equal(
            "WCHUGKZZSFNKIOELIWWROCKSIETXXQPZFICEUZFF",
            Encipher(Kd(), new string('A', 40)));
    }

    [Fact]
    public void TheKdWithTheWheelsSetElsewhere()
    {
        var sheet = Kd();
        sheet.Positions = "QCW";

        Assert.Equal("TUMLUQGRWWJOS", Encipher(sheet, "ATTACKATDAWNX"));
    }

    [Fact]
    public void TheKdWithRingSettings()
    {
        var sheet = Kd();
        sheet.RingSettings = "DGK";

        Assert.Equal("VVVINHYRWUWZSSBKGSOOQEOQZJGQHT", Encipher(sheet, new string('A', 30)));
    }

    [Fact]
    public void TheKdsReflectorIsRewiredRatherThanSet()
    {
        // A UKW-D has no position, which is why the KD is a layout of its own rather
        // than the plain commercial one: that would refuse a reflector it cannot set.
        var session = Open(Kd());

        Assert.Equal("KD", session.Model);
        Assert.Null(session.ReflectorPosition);
        Assert.False(session.HasPlugBoard);
    }

    [Fact]
    public void ACommercialMachineStillNeedsAReflectorItCanSet()
    {
        // The KD relaxes that requirement; the machines it was written for do not.
        var sheet = Kd();
        sheet.Model = "Commercial";

        var result = EnigmaSession.Open(Factory(), sheet);

        Assert.False(result.Succeeded);
        Assert.Contains("is fixed", result.Error);
    }

    [Fact]
    public void EveryKdWheelHasNineNotches()
    {
        var parts = new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IPartsCatalogue>();

        Assert.All(
            new[] { "KD-I", "KD-II", "KD-III" },
            name => Assert.Equal(9, parts.CreateRotor(name).GetTurnoverPositions().Count()));
    }

    [Theory]
    [InlineData("norenigma")]
    [InlineData("kd")]
    public void BothAreStillReciprocalAndNeverEncipherALetterToItself(string preset)
    {
        var plain = "ATTACKATDAWNXTHEWEATHERISFINEXX";
        var sheet = preset == "kd" ? Kd() : Norenigma();

        Assert.Equal(plain, Encipher(Sheet(sheet), Encipher(Sheet(sheet), plain)));

        var session = Open(Sheet(sheet));

        Assert.All(plain, letter => Assert.NotEqual(letter, session.Press(letter)));
    }

    private static KeySheet Sheet(KeySheet sheet) => sheet.Copy();

    private static KeySheet Norenigma() => new()
    {
        Name = "Norenigma",
        Reflector = "N",
        Rotors = "N-III N-II N-I",
        RingSettings = "AAA",
        Positions = "AAA"
    };

    /// <summary>
    /// The UKW-D wiring of the KD machine held by the FRA in Sweden, written as the
    /// plain letter pairs this library takes rather than in the printed UKW-D
    /// notation, which uses the wheel's own contact lettering and is not applied here.
    /// </summary>
    private static KeySheet Kd() => new()
    {
        Name = "Enigma KD",
        Model = "KD",
        Reflector = "D",
        ReflectorPairs = "AK BO CT DV EP FN GL HM IJ QW RY SX UZ",
        Rotors = "KD-III KD-II KD-I",
        RingSettings = "AAA",
        Positions = "AAA"
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
