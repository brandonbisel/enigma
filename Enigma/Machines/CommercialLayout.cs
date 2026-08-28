namespace Enigma.Machines;

/// <summary>
/// The commercial machines that were not Zählwerk machines: the Enigma D of 1926
/// and the Enigma K that followed it, along with the Swiss K, the Railway Enigma
/// and Tirpitz, which are the same arrangement with other wheels.
///
/// Three wheels driven by pawls, so the double step is there; a keyboard-order
/// entry wheel; no plugboard, that being an Army fitting; and a reflector which is
/// settable to any of its positions but, unlike a Zählwerk machine's, stays where
/// it is put. Nothing drives it, so nothing here has to stop it turning.
///
/// What tells a D from a K is the wheels fitted, not the machine: the D's notches
/// are cut into the rotor bodies and the K's into the letter rings.
/// </summary>
public class CommercialLayout : IMachineLayout
{
    public CommercialLayout(IStepping drive)
    {
        Drive = drive;
    }

    public string Name => "Commercial";
    public IStepping Drive { get; }
    public string DefaultEntryWheel => "QWERTZ";
    public bool AllowsPlugBoard => false;

    public void Validate(IReadOnlyList<IRotor> rotors, IReflector reflector, IEntryWheel entryWheel)
    {
        if (rotors.Count != 3)
        {
            throw new ArgumentException(
                $"A commercial Enigma carries three wheels, but {rotors.Count} were given.");
        }

        if (rotors.Any(rotor => rotor.IsThin))
        {
            throw new ArgumentException(
                "A commercial Enigma has no thin wheels; those belong to the naval M4.");
        }

        if (reflector is not IRotatingReflector)
        {
            throw new ArgumentException(
                $"A commercial Enigma's reflector is set to a position, but {reflector.Name} is fixed.");
        }
    }
}
