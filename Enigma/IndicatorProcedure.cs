using Enigma.Models;

namespace Enigma;

public class IndicatorProcedure : IIndicatorProcedure
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly ICharacterMap _characterMap;

    public IndicatorProcedure(IEnigmaMachineFactory factory, ICharacterMap characterMap)
    {
        _factory = factory;
        _characterMap = characterMap;
    }

    public string EncipherMessageKey(
        KeySheet dailyKey,
        string groundSetting,
        string messageKey,
        bool doubled = false)
    {
        var wheels = dailyKey.Wheels().Count;

        Validate(groundSetting, wheels, nameof(groundSetting));
        Validate(messageKey, wheels, nameof(messageKey));

        // Sent twice until 1938, so the receiver could tell a garbled indicator from
        // a good one. It also handed Rejewski the relation that broke the machine.
        var plain = doubled ? messageKey + messageKey : messageKey;

        return Run(dailyKey, groundSetting, plain);
    }

    public string RecoverMessageKey(KeySheet dailyKey, string groundSetting, string indicator)
    {
        var wheels = dailyKey.Wheels().Count;

        Validate(groundSetting, wheels, nameof(groundSetting));

        var cleaned = Clean(indicator);

        if (cleaned.Length != wheels && cleaned.Length != wheels * 2)
        {
            throw new ArgumentException(
                $"An indicator for {wheels} rotors is {wheels} letters, or {wheels * 2} if it was sent twice, " +
                $"but '{indicator}' is {cleaned.Length}.",
                nameof(indicator));
        }

        var deciphered = Run(dailyKey, groundSetting, cleaned);

        if (cleaned.Length == wheels)
        {
            return deciphered;
        }

        var first = deciphered[..wheels];
        var second = deciphered[wheels..];

        if (first != second)
        {
            throw new ArgumentException(
                $"The two halves of the indicator disagree ('{first}' and '{second}'), " +
                "so the message key did not come through intact.",
                nameof(indicator));
        }

        return first;
    }

    // Each call gets its own machine: the rotors must start at the ground setting
    // and nothing else may have stepped them first.
    private string Run(KeySheet dailyKey, string groundSetting, string text)
    {
        var machine = _factory.Create(dailyKey.WithPositions(Clean(groundSetting)));
        var input = Clean(text).Select(_characterMap.GetIndex);

        return string.Concat(machine.Translate(input).Select(_characterMap.GetCharacter));
    }

    private static string Clean(string? value) =>
        new((value ?? string.Empty).Where(char.IsLetter).Select(char.ToUpperInvariant).ToArray());

    private static void Validate(string value, int wheels, string field)
    {
        var cleaned = Clean(value);

        if (cleaned.Length != wheels)
        {
            throw new ArgumentException(
                $"{field} must give one letter per rotor, so {wheels} for this machine, but '{value}' has {cleaned.Length}.",
                field);
        }
    }
}
