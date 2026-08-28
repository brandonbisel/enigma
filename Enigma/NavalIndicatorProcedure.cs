using Enigma.Models;

namespace Enigma;

/// <inheritdoc cref="INavalIndicatorProcedure"/>
public class NavalIndicatorProcedure : INavalIndicatorProcedure
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly ICharacterMap _characterMap;

    public NavalIndicatorProcedure(IEnigmaMachineFactory factory, ICharacterMap characterMap)
    {
        _factory = factory;
        _characterMap = characterMap;
    }

    public NavalIndicator Send(
        KeySheet dailyKey,
        BigramTable table,
        string keyGroup,
        string messageGroup,
        char firstFiller = 'X',
        char lastFiller = 'X')
    {
        ArgumentNullException.ThrowIfNull(dailyKey);
        ArgumentNullException.ThrowIfNull(table);

        var key = Trigram(keyGroup, nameof(keyGroup));
        var message = Trigram(messageGroup, nameof(messageGroup));

        return new NavalIndicator(
            key,
            message,
            MessageKey(dailyKey, message, lastFiller),
            Substitute(table, firstFiller + key, message + lastFiller));
    }

    public NavalIndicator Receive(KeySheet dailyKey, BigramTable table, string indicator)
    {
        ArgumentNullException.ThrowIfNull(dailyKey);
        ArgumentNullException.ThrowIfNull(table);

        var letters = Clean(indicator);

        if (letters.Length != 8)
        {
            throw new ArgumentException(
                $"A naval indicator is eight letters, four bigrams, but '{indicator}' is {letters.Length}.",
                nameof(indicator));
        }

        // The table is its own inverse, so reading is the same operation as writing.
        var columns = Enumerable
            .Range(0, 4)
            .Select(column => table.Substitute(letters.Substring(column * 2, 2)))
            .ToArray();

        var upper = string.Concat(columns.Select(pair => pair[0]));
        var lower = string.Concat(columns.Select(pair => pair[1]));

        // The filler that padded the trigram out is recovered along with it, which
        // is what lets the receiving station key a four wheel machine.
        var key = upper[1..];
        var message = lower[..3];

        return new NavalIndicator(key, message, MessageKey(dailyKey, message, lower[3]), letters);
    }

    /// <summary>
    /// Where the rotors start: one letter per wheel, read off the machine at the
    /// day's ground setting.
    ///
    /// The Kenngruppenbuch lists trigrams, which is the whole message key on a three
    /// wheel machine. An M4 has a fourth wheel to set, and what sets it is the
    /// filler — the letter that padded the trigram out to fill its bigram column.
    /// Both stations have it: the sender chose it, and the receiver reads it out of
    /// the indicator along with everything else. So the group typed is the trigram
    /// and its filler, taken to as many letters as the machine has wheels.
    /// </summary>
    private string MessageKey(KeySheet dailyKey, string messageGroup, char filler)
    {
        var wheels = dailyKey.Wheels().Count;
        var group = messageGroup + char.ToUpperInvariant(filler);

        if (wheels > group.Length)
        {
            throw new ArgumentException(
                $"A trigram and its filler set four wheels at most, but this machine has {wheels}.",
                nameof(dailyKey));
        }

        return Run(dailyKey, dailyKey.Positions, group[..wheels]);
    }

    // Its own machine, started at the ground setting and stepped by nothing else.
    private string Run(KeySheet dailyKey, string groundSetting, string text)
    {
        var machine = _factory.Create(dailyKey.WithPositions(Clean(groundSetting)));
        var input = Clean(text).Select(_characterMap.GetIndex);

        return string.Concat(machine.Translate(input).Select(_characterMap.GetCharacter));
    }

    /// <summary>
    /// The two padded trigrams are written one above the other and read downwards,
    /// so the letters that travel together were never next to each other.
    /// </summary>
    private static string Substitute(BigramTable table, string upper, string lower) =>
        string.Concat(upper.Zip(lower, (a, b) => table.Substitute(string.Concat(a, b))));

    private static string Trigram(string value, string field)
    {
        var cleaned = Clean(value);

        return cleaned.Length == 3
            ? cleaned
            : throw new ArgumentException(
                $"{field} is a trigram from the Kenngruppenbuch, but '{value}' is {cleaned.Length} letters.",
                field);
    }

    private static string Clean(string? value) =>
        new((value ?? string.Empty).Where(char.IsLetter).Select(char.ToUpperInvariant).ToArray());
}
