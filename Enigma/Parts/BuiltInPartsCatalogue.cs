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

    public IRotor CreateRotor(string name) =>
        _services.GetKeyedService<IRotor>(Normalise(name))
        ?? throw PartsCatalogue.UnknownRotor(name, RotorNames);

    public IReflector GetReflector(string name) =>
        _services.GetKeyedService<IReflector>(Normalise(name))
        ?? throw PartsCatalogue.UnknownReflector(name, ReflectorNames);

    public IEntryWheel GetEntryWheel(string name) =>
        _services.GetKeyedService<IEntryWheel>(Normalise(name))
        ?? throw PartsCatalogue.UnknownEntryWheel(name, EntryWheelNames);

    private static string Normalise(string name) => (name ?? string.Empty).Trim().ToUpperInvariant();
}
