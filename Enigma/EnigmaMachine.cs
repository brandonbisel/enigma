using Enigma.Machines;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Enigma;

public class EnigmaMachine : IEnigmaMachine
{
    private readonly IRotor[] _rotors;
    private readonly IEntryWheel _entryWheel;
    private readonly IStepping _stepping;
    private readonly ILogger _logger;

    public IPlugBoard PlugBoard { get; }
    public IEnumerable<IRotor> Rotors => _rotors;
    public IReflector Reflector { get; }
    public ICharacterMap CharacterMap { get; }

    // Rotors are given in the order they sit in the machine, left to right,
    // so the last one is the fast rotor next to the entry wheel. The logger and
    // character map are optional and only shape the diagnostic trace.
    public EnigmaMachine(
        IPlugBoard plugBoard,
        IEnumerable<IRotor> rotors,
        IReflector reflector,
        ILogger<EnigmaMachine>? logger = null,
        ICharacterMap? characterMap = null,
        IEntryWheel? entryWheel = null,
        IStepping? stepping = null)
    {
        _entryWheel = entryWheel ?? EntryWheel.Standard;
        _stepping = stepping ?? new PawlDrive();
        PlugBoard = plugBoard;
        Reflector = reflector;
        _rotors = rotors.ToArray();
        _logger = logger ?? NullLogger<EnigmaMachine>.Instance;
        CharacterMap = characterMap ?? Enigma.CharacterMap.Latin;

        if (_rotors.Length == 0)
        {
            throw new ArgumentException("An Enigma machine needs at least one rotor.", nameof(rotors));
        }
    }

    public int Translate(int input)
    {
        _stepping.Advance(_rotors, Reflector);

        // The trace is only assembled when someone is listening, so the ordinary
        // path costs nothing more than a null check per component.
        var trace = _logger.IsEnabled(LogLevel.Debug) ? new List<string>() : null;

        var value = PlugBoard.Translate(input);
        trace?.Add($"plug {Format(input)}>{Format(value)}");

        value = Step(_entryWheel.Name, value, _entryWheel.ToContact, trace);

        for (var i = _rotors.Length - 1; i >= 0; i--)
        {
            value = Step(_rotors[i].Name, value, _rotors[i].Translate, trace);
        }

        value = Step(Reflector.Name, value, Reflector.Translate, trace);

        for (var i = 0; i < _rotors.Length; i++)
        {
            value = Step($"{_rotors[i].Name}'", value, _rotors[i].TranslateReverse, trace);
        }

        value = Step($"{_entryWheel.Name}'", value, _entryWheel.ToLamp, trace);

        var output = PlugBoard.TranslateReverse(value);
        trace?.Add($"plug {Format(value)}>{Format(output)}");

        if (trace is not null)
        {
            _logger.LogDebug(
                "{Input} -> {Output}   window {Window}   {Path}",
                Format(input),
                Format(output),
                Window(),
                string.Join(" | ", trace));
        }

        return output;
    }

    public IEnumerable<int> Translate(IEnumerable<int> input)
    {
        // Materialised deliberately: the rotors advance on every character, so deferring
        // execution would make the result depend on when the caller enumerates it.
        return input.Select(Translate).ToList();
    }

    private int Step(string name, int value, Func<int, int> translate, List<string>? trace)
    {
        var output = translate(value);

        trace?.Add($"{name} {Format(value)}>{Format(output)}");

        return output;
    }

    private string Window() => string.Concat(_rotors.Select(rotor => Format(rotor.Position)));

    private string Format(int contact) => CharacterMap.GetCharacter(contact).ToString();
}
