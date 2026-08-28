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
        services.AddKeyedSingleton<ICharacterMap>("DIGITS", (_, _) => CharacterMap.Digits);

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

        // The commercial wheels: the same three wirings, notched differently. The D's
        // notches are cut into the rotor bodies, the K's into the letter rings.
        services.AddKeyedTransient<IRotor, RotorDI>("D-I");
        services.AddKeyedTransient<IRotor, RotorDII>("D-II");
        services.AddKeyedTransient<IRotor, RotorDIII>("D-III");
        services.AddKeyedTransient<IRotor, RotorKI>("K-I");
        services.AddKeyedTransient<IRotor, RotorKII>("K-II");
        services.AddKeyedTransient<IRotor, RotorKIII>("K-III");

        // The Swiss Air Force's rewired K, the Railway Enigma as found in K438, and
        // the eight five-notched wheels of the Enigma T.
        services.AddKeyedTransient<IRotor, RotorSwissKI>("SK-I");
        services.AddKeyedTransient<IRotor, RotorSwissKII>("SK-II");
        services.AddKeyedTransient<IRotor, RotorSwissKIII>("SK-III");
        services.AddKeyedTransient<IRotor, RotorRailwayI>("R-I");
        services.AddKeyedTransient<IRotor, RotorRailwayII>("R-II");
        services.AddKeyedTransient<IRotor, RotorRailwayIII>("R-III");
        services.AddKeyedTransient<IRotor, RotorTirpitzI>("T-I");
        services.AddKeyedTransient<IRotor, RotorTirpitzII>("T-II");
        services.AddKeyedTransient<IRotor, RotorTirpitzIII>("T-III");
        services.AddKeyedTransient<IRotor, RotorTirpitzIV>("T-IV");
        services.AddKeyedTransient<IRotor, RotorTirpitzV>("T-V");
        services.AddKeyedTransient<IRotor, RotorTirpitzVI>("T-VI");
        services.AddKeyedTransient<IRotor, RotorTirpitzVII>("T-VII");
        services.AddKeyedTransient<IRotor, RotorTirpitzVIII>("T-VIII");

        // The Norwegian machines, and the K fitted with a rewirable UKW-D.
        services.AddKeyedTransient<IRotor, RotorNorwayNI>("N-I");
        services.AddKeyedTransient<IRotor, RotorNorwayNII>("N-II");
        services.AddKeyedTransient<IRotor, RotorNorwayNIII>("N-III");
        services.AddKeyedTransient<IRotor, RotorNorwayNIV>("N-IV");
        services.AddKeyedTransient<IRotor, RotorNorwayNV>("N-V");
        services.AddKeyedTransient<IRotor, RotorKDI>("KD-I");
        services.AddKeyedTransient<IRotor, RotorKDII>("KD-II");
        services.AddKeyedTransient<IRotor, RotorKDIII>("KD-III");

        // The ten contact wheels of the numbers-only Enigma Z30.
        services.AddKeyedTransient<IRotor, RotorZI>("Z-I");
        services.AddKeyedTransient<IRotor, RotorZII>("Z-II");
        services.AddKeyedTransient<IRotor, RotorZIII>("Z-III");

        // Reflectors hold no state, so one of each is enough.
        services.AddKeyedSingleton<IReflector, ReflectorA>("A");
        services.AddKeyedSingleton<IReflector, ReflectorB>("B");
        services.AddKeyedSingleton<IReflector, ReflectorC>("C");
        services.AddKeyedSingleton<IReflector, ReflectorBThin>("B-THIN");
        services.AddKeyedSingleton<IReflector, ReflectorCThin>("C-THIN");
        services.AddKeyedSingleton<IReflector, ReflectorNorway>("N");

        // The Zählwerk reflectors turn, so each machine needs its own.
        services.AddKeyedTransient<IReflector, ReflectorG>("G");
        services.AddKeyedTransient<IReflector, ReflectorG312>("G312");
        services.AddKeyedTransient<IReflector, ReflectorRailway>("R");
        services.AddKeyedTransient<IReflector, ReflectorTirpitz>("T");
        services.AddKeyedTransient<IReflector, ReflectorZ>("Z");

        // The board is patched per machine, so it is transient for the same reason as the rotors.
        services.AddTransient<IPlugBoard, PlugBoard>();

        // Entry wheels hold no state, and the service machines all used the
        // straight-through one.
        services.AddKeyedSingleton<IEntryWheel>("STANDARD", (_, _) => EntryWheel.Standard);
        services.AddKeyedSingleton<IEntryWheel>("QWERTZ", (_, _) => EntryWheel.Qwertz);
        services.AddKeyedSingleton<IEntryWheel>("TIRPITZ", (_, _) => EntryWheel.Tirpitz);

        services.AddSingleton<IStepping, PawlDrive>();
        services.AddKeyedSingleton<IMachineLayout>("SERVICE",
            (provider, _) => new ServiceLayout(provider.GetRequiredService<IStepping>()));
        services.AddKeyedSingleton<IMachineLayout>("G-31",
            (provider, _) => new GearLayout(
                new GearDrive(provider.GetService<ILogger<GearDrive>>())));

        services.AddKeyedSingleton<IMachineLayout>("COMMERCIAL",
            (provider, _) => new CommercialLayout(provider.GetRequiredService<IStepping>()));
        services.AddKeyedSingleton<IMachineLayout>("TIRPITZ",
            (provider, _) => new CommercialLayout(
                provider.GetRequiredService<IStepping>(), "Tirpitz", "TIRPITZ"));
        services.AddKeyedSingleton<IMachineLayout>("KD",
            (provider, _) => new CommercialLayout(
                provider.GetRequiredService<IStepping>(), "KD", "QWERTZ", settableReflector: false));
        services.AddKeyedSingleton<IMachineLayout>("Z30",
            (provider, _) => new NumericLayout(
                new ReflectorPawlDrive(provider.GetService<ILogger<ReflectorPawlDrive>>())));

        services.AddSingleton<IPartsCatalogue, BuiltInPartsCatalogue>();
        services.AddSingleton<IEnigmaMachineFactory, EnigmaMachineFactory>();
        services.AddSingleton<IIndicatorProcedure, IndicatorProcedure>();

        return services;
    }
}
