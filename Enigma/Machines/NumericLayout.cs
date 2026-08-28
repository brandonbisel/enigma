namespace Enigma.Machines;

/// <summary>
/// The Enigma Z30, a numbers-only machine built for traffic that was numeric to
/// begin with — weather reports and the like. Three wheels of ten contacts, a
/// reflector that is both set and driven, no plugboard, and a keyboard of figures.
/// </summary>
public class NumericLayout : IMachineLayout
{
    public NumericLayout(IStepping drive)
    {
        Drive = drive;
    }

    public string Name => "Z30";
    public IStepping Drive { get; }
    public string DefaultEntryWheel => "STANDARD";
    public bool AllowsPlugBoard => false;

    public void Validate(IReadOnlyList<IRotor> rotors, IReflector reflector, IEntryWheel entryWheel)
    {
        if (rotors.Count != 3)
        {
            throw new ArgumentException(
                $"An Enigma Z carries three wheels, but {rotors.Count} were given.");
        }

        if (rotors.Any(rotor => rotor.IsThin))
        {
            throw new ArgumentException("An Enigma Z has no thin wheels; those belong to the naval M4.");
        }

        if (reflector is not IRotatingReflector)
        {
            throw new ArgumentException(
                $"An Enigma Z's reflector is driven by the leftmost wheel, but {reflector.Name} is fixed.");
        }
    }
}
