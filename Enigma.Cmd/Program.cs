using Enigma;
using System.CommandLine;
using System.Text.Json;
using Enigma.App;
using Enigma.Cmd;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Enigma.Parts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

// The packaged key sheets, behind the catalogue both front ends look them up in.
var keySheets = new KeySheetCatalogue();

var keySheetOption = new Option<FileInfo?>("--key-sheet", "-k", "--settings")
{
    Description = "Key sheet file to use. Defaults to the KeySheet section of appsettings.json."
};

var partsOption = new Option<FileInfo?>("--parts")
{
    Description = "File defining extra rotors and reflectors, or replacing built-in ones."
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

var messageKeyOption = new Option<string?>("--message-key")
{
    Description = "Encipher with this message key, using the key sheet positions as the ground setting."
};

var indicatorOption = new Option<string?>("--indicator")
{
    Description = "Recover the message key from this indicator, then decipher with it."
};

var setOption = new Option<string?>("--set")
{
    Description = "Naval: which set of tables to use. See --list-sets. Defaults to Quelle."
};

var listSetsOption = new Option<bool>("--list-sets")
{
    Description = "List the bigram table sets that ship and exit."
};

var tafelOption = new Option<string?>("--tafel")
{
    Description = "Naval: the Doppelbuchstabentauschtafel to use, named by its letter. See --list-sets."
};

var kennzifferOption = new Option<int?>("--kennziffer")
{
    Description = "Naval: take the table from the Tauschtafelplan instead, using this column. " +
                  "See --list-sets for how many a set has. Needs --monatstag."
};

var monatstagOption = new Option<int?>("--monatstag")
{
    Description = "Naval: the day of the month to read the Tauschtafelplan at (1-31)."
};

var kenngruppenOption = new Option<string?>("--kenngruppen")
{
    Description = "Naval: send with these two trigrams, Schlüsselkenngruppe then Verfahrenkenngruppe. Six letters."
};

var fillersOption = new Option<string?>("--fillers")
{
    Description = "Naval: the two padding letters, first and last. Defaults to XX."
};

var doubledOption = new Option<bool>("--doubled")
{
    Description = "Send the message key twice, as the procedure required until 1938."
};

var prepareOption = new Option<bool>("--prepare")
{
    Description = "Fit the text to the keyboard first: umlauts expanded, digits spelled out, spaces as X."
};

var groupsOption = new Option<int?>("--groups")
{
    Description = "Write the result in groups of this many letters, as a signaller would. Try 5."
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
    partsOption,
    presetOption,
    listPresetsOption,
    inputOption,
    outputOption,
    messageKeyOption,
    indicatorOption,
    doubledOption,
    setOption,
    listSetsOption,
    tafelOption,
    kennzifferOption,
    monatstagOption,
    kenngruppenOption,
    fillersOption,
    prepareOption,
    groupsOption,
    verboseOption,
    logFileOption,
    initKeySheetOption
};

// Individual settings can still be overridden with configuration keys such as
// --KeySheet:Rotors="II IV V", so unmatched tokens are passed through rather than rejected.
root.TreatUnmatchedTokensAsErrors = false;

root.SetAction((parseResult, cancellationToken) => RunAsync(
    parseResult.GetValue(keySheetOption),
    parseResult.GetValue(partsOption),
    parseResult.GetValue(presetOption),
    parseResult.GetValue(listPresetsOption),
    parseResult.GetValue(inputOption),
    parseResult.GetValue(outputOption),
    parseResult.GetValue(messageKeyOption),
    parseResult.GetValue(indicatorOption),
    parseResult.GetValue(doubledOption),
    parseResult.GetValue(setOption),
    parseResult.GetValue(listSetsOption),
    parseResult.GetValue(tafelOption),
    parseResult.GetValue(kennzifferOption),
    parseResult.GetValue(monatstagOption),
    parseResult.GetValue(kenngruppenOption),
    parseResult.GetValue(fillersOption),
    parseResult.GetValue(prepareOption),
    parseResult.GetValue(groupsOption),
    parseResult.GetValue(verboseOption),
    parseResult.GetValue(logFileOption),
    parseResult.GetValue(initKeySheetOption),
    args,
    cancellationToken));

return await root.Parse(args).InvokeAsync();

async Task<int> RunAsync(
    FileInfo? keySheetFile,
    FileInfo? partsFile,
    string? preset,
    bool listPresets,
    FileInfo? input,
    FileInfo? output,
    string? messageKey,
    string? indicator,
    bool doubled,
    string? setName,
    bool listSets,
    string? tafel,
    int? kennziffer,
    int? monatstag,
    string? kenngruppen,
    string? fillers,
    bool prepare,
    int? groups,
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

    if (listSets)
    {
        return ListSets();
    }

    if (initKeySheet is not null)
    {
        return await WriteDefaultKeySheetAsync(initKeySheet, cancellationToken);
    }

    if (preset is not null && !keySheets.TryGet(preset, out _))
    {
        await Console.Error.WriteLineAsync(
            $"Unknown key sheet '{preset}'. Known: {string.Join(", ", keySheets.Names)}");

        return 1;
    }

    if (keySheetFile is not null && !keySheetFile.Exists)
    {
        await Console.Error.WriteLineAsync($"Key sheet file not found: {keySheetFile.FullName}");
        return 1;
    }

    if (partsFile is not null && !partsFile.Exists)
    {
        await Console.Error.WriteLineAsync($"Parts file not found: {partsFile.FullName}");
        return 1;
    }

    if (groups is { } size && size < 1)
    {
        await Console.Error.WriteLineAsync("--groups must be at least 1.");
        return 1;
    }

    if (messageKey is not null && indicator is not null)
    {
        await Console.Error.WriteLineAsync(
            "Give either --message-key to encipher or --indicator to decipher, not both.");

        return 1;
    }

    if (input is not null && !input.Exists)
    {
        await Console.Error.WriteLineAsync($"Input file not found: {input.FullName}");
        return 1;
    }

    // The naval procedure. Read and checked in one place so it can be tested
    // without a host, a console or a machine.
    var naval = NavalArguments.Read(
        setName, tafel, kennziffer, monatstag, kenngruppen, fillers, messageKey, indicator, doubled);

    if (naval.Error is { } refusal)
    {
        await Console.Error.WriteLineAsync(refusal);
        return 1;
    }

    if (naval.Note is { } note)
    {
        await Console.Error.WriteLineAsync(note);
    }

    var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
    {
        Args = arguments,
        ContentRootPath = AppContext.BaseDirectory
    });

    ConfigureLogging(builder, verbose, logFile);

    builder.Services.AddEnigmaServices();

    if (partsFile is not null)
    {
        var definitions = new ConfigurationBuilder()
            .AddJsonFile(partsFile.FullName, optional: false)
            .Build()
            .Get<PartsFile>() ?? new PartsFile();

        // Built here rather than on first use: a fault in the file is the reader's
        // mistake and deserves one line, not a failure to start the host.
        try
        {
            var catalogue = new PartsCatalogue(
                new BuiltInPartsCatalogue(builder.Services.BuildServiceProvider()),
                definitions,
                LoggerFactory.Create(logging => logging.AddSerilog()).CreateLogger<PartsCatalogue>());

            // Layered over the built-in catalogue rather than replacing it, so a file
            // need only define the parts the machine actually needs.
            builder.Services.AddSingleton<IPartsCatalogue>(catalogue);
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            await Console.Error.WriteLineAsync(exception.Message);
            return 1;
        }
    }
    builder.Services.AddSingleton(new ConsoleOptions(
        input, output, messageKey, indicator, doubled, prepare, groups,
        naval.Table, naval.KeyGroup, naval.MessageGroup, naval.FirstFiller, naval.LastFiller));
    builder.Services.AddHostedService<EnigmaConsole>();
    if (preset is not null && keySheets.TryGet(preset, out var packaged))
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

int ListSets()
{
    foreach (var set in BigramTableSet.All.Values)
    {
        var letters = string.Concat(set.Tables.Keys.Order());

        Console.WriteLine($"{set.Name,-10} {set.Serial}");
        Console.WriteLine(
            $"{string.Empty,-10} Tafeln {letters}, {set.Plan.Columns} Kennziffer columns" +
            (set.IsComplete ? string.Empty : ", incomplete: some days name a table that is not published"));
    }

    return 0;
}

int ListPresets()
{
    foreach (var (key, sheet) in keySheets.All)
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
