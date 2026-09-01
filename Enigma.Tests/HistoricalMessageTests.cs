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

    private const string DoenitzCiphertext =
        "LANOTCTOUARBBFPMHPHGCZXTDYGAHGUFXGEWKBLKGJWLQXXTGPJJAVTOYJFGSLPPQIHZFXOEBWIIEKFZLCLOAQJULJ" +
        "OYHSSMBBGWHZANVOIIPYRBRTDJQDJJOQKCXWDNBBTYVXLYTAPGVEATXSONPNYNQFUDBBHHVWEPYEYDOHNLXKZDNWRH" +
        "DUWUJUMWWVIIWZXIVIUQDRHYMNCYEFUAPNHOTKHKGDNPSAKNUAGHJZSMJBMHVTREQEDGXHLZWIFUSKDQVELNMIMITH" +
        "BHDBWVHDFYHJOQIHORTDJDBWXEMEAYXGYQXOHFDMYUXXNOJAZRSGHPLWMLRECWWUTLRTTVLBHYOORGLGOWUXNXHMHY" +
        "FAACQEKTHSJW";

    /// <summary>
    /// As published, garbles and all: a signal this long, relayed and taken down by
    /// ear, does not come through clean. ANSTERLE is ANSTELLE and HVRRGRZSSADMIRAL
    /// is HERR GROSSADMIRAL. The J's around GOERING and BORMANN are quotation marks
    /// and KK ... KK brackets a covername, which is signalling rather than damage.
    /// </summary>
    private const string DoenitzPlaintext =
        "KRKRALLEXXFOLGENDESISTSOFORTBEKANNTZUGEBENXXICHHABEFOLGENDENBEFEHLERHALTENXXJANSTERLEDESBI" +
        "SHERIGXNREICHSMARSCHALLSJGOERINGJSETZTDERFUEHRERSIEYHVRRGRZSSADMIRALYALSSEINENNACHFOLGERE" +
        "INXSCHRIFTLSCHEVOLLMACHTUNTERWEGSXABSOFORTSOLLENSIESAEMTLICHEMASSNAHMENVERFUEGENYDIESICHA" +
        "USDERGEGENWAERTIGENLAGEERGEBENXGEZXREICHSLEITEIKKTULPEKKJBORMANNJXXOBXDXMMMDURNHFKSTXKOMX" +
        "ADMXUUUBOOIEXKP";

    private const string GrafSpeeCiphertext =
        "QQMWTQJWJTMNSXYLNSACMHXZZRXWLNQZZZDZVLVUTKXDXWSLSKNWEZNFCFRGLIUHXPVKINZAJVECMOTVPVNZIMRBUI" +
        "EUBZGFMZYPRMMEWTFZFGVLQSYQGWNDMQDNRZOVYMGAVLFMRAFZRYRMICPEZSMKBMHJUTDUQSBABROQLEFPRBZFJQSR" +
        "PMRYXWOXIJLDVIFVXJLMRQFGYHIMELRSDOTAIVVXMXMDPISBHLIMMBFMKJSWTJMCCPEGKPJAVBWKJUZQBDEWDJYBMT" +
        "YM";

    private const string GrafSpeePlaintext =
        "GRAFSPEEVONSEEKRIEGSLTGXXJDEVONSHIREJJDEYONSHIRAJWESTLICHSCHOTTLANDXJYOLBERTJJCOLBERTJJAIG" +
        "LEJJAIGLEJMIGTELMEERYJALGERIEJJALGERIEJJJULOSVERNEJJJULESDERNEJVONXASABLANCANACHGIBRALTARX" +
        "ENGLISCHJAFRICSTARJJAFRICSTARJMITZWOHECKGESCHUETZENNEUNXEINSLWOXTENERIFEAUSNACHBUENOSAIRES" +
        "DF";

    private const string RaschCiphertext =
        "HCEYZTCSOPUPPZDICQRDLWXXFACTTJMBRDVCJJMMZRPYIKHZAWGLYXWTMJPQUEFSZBOTVRLALZXWVXTSLFFFAUDQFBW" +
        "RRYAPSBOWJMKLDUYUPFUQDOWVHAHCDWAUARSWTKOFVOYFPUFHVZFDGGPOOVGRMBPXXZCANKMONFHXPCKHJZBUMXJWXK" +
        "AUODXZUCVCXPFT";

    private const string RaschPlaintext =
        "BOOTKLARXBEIJSCHNOORBETWAZWOSIBENXNOVXSECHSNULCBMXPROVIANTBISZWONULXDEZXBENOETIGEGLMESERYNO" +
        "CHVIEFKLHRXSTEHEMARQUBRUNOBRUNFZWOFUHFXLAGWWIEJKCHAEFERJXNNTWWWFUNFYEINSFUNFMBSTEIGENDYGUTE" +
        "SIWXDVVVJRASCH";

    /// <summary>Long enough for two settings that differ at all to show that they do.</summary>
    private static readonly string LongRun = new('A', 5000);

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
    public void Rasch_DecryptsToTheKnownPlaintext()
    {
        // The last of Erskine's three challenge signals to be broken. Its ciphertext
        // is a corrected reading: the HF/DF operator took the signal down with
        // garbles, and the letters that would not decipher were recovered with the
        // break. The plaintext keeps the garbles that are the sender's own.
        Assert.Equal(RaschPlaintext, Decrypt("rasch", RaschCiphertext));
    }

    [Fact]
    public void TheAlternateRaschSettingReadsTheMessageWithoutBeingTheSameMachine()
    {
        // A second Ringstellung and Grundstellung are published with the break. They
        // are not an equivalent setting: they agree with the first for 219 letters
        // and then part. The message is 196, so nothing in it can tell the two
        // apart -- which is what a break recovers, the settings the traffic pins.
        Assert.True(KeySheets.TryGet("rasch", out var sheet));

        var alternate = sheet.Copy();
        alternate.RingSettings = "ZZTG";
        alternate.Positions = "NBHL";

        Assert.Equal(RaschPlaintext, Decrypt(alternate, RaschCiphertext));
        Assert.NotEqual(Decrypt(sheet, LongRun), Decrypt(alternate, LongRun));
    }

    [Fact]
    public void Doenitz_DecryptsToTheKnownPlaintext()
    {
        // The M4's other thin reflector, on the longest message here: 372 letters,
        // which is over the 320 a naval signal was supposed to be held to.
        Assert.Equal(DoenitzPlaintext, Decrypt("doenitz", DoenitzCiphertext));
    }

    [Fact]
    public void ThePublishedDoenitzSettingIsTheSameMachineWrittenAnotherWay()
    {
        // Rings AAEL at YOSZ are published beside the settings the operator turned,
        // and here -- unlike Rasch's alternate -- the two really are one machine. Ring
        // and position are shifted together on the Greek wheel, which never turns,
        // and on the left wheel, which is driven by the middle wheel's notch rather
        // than by its own. Both shifts cancel, so no length of message parts them.
        Assert.True(KeySheets.TryGet("doenitz", out var sheet));

        var published = sheet.Copy();
        published.RingSettings = "AAEL";
        published.Positions = "YOSZ";

        Assert.Equal(DoenitzPlaintext, Decrypt(published, DoenitzCiphertext));
        Assert.Equal(Decrypt(sheet, LongRun), Decrypt(published, LongRun));
    }

    [Fact]
    public void GrafSpee_DecryptsToTheKnownPlaintext()
    {
        // The earliest wartime message here, and the only one on eight plugs: in
        // 1939 the Kriegsmarine was still cabling eight pairs rather than ten. A
        // three wheel M3 with naval wheels, intercepted by the Swedish station at
        // Norrkoeping the day before the Battle of the River Plate.
        Assert.Equal(GrafSpeePlaintext, Decrypt("graf-spee", GrafSpeeCiphertext));
    }

    [Fact]
    public void TheTwoGrafSpeeSettingsAreOneMachine()
    {
        // The setting is published two ways, rings AHX at EKD and rings AUX at EXD,
        // and that is not a doubt about the reading. Ring and position are shifted
        // together by thirteen on the middle wheel, so its offset is unchanged, and
        // the middle wheel is VI -- one of the naval wheels with two notches, M and
        // Z, which are themselves thirteen apart. The shift maps the notch set onto
        // itself, so the two settings step alike and no length of message parts them.
        Assert.True(KeySheets.TryGet("graf-spee", out var sheet));

        var alternate = sheet.Copy();
        alternate.RingSettings = "AUX";
        alternate.Positions = "EXD";

        Assert.Equal(GrafSpeePlaintext, Decrypt(alternate, GrafSpeeCiphertext));
        Assert.Equal(Decrypt(sheet, LongRun), Decrypt(alternate, LongRun));

        var middle = BuildFactory().Create(sheet).Rotors.Skip(1).First();
        var turnovers = middle.GetTurnoverPositions().Order().ToArray();

        Assert.Equal("VI", middle.Name);
        Assert.Equal(13, turnovers[1] - turnovers[0]);
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
    [InlineData("rasch")]
    [InlineData("doenitz")]
    [InlineData("graf-spee")]
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

        return Decrypt(sheet, cipher);
    }

    private static string Decrypt(KeySheet sheet, string cipher)
    {
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
