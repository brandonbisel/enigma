using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enigma;

public class EnigmaMachineFactory : IEnigmaMachineFactory
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EnigmaMachineFactory>? _logger;

    public EnigmaMachineFactory(IServiceProvider services)
    {
        _services = services;
        _logger = services.GetService<ILogger<EnigmaMachineFactory>>();
    }

    public IEnigmaMachine Create(EnigmaSettings settings)
    {
        var rotors = settings.Rotors.Select(CreateRotor).ToList();
        var reflector = _services.GetRequiredKeyedService<IReflector>(settings.Reflector);
        var plugBoard = _services.GetRequiredService<IPlugBoard>();

        foreach (var (input, output) in settings.Plugboard)
        {
            plugBoard.Connect(input, output);
        }

        _logger?.LogDebug(
            "Machine {Name}: rotors {Rotors}, reflector {Reflector}, positions {Positions}, rings {Rings}, plugs {Plugs}",
            settings.Name,
            string.Join(' ', settings.Rotors.Select(rotor => rotor.Name)),
            settings.Reflector,
            string.Join(' ', settings.Rotors.Select(rotor => rotor.Position)),
            string.Join(' ', settings.Rotors.Select(rotor => rotor.RingSetting)),
            plugBoard.GetConnections().Count() / 2);

        return new EnigmaMachine(
            plugBoard,
            rotors,
            reflector,
            _services.GetService<ILogger<EnigmaMachine>>(),
            _services.GetService<ICharacterMap>());
    }

    private IRotor CreateRotor(RotorSettings settings)
    {
        var rotor = _services.GetRequiredKeyedService<IRotor>(settings.Name);

        rotor.SetRingSetting(settings.RingSetting);
        rotor.SetPosition(settings.Position);

        return rotor;
    }
}
