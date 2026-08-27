namespace Enigma.Machines;

/// <summary>
/// The service Enigmas: the three wheel Enigma I and M3, and the four wheel naval
/// M4. They are one layout because they differ in the wheels fitted rather than in
/// how the machine works, and the fitment rules below are what tell them apart.
/// </summary>
public class ServiceLayout : IMachineLayout
{
    public ServiceLayout(IStepping drive)
    {
        Drive = drive;
    }

    public string Name => "Service";
    public IStepping Drive { get; }
    public string DefaultEntryWheel => "STANDARD";
    public bool AllowsPlugBoard => true;

    /// <summary>
    /// Rejects machines that could not be assembled. The thin rotors are half width
    /// and only fit in the space a thin reflector frees, so the fourth wheel and the
    /// thin reflector always come as a pair, and the thin wheel is always leftmost.
    /// </summary>
    public void Validate(IReadOnlyList<IRotor> rotors, IReflector reflector, IEntryWheel entryWheel)
    {
        if (rotors.Count is not (3 or 4))
        {
            throw new ArgumentException(
                $"An Enigma carries three rotors, or four on the naval M4, but {rotors.Count} were given.");
        }

        var thin = rotors.Where(rotor => rotor.IsThin).ToList();

        if (rotors.Count == 4)
        {
            if (!reflector.IsThin)
            {
                throw new ArgumentException(
                    $"A fourth rotor only fits beside a thin reflector, but {reflector.Name} is full width.");
            }

            if (thin.Count != 1 || !rotors[0].IsThin)
            {
                throw new ArgumentException(
                    "A four rotor machine carries exactly one thin rotor, and it sits leftmost.");
            }

            return;
        }

        if (reflector.IsThin)
        {
            throw new ArgumentException(
                $"The thin reflector {reflector.Name} leaves a gap unless a fourth rotor fills it.");
        }

        if (thin.Count > 0)
        {
            throw new ArgumentException(
                $"The thin rotor {thin[0].Name} only fits in a four rotor machine beside a thin reflector.");
        }
    }
}
