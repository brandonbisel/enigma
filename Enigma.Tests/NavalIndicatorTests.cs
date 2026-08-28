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

    // -- a real message ---------------------------------------------------------

    [Fact]
    public void TheU534MessageIsReadRightThrough()
    {
        // Message P1030690 from U-534, 1 May 1945, as worked through by Michael
        // Hörenberg. The indicator as transmitted, the day's key, and the whole of
        // "Quelle" Tafel A — the table that boat was actually using — end to end on
        // traffic that was actually sent.
        var received = Procedure().Receive(U534(), BigramTables.QuelleA, "FNHC GVET");

        Assert.Equal("DUZ", received.KeyGroup);        // says the key is Potsdam's
        Assert.Equal("YMU", received.MessageGroup);
        Assert.Equal("ODFF", received.MessageKey);     // where the message begins
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("C")]
    [InlineData("D")]
    [InlineData("E")]
    [InlineData("F")]
    [InlineData("G")]
    [InlineData("H")]
    public void EveryShippedTableIsWholeAndReciprocal(string tafel)
    {
        // The check that makes a transcription trustworthy: 676 entries, every one
        // paired with its mirror, none enciphering to itself. A single mistyped cell
        // breaks a pair and fails here.
        var table = Tafel(tafel);
        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        Assert.True(table.IsComplete);
        Assert.Equal(676, table.Count);

        Assert.All(
            from first in alphabet from second in alphabet select $"{first}{second}",
            bigram =>
            {
                Assert.Equal(bigram, table.Substitute(table.Substitute(bigram)));
                Assert.NotEqual(bigram, table.Substitute(bigram));
            });
    }

    [Fact]
    public void TheTablesOfASetAreDifferentTables()
    {
        // Nine tables to a set, and a calendar to say which applied. If two agreed
        // everywhere one of them would have been transcribed twice.
        Assert.Equal(
            8,
            new[]
                {
                    BigramTables.QuelleA, BigramTables.QuelleB,
                    BigramTables.QuelleC, BigramTables.QuelleD,
                    BigramTables.QuelleE, BigramTables.QuelleF,
                    BigramTables.QuelleG, BigramTables.QuelleH,
                }
                .Select(table => table.Substitute("AA"))
                .Distinct()
                .Count());
    }

    private static BigramTable Tafel(string letter) => letter switch
    {
        "A" => BigramTables.QuelleA,
        "B" => BigramTables.QuelleB,
        "C" => BigramTables.QuelleC,
        "D" => BigramTables.QuelleD,
        "E" => BigramTables.QuelleE,
        "F" => BigramTables.QuelleF,
        "G" => BigramTables.QuelleG,
        "H" => BigramTables.QuelleH,
        _ => throw new ArgumentOutOfRangeException(nameof(letter), letter, "No such tafel is shipped."),
    };

    [Fact]
    public void TheShippedTableAgreesWithTheMessageItCameFrom()
    {
        // The four entries published with the U-534 working, checked against the
        // table read off the booklet. They were transcribed independently of each
        // other, so agreeing is worth something.
        Assert.Equal("KY", BigramTables.QuelleA.Substitute("FN"));
        Assert.Equal("DM", BigramTables.QuelleA.Substitute("HC"));
        Assert.Equal("UU", BigramTables.QuelleA.Substitute("GV"));
        Assert.Equal("ZZ", BigramTables.QuelleA.Substitute("ET"));
    }

    [Fact]
    public void AMessageSentWithTheRealTableComesBack()
    {
        var procedure = Procedure();
        var sent = procedure.Send(U534(), BigramTables.QuelleA, "DUZ", "YMU", 'K', 'Z');

        Assert.Equal("FNHCGVET", sent.Indicator);
        Assert.Equal("ODFF", sent.MessageKey);
    }

    [Fact]
    public void OnAnM4TheFillerSetsTheFourthWheel()
    {
        // The Kenngruppenbuch lists trigrams, and three letters cannot key four
        // wheels. What sets the Greek wheel is the filler that padded the trigram
        // out to fill its bigram column: the group typed is YMU plus its Z.
        var sheet = U534();
        var army = Services().GetRequiredService<IIndicatorProcedure>();

        Assert.Equal("ODFF", army.EncipherMessageKey(sheet, sheet.Positions, "YMUZ"));
    }

    [Fact]
    public void ChangingTheFillerChangesAnM4MessageKey()
    {
        // Which is the difference between a four wheel machine and a three wheel
        // one: on an M3 the filler is discarded, and here it is part of the key.
        var procedure = Procedure();
        var table = BigramTable.Parse(Everything());

        Assert.NotEqual(
            procedure.Send(U534(), table, "DUZ", "YMU", 'K', 'Z').MessageKey,
            procedure.Send(U534(), table, "DUZ", "YMU", 'K', 'Q').MessageKey);

        Assert.Equal(
            procedure.Send(Sheet(), table, "DUZ", "YMU", 'K', 'Z').MessageKey,
            procedure.Send(Sheet(), table, "DUZ", "YMU", 'K', 'Q').MessageKey);
    }

    [Fact]
    public void AnM4IndicatorStillReadsBackToTheSameKey()
    {
        var procedure = Procedure();
        var table = BigramTable.Parse(Everything());
        var sent = procedure.Send(U534(), table, "HLG", "KQK", 'A', 'Z');
        var received = procedure.Receive(U534(), table, sent.Indicator);

        Assert.Equal(sent.MessageKey, received.MessageKey);
        Assert.Equal(4, sent.MessageKey.Length);
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

    /// <summary>
    /// The day's key for U-534 on 1 May 1945, as published with message P1030690:
    /// reflector B, Greek C — which is Gamma, Beta gives the wrong key — wheels
    /// 4 3 8, rings VCCH, ten plugs, and the Grundstellung the operator wrote at the
    /// top of the message sheet.
    /// </summary>
    private static KeySheet U534() => new()
    {
        Name = "U-534, 1 May 1945",
        Reflector = "B-Thin",
        Rotors = "Gamma IV III VIII",
        RingSettings = "VCCH",
        Positions = "IBFK",
        Plugboard = "CH EJ NV OU TY LG SZ PK DI QB"
    };

    /// <summary>
    /// The four entries of "Quelle" Tafel A that message P1030690 needs. The rest of
    /// that table is not published as data, only as photographs of the original.
    /// </summary>
    private static readonly BigramTable QuelleTafelA =
        BigramTable.Parse("FN=KY HC=DM GV=UU ET=ZZ");

    private static INavalIndicatorProcedure Procedure() =>
        Services().GetRequiredService<INavalIndicatorProcedure>();

    private static IServiceProvider Services() =>
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider();
}
