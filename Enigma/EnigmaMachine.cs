namespace Enigma;

public class EnigmaMachine : IEnigmaMachine
{
    private readonly IRotor[] _rotors;

    public IPlugBoard PlugBoard { get; }
    public IEnumerable<IRotor> Rotors => _rotors;
    public IReflector Reflector { get; }

    // Rotors are given in the order they sit in the machine, left to right,
    // so the last one is the fast rotor next to the entry wheel.
    public EnigmaMachine(IPlugBoard plugBoard, IEnumerable<IRotor> rotors, IReflector reflector)
    {
        PlugBoard = plugBoard;
        Reflector = reflector;
        _rotors = rotors.ToArray();

        if (_rotors.Length == 0)
        {
            throw new ArgumentException("An Enigma machine needs at least one rotor.", nameof(rotors));
        }
    }

    public int Translate(int input)
    {
        StepRotors();

        var value = PlugBoard.Translate(input);

        for (var i = _rotors.Length - 1; i >= 0; i--)
        {
            value = _rotors[i].Translate(value);
        }

        value = Reflector.Translate(value);

        for (var i = 0; i < _rotors.Length; i++)
        {
            value = _rotors[i].TranslateReverse(value);
        }

        return PlugBoard.Translate(value);
    }

    public IEnumerable<int> Translate(IEnumerable<int> input)
    {
        // Materialised deliberately: the rotors advance on every character, so deferring
        // execution would make the result depend on when the caller enumerates it.
        return input.Select(Translate).ToList();
    }

    private void StepRotors()
    {
        var fast = _rotors[^1];

        if (_rotors.Length >= 3)
        {
            var middle = _rotors[^2];

            if (middle.IsTurnoverPosition())
            {
                // The double step: sitting on its own turnover, the middle rotor is driven
                // by the pawl to its right and carries its left neighbour with it.
                middle.Step();
                _rotors[^3].Step();
            }
            else if (fast.IsTurnoverPosition())
            {
                middle.Step();
            }
        }
        else if (_rotors.Length == 2 && fast.IsTurnoverPosition())
        {
            _rotors[0].Step();
        }

        fast.Step();
    }
}
