using Enigma.Models;

namespace Enigma.App;

/// <summary>
/// A machine keyed for the day and worked one keystroke at a time, which is how an
/// operator used it and how a front end drives it. The console reads whole lines
/// and a panel reads single keys, but both are pressing the same keyboard.
///
/// Nothing here decides how a message is written out: preparing text and breaking
/// it into groups are conventions of the signaller, not of the machine, and live in
/// <see cref="MessageText"/>.
/// </summary>
public sealed class EnigmaSession
{
    private readonly IEnigmaMachineFactory _factory;

    private EnigmaSession(IEnigmaMachineFactory factory, KeySheet keySheet, IEnigmaMachine machine)
    {
        _factory = factory;
        KeySheet = keySheet;
        Machine = machine;
    }

    /// <summary>
    /// Keys a machine from a key sheet, reporting a sheet it cannot be built from
    /// rather than throwing. The settings faults are the ones an operator can fix.
    /// </summary>
    public static MachineBuildResult Open(IEnigmaMachineFactory factory, KeySheet keySheet)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(keySheet);

        try
        {
            return MachineBuildResult.Keyed(
                new EnigmaSession(factory, keySheet, factory.Create(keySheet)));
        }
        catch (Exception exception)
            when (exception is ArgumentException or FormatException or InvalidOperationException)
        {
            return MachineBuildResult.Rejected(exception);
        }
    }

    public IEnigmaMachine Machine { get; private set; }

    /// <summary>The settings this machine was keyed from. Resetting returns to them.</summary>
    public KeySheet KeySheet { get; }

    public ICharacterMap Alphabet => Machine.CharacterMap;

    /// <summary>The wheels as they stand, left to right, in the machine's own alphabet.</summary>
    public IReadOnlyList<int> Window => Machine.Rotors.Select(rotor => rotor.Position).ToArray();

    /// <summary>The rotor window as the operator read it.</summary>
    public string WindowText => string.Concat(Window.Select(Alphabet.GetCharacter));

    /// <summary>
    /// Where a turning reflector stands, on the machines that have one, and null on
    /// every machine whose reflector is fixed.
    /// </summary>
    public int? ReflectorPosition =>
        Machine.Reflector is IRotatingReflector turning ? turning.Position : null;

    /// <summary>
    /// The path the current took on the last keypress. Only recorded while something
    /// is subscribed to <see cref="Traced"/>: a machine nobody is watching does not
    /// build traces, which is what keeps bulk work cheap.
    /// </summary>
    public TranslationTrace? LastTrace { get; private set; }

    /// <summary>
    /// Presses a key. Returns the lamp that lit, or null when the character is not
    /// on this machine's keyboard — a key that is not there cannot be pressed, so
    /// the wheels do not turn for it either.
    /// </summary>
    public char? Press(char key)
    {
        var contact = IndexOf(key);

        if (contact < 0)
        {
            return null;
        }

        return Alphabet.GetCharacter(Machine.Translate(contact));
    }

    /// <summary>
    /// Types a run of text, dropping anything the keyboard has no key for. The
    /// wheels keep turning across the whole run, exactly as they would under an
    /// operator's hands.
    /// </summary>
    public string Type(string text)
    {
        var keyed = (text ?? string.Empty).Select(IndexOf).Where(contact => contact >= 0);

        return string.Concat(Machine.Translate(keyed).Select(Alphabet.GetCharacter));
    }

    /// <summary>
    /// Puts the machine back to the settings it was keyed from: the wheels returned
    /// to their starting positions, a turning reflector back to its own, and the
    /// board patched as the key sheet has it. Built afresh rather than wound back,
    /// so this cannot drift from what keying a machine does in the first place.
    /// </summary>
    public void Reset()
    {
        Machine = _factory.Create(KeySheet);

        // The recorder is what feeds both LastTrace and the watchers, so it is the
        // thing that must follow the machine, not the watchers themselves.
        if (Watching is not null)
        {
            Machine.Translated += Record;
        }

        LastTrace = null;
    }

    /// <summary>
    /// Runs a cable between two letters on the board. Refused, as the board itself
    /// refuses it, if either letter already has a cable.
    /// </summary>
    public void Patch(char from, char to) =>
        Machine.PlugBoard.Connect(Require(from, nameof(from)), Require(to, nameof(to)));

    /// <summary>Pulls a cable out. Refused if those two letters are not joined.</summary>
    public void Unpatch(char from, char to) =>
        Machine.PlugBoard.Disconnect(Require(from, nameof(from)), Require(to, nameof(to)));

    public bool IsPatched(char from, char to) =>
        Machine.PlugBoard.IsConnected(Require(from, nameof(from)), Require(to, nameof(to)));

    /// <summary>The cables on the board, each reported once.</summary>
    public IEnumerable<(char From, char To)> Cables() =>
        Machine.PlugBoard.GetConnections()
            .Select(cable => (Alphabet.GetCharacter(cable.Item1), Alphabet.GetCharacter(cable.Item2)));

    private Action<TranslationTrace>? Watching { get; set; }

    /// <summary>
    /// Watches the current flow. The subscription follows the machine across a
    /// <see cref="Reset"/>, which builds a new one.
    /// </summary>
    public event Action<TranslationTrace>? Traced
    {
        add
        {
            if (Watching is null)
            {
                Machine.Translated += Record;
            }

            Watching += value;
        }
        remove
        {
            Watching -= value;

            if (Watching is null)
            {
                Machine.Translated -= Record;
            }
        }
    }

    private void Record(TranslationTrace trace)
    {
        LastTrace = trace;
        Watching?.Invoke(trace);
    }

    // Typing in lower case is a convenience the machine never had. It is applied
    // only as a fallback, so an alphabet that distinguishes case keeps both.
    private int IndexOf(char character)
    {
        var index = Alphabet.GetIndex(character);

        return index >= 0 ? index : Alphabet.GetIndex(char.ToUpperInvariant(character));
    }

    private int Require(char character, string parameter)
    {
        var index = IndexOf(character);

        return index >= 0
            ? index
            : throw new ArgumentException(
                $"'{character}' is not in the {Alphabet.Name} alphabet.", parameter);
    }
}
