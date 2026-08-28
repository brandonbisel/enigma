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
            MessageKey(dailyKey, message),
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

        var key = string.Concat(columns.Select(pair => pair[0]))[1..];
        var message = string.Concat(columns.Select(pair => pair[1]))[..3];

        return new NavalIndicator(key, message, MessageKey(dailyKey, message), letters);
    }

    /// <summary>
    /// Where the rotors start. The Kenngruppenbuch lists trigrams, so what comes
    /// back from the machine sets three wheels however many the machine has: on an
    /// M4 the Greek wheel is not touched, and stays where the key sheet left it.
    /// </summary>
    private string MessageKey(KeySheet dailyKey, string messageGroup)
    {
        var wheels = dailyKey.Wheels().Count;
        var enciphered = Run(dailyKey, dailyKey.Positions, messageGroup);

        if (wheels <= 3)
        {
            return enciphered;
        }

        var ground = Clean(dailyKey.Positions);

        return ground[..(wheels - 3)] + enciphered;
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
