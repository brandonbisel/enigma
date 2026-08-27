using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

public class IndicatorProcedureTests
{
    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    private static ServiceProvider Provider() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider();

    private static KeySheet DailyKey() => new()
    {
        Name = "Daily",
        Reflector = "B",
        Rotors = "II IV V",
        RingSettings = "BUL",
        Positions = "AAA",
        Plugboard = "AV BS CG DL FU HZ IN KM OW RX"
    };

    [Fact]
    public void AMessageKeyIsRecoveredFromItsIndicator()
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        var indicator = procedure.EncipherMessageKey(DailyKey(), "WZA", "BLA");

        Assert.Equal("BLA", procedure.RecoverMessageKey(DailyKey(), "WZA", indicator));
    }

    [Fact]
    public void TheIndicatorIsNotTheMessageKeyInClear()
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        Assert.NotEqual("BLA", procedure.EncipherMessageKey(DailyKey(), "WZA", "BLA"));
    }

    [Fact]
    public void ADoubledIndicatorIsTwiceAsLongAndStillRecovers()
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        var indicator = procedure.EncipherMessageKey(DailyKey(), "WZA", "BLA", doubled: true);

        Assert.Equal(6, indicator.Length);
        Assert.Equal("BLA", procedure.RecoverMessageKey(DailyKey(), "WZA", indicator));
    }

    [Fact]
    public void ADoubledIndicatorEnciphersTheSameLettersDifferently()
    {
        // The rotors move between the two halves, which is exactly the relation
        // that let Rejewski reconstruct the wiring.
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        var indicator = procedure.EncipherMessageKey(DailyKey(), "WZA", "BLA", doubled: true);

        Assert.NotEqual(indicator[..3], indicator[3..]);
    }

    [Fact]
    public void AGarbledDoubledIndicatorIsRejected()
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        var indicator = procedure.EncipherMessageKey(DailyKey(), "WZA", "BLA", doubled: true);
        var garbled = indicator[..5] + (indicator[5] == 'A' ? 'B' : 'A');

        Assert.Throws<ArgumentException>(
            () => procedure.RecoverMessageKey(DailyKey(), "WZA", garbled));
    }

    [Fact]
    public void TheWholeProcedureCarriesAMessageEndToEnd()
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();
        var factory = provider.GetRequiredService<IEnigmaMachineFactory>();

        const string message = "ATTACKATDAWNXXTHEENEMYISADVANCING";
        var dailyKey = DailyKey();

        // Sender: pick a ground setting and a message key, send the indicator.
        var indicator = procedure.EncipherMessageKey(dailyKey, "QWE", "RTZ");

        var sending = dailyKey.WithPositions("RTZ");
        var cipher = factory.Create(sending).Translate(message.Select(CharacterMap.GetIndex)).ToList();

        // Receiver: knows only the daily key, the ground setting and the indicator.
        var messageKey = procedure.RecoverMessageKey(dailyKey, "QWE", indicator);
        var receiving = dailyKey.WithPositions(messageKey);
        var plain = string.Concat(
            factory.Create(receiving).Translate(cipher).Select(CharacterMap.GetCharacter));

        Assert.Equal("RTZ", messageKey);
        Assert.Equal(message, plain);
    }

    [Theory]
    [InlineData("AA")]
    [InlineData("ABCD")]
    public void AGroundSettingMustGiveOneLetterPerRotor(string groundSetting)
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        Assert.Throws<ArgumentException>(
            () => procedure.EncipherMessageKey(DailyKey(), groundSetting, "BLA"));
    }

    [Fact]
    public void AFourRotorMachineUsesFourLetterKeys()
    {
        using var provider = Provider();
        var procedure = provider.GetRequiredService<IIndicatorProcedure>();

        var dailyKey = new KeySheet
        {
            Reflector = "B-Thin", Rotors = "Beta II IV I", RingSettings = "AAAV", Positions = "AAAA"
        };

        var indicator = procedure.EncipherMessageKey(dailyKey, "VJNA", "ABCD");

        Assert.Equal(4, indicator.Length);
        Assert.Equal("ABCD", procedure.RecoverMessageKey(dailyKey, "VJNA", indicator));
    }
}
