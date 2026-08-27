namespace Enigma.Machines;

/// <summary>
/// The Zählwerk machines — the commercial A28/G31 and the Abwehr G-31 variants.
/// Three wheels driven by cogs rather than pawls, a reflector that is both set and
/// driven, a keyboard-order entry wheel, and no plugboard at all.
/// </summary>
public class GearLayout : IMachineLayout
{
    public GearLayout(IStepping drive)
    {
        Drive = drive;
    }

    public string Name => "G-31";
    public IStepping Drive { get; }
    public string DefaultEntryWheel => "QWERTZ";
    public bool AllowsPlugBoard => false;

    public void Validate(IReadOnlyList<IRotor> rotors, IReflector reflector, IEntryWheel entryWheel)
    {
        if (rotors.Count != 3)
        {
            throw new ArgumentException(
                $"A Zählwerk Enigma carries three wheels, but {rotors.Count} were given.");
        }

        if (rotors.Any(rotor => rotor.IsThin))
        {
            throw new ArgumentException("A Zählwerk Enigma has no thin wheels; those belong to the naval M4.");
        }

        if (reflector is not IRotatingReflector)
        {
            throw new ArgumentException(
                $"A Zählwerk Enigma's reflector turns during encipherment, but {reflector.Name} is fixed.");
        }
    }
}
