using System.CommandLine;
using System.Text.Json;
using Enigma.Cmd;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

var keySheetOption = new Option<FileInfo?>("--key-sheet", "-k", "--settings")
{
    Description = "Key sheet file to use. Defaults to the KeySheet section of appsettings.json."
};

var presetOption = new Option<string?>("--preset", "-p")
{
    Description = "Use a packaged key sheet by name. See --list-presets."
};

var listPresetsOption = new Option<bool>("--list-presets")
{
    Description = "List the packaged key sheets and exit."
};

var inputOption = new Option<FileInfo?>("--input", "-i")
{
    Description = "Read the message from this file instead of standard input."
};

var outputOption = new Option<FileInfo?>("--output", "-o")
{
    Description = "Write the result to this file instead of standard output."
};

var verboseOption = new Option<bool>("--verbose", "-v")
{
    Description = "Trace every character through the plugboard, rotors and reflector."
};

var initKeySheetOption = new Option<FileInfo?>("--init-key-sheet", "--init-settings")
{
    Description = "Write a key sheet holding the default machine to this path, then exit."
};

var logFileOption = new Option<FileInfo?>("--log-file")
{
    Description = "Also write the diagnostic log to this file."
};

var root = new RootCommand(
    "Enigma machine simulator. Reads a message, enciphers it, and writes the result. " +
    "Because the machine is reciprocal, the same settings both encipher and decipher.")
{
    keySheetOption,
    presetOption,
    listPresetsOption,
    inputOption,
    outputOption,
    verboseOption,
    logFileOption,
    initKeySheetOption
};

// Individual settings can still be overridden with configuration keys such as
// --KeySheet:Rotors="II IV V", so unmatched tokens are passed through rather than rejected.
root.TreatUnmatchedTokensAsErrors = false;

root.SetAction((parseResult, cancellationToken) => RunAsync(
    parseResult.GetValue(keySheetOption),
    parseResult.GetValue(presetOption),
    parseResult.GetValue(listPresetsOption),
    parseResult.GetValue(inputOption),
    parseResult.GetValue(outputOption),
    parseResult.GetValue(verboseOption),
    parseResult.GetValue(logFileOption),
    parseResult.GetValue(initKeySheetOption),
    args,
    cancellationToken));

return await root.Parse(args).InvokeAsync();

async Task<int> RunAsync(
    FileInfo? keySheetFile,
    string? preset,
    bool listPresets,
    FileInfo? input,
    FileInfo? output,
    bool verbose,
    FileInfo? logFile,
    FileInfo? initKeySheet,
    string[] arguments,
    CancellationToken cancellationToken)
{
    if (listPresets)
    {
        return ListPresets();
    }

    if (initKeySheet is not null)
    {
        return await WriteDefaultKeySheetAsync(initKeySheet, cancellationToken);
    }

    if (preset is not null && !KeySheets.TryGet(preset, out _))
    {
        await Console.Error.WriteLineAsync(
            $"Unknown key sheet '{preset}'. Known: {string.Join(", ", KeySheets.All.Keys)}");

        return 1;
    }

    if (keySheetFile is not null && !keySheetFile.Exists)
    {
        await Console.Error.WriteLineAsync($"Key sheet file not found: {keySheetFile.FullName}");
        return 1;
    }

    if (input is not null && !input.Exists)
    {
        await Console.Error.WriteLineAsync($"Input file not found: {input.FullName}");
        return 1;
    }

    var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
    {
        Args = arguments,
        ContentRootPath = AppContext.BaseDirectory
    });

    ConfigureLogging(builder, verbose, logFile);

    builder.Services.AddEnigmaServices();
    builder.Services.AddSingleton(new ConsoleOptions(input, output));
    builder.Services.AddHostedService<EnigmaConsole>();
    if (preset is not null && KeySheets.TryGet(preset, out var packaged))
    {
        builder.Services.AddSingleton(Options.Create(packaged));
    }
    else
    {
        builder.Services.Configure<KeySheet>(KeySheetConfiguration(builder, keySheetFile));
    }

    await builder.Build().RunAsync(cancellationToken);

    return Environment.ExitCode;
}

// A key sheet file replaces the configured machine outright. It may hold the key
// sheet at the root or wrapped in a "KeySheet" section, so a copy of
// appsettings.json works as a starting point either way.
IConfiguration KeySheetConfiguration(HostApplicationBuilder builder, FileInfo? keySheetFile)
{
    if (keySheetFile is null)
    {
        return builder.Configuration.GetSection("KeySheet");
    }

    var configuration = new ConfigurationBuilder()
        .AddJsonFile(keySheetFile.FullName, optional: false)
        .Build();

    var section = configuration.GetSection("KeySheet");

    return section.Exists() ? section : configuration;
}

int ListPresets()
{
    foreach (var (key, sheet) in KeySheets.All)
    {
        Console.WriteLine($"{key,-20} {sheet.Name}");
        Console.WriteLine(
            $"{string.Empty,-20} rotors {sheet.Rotors}, reflector {sheet.Reflector}, " +
            $"Ringstellung {sheet.RingSettings}, Grundstellung {sheet.Positions}");
    }

    return 0;
}

// The defaults are read back out of appsettings.json rather than duplicated here,
// so the generated file cannot drift from what the app actually starts with.
async Task<int> WriteDefaultKeySheetAsync(FileInfo destination, CancellationToken cancellationToken)
{
    if (destination.Exists)
    {
        await Console.Error.WriteLineAsync(
            $"Refusing to overwrite an existing file: {destination.FullName}");

        return 1;
    }

    var defaults = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build()
        .GetSection("KeySheet")
        .Get<KeySheet>() ?? new KeySheet();

    var json = JsonSerializer.Serialize(defaults, new JsonSerializerOptions { WriteIndented = true });

    await File.WriteAllTextAsync(destination.FullName, json + Environment.NewLine, cancellationToken);
    await Console.Error.WriteLineAsync($"Wrote default key sheet to {destination.FullName}");

    return 0;
}

void ConfigureLogging(HostApplicationBuilder builder, bool verbose, FileInfo? logFile)
{
    var configuration = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .MinimumLevel.Is(verbose ? LogEventLevel.Debug : LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        // Everything goes to standard error so that enciphered text on standard
        // output stays clean enough to pipe or redirect.
        .WriteTo.Console(
            standardErrorFromLevel: LogEventLevel.Verbose,
            outputTemplate: "{Message:lj}{NewLine}{Exception}");

    if (logFile is not null)
    {
        configuration.WriteTo.File(
            logFile.FullName,
            outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    }

    builder.Logging.ClearProviders();
    builder.Services.AddSerilog(configuration.CreateLogger(), dispose: true);
}
