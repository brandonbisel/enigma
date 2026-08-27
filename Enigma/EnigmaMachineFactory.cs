using Enigma.Models;
using Enigma.Parts;
using Enigma.Reflectors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enigma;

public class EnigmaMachineFactory : IEnigmaMachineFactory
{
    private readonly IServiceProvider _services;
    private readonly IPartsCatalogue _parts;
    private readonly ILogger<EnigmaMachineFactory>? _logger;

    private readonly Func<ICharacterMap, IPlugBoard> _plugBoards;

    public EnigmaMachineFactory(
        IServiceProvider services,
        IPartsCatalogue parts,
        Func<ICharacterMap, IPlugBoard> plugBoards)
    {
        _services = services;
        _parts = parts;
        _plugBoards = plugBoards;
        _logger = services.GetService<ILogger<EnigmaMachineFactory>>();
    }

    public IEnigmaMachine Create(KeySheet keySheet)
    {
        // The alphabet comes first: it decides how many contacts everything has and
        // what the letters on the key sheet mean.
        var alphabet = _parts.GetCharacterMap(keySheet.CharacterMapName());

        var wheels = keySheet.Wheels(alphabet);
        var cables = keySheet.Cables(alphabet);

        var rotors = wheels.Select(wheel => CreateRotor(wheel, alphabet)).ToList();
        // A reflector given as wire pairs is rewired in the field and so is part of
        // the key, not a part to be looked up.
        var reflector = string.IsNullOrWhiteSpace(keySheet.ReflectorPairs)
            ? _parts.GetReflector(keySheet.ReflectorName(), alphabet)
            : new RewirableReflector(keySheet.ReflectorName(), keySheet.ReflectorPairs, alphabet);
        var entryWheel = _parts.GetEntryWheel(keySheet.EntryWheelName(), alphabet);
        var plugBoard = _plugBoards(alphabet);

        foreach (var (input, output) in cables)
        {
            plugBoard.Connect(input, output);
        }

        Validate(rotors, reflector, entryWheel, alphabet);

        _logger?.LogDebug(
            "Key sheet {Name}: rotors {Rotors}, reflector {Reflector}, Ringstellung {Rings}, Grundstellung {Positions}, {Cables} cables",
            keySheet.Name,
            keySheet.Rotors,
            keySheet.ReflectorName(),
            keySheet.RingSettings,
            keySheet.Positions,
            cables.Count);

        return new EnigmaMachine(
            plugBoard,
            rotors,
            reflector,
            _services.GetService<ILogger<EnigmaMachine>>(),
            alphabet,
            entryWheel);
    }

    /// <summary>
    /// Rejects machines that could not be assembled. The thin rotors are half width
    /// and only fit in the space a thin reflector frees, so the fourth wheel and the
    /// thin reflector always come as a pair, and the thin wheel is always leftmost.
    /// </summary>
    private static void Validate(
        IReadOnlyList<IRotor> rotors,
        IReflector reflector,
        IEntryWheel entryWheel,
        ICharacterMap alphabet)
    {
        // One authority for size. Without this a wheel built for a different
        // alphabet would fail somewhere downstream with an index error instead.
        foreach (var part in rotors.Select(rotor => (rotor.Name, rotor.Contacts))
                     .Append((reflector.Name, reflector.Contacts))
                     .Append((entryWheel.Name, entryWheel.Contacts)))
        {
            if (part.Item2 != alphabet.Count)
            {
                throw new ArgumentException(
                    $"'{part.Item1}' has {part.Item2} contacts, but the {alphabet.Name} alphabet " +
                    $"has {alphabet.Count} characters.");
            }
        }

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

    private IRotor CreateRotor(RotorPlacement placement, ICharacterMap alphabet)
    {
        var rotor = _parts.CreateRotor(placement.Name, alphabet);

        rotor.SetRingSetting(placement.RingSetting);
        rotor.SetPosition(placement.Position);

        return rotor;
    }
}
