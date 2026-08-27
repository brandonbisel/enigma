using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Parts;

/// <summary>The wheels and reflectors registered by <c>AddEnigmaServices</c>.</summary>
public class BuiltInPartsCatalogue : IPartsCatalogue
{
    private readonly IServiceProvider _services;

    public BuiltInPartsCatalogue(IServiceProvider services)
    {
        _services = services;
    }

    public IReadOnlyList<string> RotorNames => MachineParts.RotorNames;
    public IReadOnlyList<string> ReflectorNames => MachineParts.ReflectorNames;
    public IReadOnlyList<string> EntryWheelNames => MachineParts.EntryWheelNames;
    public IReadOnlyList<string> CharacterMapNames => MachineParts.CharacterMapNames;
    public IReadOnlyList<string> LayoutNames => MachineParts.LayoutNames;

    public IRotor CreateRotor(string name, ICharacterMap? characterMap = null) =>
        _services.GetKeyedService<IRotor>(Normalise(name))
        ?? throw PartsCatalogue.UnknownRotor(name, RotorNames);

    public IReflector GetReflector(string name, ICharacterMap? characterMap = null) =>
        _services.GetKeyedService<IReflector>(Normalise(name))
        ?? throw PartsCatalogue.UnknownReflector(name, ReflectorNames);

    public IEntryWheel GetEntryWheel(string name, ICharacterMap? characterMap = null)
    {
        // The straight-through wheel is the identity, so it is built for whichever
        // alphabet is asked for rather than being a fixed twenty six contact part.
        if (Normalise(name) == "STANDARD")
        {
            return EntryWheel.StraightThrough(characterMap ?? CharacterMap.Latin);
        }

        return _services.GetKeyedService<IEntryWheel>(Normalise(name))
               ?? throw PartsCatalogue.UnknownEntryWheel(name, EntryWheelNames);
    }

    public ICharacterMap GetCharacterMap(string name) =>
        _services.GetKeyedService<ICharacterMap>(Normalise(name))
        ?? throw PartsCatalogue.UnknownCharacterMap(name, CharacterMapNames);

    public IMachineLayout GetLayout(string name) =>
        _services.GetKeyedService<IMachineLayout>(Normalise(name))
        ?? throw PartsCatalogue.UnknownLayout(name, LayoutNames);

    private static string Normalise(string name) => (name ?? string.Empty).Trim().ToUpperInvariant();
}
