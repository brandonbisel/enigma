namespace Enigma.Models;

/// <summary>
/// Key sheets that ship with the program. The historical ones are each verified
/// in the test suite against the published ciphertext of a real message, so they
/// are settings that are known to be right rather than plausible.
/// </summary>
public static class KeySheets
{
    public static IReadOnlyDictionary<string, KeySheet> All { get; } =
        new Dictionary<string, KeySheet>(StringComparer.OrdinalIgnoreCase)
        {
            ["default"] = new()
            {
                Name = "Wehrmacht Enigma I",
                Reflector = "B",
                Rotors = "I II III",
                RingSettings = "AAA",
                Positions = "AAA"
            },
            ["barbarossa"] = new()
            {
                Name = "Operation Barbarossa, 7 July 1941",
                Reflector = "B",
                Rotors = "II IV V",
                RingSettings = "BUL",
                Positions = "BLA",
                Plugboard = "AV BS CG DL FU HZ IN KM OW RX"
            },
            ["scharnhorst"] = new()
            {
                Name = "Scharnhorst, 26 December 1943",
                Reflector = "B",
                Rotors = "III VI VIII",
                RingSettings = "AHM",
                Positions = "UZV",
                Plugboard = "AN EZ HK IJ LR MQ OT PV SW UX"
            },
            ["u264"] = new()
            {
                Name = "U-264 (Looks), 19 November 1942",
                Reflector = "B-Thin",
                Rotors = "Beta II IV I",
                RingSettings = "AAAV",
                Positions = "VJNA",
                Plugboard = "AT BL DF GJ HM NW OP QY RZ VX"
            },
            ["u534"] = new()
            {
                // U-534's P1030690 of 1 May 1945, the message the naval indicator
                // procedure is pinned by. The key sheet itself does not survive --
                // they were printed on water-soluble paper -- so this is recovered
                // rather than transcribed: the Grundstellung is the one the operator
                // wrote at the top of the message sheet, and the Ringstellung was
                // recovered by Dan Girard and holds for the other messages of that
                // day. Published by Michael Hoerenberg at enigma.hoerenberg.com.
                Name = "U-534, 1 May 1945",
                Reflector = "B-Thin",
                Rotors = "Gamma IV III VIII",
                RingSettings = "VCCH",
                Positions = "IBFK",
                Plugboard = "CH EJ NV OU TY LG SZ PK DI QB"
            },
            ["enigma-d"] = new()
            {
                Name = "Enigma D, commercial",
                Model = "Commercial",
                Reflector = "G",
                Rotors = "D-III D-II D-I",
                RingSettings = "AAA",
                Positions = "AAA",
                ReflectorPosition = "A"
            },
            ["enigma-k"] = new()
            {
                Name = "Enigma K, commercial",
                Model = "Commercial",
                Reflector = "G",
                Rotors = "K-III K-II K-I",
                RingSettings = "AAA",
                Positions = "AAA",
                ReflectorPosition = "A"
            },
            ["norenigma"] = new()
            {
                Name = "Norenigma (Norwegian police security service)",
                Reflector = "N",
                Rotors = "N-III N-II N-I",
                RingSettings = "AAA",
                Positions = "AAA"
            },
            ["kd"] = new()
            {
                // The UKW-D wiring of the KD machine held by the FRA in Sweden,
                // written as the plain letter pairs this library takes.
                Name = "Enigma KD, with a rewired UKW-D",
                Model = "KD",
                Reflector = "D",
                ReflectorPairs = "AK BO CT DV EP FN GL HM IJ QW RY SX UZ",
                Rotors = "KD-III KD-II KD-I",
                RingSettings = "AAA",
                Positions = "AAA"
            },
            ["swiss-k"] = new()
            {
                Name = "Swiss Enigma K",
                Model = "Commercial",
                Reflector = "G",
                Rotors = "SK-III SK-II SK-I",
                RingSettings = "AAA",
                Positions = "AAA",
                ReflectorPosition = "A"
            },
            ["railway"] = new()
            {
                Name = "Railway Enigma (Rocket)",
                Model = "Commercial",
                Reflector = "R",
                Rotors = "R-III R-II R-I",
                RingSettings = "AAA",
                Positions = "AAA",
                ReflectorPosition = "A"
            },
            ["tirpitz"] = new()
            {
                Name = "Enigma T (Tirpitz)",
                Model = "Tirpitz",
                Reflector = "T",
                Rotors = "T-III T-II T-I",
                RingSettings = "AAA",
                Positions = "AAA",
                ReflectorPosition = "A"
            },
            ["z30"] = new()
            {
                Name = "Enigma Z30, numbers only",
                Model = "Z30",
                CharacterMap = "Digits",
                Reflector = "Z",
                Rotors = "Z-III Z-II Z-I",
                RingSettings = "000",
                Positions = "000",
                ReflectorPosition = "0",
                ReflectorRingSetting = "0"
            },
            ["instruction-manual"] = new()
            {
                Name = "Enigma Instruction Manual, 1930",
                Reflector = "A",
                Rotors = "II I III",
                RingSettings = "XMV",
                Positions = "ABL",
                Plugboard = "AM FI NV PS TU WZ"
            }
        };

    public static bool TryGet(string name, out KeySheet keySheet) => All.TryGetValue(name, out keySheet!);
}
