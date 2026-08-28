using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The Kriegsmarine indicator procedure and the double-letter conversion table it
/// hid behind.
///
/// The worked example is Dirk Rijmenants' (Cipher Machines and Cryptology, "Enigma
/// Procedures"): indicator groups HLG and KQK, fillers A and Z, giving the eight
/// letters BDBJEMEJ under bigram table B. Only four of that table's entries are
/// published with it, and only those four are used here.
/// </summary>
public class NavalIndicatorTests
{
    private const string PublishedEntries = "AK=BD HQ=BJ LK=EM GZ=EJ";

    [Fact]
    public void ThePublishedExampleIsReproduced()
    {
        //  A H L G   filler, then the Schlüsselkenngruppe
        //  K Q K Z   the Verfahrenkenngruppe, then a filler
        //  read downwards: AK HQ LK GZ, substituted to BD BJ EM EJ
        var sent = Procedure().Send(Sheet(), Table(), "HLG", "KQK", 'A', 'Z');

        Assert.Equal("BDBJEMEJ", sent.Indicator);
    }

    [Fact]
    public void TheReceivingStationReadsItBack()
    {
        var received = Procedure().Receive(Sheet(), Table(), "BDBJEMEJ");

        Assert.Equal("HLG", received.KeyGroup);
        Assert.Equal("KQK", received.MessageGroup);
    }

    [Fact]
    public void BothStationsArriveAtTheSameMessageKey()
    {
        // Which is the point of the whole procedure.
        var procedure = Procedure();
        var sent = procedure.Send(Sheet(), Table(), "HLG", "KQK", 'A', 'Z');
        var received = procedure.Receive(Sheet(), Table(), sent.Indicator);

        Assert.Equal(sent.MessageKey, received.MessageKey);
        Assert.Equal(3, sent.MessageKey.Length);
    }

    [Fact]
    public void TheMessageKeyIsTheGroupTypedAtTheGroundSetting()
    {
        // The Navy reads its message key off the machine rather than choosing it,
        // but the operation is the Army's: a trigram run at the Grundstellung.
        var sheet = Sheet();
        var army = Services().GetRequiredService<IIndicatorProcedure>();

        Assert.Equal(
            army.EncipherMessageKey(sheet, sheet.Positions, "KQK"),
            Procedure().Send(sheet, Table(), "HLG", "KQK", 'A', 'Z').MessageKey);
    }

    [Fact]
    public void TheFillersDoNotReachTheMessageKey()
    {
        // They pad the trigrams out to bigrams and nothing more, so two stations
        // choosing different ones still meet. A whole table is needed to ask this:
        // changing the fillers changes which bigrams come up.
        var procedure = Procedure();
        var table = BigramTable.Parse(Everything());
        var one = procedure.Send(Sheet(), table, "HLG", "KQK", 'A', 'Z');
        var other = procedure.Send(Sheet(), table, "HLG", "KQK", 'Q', 'W');

        Assert.Equal(one.MessageKey, other.MessageKey);
        Assert.NotEqual(one.Indicator, other.Indicator);
    }

    [Fact]
    public void OnAnM4TheTrigramSetsThreeWheelsAndTheGreekOneStaysPut()
    {
        // The Kenngruppenbuch lists trigrams, but an M4 has four wheels. The Greek
        // wheel was set from the key sheet and left alone for the message, so the
        // trigram sets the three to its right.
        var sheet = FourWheelSheet();
        var sent = Procedure().Send(sheet, Table(), "HLG", "KQK", 'A', 'Z');

        Assert.Equal(4, sent.MessageKey.Length);
        Assert.Equal(sheet.Positions[0], sent.MessageKey[0]);
    }

    [Fact]
    public void AnM4IndicatorStillReadsBackToTheSameKey()
    {
        var procedure = Procedure();
        var sent = procedure.Send(FourWheelSheet(), Table(), "HLG", "KQK", 'A', 'Z');
        var received = procedure.Receive(FourWheelSheet(), Table(), sent.Indicator);

        Assert.Equal(sent.MessageKey, received.MessageKey);
    }

    [Fact]
    public void TheKeyGroupSaysWhichKeyAndEnciphersNothing()
    {
        // The Schlüsselkenngruppe tells the receiver which key sheet is in force. It
        // never reaches the rotors, so two messages differing only in it start at
        // the same place and are enciphered identically.
        var procedure = Procedure();
        var table = BigramTable.Parse(Everything());
        var one = procedure.Send(Sheet(), table, "HLG", "KQK", 'A', 'Z');
        var other = procedure.Send(Sheet(), table, "PQR", "KQK", 'A', 'Z');

        Assert.Equal(one.MessageKey, other.MessageKey);
        Assert.NotEqual(one.Indicator, other.Indicator);
    }

    // -- the table --------------------------------------------------------------

    [Fact]
    public void TheTableIsItsOwnInverse()
    {
        // "If a bigram AB was encoded in KW, the bigram KW would also decode to AB",
        // which is what lets both stations use it without reversing anything.
        var table = Table();

        Assert.All(
            new[] { "AK", "HQ", "LK", "GZ", "BD", "BJ", "EM", "EJ" },
            bigram => Assert.Equal(bigram, table.Substitute(table.Substitute(bigram))));
    }

    [Fact]
    public void WritingAnEntryOnceIsEnough()
    {
        // Every entry implies its own reverse, so a table need only be written
        // through once.
        Assert.Equal("AK", Table().Substitute("BD"));
        Assert.Equal(8, Table().Count);
    }

    [Fact]
    public void WritingItBothWaysIsAllowedWhenTheyAgree()
    {
        Assert.Equal("BD", BigramTable.Parse("AK=BD BD=AK").Substitute("AK"));
    }

    [Fact]
    public void ATableThatContradictsItselfIsRefused()
    {
        Assert.Throws<FormatException>(() => BigramTable.Parse("AK=BD AK=EM"));
        Assert.Throws<FormatException>(() => BigramTable.Parse("AK=BD BD=EM"));
    }

    [Fact]
    public void AnEntryThatIsNotASubstitutionIsRefused()
    {
        Assert.Throws<FormatException>(() => BigramTable.Parse("AKBD"));
        Assert.Throws<FormatException>(() => BigramTable.Parse("A=BD"));
        Assert.Throws<FormatException>(() => BigramTable.Parse("A1=BD"));
    }

    [Fact]
    public void ABigramTheTableDoesNotCoverIsReportedRatherThanPassedThrough()
    {
        // A partial table is fine for study; sending with one is not, and silently
        // leaving a bigram alone would be the worst of both.
        var thrown = Assert.Throws<ArgumentException>(() => Table().Substitute("ZZ"));

        Assert.Contains("ZZ", thrown.Message);
    }

    [Fact]
    public void APartialTableKnowsItIsPartial()
    {
        // A real set covered every bigram; none is shipped here, because the
        // recovered tables survive as photographs rather than as data.
        Assert.False(Table().IsComplete);
        Assert.Equal(26 * 26, BigramTable.Parse(Everything()).Count);
        Assert.True(BigramTable.Parse(Everything()).IsComplete);
    }

    [Fact]
    public void ACompleteTableCarriesEveryMessageThrough()
    {
        var table = BigramTable.Parse(Everything());
        var procedure = Procedure();
        var sent = procedure.Send(Sheet(), table, "ABC", "DEF", 'Q', 'R');
        var received = procedure.Receive(Sheet(), table, sent.Indicator);

        Assert.Equal("ABC", received.KeyGroup);
        Assert.Equal("DEF", received.MessageGroup);
        Assert.Equal(sent.MessageKey, received.MessageKey);
    }

    [Fact]
    public void AnIndicatorOfTheWrongLengthIsRefused()
    {
        var thrown = Assert.Throws<ArgumentException>(
            () => Procedure().Receive(Sheet(), Table(), "BDBJEM"));

        Assert.Contains("eight letters", thrown.Message);
    }

    [Fact]
    public void AGroupThatIsNotATrigramIsRefused()
    {
        Assert.Throws<ArgumentException>(() => Procedure().Send(Sheet(), Table(), "HL", "KQK"));
        Assert.Throws<ArgumentException>(() => Procedure().Send(Sheet(), Table(), "HLG", "KQKZ"));
    }

    /// <summary>A whole table, so the procedure can be exercised end to end. Not a real one.</summary>
    private static string Everything()
    {
        // Pairs each bigram with the one whose letters are shifted by thirteen,
        // which is an involution and covers all six hundred and seventy-six.
        var entries = new List<string>();

        for (var first = 0; first < 26; first++)
        {
            for (var second = 0; second < 26; second++)
            {
                entries.Add(
                    $"{(char)('A' + first)}{(char)('A' + second)}=" +
                    $"{(char)('A' + (first + 13) % 26)}{(char)('A' + (second + 13) % 26)}");
            }
        }

        return string.Join(' ', entries);
    }

    private static BigramTable Table(string entries = PublishedEntries) => BigramTable.Parse(entries);

    /// <summary>A three wheel machine, where a trigram is the whole message key.</summary>
    private static KeySheet Sheet()
    {
        Assert.True(KeySheets.TryGet("scharnhorst", out var sheet));

        return sheet.Copy();
    }

    /// <summary>The naval M4, where it is not.</summary>
    private static KeySheet FourWheelSheet()
    {
        Assert.True(KeySheets.TryGet("u264", out var sheet));

        return sheet.Copy();
    }

    private static INavalIndicatorProcedure Procedure() =>
        Services().GetRequiredService<INavalIndicatorProcedure>();

    private static IServiceProvider Services() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider();
}
