namespace Enigma.Tests.Reference;

/// <summary>
/// An independent Enigma implementation used only as a test oracle. It was written
/// from the mechanical description of the machine - rotors as permutations on a ring
/// that rotates (position) and whose letter ring can slip against the wiring
/// (Ringstellung), with pawls stepping the rotors before the current flows - and then
/// anchored on the published test vectors.
///
/// It deliberately duplicates the wiring tables and shares no code with the Enigma
/// library: that duplication is the entire point, so do not refactor it away. The
/// original Python it was transcribed from sits beside this file.
/// </summary>
internal sealed class ReferenceEnigma
{
    private const int Size = 26;

    private static readonly IReadOnlyDictionary<string, (string Wiring, string Notches)> Rotors =
        new Dictionary<string, (string, string)>
        {
            ["I"] = ("EKMFLGDQVZNTOWYHXUSPAIBRCJ", "Q"),
            ["II"] = ("AJDKSIRUXBLHWTMCQGZNPYFVOE", "E"),
            ["III"] = ("BDFHJLCPRTXVZNYEIWGAKMUSQO", "V"),
            ["IV"] = ("ESOVPZJAYQUIRHXLNFTGKDCMWB", "J"),
            ["V"] = ("VZBRGITYUPSDNHLXAWMJQOFECK", "Z"),
            ["VI"] = ("JPGVOUMFYQBENHZRDKASXLICTW", "MZ"),
            ["VII"] = ("NZJHGRCXMYSWBOUFAIVLPEKQDT", "MZ"),
            ["VIII"] = ("FKQHTLXOCBJSPDZRAMEWNIUYGV", "MZ")
        };

    private static readonly IReadOnlyDictionary<string, string> Reflectors =
        new Dictionary<string, string>
        {
            ["B"] = "YRUHQSLDPXNGOKMIEBFZCWVJAT",
            ["C"] = "FVPJIAOYEDRZXWGCTKUQSBNMHL"
        };

    private readonly ReferenceRotor[] _rotors;
    private readonly int[] _reflector;
    private readonly int[] _plugs;

    /// <param name="rotorNames">Left to right, as written on a key sheet.</param>
    public ReferenceEnigma(
        IReadOnlyList<string> rotorNames,
        string reflector,
        IReadOnlyList<int> positions,
        IReadOnlyList<int> ringSettings,
        IEnumerable<(int, int)>? plugs = null)
    {
        _rotors = rotorNames
            .Select((name, i) => new ReferenceRotor(Rotors[name], positions[i], ringSettings[i]))
            .ToArray();

        _reflector = Reflectors[reflector].Select(Index).ToArray();
        _plugs = Enumerable.Range(0, Size).ToArray();

        foreach (var (a, b) in plugs ?? [])
        {
            _plugs[a] = b;
            _plugs[b] = a;
        }
    }

    public IReadOnlyList<int> Positions => _rotors.Select(rotor => rotor.Position).ToList();

    public string Encrypt(string text) => string.Concat(text.Select(Press));

    private char Press(char character)
    {
        StepRotors();

        var value = _plugs[Index(character)];

        for (var i = _rotors.Length - 1; i >= 0; i--)
        {
            value = _rotors[i].Forward(value);
        }

        value = _reflector[value];

        for (var i = 0; i < _rotors.Length; i++)
        {
            value = _rotors[i].Backward(value);
        }

        return Character(_plugs[value]);
    }

    private void StepRotors()
    {
        var left = _rotors[0];
        var middle = _rotors[1];
        var right = _rotors[2];

        if (middle.AtNotch)
        {
            middle.Step();
            left.Step();
        }
        else if (right.AtNotch)
        {
            middle.Step();
        }

        right.Step();
    }

    private static int Index(char character) => character - 'A';

    private static char Character(int index) => (char)(index + 'A');

    private sealed class ReferenceRotor
    {
        private readonly int[] _map;
        private readonly int[] _inverse;
        private readonly int[] _notches;
        private readonly int _ring;

        public ReferenceRotor((string Wiring, string Notches) spec, int position, int ring)
        {
            _map = spec.Wiring.Select(Index).ToArray();
            _inverse = new int[Size];

            for (var contact = 0; contact < _map.Length; contact++)
            {
                _inverse[_map[contact]] = contact;
            }

            _notches = spec.Notches.Select(Index).ToArray();
            _ring = ring;
            Position = position;
        }

        public int Position { get; private set; }

        public bool AtNotch => _notches.Contains(Position);

        private int Offset => Wrap(Position - _ring);

        public void Step() => Position = Wrap(Position + 1);

        public int Forward(int contact) => Wrap(_map[Wrap(contact + Offset)] - Offset);

        public int Backward(int contact) => Wrap(_inverse[Wrap(contact + Offset)] - Offset);

        // Deliberately a different formulation from the library's, so that a mistake
        // in one cannot be mirrored by the same mistake in the other. Every caller
        // stays well inside a single wrap of the alphabet.
        private static int Wrap(int value) => (value + Size) % Size;
    }
}
