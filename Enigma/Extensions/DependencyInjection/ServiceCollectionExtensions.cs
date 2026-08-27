using Enigma.Machines;
using Microsoft.Extensions.Logging;
using Enigma.Parts;
using Enigma.Reflectors;
using Enigma.Rotors;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnigmaServices(this IServiceCollection services)
    {
        services.AddSingleton<ICharacterMap, DefaultCharacterMap>();
        services.AddKeyedSingleton<ICharacterMap>("LATIN", (_, _) => CharacterMap.Latin);

        // The plugboard has one contact per character, so it is built for whichever
        // alphabet the machine works in rather than resolved ready-made.
        services.AddSingleton<Func<ICharacterMap, IPlugBoard>>(_ => map => new PlugBoard(map));

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

        // The Zählwerk wheels, with seventeen, fifteen and eleven notches.
        services.AddKeyedTransient<IRotor, RotorGI>("G-I");
        services.AddKeyedTransient<IRotor, RotorGII>("G-II");
        services.AddKeyedTransient<IRotor, RotorGIII>("G-III");
        services.AddKeyedTransient<IRotor, RotorG312I>("G312-I");
        services.AddKeyedTransient<IRotor, RotorG312II>("G312-II");
        services.AddKeyedTransient<IRotor, RotorG312III>("G312-III");

        // Reflectors hold no state, so one of each is enough.
        services.AddKeyedSingleton<IReflector, ReflectorA>("A");
        services.AddKeyedSingleton<IReflector, ReflectorB>("B");
        services.AddKeyedSingleton<IReflector, ReflectorC>("C");
        services.AddKeyedSingleton<IReflector, ReflectorBThin>("B-THIN");
        services.AddKeyedSingleton<IReflector, ReflectorCThin>("C-THIN");

        // The Zählwerk reflectors turn, so each machine needs its own.
        services.AddKeyedTransient<IReflector, ReflectorG>("G");
        services.AddKeyedTransient<IReflector, ReflectorG312>("G312");

        // The board is patched per machine, so it is transient for the same reason as the rotors.
        services.AddTransient<IPlugBoard, PlugBoard>();

        // Entry wheels hold no state, and the service machines all used the
        // straight-through one.
        services.AddKeyedSingleton<IEntryWheel>("STANDARD", (_, _) => EntryWheel.Standard);
        services.AddKeyedSingleton<IEntryWheel>("QWERTZ", (_, _) => EntryWheel.Qwertz);

        services.AddSingleton<IStepping, PawlDrive>();
        services.AddKeyedSingleton<IMachineLayout>("SERVICE",
            (provider, _) => new ServiceLayout(provider.GetRequiredService<IStepping>()));
        services.AddKeyedSingleton<IMachineLayout>("G-31",
            (provider, _) => new GearLayout(
                new GearDrive(provider.GetService<ILogger<GearDrive>>())));

        services.AddSingleton<IPartsCatalogue, BuiltInPartsCatalogue>();
        services.AddSingleton<IEnigmaMachineFactory, EnigmaMachineFactory>();
        services.AddSingleton<IIndicatorProcedure, IndicatorProcedure>();

        return services;
    }
}
