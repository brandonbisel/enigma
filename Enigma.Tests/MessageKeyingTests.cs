using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// Where the rotors actually start. The key sheet fixed everything for the day
/// except that, and the indicator is how the sender told the receiver.
/// </summary>
public class MessageKeyingTests
{
    [Fact]
    public void SendingKeysTheMachineAtTheMessageKeyNotTheGroundSetting()
    {
        var keyed = MessageKeying.Send(Procedure(), Daily(), "QWE");

        Assert.True(keyed.Succeeded, keyed.Error);
        Assert.Equal("BLA", keyed.GroundSetting);
        Assert.Equal("QWE", keyed.MessageKey);
        Assert.Equal("QWE", keyed.Sheet!.Positions);
    }

    [Fact]
    public void SendingLeavesEverythingElseOnTheSheetAlone()
    {
        // Only where the rotors start changes; the day's wheels, rings and cables
        // are not the sender's to choose.
        var daily = Daily();
        var keyed = MessageKeying.Send(Procedure(), daily, "QWE");

        Assert.Equal(daily.Rotors, keyed.Sheet!.Rotors);
        Assert.Equal(daily.RingSettings, keyed.Sheet.RingSettings);
        Assert.Equal(daily.Plugboard, keyed.Sheet.Plugboard);
        Assert.Equal("BLA", daily.Positions);
    }

    [Fact]
    public void TheIndicatorIsTheMessageKeyEncipheredAtTheGroundSetting()
    {
        var procedure = Procedure();
        var daily = Daily();

        Assert.Equal(
            procedure.EncipherMessageKey(daily, daily.Positions, "QWE"),
            MessageKeying.Send(procedure, daily, "QWE").Indicator);
    }

    [Fact]
    public void WhatIsSentIsWhatIsReceived()
    {
        // The whole point of the procedure: the receiver recovers the key the
        // sender chose, knowing only the day's settings and the indicator.
        var procedure = Procedure();
        var sent = MessageKeying.Send(procedure, Daily(), "QWE");
        var received = MessageKeying.Receive(procedure, Daily(), sent.Indicator!);

        Assert.True(received.Succeeded, received.Error);
        Assert.Equal("QWE", received.MessageKey);
        Assert.Equal(sent.Sheet!.Positions, received.Sheet!.Positions);
    }

    [Fact]
    public void AnIndicatorSentTwiceIsTwiceAsLong()
    {
        // Doubled until 1938, so the receiver could tell a garbled indicator from a
        // good one — and it was what let Rejewski in.
        var sent = MessageKeying.Send(Procedure(), Daily(), "QWE", doubled: true);

        Assert.Equal(6, sent.Indicator!.Length);
    }

    [Fact]
    public void ADoubledIndicatorIsReceivedJustAsWell()
    {
        var procedure = Procedure();
        var sent = MessageKeying.Send(procedure, Daily(), "QWE", doubled: true);

        Assert.Equal("QWE", MessageKeying.Receive(procedure, Daily(), sent.Indicator!).MessageKey);
    }

    [Fact]
    public void AMessageKeyOfTheWrongLengthIsReportedNotThrown()
    {
        var keyed = MessageKeying.Send(Procedure(), Daily(), "QW");

        Assert.False(keyed.Succeeded);
        Assert.Null(keyed.Sheet);
        Assert.Contains("one letter per rotor", keyed.Error);
    }

    [Fact]
    public void AnIndicatorOfTheWrongLengthIsReportedNotThrown()
    {
        var keyed = MessageKeying.Receive(Procedure(), Daily(), "TOOLONG");

        Assert.False(keyed.Succeeded);
        Assert.Contains("3 letters", keyed.Error);
    }

    [Fact]
    public void ADoubledIndicatorWhoseHalvesDisagreeIsReportedNotThrown()
    {
        // Which is exactly what the doubling was for: a garbled transmission shows
        // itself rather than deciphering to nonsense.
        var keyed = MessageKeying.Receive(Procedure(), Daily(), "ABCDEF");

        Assert.False(keyed.Succeeded);
        Assert.Contains("did not come through intact", keyed.Error);
    }

    [Fact]
    public void AGroupThatIsNoTrigramIsReportedInTheWordsOnThePanel()
    {
        // ArgumentException writes the parameter it was thrown for on a second
        // line. That line named the field 'keyGroup' at an operator, and in the
        // trimmed WebAssembly build it arrived as a resource key rather than as
        // words at all, so only the sentence is kept.
        var keyed = MessageKeying.SendNaval(
            NavalProcedure(), Daily(), Table, string.Empty, "QWE", 'X', 'X');

        Assert.False(keyed.Succeeded);
        Assert.Contains("Schlüsselkenngruppe", keyed.Error);
        Assert.DoesNotContain("keyGroup", keyed.Error);
        Assert.DoesNotContain("Parameter", keyed.Error);
        Assert.DoesNotContain("Arg_", keyed.Error);
    }

    [Fact]
    public void AKeySheetTheMachineCannotBeBuiltFromIsReportedNotThrown()
    {
        var broken = Daily();
        broken.Rotors = "I II NOSUCHWHEEL";

        Assert.False(MessageKeying.Send(Procedure(), broken, "QWE").Succeeded);
    }

    [Fact]
    public void TheMachineKeyedFromTheResultStartsAtTheMessageKey()
    {
        // The end of it: a session opened on that sheet has its wheels at QWE.
        var keyed = MessageKeying.Send(Procedure(), Daily(), "QWE");
        var session = EnigmaSession.Open(Factory(), keyed.Sheet!);

        Assert.True(session.Succeeded, session.Error);
        Assert.Equal("QWE", session.Session!.WindowText);
    }

    private static KeySheet Daily()
    {
        Assert.True(KeySheets.TryGet("barbarossa", out var sheet));

        return sheet.Copy();
    }

    private static IServiceProvider Services() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider();

    private static IIndicatorProcedure Procedure() =>
        Services().GetRequiredService<IIndicatorProcedure>();

    private static INavalIndicatorProcedure NavalProcedure() =>
        Services().GetRequiredService<INavalIndicatorProcedure>();

    /// <summary>Enough of a table to fail before it is ever consulted.</summary>
    private static readonly BigramTable Table = BigramTable.Parse("FN=KY HC=DM GV=UU ET=ZZ");

    private static IEnigmaMachineFactory Factory() =>
        Services().GetRequiredService<IEnigmaMachineFactory>();
}
