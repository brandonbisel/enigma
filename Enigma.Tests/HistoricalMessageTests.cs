using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// Real intercepts, decrypted with their published key sheets. These are the
/// strongest vectors available: the settings, the wiring, the stepping and the
/// ring handling all have to be right at once or the German does not appear.
/// </summary>
public class HistoricalMessageTests
{
    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    [Fact]
    public void Barbarossa_DecryptsToTheKnownPlaintext()
    {
        const string cipher =
            "EDPUDNRGYSZRCXNUYTPOMRMBOFKTBZREZKMLXLVEFGUEYSIOZVEQMIKUBPMMYLKLTTDEISMDICAGYKUACTCDOMOHWX" +
            "MUUIAUBSTSLRNBZSZWNRFXWFYSSXJZVIJHIDISHPRKLKAYUPADTXQSPINQMATLPIFSVKDASCTACDPBOPVHJK";

        const string plain =
            "AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZXSEBEZXUAFFLIEGERSTRASZERIQTUNGXDUB" +
            "ROWKIXDUBROWKIXOPOTSCHKAXOPOTSCHKAXUMXEINSAQTDREINULLXUHRANGETRETENXANGRIFFXINFXRGTX";

        Assert.Equal(plain, Decrypt("barbarossa", cipher));
    }

    [Fact]
    public void Scharnhorst_DecryptsToTheKnownPlaintext()
    {
        const string cipher =
            "YKAENZAPMSCHZBFOCUVMRMDPYCOFHADZIZMEFXTHFLOLPZLFGGBOTGOXGRETDWTJIQHLMXVJWKZUASTR";

        const string plain =
            "STEUEREJTANAFJORDJANSTANDORTQUAAACCCVIERNEUNNEUNZWOFAHRTZWONULSMXXSCHARNHORSTHCO";

        Assert.Equal(plain, Decrypt("scharnhorst", cipher));
    }

    [Fact]
    public void InstructionManual_DecryptsToTheKnownPlaintext()
    {
        const string cipher =
            "GCDSEAHUGWTQGRKVLFGXUCALXVYMIGMMNMFDXTGNVHVRMMEVOUYFZSLRHDRRXFJWCFHUHMUNZEFRDISIKBGPMYVXUZ";

        const string plain =
            "FEINDLIQEINFANTERIEKOLONNEBEOBAQTETXANFANGSUEDAUSGANGBAERWALDEXENDEDREIKMOSTWAERTSNEUSTADT";

        Assert.Equal(plain, Decrypt("instruction-manual", cipher));
    }

    [Fact]
    public void U264_DecryptsToTheKnownPlaintext()
    {
        // The naval M4, with a thin reflector and a fourth rotor that never moves.
        const string cipher =
            "NCZWVUSXPNYMINHZXMQXSFWXWLKJAHSHNMCOCCAKUQPMKCSMHKSEINJUSBLKIOSXCKUBHMLLXCSJUSRRDVKOHULXWCC" +
            "BGVLIYXEOAHXRHKKFVDREWEZLXOBAFGYUJQUKGRTVUKAMEURBVEKSUHHVOYHABCJWMAKLFKLMYFVNRIZRVVRTKOFDA" +
            "NJMOLBGFFLEOPRGTFLVRHOWOPBEKVWMUQFMPWPARMFHAGKXIIBG";

        const string plain =
            "VONVONJLOOKSJHFFTTTEINSEINSDREIZWOYYQNNSNEUNINHALTXXBEIANGRIFFUNTERWASSERGEDRUECKTYWABOSXLE" +
            "TZTERGEGNERSTANDNULACHTDREINULUHRMARQUANTONJOTANEUNACHTSEYHSDREIYZWOZWONULGRADYACHTSMYSTOSS" +
            "ENACHXEKNSVIERMBFAELLTYNNNNNNOOOVIERYSICHTEINSNULL";

        Assert.Equal(plain, Decrypt("u264", cipher));
    }

    [Fact]
    public void TheThinRotorNeverMoves()
    {
        // It has no ratchet, so nothing can drive it however long the message runs.
        Assert.True(KeySheets.TryGet("u264", out var keySheet));

        var machine = BuildFactory().Create(keySheet);
        var thin = machine.Rotors.First();
        var start = thin.Position;

        for (var i = 0; i < 1000; i++)
        {
            machine.Translate(0);
        }

        Assert.Equal(start, thin.Position);
        Assert.Equal("Beta", thin.Name);
    }

    [Theory]
    [InlineData("barbarossa")]
    [InlineData("scharnhorst")]
    [InlineData("instruction-manual")]
    [InlineData("u264")]
    [InlineData("default")]
    public void EveryPackagedKeySheetBuildsAMachine(string name)
    {
        Assert.True(KeySheets.TryGet(name, out var keySheet));

        var machine = BuildFactory().Create(keySheet);

        Assert.NotEmpty(machine.Rotors);
    }

    private static string Decrypt(string keySheet, string cipher)
    {
        Assert.True(KeySheets.TryGet(keySheet, out var sheet));

        var machine = BuildFactory().Create(sheet);
        var input = cipher.Select(CharacterMap.GetIndex);

        return string.Concat(machine.Translate(input).Select(CharacterMap.GetCharacter));
    }

    private static IEnigmaMachineFactory BuildFactory() =>
        new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();
}
