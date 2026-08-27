using Enigma.Models;
using Enigma.Machines;
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
        var layout = _parts.GetLayout(keySheet.ModelName());

        var wheels = keySheet.Wheels(alphabet);
        var cables = keySheet.Cables(alphabet);

        var rotors = wheels.Select(wheel => CreateRotor(wheel, alphabet)).ToList();
        // A reflector given as wire pairs is rewired in the field and so is part of
        // the key, not a part to be looked up.
        var reflector = string.IsNullOrWhiteSpace(keySheet.ReflectorPairs)
            ? _parts.GetReflector(keySheet.ReflectorName(), alphabet)
            : new RewirableReflector(keySheet.ReflectorName(), keySheet.ReflectorPairs, alphabet);
        if (reflector is IRotatingReflector turning &&
            keySheet.ReflectorSetting(alphabet) is { } setting)
        {
            turning.SetRingSetting(setting.RingSetting);
            turning.SetPosition(setting.Position);
        }

        var entryWheel = _parts.GetEntryWheel(
            keySheet.EntryWheelName(layout.DefaultEntryWheel), alphabet);
        var plugBoard = BuildPlugBoard(keySheet, alphabet, cables);

        ValidateContacts(rotors, reflector, entryWheel, alphabet);
        layout.Validate(rotors, reflector, entryWheel);

        if (!layout.AllowsPlugBoard && cables.Count > 0)
        {
            throw new ArgumentException(
                $"A {layout.Name} machine has no plugboard, but {cables.Count} cables were given.");
        }

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
            entryWheel,
            layout.Drive);
    }

    /// <summary>
    /// One authority for size. Without this a wheel built for a different alphabet
    /// would fail somewhere downstream with an index error instead.
    /// </summary>
    private static void ValidateContacts(
        IReadOnlyList<IRotor> rotors,
        IReflector reflector,
        IEntryWheel entryWheel,
        ICharacterMap alphabet)
    {
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
    }

    // With an Uhr fitted the cables run into the box rather than into each other,
    // and the order they were listed in is part of the setting.
    private IPlugBoard BuildPlugBoard(
        KeySheet keySheet,
        ICharacterMap alphabet,
        IReadOnlyDictionary<int, int> cables)
    {
        if (keySheet.UhrPosition() is not { } position)
        {
            var board = _plugBoards(alphabet);

            foreach (var (input, output) in cables)
            {
                board.Connect(input, output);
            }

            return board;
        }

        return new EnigmaUhr(alphabet, keySheet.CableOrder(alphabet), position);
    }

    private IRotor CreateRotor(RotorPlacement placement, ICharacterMap alphabet)
    {
        var rotor = _parts.CreateRotor(placement.Name, alphabet);

        rotor.SetRingSetting(placement.RingSetting);
        rotor.SetPosition(placement.Position);

        return rotor;
    }
}
