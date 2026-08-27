using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma;

public class EnigmaMachineFactory : IEnigmaMachineFactory
{
    private readonly IServiceProvider _services;

    public EnigmaMachineFactory(IServiceProvider services)
    {
        _services = services;
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

        return new EnigmaMachine(plugBoard, rotors, reflector);
    }

    private IRotor CreateRotor(RotorSettings settings)
    {
        var rotor = _services.GetRequiredKeyedService<IRotor>(settings.Name);

        rotor.SetRingSetting(settings.RingSetting);
        rotor.SetPosition(settings.Position);

        return rotor;
    }
}
