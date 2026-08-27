using Enigma.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The Uhr's dial, checked against published test vectors for all forty positions.
/// The vectors use ten cables plugged in order, 1a-1b as A-B through 10a-10b as
/// S-T, leaving UVWXYZ unplugged. See the README for where they come from.
/// </summary>
public class EnigmaUhrTests
{
    private static readonly ICharacterMap Alphabet = new DefaultCharacterMap();

    /// <summary>Cables A-B, C-D, ... S-T, in the order the key sheet gives them.</summary>
    private static List<Tuple<int, int>> Cables() =>
        Enumerable.Range(0, 10).Select(i => Tuple.Create(i * 2, i * 2 + 1)).ToList();

    private static string Substitution(IPlugBoard board) =>
        string.Concat(Enumerable.Range(0, 26)
            .Select(letter => Alphabet.GetCharacter(board.Translate(letter))));

    public static TheoryData<int, string> PublishedVectors() => new()
    {
        { 0, "BADCFEHGJILKNMPORQTSUVWXYZ" },
        { 1, "FOHKRAPGJCNQBETSDILMUVWXYZ" },
        { 2, "NSLAJCTERGPIHKBMDOFQUVWXYZ" },
        { 3, "JKROHSTADGLQFINEPCBMUVWXYZ" },
        { 4, "LMJOTQRKPCHABSDIFGNEUVWXYZ" },
        { 5, "TQDGBKRCHSNIFMLOPAJEUVWXYZ" },
        { 6, "PKRMFODIBATSNQLGJEHCUVWXYZ" },
        { 7, "DCTAFKLGPQJMHIBONERSUVWXYZ" },
        { 8, "RGFEDCBQTONMLKJSHAPIUVWXYZ" },
        { 9, "LMNSDCTKHIJEPOBGRQFAUVWXYZ" },
        { 10, "DEJCLANOFMHKPIRQTSBGUVWXYZ" },
        { 11, "FMJCPQBGRITSNAHKDOLEUVWXYZ" },
        { 12, "JQLSNGFIHAPCRETKBMDOUVWXYZ" },
        { 13, "HELOFSTMRABQNKDCJGPIUVWXYZ" },
        { 14, "ROPQHEJGTSBADCFINKLMUVWXYZ" },
        { 15, "RIBMNCDSFOHATKLQPGJEUVWXYZ" },
        { 16, "PIHKJMTCBEDQFONALSRGUVWXYZ" },
        { 17, "PAJEFGDINQHMLCROBSTKUVWXYZ" },
        { 18, "BGTIRKFANCLOJMHSPQDEUVWXYZ" },
        { 19, "NOHELCJITAFKPGBSRMDQUVWXYZ" },
        { 20, "TSRQFENMLKJIHGPODCBAUVWXYZ" },
        { 21, "RMJQLIHATEPCDSNKFOBGUVWXYZ" },
        { 22, "FQDOJCHKPIRGTEBMLANSUVWXYZ" },
        { 23, "TQPKROFEJABCNMDILSHGUVWXYZ" },
        { 24, "DMJAHSPERCTOBQLGNIFKUVWXYZ" },
        { 25, "RIPMTEFQBGLOHKJANCDSUVWXYZ" },
        { 26, "LKRSTQBCDAFMNOPEHGJIUVWXYZ" },
        { 27, "BGDQJERKNMHILOPATSFCUVWXYZ" },
        { 28, "RETGBIDOFQNSPKHMJALCUVWXYZ" },
        { 29, "BSFAJQNIPCTKRGHMLEDOUVWXYZ" },
        { 30, "DCFENGLMJOHQBITKRSPAUVWXYZ" },
        { 31, "LSRQDAHMTCPOBEFGJKNIUVWXYZ" },
        { 32, "FKNSLAJIHGBETCRQPODMUVWXYZ" },
        { 33, "JKRCHMBEFODSTQPILANGUVWXYZ" },
        { 34, "JIHQPSRGTENCFADOBMLKUVWXYZ" },
        { 35, "DELITGFCBSNOJQRMHAPKUVWXYZ" },
        { 36, "HOPMRKTANSFQDIBCLEJGUVWXYZ" },
        { 37, "DGTSNEJOLKFABIPQHMRCUVWXYZ" },
        { 38, "TMBKDIFSHQJOLGNAPCREUVWXYZ" },
        { 39, "PKFSJINOHERGDCTMBQLAUVWXYZ" }
    };

    [Theory]
    [MemberData(nameof(PublishedVectors))]
    public void EveryDialPositionMatchesItsPublishedVector(int position, string expected)
    {
        var uhr = new EnigmaUhr(Alphabet, Cables(), position);

        Assert.Equal(expected, Substitution(uhr));
    }

    [Fact]
    public void PositionZeroIsAPlainSetOfCables()
    {
        var uhr = new EnigmaUhr(Alphabet, Cables(), 0);

        foreach (var (a, b) in Cables().Select(cable => (cable.Item1, cable.Item2)))
        {
            Assert.Equal(b, uhr.Translate(a));
            Assert.Equal(a, uhr.Translate(b));
        }
    }

    [Fact]
    public void ExactlyEveryFourthPositionIsReciprocal()
    {
        // The flaw in the device: those positions throw away the very advantage
        // the box was fitted to provide.
        var reciprocal = Enumerable.Range(0, EnigmaUhr.Positions)
            .Where(position =>
            {
                var uhr = new EnigmaUhr(Alphabet, Cables(), position);

                return Enumerable.Range(0, 26)
                    .All(letter => uhr.Translate(uhr.Translate(letter)) == letter);
            })
            .ToList();

        Assert.Equal(Enumerable.Range(0, 10).Select(i => i * 4), reciprocal);
        Assert.All(reciprocal, position => Assert.True(EnigmaUhr.IsReciprocalAt(position)));
    }

    [Fact]
    public void MostPositionsAreNotReciprocal()
    {
        var uhr = new EnigmaUhr(Alphabet, Cables(), 1);

        Assert.Contains(
            Enumerable.Range(0, 26),
            letter => uhr.Translate(uhr.Translate(letter)) != letter);
    }

    [Fact]
    public void AnIndependentVectorWithDifferentCablesAlsoMatches()
    {
        // Published separately from the forty above, with its own plug set, so it
        // is a genuine cross-check on the table rather than a restatement of it.
        var plugs = new[] { "QP", "WY", "EX", "RC", "TV", "ZB", "UN", "IM", "JK", "OL" };

        var cables = plugs
            .Select(plug => Tuple.Create(Alphabet.GetIndex(plug[0]), Alphabet.GetIndex(plug[1])))
            .ToList();

        var uhr = new EnigmaUhr(Alphabet, cables, 6);

        Assert.Equal("AOTDXFGHBVEWRJCZMYSPNQKIUL", Substitution(uhr));
    }

    [Fact]
    public void AnAPlugAlwaysReachesABPlugAndTheReverse()
    {
        // True in every position, not only at rest.
        for (var position = 0; position < EnigmaUhr.Positions; position++)
        {
            var uhr = new EnigmaUhr(Alphabet, Cables(), position);

            for (var cable = 0; cable < 10; cable++)
            {
                Assert.Equal(1, uhr.Translate(cable * 2) % 2);       // a reaches a b
                Assert.Equal(0, uhr.Translate(cable * 2 + 1) % 2);   // b reaches an a
            }
        }
    }

    [Fact]
    public void UnpluggedLettersArePassedStraightThrough()
    {
        for (var position = 0; position < EnigmaUhr.Positions; position++)
        {
            var uhr = new EnigmaUhr(Alphabet, Cables(), position);

            foreach (var letter in Enumerable.Range(20, 6))
            {
                Assert.Equal(letter, uhr.Translate(letter));
            }
        }
    }

    [Fact]
    public void TheBoardIsAlwaysAPermutation()
    {
        for (var position = 0; position < EnigmaUhr.Positions; position++)
        {
            var uhr = new EnigmaUhr(Alphabet, Cables(), position);

            Assert.Equal(26, Substitution(uhr).Distinct().Count());
            Assert.All(
                Enumerable.Range(0, 26),
                letter => Assert.Equal(letter, uhr.TranslateReverse(uhr.Translate(letter))));
        }
    }

    [Fact]
    public void AMachineWithTheDialAtZeroIsTheMachineWithPlainCables()
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider();

        var factory = services.GetRequiredService<IEnigmaMachineFactory>();

        var sheet = new Models.KeySheet
        {
            Reflector = "B", Rotors = "II IV V", RingSettings = "BUL", Positions = "BLA",
            Plugboard = "AV BS CG DL FU HZ IN KM OW RX"
        };

        var plain = factory.Create(sheet);

        sheet.Uhr = "00";
        var withUhr = factory.Create(sheet);

        var message = "ATTACKATDAWNXXTHEENEMYISADVANCING".Select(Alphabet.GetIndex).ToList();

        Assert.Equal(plain.Translate(message), withUhr.Translate(message));
    }

    [Fact]
    public void TheUhrTakesExactlyTenCables()
    {
        Assert.Throws<ArgumentException>(
            () => new EnigmaUhr(Alphabet, Cables().Take(9), 0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(40)]
    public void OnlyTheFortyDialPositionsExist(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EnigmaUhr(Alphabet, Cables(), position));
    }
}
