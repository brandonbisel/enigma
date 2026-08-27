using Enigma.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Enigma.Cmd;

public class EnigmaConsole : BackgroundService
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly ICharacterMap _characterMap;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly EnigmaSettings _settings;

    public EnigmaConsole(
        IEnigmaMachineFactory factory,
        ICharacterMap characterMap,
        IHostApplicationLifetime lifetime,
        IOptions<EnigmaSettings> settings)
    {
        _factory = factory;
        _characterMap = characterMap;
        _lifetime = lifetime;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var machine = _factory.Create(_settings);

        WriteBanner(machine);

        try
        {
            // The machine keeps stepping across lines, exactly as the real one does:
            // restarting the app is what returns the rotors to their configured start.
            while (await Console.In.ReadLineAsync(stoppingToken) is { } line)
            {
                Console.WriteLine(Translate(machine, line));
            }
        }
        catch (OperationCanceledException)
        {
            // Ctrl+C while waiting for input.
        }

        _lifetime.StopApplication();
    }

    private string Translate(IEnigmaMachine machine, string line)
    {
        var input = line
            .ToUpperInvariant()
            .Select(_characterMap.GetIndex)
            .Where(index => index >= 0);

        return string.Concat(machine.Translate(input).Select(_characterMap.GetCharacter));
    }

    private void WriteBanner(IEnigmaMachine machine)
    {
        var rotors = machine.Rotors.Select(rotor => rotor.Name);

        Console.WriteLine(_settings.Name);
        Console.WriteLine($"Rotors {string.Join(' ', rotors)}, reflector {machine.Reflector.Name}");
        Console.WriteLine("Enter a message, or Ctrl+D to quit.");
        Console.WriteLine();
    }
}
