using Microsoft.Extensions.Logging;

namespace Enigma.Machines;

/// <summary>
/// The pawl and ratchet drive of the service Enigmas. A pawl rides on the notched
/// ring of the wheel to its right, which produces the double step: a middle rotor
/// sitting on its own turnover is driven by that pawl and carries its left
/// neighbour with it, so it advances twice in successive keypresses.
/// </summary>
public class PawlDrive : IStepping
{
    private readonly ILogger? _logger;

    public PawlDrive(ILogger<PawlDrive>? logger = null)
    {
        _logger = logger;
    }

    public string Name => "Pawl";

    public void Advance(IReadOnlyList<IRotor> rotors, IReflector reflector)
    {
        var fast = rotors[^1];

        if (rotors.Count >= 3)
        {
            var middle = rotors[^2];

            if (middle.IsTurnoverPosition())
            {
                middle.Step();
                rotors[^3].Step();

                _logger?.LogTrace("Double step: {Rotor} and {Left} advanced", middle.Name, rotors[^3].Name);
            }
            else if (fast.IsTurnoverPosition())
            {
                middle.Step();

                _logger?.LogTrace("Turnover: {Rotor} advanced {Middle}", fast.Name, middle.Name);
            }
        }
        else if (rotors.Count == 2 && fast.IsTurnoverPosition())
        {
            rotors[0].Step();
        }

        fast.Step();
    }
}
