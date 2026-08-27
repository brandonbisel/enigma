using System.CommandLine;
using Enigma.Cmd;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

var settingsOption = new Option<FileInfo?>("--settings", "-s")
{
    Description = "JSON file describing the machine. Defaults to the Enigma section of appsettings.json."
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

var logFileOption = new Option<FileInfo?>("--log-file")
{
    Description = "Also write the diagnostic log to this file."
};

var root = new RootCommand(
    "Enigma machine simulator. Reads a message, enciphers it, and writes the result. " +
    "Because the machine is reciprocal, the same settings both encipher and decipher.")
{
    settingsOption,
    inputOption,
    outputOption,
    verboseOption,
    logFileOption
};

// Individual settings can still be overridden with configuration keys such as
// --Enigma:Rotors:0:Name=V, so unmatched tokens are passed through rather than rejected.
root.TreatUnmatchedTokensAsErrors = false;

root.SetAction((parseResult, cancellationToken) => RunAsync(
    parseResult.GetValue(settingsOption),
    parseResult.GetValue(inputOption),
    parseResult.GetValue(outputOption),
    parseResult.GetValue(verboseOption),
    parseResult.GetValue(logFileOption),
    args,
    cancellationToken));

return await root.Parse(args).InvokeAsync();

async Task<int> RunAsync(
    FileInfo? settingsFile,
    FileInfo? input,
    FileInfo? output,
    bool verbose,
    FileInfo? logFile,
    string[] arguments,
    CancellationToken cancellationToken)
{
    if (settingsFile is not null && !settingsFile.Exists)
    {
        await Console.Error.WriteLineAsync($"Settings file not found: {settingsFile.FullName}");
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
    builder.Services.Configure<EnigmaSettings>(MachineConfiguration(builder, settingsFile));

    await builder.Build().RunAsync(cancellationToken);

    return Environment.ExitCode;
}

// A settings file replaces the machine configuration outright. It may hold the
// settings at the root or wrapped in an "Enigma" section, so a copy of
// appsettings.json works as a starting point either way.
IConfiguration MachineConfiguration(HostApplicationBuilder builder, FileInfo? settingsFile)
{
    if (settingsFile is null)
    {
        return builder.Configuration.GetSection("Enigma");
    }

    var configuration = new ConfigurationBuilder()
        .AddJsonFile(settingsFile.FullName, optional: false)
        .Build();

    var section = configuration.GetSection("Enigma");

    return section.Exists() ? section : configuration;
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
