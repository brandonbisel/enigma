using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Parts;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

public class CustomPartsTests
{
    private static IPartsCatalogue BuiltIn() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider()
            .GetRequiredService<IPartsCatalogue>();

    private static IPartsCatalogue With(PartsFile parts) => new PartsCatalogue(BuiltIn(), parts);

    private static PartsFile OneRotor(string name, string wiring, string notches = "Q", bool thin = false) =>
        new() { Rotors = [new RotorDefinition { Name = name, Wiring = wiring, Notches = notches, Thin = thin }] };

    [Fact]
    public void ADefinedRotorCanBeUsedByName()
    {
        var catalogue = With(OneRotor("Norway-I", "WTOKASUYVRBXJHQCPZEFMDINGL"));

        var rotor = catalogue.CreateRotor("norway-i");

        Assert.Equal("Norway-I", rotor.Name);
        Assert.Equal([16], rotor.GetTurnoverPositions());
    }

    [Fact]
    public void ADefinedRotorIsANewInstanceEveryTime()
    {
        // Rotors carry a position, so a catalogue must never hand out a shared one.
        var catalogue = With(OneRotor("Norway-I", "WTOKASUYVRBXJHQCPZEFMDINGL"));

        var first = catalogue.CreateRotor("Norway-I");
        var second = catalogue.CreateRotor("Norway-I");

        first.Step();

        Assert.NotSame(first, second);
        Assert.NotEqual(first.Position, second.Position);
    }

    [Fact]
    public void ADefinedPartReplacesTheBuiltInOneOfTheSameName()
    {
        var catalogue = With(OneRotor("I", "PEZUOHXSCVFMTBGLRINQJWAYDK", "Y"));

        Assert.Equal([24], catalogue.CreateRotor("I").GetTurnoverPositions());
    }

    [Fact]
    public void PartsThatAreNotRedefinedStillComeFromTheLibrary()
    {
        var catalogue = With(OneRotor("Norway-I", "WTOKASUYVRBXJHQCPZEFMDINGL"));

        Assert.Equal([16], catalogue.CreateRotor("I").GetTurnoverPositions());
        Assert.Equal("B", catalogue.GetReflector("B").Name);
    }

    [Fact]
    public void BadWiringIsReportedWhenTheFileIsRead()
    {
        // Not part way through enciphering a message.
        Assert.Throws<ArgumentException>(
            () => With(OneRotor("Broken", "AAKMFLGDQVZNTOWYHXUSPAIBRC")));
    }

    [Fact]
    public void ADefinedReflectorMustBePaired()
    {
        var parts = new PartsFile
        {
            Reflectors = [new ReflectorDefinition { Name = "Bad", Wiring = "BCAEFDHIGKLJNOMQRPTUSWXVZY" }]
        };

        Assert.Throws<ArgumentException>(() => With(parts));
    }

    [Fact]
    public void ADefinedThinRotorMayNotHaveNotches()
    {
        Assert.Throws<ArgumentException>(
            () => With(OneRotor("Thin-X", "LEYJVCNIXWPBQMDRTAKZGFUHOS", "Q", thin: true)));
    }

    [Fact]
    public void ADefinedThinRotorNeverSteps()
    {
        var catalogue = With(OneRotor("Thin-X", "LEYJVCNIXWPBQMDRTAKZGFUHOS", string.Empty, thin: true));

        var rotor = catalogue.CreateRotor("Thin-X");
        rotor.Step();

        Assert.True(rotor.IsThin);
        Assert.Equal(0, rotor.Position);
    }

    [Fact]
    public void AnUnknownNameListsBothTheDefinedAndTheBuiltInParts()
    {
        var catalogue = With(OneRotor("Norway-I", "WTOKASUYVRBXJHQCPZEFMDINGL"));

        var exception = Assert.Throws<ArgumentException>(() => catalogue.CreateRotor("Nope"));

        Assert.Contains("NORWAY-I", exception.Message);
        Assert.Contains("VIII", exception.Message);
    }

    [Fact]
    public void AMachineCanBeBuiltEntirelyFromDefinedParts()
    {
        var parts = new PartsFile
        {
            Rotors =
            [
                new RotorDefinition { Name = "K-I", Wiring = "PEZUOHXSCVFMTBGLRINQJWAYDK", Notches = "Y" },
                new RotorDefinition { Name = "K-II", Wiring = "ZOUESYDKFWPCIQXHMVBLGNJRAT", Notches = "E" },
                new RotorDefinition { Name = "K-III", Wiring = "EHRVXGAOBQUSIMZFLYNWKTPDJC", Notches = "N" }
            ],
            Reflectors = [new ReflectorDefinition { Name = "UKW-K", Wiring = "IMETCGFRAYSQBZXWLHKDVUPOJN" }]
        };

        var services = new ServiceCollection().AddEnigmaServices();
        services.AddSingleton<IPartsCatalogue>(provider => new PartsCatalogue(
            ActivatorUtilities.CreateInstance<BuiltInPartsCatalogue>(provider), parts));

        var factory = services.BuildServiceProvider().GetRequiredService<IEnigmaMachineFactory>();
        var characterMap = new DefaultCharacterMap();

        var sheet = new KeySheet
        {
            Reflector = "UKW-K", Rotors = "K-I K-II K-III", RingSettings = "AAA", Positions = "AAA"
        };

        var encipher = factory.Create(sheet);
        var decipher = factory.Create(sheet);

        var cipher = encipher.Translate("ATTACKATDAWN".Select(characterMap.GetIndex)).ToList();
        var back = string.Concat(decipher.Translate(cipher).Select(characterMap.GetCharacter));

        Assert.Equal("ATTACKATDAWN", back);
    }
}
