using Enigma.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigma.Cmd;

public class EnigmaConsole : BackgroundService
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly IIndicatorProcedure _procedure;
    private readonly ICharacterMap _characterMap;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<EnigmaConsole> _logger;
    private readonly ConsoleOptions _options;
    private readonly KeySheet _keySheet;

    public EnigmaConsole(
        IEnigmaMachineFactory factory,
        IIndicatorProcedure procedure,
        ICharacterMap characterMap,
        IHostApplicationLifetime lifetime,
        ILogger<EnigmaConsole> logger,
        ConsoleOptions options,
        IOptions<KeySheet> keySheet)
    {
        _factory = factory;
        _procedure = procedure;
        _characterMap = characterMap;
        _lifetime = lifetime;
        _logger = logger;
        _options = options;
        _keySheet = keySheet.Value;
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
            when (exception is ArgumentException or FormatException or InvalidOperationException)
        {
            // A key sheet the machine cannot be built from is a mistake in the
            // settings, not a fault in the program, so say what is wrong and stop.
            // The detail is still there under --verbose.
            await Console.Error.WriteLineAsync(exception.Message);
            _logger.LogDebug(exception, "Key sheet rejected");
            Environment.ExitCode = 1;
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
        var machine = _factory.Create(KeyTheMachine());

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

    /// <summary>
    /// Works out where the rotors start. With no indicator procedure that is simply
    /// what the key sheet says. With one, the key sheet's positions are the ground
    /// setting, and the rotors start at the message key instead.
    /// </summary>
    private KeySheet KeyTheMachine()
    {
        if (_options.MessageKey is { } messageKey)
        {
            var indicator = _procedure.EncipherMessageKey(
                _keySheet, _keySheet.Positions, messageKey, _options.Doubled);

            // This travels with the message, so it goes to the operator rather than
            // into the ciphertext on standard output.
            Console.Error.WriteLine($"Ground setting {_keySheet.Positions}, indicator {indicator}");

            return _keySheet.WithPositions(messageKey);
        }

        if (_options.Indicator is { } sent)
        {
            var recovered = _procedure.RecoverMessageKey(_keySheet, _keySheet.Positions, sent);

            _logger.LogDebug("Recovered message key {MessageKey}", recovered);

            return _keySheet.WithPositions(recovered);
        }

        return _keySheet;
    }

    private string Translate(IEnigmaMachine machine, string line)
    {
        var alphabet = machine.CharacterMap;

        // Preparing first means the substitutions are enciphered, which is what
        // happened: the signaller fitted the text to the keyboard, then typed it.
        var text = _options.Prepare ? MessageText.Prepare(line) : line;

        var input = text
            .Select(character => IndexOf(alphabet, character))
            .Where(index => index >= 0);

        var output = string.Concat(machine.Translate(input).Select(alphabet.GetCharacter));

        return _options.Groups is { } size ? MessageText.InGroups(output, size) : output;
    }

    // Typing in lower case is a convenience the machine never had. It is applied
    // only as a fallback, so an alphabet that distinguishes case keeps both.
    private static int IndexOf(ICharacterMap alphabet, char character)
    {
        var index = alphabet.GetIndex(character);

        return index >= 0 ? index : alphabet.GetIndex(char.ToUpperInvariant(character));
    }

    private void WriteBanner(IEnigmaMachine machine)
    {
        var rotors = machine.Rotors.Select(rotor => rotor.Name);

        Console.WriteLine(_keySheet.Name);
        Console.WriteLine($"Rotors {string.Join(' ', rotors)}, reflector {machine.Reflector.Name}");
        Console.WriteLine("Enter a message, or Ctrl+D to quit.");
        Console.WriteLine();
    }
}
