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
    public IMachineLayout Layout { get; }

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
        IMachineLayout? layout = null)
    {
        _entryWheel = entryWheel ?? EntryWheel.Standard;
        // The drive is a property of the model, so it is taken from the layout
        // rather than given separately: the two could not then disagree.
        Layout = layout ?? new ServiceLayout(new PawlDrive());
        _stepping = Layout.Drive;
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

    public event Action<TranslationTrace>? Translated;

    public int Translate(int input)
    {
        _stepping.Advance(_rotors, Reflector);

        // The trace is only assembled when someone is listening, so the ordinary
        // path costs nothing more than a null check per component.
        var watchers = Translated;
        var tracing = watchers is not null || _logger.IsEnabled(LogLevel.Debug);
        var steps = tracing ? new List<TranslationStep>(2 * _rotors.Length + 4) : null;

        var value = PlugBoard.Translate(input);
        steps?.Add(new TranslationStep("plug", input, value));

        value = Step(_entryWheel.Name, value, _entryWheel.ToContact, steps);

        for (var i = _rotors.Length - 1; i >= 0; i--)
        {
            value = Step(_rotors[i].Name, value, _rotors[i].Translate, steps);
        }

        value = Step(Reflector.Name, value, Reflector.Translate, steps);

        for (var i = 0; i < _rotors.Length; i++)
        {
            value = Step($"{_rotors[i].Name}'", value, _rotors[i].TranslateReverse, steps);
        }

        value = Step($"{_entryWheel.Name}'", value, _entryWheel.ToLamp, steps);

        var output = PlugBoard.TranslateReverse(value);
        steps?.Add(new TranslationStep("plug", value, output));

        if (steps is not null)
        {
            // One structure, two readers: the log line is formatted from the same
            // trace the watchers get, so a diagnostic and a display cannot disagree.
            var trace = new TranslationTrace(input, output, Window(), steps);

            LogTrace(trace);
            watchers?.Invoke(trace);
        }

        return output;
    }

    public IEnumerable<int> Translate(IEnumerable<int> input)
    {
        // Materialised deliberately: the rotors advance on every character, so deferring
        // execution would make the result depend on when the caller enumerates it.
        return input.Select(Translate).ToList();
    }

    private int Step(string name, int value, Func<int, int> translate, List<TranslationStep>? steps)
    {
        var output = translate(value);

        steps?.Add(new TranslationStep(name, value, output));

        return output;
    }

    private void LogTrace(TranslationTrace trace)
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }

        _logger.LogDebug(
            "{Input} -> {Output}   window {Window}   {Path}",
            Format(trace.Input),
            Format(trace.Output),
            string.Concat(trace.Window.Select(Format)),
            string.Join(" | ", trace.Steps.Select(Describe)));
    }

    private string Describe(TranslationStep step) =>
        $"{step.Component} {Format(step.Input)}>{Format(step.Output)}";

    private int[] Window() => _rotors.Select(rotor => rotor.Position).ToArray();

    private string Format(int contact) => CharacterMap.GetCharacter(contact).ToString();
}
