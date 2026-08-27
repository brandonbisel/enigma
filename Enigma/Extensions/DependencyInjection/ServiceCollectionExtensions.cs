using Enigma.Reflectors;
using Enigma.Rotors;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnigmaServices(this IServiceCollection services)
    {
        services.AddSingleton<ICharacterMap, DefaultCharacterMap>();

        // Rotors are keyed by the name they are known by on a key sheet, and are transient
        // because each one carries its own position: two machines must never share an instance.
        services.AddKeyedTransient<IRotor, RotorI>("I");
        services.AddKeyedTransient<IRotor, RotorII>("II");
        services.AddKeyedTransient<IRotor, RotorIII>("III");
        services.AddKeyedTransient<IRotor, RotorIV>("IV");
        services.AddKeyedTransient<IRotor, RotorV>("V");
        services.AddKeyedTransient<IRotor, RotorVI>("VI");
        services.AddKeyedTransient<IRotor, RotorVII>("VII");
        services.AddKeyedTransient<IRotor, RotorVIII>("VIII");

        // The M4's thin fourth rotors, which sit to the left of the others.
        services.AddKeyedTransient<IRotor, RotorBeta>("BETA");
        services.AddKeyedTransient<IRotor, RotorGamma>("GAMMA");

        // Reflectors hold no state, so one of each is enough.
        services.AddKeyedSingleton<IReflector, ReflectorA>("A");
        services.AddKeyedSingleton<IReflector, ReflectorB>("B");
        services.AddKeyedSingleton<IReflector, ReflectorC>("C");
        services.AddKeyedSingleton<IReflector, ReflectorBThin>("B-THIN");
        services.AddKeyedSingleton<IReflector, ReflectorCThin>("C-THIN");

        // The board is patched per machine, so it is transient for the same reason as the rotors.
        services.AddTransient<IPlugBoard, PlugBoard>();

        services.AddSingleton<IEnigmaMachineFactory, EnigmaMachineFactory>();

        return services;
    }
}
