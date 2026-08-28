using Microsoft.Extensions.Logging;

namespace Enigma.Machines;

/// <summary>
/// Pawls and ratchets, as <see cref="PawlDrive"/>, but with one more pawl than
/// there are wheels: the extra one rides the leftmost wheel's notch ring and drives
/// the reflector. The Enigma Z30 is built this way.
///
/// That extra pawl also gives the leftmost wheel a double step, for the same reason
/// the middle wheel has one on a service machine — a pawl dropping into a notch
/// engages the ratchet of the wheel it rests on as well as the one to its left. It
/// is precisely the pawl a service Enigma does not have, which is why its leftmost
/// wheel never advances twice and this one does.
///
/// Kept apart from <see cref="PawlDrive"/> rather than folded into it: how far the
/// chain of pawls reaches is what tells the two machines apart, and the service
/// drive is pinned by four historical messages that would be poor things to risk
/// for the sake of sharing a dozen lines.
/// </summary>
public class ReflectorPawlDrive : IStepping
{
    private readonly ILogger? _logger;

    public ReflectorPawlDrive(ILogger<ReflectorPawlDrive>? logger = null)
    {
        _logger = logger;
    }

    public string Name => "Pawl, with a driven reflector";

    public void Advance(IReadOnlyList<IRotor> rotors, IReflector reflector)
    {
        // Every notch is read before anything turns, because a wheel that has just
        // moved off its notch must still drive its neighbour this keypress.
        var driven = new bool[rotors.Count];
        var reflectorDriven = false;

        for (var wheel = 0; wheel < rotors.Count; wheel++)
        {
            if (!rotors[wheel].IsTurnoverPosition())
            {
                continue;
            }

            // The pawl over the wheel to the left drops through this notch and
            // engages both ratchets. Over the leftmost wheel that pawl is the one
            // carrying the reflector.
            driven[wheel] = true;

            if (wheel > 0)
            {
                driven[wheel - 1] = true;
            }
            else
            {
                reflectorDriven = true;
            }
        }

        // The rightmost pawl rests on nothing, so it engages on every keypress.
        driven[^1] = true;

        for (var wheel = 0; wheel < rotors.Count; wheel++)
        {
            if (driven[wheel])
            {
                rotors[wheel].Step();
            }
        }

        if (reflectorDriven && reflector is IRotatingReflector turning)
        {
            turning.Step();

            _logger?.LogTrace("The reflector was carried by {Rotor}", rotors[0].Name);
        }
    }
}
