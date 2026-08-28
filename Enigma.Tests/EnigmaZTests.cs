using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The numbers-only Enigma Z30, from the Swedish machine s/n Z-103.
///
/// The wirings are published by the Crypto Museum indexed 1 to 0, and by Daniel
/// Palloks' Enigma Z simulator indexed 0 to 9; converting between the two
/// conventions makes them identical, which is two independent sources agreeing.
/// The vectors below were taken from that simulator, which is the same one whose
/// gear drive this project's Zählwerk machine was checked against.
/// </summary>
public class EnigmaZTests
{
    [Fact]
    public void FortyKeysFromTheStart()
    {
        Assert.Equal(
            "2679588363842804797334273575967420965486",
            Encipher(Sheet(), new string('1', 40)));
    }

    [Fact]
    public void WithTheWheelsAndTheReflectorSetElsewhere()
    {
        var sheet = Sheet();
        sheet.Positions = "572";
        sheet.ReflectorPosition = "3";

        Assert.Equal("8617124547", Encipher(sheet, "1234567890"));
    }

    [Fact]
    public void TheNotchIsOnTheLetterRing()
    {
        // Where this library and Daniel Palloks' Z30 simulator part company, and
        // the one thing about this machine no source settles.
        //
        // On a service Enigma the notch is cut into the letter ring, so a wheel
        // carries its neighbour at a fixed window figure whatever the Ringstellung
        // is -- rotor I always at Q. That is what this library does everywhere, and
        // four historical messages with non-zero ring settings say it is right for
        // those machines. The simulator instead moves the turnover with the ring,
        // on the Z30 and on the service machines alike.
        //
        // Nothing published says the Z30 differs from every other Enigma here, so
        // it is modelled like the rest. This test pins that choice so a future
        // source can change it deliberately rather than by accident. The vectors
        // above are all taken at ring zero, where the two agree exactly.
        var sheet = Sheet();
        sheet.RingSettings = "005";
        sheet.Positions = "009";

        var session = Open(sheet);

        session.Press('1');

        Assert.Equal("010", session.WindowText);
    }

    // -- the mechanism ----------------------------------------------------------

    [Theory]
    // start           after one key   what it shows
    [InlineData("900", "0", "001", "1")] // the leftmost wheel carries the reflector, and itself
    [InlineData("999", "0", "000", "1")] // everything carries at once
    [InlineData("090", "0", "101", "0")] // the middle wheel double steps, as on any pawl machine
    [InlineData("989", "0", "090", "1")] // both pawls at once: left drives the reflector, fast drives the middle
    public void ThePawlsReachTheReflector(
        string positions, string reflector, string expected, string expectedReflector)
    {
        var sheet = Sheet();
        sheet.Positions = positions;
        sheet.ReflectorPosition = reflector;

        var session = Open(sheet);

        session.Press('1');

        Assert.Equal(expected, session.WindowText);
        Assert.Equal(expectedReflector, Digit(session.ReflectorPosition!.Value));
    }

    [Fact]
    public void OverALongMessageEachWheelMovesAsOftenAsItShould()
    {
        // Derived rather than asserted by hand. Eleven hundred keys give a hundred
        // and ten carries into the middle wheel, plus one double step each time it
        // sits on its own notch; the same one level up; and the reflector once.
        var session = Open(Sheet());
        var moves = new int[4];
        var previous = Window(session);

        for (var key = 0; key < 1100; key++)
        {
            session.Press('1');

            var current = Window(session);

            for (var wheel = 0; wheel < 4; wheel++)
            {
                if (current[wheel] != previous[wheel])
                {
                    moves[wheel]++;
                }
            }

            previous = current;
        }

        Assert.Equal([1, 13, 122, 1100], moves);
    }

    [Fact]
    public void TheMachineIsStillReciprocal()
    {
        var plain = "1234567890" + new string('7', 30);

        Assert.Equal(plain, Encipher(Sheet(), Encipher(Sheet(), plain)));
    }

    [Fact]
    public void NoDigitIsEverEncipheredToItself()
    {
        var session = Open(Sheet());

        for (var key = 0; key < 500; key++)
        {
            var digit = (char)('0' + key % 10);

            Assert.NotEqual(digit, session.Press(digit));
        }
    }

    [Fact]
    public void ItWorksInTenContactsNotTwentySix()
    {
        var session = Open(Sheet());

        Assert.Equal(10, session.Alphabet.Count);
        Assert.All(session.Machine.Rotors, rotor => Assert.Equal(10, rotor.Contacts));
        Assert.Equal(10, session.Machine.Reflector.Contacts);
    }

    [Fact]
    public void ThereIsNoPlugboardOnANumericMachine()
    {
        Assert.False(Open(Sheet()).HasPlugBoard);

        var sheet = Sheet();
        sheet.Plugboard = "12 34";

        Assert.False(EnigmaSession.Open(Factory(), sheet).Succeeded);
    }

    [Fact]
    public void ALetterCannotBeKeyedOnAMachineOfFigures()
    {
        Assert.Null(Open(Sheet()).Press('A'));
    }

    private static KeySheet Sheet() => new()
    {
        Name = "Enigma Z30",
        Model = "Z30",
        CharacterMap = "Digits",
        Reflector = "Z",
        // The simulator numbers its wheel slots from the right, so its default of
        // I, II, III reads III II I when written left to right as a key sheet does.
        Rotors = "Z-III Z-II Z-I",
        RingSettings = "000",
        Positions = "000",
        ReflectorPosition = "0",
        ReflectorRingSetting = "0"
    };

    private static string Window(EnigmaSession session) =>
        Digit(session.ReflectorPosition!.Value) + session.WindowText;

    private static string Digit(int contact) => CharacterMap.Digits.GetCharacter(contact).ToString();

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
