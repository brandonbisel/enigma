using Enigma.App;
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

    [Theory]
    [InlineData('A')]
    [InlineData('B')]
    [InlineData('C')]
    [InlineData('D')]
    [InlineData('E')]
    [InlineData('F')]
    [InlineData('G')]
    [InlineData('H')]
    [InlineData('J')]
    public void EveryShippedTableOfMeerIsWholeAndReciprocal(char tafel)
    {
        // The same check the "Quelle" tables get. A second set is a second chance
        // to have transcribed six hundred and seventy-six cells wrongly.
        var table = BigramTables.Meer[tafel];
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
    public void TheMeerSetIsComplete()
    {
        // Nine tables, A to J without I, exactly as the booklet's cover says the
        // edition holds. "Quelle" cannot pass this: its scan stops at H.
        Assert.Equal(9, BigramTables.Meer.Count);
        Assert.Equal("ABCDEFGHJ", string.Concat(BigramTables.Meer.Keys.Order()));
        Assert.DoesNotContain('I', BigramTables.Meer.Keys);

        Assert.Equal(8, BigramTables.Quelle.Count);
        Assert.DoesNotContain('J', BigramTables.Quelle.Keys);
    }

    [Fact]
    public void EveryDayOfTheMeerCalendarLeadsToATableThatIsActuallyHere()
    {
        // The property that makes a set usable rather than merely present: pick any
        // cipher net and any day of the month, and the table the plan names can be
        // supplied. This is the first set in this library where that holds.
        Assert.All(
            from kennziffer in Enumerable.Range(1, Tauschtafelplan.BrunoMeer.Columns)
            from day in Enumerable.Range(1, 31)
            select Tauschtafelplan.BrunoMeer.Tafel(kennziffer, day),
            letter => Assert.True(
                BigramTables.Meer.ContainsKey(letter),
                $"The calendar names Tafel {letter}, which is not shipped."));
    }

    [Fact]
    public void TheNineMeerTablesAreNineDifferentTables()
    {
        // If two agreed everywhere, one of them was transcribed twice.
        Assert.Equal(
            9,
            BigramTables.Meer.Values.Select(table => table.Substitute("AA")).Distinct().Count());
    }

    [Fact]
    public void TheTwoSetsAreDifferentSets()
    {
        // Same letter, different booklet: nothing should carry over.
        Assert.NotEqual(
            BigramTables.QuelleA.Substitute("AA"), BigramTables.MeerA.Substitute("AA"));
        Assert.NotEqual(
            BigramTables.QuelleB.Substitute("AA"), BigramTables.MeerB.Substitute("AA"));
    }

    [Fact]
    public void TheMeerCalendarCarriesTwelveColumns()
    {
        // Both sides of the sheet are reproduced, which is how we know a full plan
        // runs to twelve Kennziffern -- the Quelle photograph stops at six.
        Assert.Equal(12, Tauschtafelplan.BrunoMeer.Columns);
        Assert.Equal(6, Tauschtafelplan.BrunoQuelle.Columns);
    }

    [Fact]
    public void EveryColumnOfTheMeerCalendarUsesTheWholeSet()
    {
        Assert.All(
            Enumerable.Range(1, Tauschtafelplan.BrunoMeer.Columns),
            kennziffer => Assert.Equal(
                9,
                Enumerable.Range(1, 31)
                    .Select(day => Tauschtafelplan.BrunoMeer.Tafel(kennziffer, day))
                    .Distinct()
                    .Count()));
    }

    [Fact]
    public void TheMeerCalendarNamesNoTableOutsideTheSet()
    {
        Assert.All(
            from kennziffer in Enumerable.Range(1, Tauschtafelplan.BrunoMeer.Columns)
            from day in Enumerable.Range(1, 31)
            select Tauschtafelplan.BrunoMeer.Tafel(kennziffer, day),
            letter => Assert.Contains(letter, "ABCDEFGHJ"));
    }

    // -- Flußlauf ----------------------------------------------------------------

    [Theory]
    [InlineData('A')]
    [InlineData('B')]
    [InlineData('C')]
    [InlineData('D')]
    [InlineData('E')]
    [InlineData('F')]
    [InlineData('G')]
    [InlineData('H')]
    [InlineData('J')]
    [InlineData('K')]
    public void EveryShippedTableOfFlusslaufIsWholeAndReciprocal(char tafel)
    {
        // The same check the other two sets get, at half their scan resolution. Most
        // of these came through with nothing to adjudicate, which is the pipeline's
        // doing and not the paper's: cut on the printed rules and magnified from
        // native pixels rather than from an upsampled render. Where one did not, it
        // was this reciprocity that said so.
        var table = BigramTables.Flusslauf[tafel];
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
    public void FlusslaufTafelAIsNobodyElsesTafelA()
    {
        // Three booklets, three different tables under the same letter.
        Assert.Equal(
            3,
            new[] { BigramTables.QuelleA, BigramTables.MeerA, BigramTables.FlusslaufA }
                .Select(table => table.Substitute("AA"))
                .Distinct()
                .Count());
    }

    [Fact]
    public void TheFlusslaufSetIsIncompleteAndSaysWhichTableIsWanted()
    {
        // A set whose calendar outruns its tables is the case BigramTableChoice was
        // written for, and this is the first one where most of the set is missing.
        Assert.False(BigramTableSet.Flusslauf.IsComplete);
        Assert.Equal("ABCDEFGHJK", string.Concat(BigramTableSet.Flusslauf.Tables.Keys));

        var chosen = BigramTableChoice.From(BigramTableSet.Flusslauf, kennziffer: 1, monatstag: 2, tafel: 'A');

        Assert.Equal('L', chosen.Letter);
        Assert.False(chosen.Found);
        Assert.Contains("15 tables, A to P without I", chosen.Missing);
        Assert.Contains("holds 10 of them", chosen.Missing);
    }

    [Fact]
    public void TheFlusslaufDaysThatWorkAreTheOnesItsTablesCover()
    {
        // The transcribed tables are in force on some days and not others, and the
        // calendar decides which. If this ever found no day at all, the tables and the
        // plan would have come from different booklets.
        var days =
            (from kennziffer in Enumerable.Range(1, BigramTableSet.Flusslauf.Plan.Columns)
             from day in Enumerable.Range(1, 31)
             select BigramTableChoice.From(BigramTableSet.Flusslauf, kennziffer, day, 'A'))
            .ToArray();

        Assert.Contains(days, chosen => chosen.Found);
        Assert.Contains(days, chosen => !chosen.Found);
        Assert.All(days, chosen => Assert.Equal("ABCDEFGHJK".Contains(chosen.Letter), chosen.Found));
    }


    [Fact]
    public void TheFlusslaufCalendarCarriesTwelveColumns()
    {
        Assert.Equal(12, Tauschtafelplan.BrunoFlusslauf.Columns);
    }

    [Fact]
    public void TheFlusslaufCalendarNamesFifteenTables()
    {
        // The finding this sheet brings: a set was not always nine tables. Quelle and
        // Meer run A to J; this one runs A to P, still skipping I.
        Assert.Equal(15, Tauschtafelplan.BrunoFlusslauf.Tafeln.Count);
        Assert.Equal("A to P without I", Tauschtafelplan.BrunoFlusslauf.TafelRange);
    }

    [Fact]
    public void TheOlderPlansStillNameNine()
    {
        // The same property read off the sets that were here first, so that the
        // generalisation is checked against what it replaced.
        Assert.Equal(9, Tauschtafelplan.BrunoMeer.Tafeln.Count);
        Assert.Equal("A to J without I", Tauschtafelplan.BrunoMeer.TafelRange);
    }

    [Fact]
    public void EveryColumnOfTheFlusslaufCalendarUsesTheWholeSet()
    {
        Assert.All(
            Enumerable.Range(1, Tauschtafelplan.BrunoFlusslauf.Columns),
            kennziffer => Assert.Equal(
                15,
                Enumerable.Range(1, 31)
                    .Select(day => Tauschtafelplan.BrunoFlusslauf.Tafel(kennziffer, day))
                    .Distinct()
                    .Count()));
    }

    [Fact]
    public void TheFlusslaufCalendarNamesNoTableOutsideItsFifteen()
    {
        Assert.All(
            from kennziffer in Enumerable.Range(1, Tauschtafelplan.BrunoFlusslauf.Columns)
            from day in Enumerable.Range(1, 31)
            select Tauschtafelplan.BrunoFlusslauf.Tafel(kennziffer, day),
            letter => Assert.Contains(letter, "ABCDEFGHJKLMNOP"));
    }

    [Fact]
    public void ElevenFlusslaufColumnsRepeatOneTableAndTheTwelfthDoesNot()
    {
        // Fifteen letters over 31 days is fourteen twice and one three times, and
        // that is how eleven of the twelve columns are set. Kennziffer four is not:
        // K appears once there and D and O three times each. It was read cell by cell
        // twice, so the irregularity is recorded rather than corrected -- and this
        // test is what would notice if a later pass quietly tidied it away.
        var shapes = Enumerable
            .Range(1, Tauschtafelplan.BrunoFlusslauf.Columns)
            .Select(kennziffer => Enumerable
                .Range(1, 31)
                .Select(day => Tauschtafelplan.BrunoFlusslauf.Tafel(kennziffer, day))
                .GroupBy(letter => letter)
                .Select(group => group.Count())
                .OrderDescending()
                .ToArray())
            .ToArray();

        Assert.Equal(11, shapes.Count(shape => shape is [3, ..] && shape.Count(n => n == 3) == 1));

        var odd = shapes[3];
        Assert.Equal(2, odd.Count(n => n == 3));
        Assert.Equal(1, odd.Count(n => n == 1));
    }

    [Fact]
    public void TheFlusslaufSheetIsNotTheMeerSheet()
    {
        // Different booklets, so no cell-for-cell agreement is expected anywhere.
        Assert.NotEqual(
            string.Concat(
                from kennziffer in Enumerable.Range(1, 12)
                from day in Enumerable.Range(1, 31)
                select Tauschtafelplan.BrunoMeer.Tafel(kennziffer, day)),
            string.Concat(
                from kennziffer in Enumerable.Range(1, 12)
                from day in Enumerable.Range(1, 31)
                select Tauschtafelplan.BrunoFlusslauf.Tafel(kennziffer, day)));
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

    [Fact]
    public void TheCalendarPutsU534OnTheTableItsMessageWasSentWith()
    {
        // The one point where the Tauschtafelplan can be checked against real
        // traffic. U-534's P1030690 of 1 May 1945 was sent on Tafel A, and column
        // six is the one pencilled "Mai 45" on the sheet.
        Assert.Equal('A', Tauschtafelplan.BrunoQuelle.Tafel(kennziffer: 6, dayOfMonth: 1));
    }

    [Fact]
    public void TheCalendarNamesATableForEveryDayOfEveryColumn()
    {
        // Nine tables to the set, lettered A to J with no I, so no other letter may
        // appear anywhere in the plan.
        Assert.All(
            from kennziffer in Enumerable.Range(1, Tauschtafelplan.BrunoQuelle.Columns)
            from day in Enumerable.Range(1, 31)
            select Tauschtafelplan.BrunoQuelle.AsPrinted(kennziffer, day),
            letter => Assert.Contains(letter, "ABCDEFGHJ"));
    }

    [Fact]
    public void EveryPrintedColumnUsesTheWholeSet()
    {
        // A calendar that left a table out would be a transcription slip, not a plan.
        Assert.All(
            Enumerable.Range(1, Tauschtafelplan.BrunoQuelle.Columns),
            kennziffer => Assert.Equal(
                9,
                Enumerable.Range(1, 31)
                    .Select(day => Tauschtafelplan.BrunoQuelle.AsPrinted(kennziffer, day))
                    .Distinct()
                    .Count()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    public void TheCorrectedColumnsHaveNoCOrH(int kennziffer)
    {
        // The pen strikes out every C and every H in these three columns and writes
        // a replacement over it. That the letters vanish completely — and only from
        // these columns — is what makes the corrections read as a withdrawal of two
        // tables rather than as scattered amendments.
        Assert.All(
            Enumerable.Range(1, 31),
            day =>
            {
                var tafel = Tauschtafelplan.BrunoQuelle.Tafel(kennziffer, day);
                Assert.NotEqual('C', tafel);
                Assert.NotEqual('H', tafel);
            });
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void TheUncorrectedColumnsStillHaveThem(int kennziffer)
    {
        var letters = Enumerable.Range(1, 31)
            .Select(day => Tauschtafelplan.BrunoQuelle.Tafel(kennziffer, day))
            .ToList();

        Assert.Contains('C', letters);
        Assert.Contains('H', letters);
    }

    [Fact]
    public void ADayOutsideTheMonthIsRefused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Tauschtafelplan.BrunoQuelle.Tafel(kennziffer: 1, dayOfMonth: 32));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Tauschtafelplan.BrunoQuelle.Tafel(kennziffer: 7, dayOfMonth: 1));
    }
}
