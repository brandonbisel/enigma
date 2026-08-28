using Enigma.App;
using Enigma.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigma.Cmd;

public class EnigmaConsole : BackgroundService
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly IIndicatorProcedure _procedure;
    private readonly INavalIndicatorProcedure _naval;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<EnigmaConsole> _logger;
    private readonly ConsoleOptions _options;
    private readonly KeySheet _keySheet;

    public EnigmaConsole(
        IEnigmaMachineFactory factory,
        IIndicatorProcedure procedure,
        INavalIndicatorProcedure naval,
        IHostApplicationLifetime lifetime,
        ILogger<EnigmaConsole> logger,
        ConsoleOptions options,
        IOptions<KeySheet> keySheet)
    {
        _factory = factory;
        _procedure = procedure;
        _naval = naval;
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
        var keyed = EnigmaSession.Open(_factory, KeyTheMachine());

        if (!keyed.Succeeded)
        {
            // A key sheet the machine cannot be built from is a mistake in the
            // settings, not a fault in the program, so say what is wrong and stop.
            await Console.Error.WriteLineAsync(keyed.Error);
            _logger.LogDebug(keyed.Fault, "Key sheet rejected");
            Environment.ExitCode = 1;

            return;
        }

        var session = keyed.Session!;

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
                WriteBanner(session.Machine);
            }

            // The machine keeps stepping across lines, exactly as the real one does:
            // running it again is what returns the rotors to their configured start.
            while (await reader.ReadLineAsync(stoppingToken) is { } line)
            {
                await writer.WriteLineAsync(Translate(session, line));
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
        // A table means the naval procedure, which hides its indicator rather than
        // sending it in clear, so there is more to report than a ground setting.
        if (_options.Table is { } table)
        {
            var worked = _options.KeyGroup is { } keyGroup
                ? MessageKeying.SendNaval(
                    _naval,
                    _keySheet,
                    table,
                    keyGroup,
                    _options.MessageGroup ?? string.Empty,
                    _options.FirstFiller,
                    _options.LastFiller)
                : MessageKeying.ReceiveNaval(_naval, _keySheet, table, _options.Indicator ?? string.Empty);

            Console.Error.WriteLine(worked.Succeeded
                ? $"Ground setting {worked.GroundSetting}, " +
                  $"Schlüsselkenngruppe {worked.KeyGroup}, " +
                  $"Verfahrenkenngruppe {worked.MessageGroup}, " +
                  $"indicator {worked.Indicator}, rotors {worked.MessageKey}"
                : worked.Error);

            return Keyed(worked);
        }

        if (_options.MessageKey is { } messageKey)
        {
            var keyed = MessageKeying.Send(_procedure, _keySheet, messageKey, _options.Doubled);

            // This travels with the message, so it goes to the operator rather than
            // into the ciphertext on standard output.
            Console.Error.WriteLine(keyed.Succeeded
                ? $"Ground setting {keyed.GroundSetting}, indicator {keyed.Indicator}"
                : keyed.Error);

            return Keyed(keyed);
        }

        if (_options.Indicator is { } sent)
        {
            var keyed = MessageKeying.Receive(_procedure, _keySheet, sent);

            _logger.LogDebug("Recovered message key {MessageKey}", keyed.MessageKey);

            return Keyed(keyed);
        }

        return _keySheet;
    }

    // A bad indicator is a mistake in the settings like any other, so it is
    // reported the same way rather than starting a machine at the wrong place.
    private static KeySheet Keyed(IndicatorResult keyed) =>
        keyed.Sheet ?? throw new ArgumentException(keyed.Error);

    private string Translate(EnigmaSession session, string line)
    {
        // Preparing first means the substitutions are enciphered, which is what
        // happened: the signaller fitted the text to the keyboard, then typed it.
        var text = _options.Prepare ? MessageText.Prepare(line) : line;

        var output = session.Type(text);

        return _options.Groups is { } size ? MessageText.InGroups(output, size) : output;
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
