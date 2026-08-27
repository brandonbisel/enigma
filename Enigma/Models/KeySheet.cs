namespace Enigma.Models;

/// <summary>
/// A day's settings written the way they appeared on a Wehrmacht key sheet
/// (Schlüsseltafel): the rotor order (Walzenlage), the ring settings
/// (Ringstellung), the starting positions (Grundstellung) and the plugboard
/// pairs (Steckerverbindungen).
///
/// The notation is inherently the Latin alphabet, so letters map A=0 through
/// Z=25. Ring settings and positions may equally be given as the one-based
/// numbers used on printed sheets, where 01 is A.
/// </summary>
public class KeySheet
{
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Reflector (Umkehrwalze): "A", "B" or "C", or the thin "B-Thin" and "C-Thin"
    /// of the naval M4, which are the only ones that leave room for a fourth rotor.
    /// </summary>
    public string Reflector { get; set; } = "B";

    /// <summary>
    /// Rotor order (Walzenlage) left to right, e.g. "I II III", or four wheels for
    /// an M4, e.g. "Beta II IV I", where the thin rotor is always leftmost.
    /// </summary>
    public string Rotors { get; set; } = string.Empty;

    /// <summary>Ring settings (Ringstellung), e.g. "AAA" or "01 01 01".</summary>
    public string RingSettings { get; set; } = string.Empty;

    /// <summary>Starting positions (Grundstellung), e.g. "AAA".</summary>
    public string Positions { get; set; } = string.Empty;

    /// <summary>Plugboard pairs (Steckerverbindungen), e.g. "AV BS CG DL".</summary>
    public string Plugboard { get; set; } = string.Empty;

    /// <summary>The reflector name, normalised.</summary>
    public string ReflectorName() => (Reflector ?? string.Empty).Trim().ToUpperInvariant();

    /// <summary>
    /// The rotors in the order they sit in the machine, left to right, with the
    /// ring settings and starting positions resolved to zero-based indices.
    /// </summary>
    public IReadOnlyList<RotorPlacement> Wheels()
    {
        var rotors = ParseRotors(Rotors);
        var rings = ParseWheelSettings(RingSettings, rotors.Count, nameof(RingSettings));
        var positions = ParseWheelSettings(Positions, rotors.Count, nameof(Positions));

        return rotors
            .Select((name, i) => new RotorPlacement(name, positions[i], rings[i]))
            .ToList();
    }

    /// <summary>The same key sheet with the rotors started somewhere else.</summary>
    public KeySheet WithPositions(string positions) => new()
    {
        Name = Name,
        Reflector = Reflector,
        Rotors = Rotors,
        RingSettings = RingSettings,
        Positions = positions,
        Plugboard = Plugboard
    };

    /// <summary>The plugboard cables, as pairs of zero-based letter indices.</summary>
    public IReadOnlyDictionary<int, int> Cables() => ParsePlugboard(Plugboard);

    private static List<string> ParseRotors(string value)
    {
        var rotors = SplitNames(value).Select(name => name.ToUpperInvariant()).ToList();

        if (rotors.Count == 0)
        {
            throw new FormatException("No rotors given. Expected a rotor order such as \"I II III\".");
        }

        return rotors;
    }

    /// <summary>
    /// Accepts "AAA", "A A A" or the one-based numbers printed on a key sheet,
    /// "01 01 01". An empty value means every wheel sits at A.
    /// </summary>
    private static List<int> ParseWheelSettings(string value, int expected, string field)
    {
        var tokens = SplitNames(value);

        if (tokens.Count == 0)
        {
            return Enumerable.Repeat(0, expected).ToList();
        }

        // "AAA" written as one word is the commonest form, so split it per letter.
        if (tokens.Count == 1 && tokens[0].Length == expected && tokens[0].All(char.IsLetter))
        {
            tokens = tokens[0].Select(letter => letter.ToString()).ToList();
        }

        if (tokens.Count != expected)
        {
            throw new FormatException(
                $"{field} has {tokens.Count} values but there are {expected} rotors.");
        }

        return tokens.Select(token => ParseWheelSetting(token, field)).ToList();
    }

    private static int ParseWheelSetting(string token, string field)
    {
        if (token.Length == 1 && char.IsLetter(token[0]))
        {
            return char.ToUpperInvariant(token[0]) - 'A';
        }

        if (int.TryParse(token, out var number))
        {
            if (number is < 1 or > 26)
            {
                throw new FormatException(
                    $"{field} value '{token}' is out of range. Key sheets number the wheels 01 to 26.");
            }

            return number - 1;
        }

        throw new FormatException(
            $"{field} value '{token}' is neither a letter nor a number between 01 and 26.");
    }

    /// <summary>
    /// Accepts pairs as "AV BS CG", "AV-BS-CG" or "av bs cg". Each pair is one
    /// cable, so a letter may appear only once across the whole board.
    /// </summary>
    private static Dictionary<int, int> ParsePlugboard(string value)
    {
        var pairs = new Dictionary<int, int>();
        var used = new HashSet<char>();

        foreach (var pair in SplitPairs(value))
        {
            var cable = pair.ToUpperInvariant();

            if (cable.Length != 2 || !cable.All(char.IsAsciiLetter))
            {
                throw new FormatException(
                    $"Plugboard entry '{pair}' is not a pair of letters, such as \"AV\".");
            }

            if (cable[0] == cable[1])
            {
                throw new FormatException($"Plugboard entry '{pair}' connects a letter to itself.");
            }

            foreach (var letter in cable)
            {
                if (!used.Add(letter))
                {
                    throw new FormatException(
                        $"Plugboard letter '{letter}' is used more than once; each letter takes one cable.");
                }
            }

            pairs[cable[0] - 'A'] = cable[1] - 'A';
        }

        return pairs;
    }

    // Rotor names may contain a hyphen, as "K-I" or "UKW-D" do, so names are split
    // only on whitespace and commas. The hyphen is a separator in plugboard
    // notation alone, where "AV-BS" and "AV BS" mean the same thing.
    private static List<string> SplitNames(string? value) =>
        (value ?? string.Empty)
            .Split([' ', '\t', ','], StringSplitOptions.RemoveEmptyEntries)
            .ToList();

    private static List<string> SplitPairs(string? value) =>
        (value ?? string.Empty)
            .Split([' ', '\t', ',', '-', '/'], StringSplitOptions.RemoveEmptyEntries)
            .ToList();

}
