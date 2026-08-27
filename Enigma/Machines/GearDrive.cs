using Microsoft.Extensions.Logging;

namespace Enigma.Machines;

/// <summary>
/// The cogwheel drive of the Zählwerk machines. It is a plain odometer: a wheel
/// carries into the one beside it only when it passes a notch, and nothing is ever
/// driven twice. That is the whole difference from a pawl machine — there is no
/// double step, so the middle wheel advances once every twenty six keypresses and
/// not otherwise.
///
/// The chain does not stop at the leftmost wheel. On these machines the reflector
/// turns as well, and is the last thing the carry reaches.
/// </summary>
public class GearDrive : IStepping
{
    private readonly ILogger? _logger;

    public GearDrive(ILogger<GearDrive>? logger = null)
    {
        _logger = logger;
    }

    public string Name => "Gear";

    public void Advance(IReadOnlyList<IRotor> rotors, IReflector reflector)
    {
        for (var wheel = rotors.Count - 1; wheel >= 0; wheel--)
        {
            // Read the notch before stepping: the carry depends on where the wheel
            // was, not where it lands.
            var carries = rotors[wheel].IsTurnoverPosition();

            rotors[wheel].Step();

            if (!carries)
            {
                return;
            }
        }

        if (reflector is IRotatingReflector ukw)
        {
            ukw.Step();

            _logger?.LogTrace("Carry reached the reflector: {Reflector} advanced to {Position}",
                ukw.Name, ukw.Position);
        }
    }
}
