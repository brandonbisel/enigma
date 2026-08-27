using Enigma.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigma.Cmd;

public class EnigmaConsole : BackgroundService
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly ICharacterMap _characterMap;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<EnigmaConsole> _logger;
    private readonly ConsoleOptions _options;
    private readonly EnigmaSettings _settings;

    public EnigmaConsole(
        IEnigmaMachineFactory factory,
        ICharacterMap characterMap,
        IHostApplicationLifetime lifetime,
        ILogger<EnigmaConsole> logger,
        ConsoleOptions options,
        IOptions<EnigmaSettings> settings)
    {
        _factory = factory;
        _characterMap = characterMap;
        _lifetime = lifetime;
        _logger = logger;
        _options = options;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await RunAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Ctrl+C while waiting for input.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Enigma failed");
            await Console.Error.WriteLineAsync(exception.Message);
            Environment.ExitCode = 1;
        }

        _lifetime.StopApplication();
    }

    private async Task RunAsync(CancellationToken stoppingToken)
    {
        var machine = _factory.Create(_settings);

        // Only the streams we opened get disposed: closing Console.In or Console.Out
        // would take standard input and output down with them.
        var fileReader = _options.Input is null ? null : new StreamReader(_options.Input.FullName);
        var fileWriter = _options.Output is null ? null : new StreamWriter(_options.Output.FullName);

        try
        {
            var reader = fileReader ?? Console.In;
            var writer = fileWriter ?? Console.Out;

            if (_options.ShowBanner)
            {
                WriteBanner(machine);
            }

            // The machine keeps stepping across lines, exactly as the real one does:
            // running it again is what returns the rotors to their configured start.
            while (await reader.ReadLineAsync(stoppingToken) is { } line)
            {
                await writer.WriteLineAsync(Translate(machine, line));
            }

            await writer.FlushAsync(stoppingToken);
        }
        finally
        {
            fileReader?.Dispose();

            if (fileWriter is not null)
            {
                await fileWriter.DisposeAsync();
            }
        }
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
