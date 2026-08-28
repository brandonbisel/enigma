using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The commercial Enigma D of 1926 and the Enigma K that followed it. They share
/// their wheel wirings, their reflector and their entry wheel with each other and
/// with the Zählwerk A28/G31; what separates them is the notches.
///
/// Wirings and notches from the Crypto Museum's Enigma D and Enigma K pages, which
/// agree exactly with Daniel Palloks' Universal Enigma. The vectors are from that
/// simulator, taken before any of this was written.
/// </summary>
public class CommercialEnigmaTests
{
    [Fact]
    public void TheDMatchesTheReferenceFromTheStart()
    {
        Assert.Equal(
            "HLKUDTHSYVICWNZWWDMWKGEORZYEQIHDTWWLZEEU",
            Encipher(Sheet("D"), new string('A', 40)));
    }

    [Fact]
    public void TheKMatchesTheReferenceFromTheStart()
    {
        Assert.Equal(
            "HLKUDTHSYVICWNZWWDMWKGEOGZYEQIHDTWWLZEEU",
            Encipher(Sheet("K"), new string('A', 40)));
    }

    [Fact]
    public void TheDAndTheKPartCompanyWhereTheirNotchesDiffer()
    {
        // Same wheels, same reflector, same start: the only thing that can separate
        // them is when a wheel carries its neighbour. The K's fast wheel is notched
        // at Y and the D's at Z, so the K's middle wheel moves one keypress sooner.
        // They agree again immediately afterwards, both middles having moved.
        var d = Encipher(Sheet("D"), new string('A', 40));
        var k = Encipher(Sheet("K"), new string('A', 40));

        Assert.Equal(24, d.Zip(k).TakeWhile(pair => pair.First == pair.Second).Count());
        Assert.Equal(d[25..], k[25..]);
    }

    [Theory]
    [InlineData("D", "DAWBLSBPOISGJ")]
    [InlineData("K", "DAJBLSBPOISGJ")]
    public void WithTheWheelsAndTheReflectorSetElsewhere(string machine, string expected)
    {
        var sheet = Sheet(machine);
        sheet.Positions = "QCW";
        sheet.ReflectorPosition = "F";

        Assert.Equal(expected, Encipher(sheet, "ATTACKATDAWNX"));
    }

    [Theory]
    [InlineData("D", "PEWQEIEGRSHGSZTIVVYJCYWZQWUDON")]
    [InlineData("K", "PEWQEIEGROBFVYWTGCBKESLFQWUDON")]
    public void WithRingSettings(string machine, string expected)
    {
        var sheet = Sheet(machine);
        sheet.RingSettings = "DGK";
        sheet.ReflectorRingSetting = "B";

        Assert.Equal(expected, Encipher(sheet, new string('A', 30)));
    }

    [Fact]
    public void TheReflectorIsSetButNeverDriven()
    {
        // A commercial machine's reflector is put where the key sheet says and stays
        // there; only the Zählwerk machines drive theirs.
        var session = Open(Sheet("K"));
        var start = session.ReflectorPosition;

        session.Type(new string('A', 2000));

        Assert.Equal(start, session.ReflectorPosition);
    }

    [Fact]
    public void TheMiddleWheelStillDoubleSteps()
    {
        // These are pawl machines, so the anomaly is there, unlike on the Zählwerk.
        var sheet = Sheet("K");
        sheet.Positions = "ADY";

        var session = Open(sheet);

        session.Press('A');
        Assert.Equal("AEZ", session.WindowText);

        session.Press('A');
        Assert.Equal("BFA", session.WindowText);
    }

    [Fact]
    public void OnTheDTheRingstellungAddsNothingToTheKey()
    {
        // "The cryptographic effect of the Ringstellung is null. It does not enhance
        // the machine's key space." -- Crypto Museum, Enigma D. With the notch on
        // the rotor body, moving the ring and the wheel together changes nothing at
        // all, so a ring setting is only ever a different starting position.
        var plain = new string('A', 40);
        var expected = Encipher(Sheet("D"), plain);

        foreach (var setting in new[] { "DDD", "KKK", "ZZZ" })
        {
            var sheet = Sheet("D");
            sheet.RingSettings = setting;
            sheet.Positions = setting;

            Assert.Equal(expected, Encipher(sheet, plain));
        }
    }

    [Fact]
    public void OnTheKItDoesNot()
    {
        // The same test the other way: with the notch on the letter ring the two
        // settings are independent, which is the whole point of a Ringstellung.
        var sheet = Sheet("K");
        sheet.RingSettings = "DDD";
        sheet.Positions = "DDD";

        Assert.NotEqual(Encipher(Sheet("K"), new string('A', 40)), Encipher(sheet, new string('A', 40)));
    }

    [Theory]
    [InlineData("D")]
    [InlineData("K")]
    public void ThereIsNoPlugboardOnACommercialMachine(string machine)
    {
        Assert.False(Open(Sheet(machine)).HasPlugBoard);

        var sheet = Sheet(machine);
        sheet.Plugboard = "AB CD";

        Assert.False(EnigmaSession.Open(Factory(), sheet).Succeeded);
    }

    [Theory]
    [InlineData("D")]
    [InlineData("K")]
    public void TheKeyboardIsWiredToTheEntryWheelWithoutSayingSo(string machine)
    {
        // Left unset, the model supplies QWERTZ, as it does for a Zählwerk machine.
        Assert.Equal("QWERTZ", Open(Sheet(machine)).Machine.Layout.DefaultEntryWheel);
    }

    [Theory]
    [InlineData("D")]
    [InlineData("K")]
    public void ItIsStillReciprocalAndNeverEnciphersALetterToItself(string machine)
    {
        var plain = "ATTACKATDAWNXTHEWEATHERISFINEXX";

        Assert.Equal(plain, Encipher(Sheet(machine), Encipher(Sheet(machine), plain)));

        var session = Open(Sheet(machine));

        Assert.All(plain, letter => Assert.NotEqual(letter, session.Press(letter)));
    }

    private static KeySheet Sheet(string machine) => new()
    {
        Name = $"Enigma {machine}",
        Model = "Commercial",
        Reflector = "G",
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
