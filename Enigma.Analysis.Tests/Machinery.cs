using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Analysis.Tests;

/// <summary>
/// The library as a front end would have it: the real container, the real catalogue,
/// the real factory. An attack that was tested against anything less would be tested
/// against a machine nothing else uses.
/// </summary>
internal static class Machinery
{
    private static IServiceProvider Services { get; } =
        new ServiceCollection().AddEnigmaServices().BuildServiceProvider();

    public static IEnigmaMachineFactory Factory => Services.GetRequiredService<IEnigmaMachineFactory>();

    public static IPartsCatalogue Parts => Services.GetRequiredService<IPartsCatalogue>();

    public static string Encipher(KeySheet sheet, string text)
    {
        var machine = Factory.Create(sheet);
        var alphabet = machine.CharacterMap;

        return string.Concat(
            machine.Translate(text.Select(alphabet.GetIndex)).Select(alphabet.GetCharacter));
    }

    /// <summary>
    /// Real German, taken from the plaintexts of the intercepts the library already
    /// pins. Using the language as it was actually signalled matters: it is full of
    /// X as a space, Q for CH, and spelled-out numbers, none of which behaves like
    /// prose.
    /// </summary>
    public const string German =
        "AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZXSEBEZXUAFFLIEGERSTRASZERIQTUNGXDU" +
        "BROWKIXDUBROWKIXOPOTSCHKAXOPOTSCHKAXUMXEINSAQTDREINULLXUHRANGETRETENXANGRIFFXINFXRGTXFEIN" +
        "DLIQEINFANTERIEKOLONNEBEOBAQTETXANFANGSUEDAUSGANGBAERWALDEXENDEDREIKMOSTWAERTSNEUSTADTGRA" +
        "FSPEEVONSEEKRIEGSLTGXXJDEVONSHIREJJDEYONSHIRAJWESTLICHSCHOTTLANDXJYOLBERTJJCOLBERTJJAIGLE" +
        "JJAIGLEJMIGTELMEERYJALGERIEJJALGERIEJJJULOSVERNEJJJULESDERNEJVONXASABLANCANACHGIBRALTARXK" +
        "RKRALLEXXFOLGENDESISTSOFORTBEKANNTZUGEBENXXICHHABEFOLGENDENBEFEHLERHALTENXXJANSTERLEDESBI" +
        "SHERIGXNREICHSMARSCHALLSJGOERINGJSETZTDERFUEHRERSIEYHVRRGRZSSADMIRALYALSSEINENNACHFOLGERE" +
        "INXSCHRIFTLSCHEVOLLMACHTUNTERWEGSXABSOFORTSOLLENSIESAEMTLICHEMASSNAHMENVERFUEGENYDIESICHA" +
        "USDERGEGENWAERTIGENLAGEERGEBENXVONVONJLOOKSJHFFTTTEINSEINSDREIZWOYYQNNSNEUNINHALTXXBEIANG" +
        "RIFFUNTERWASSERGEDRUECKTYWABOSXLETZTERGEGNERSTANDNULACHTDREINULUHRMARQUANTONJOTANEUNACHTS" +
        "EYHSDREIYZWOZWONULGRADYACHTSMYSTOSSENACHXEKNSVIERMBFAELLTYNNNNNNOOOVIERYSICHTEINSNULL";
}
