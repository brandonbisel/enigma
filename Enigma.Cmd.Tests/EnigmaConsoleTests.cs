using Enigma.Cmd;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Enigma.Cmd.Tests;

/// <summary>
/// The console app end to end: a key sheet and a file in, a file out.
///
/// It reads and writes real files rather than a stubbed stream, because the thing
/// worth testing is the whole path an operator uses — including that the indicator
/// goes to standard error and so cannot contaminate the ciphertext on standard
/// output.
/// </summary>
public class EnigmaConsoleTests : IDisposable
{
    private readonly List<string> _files = [];
    private readonly TextWriter _stderr = Console.Error;
    private readonly StringWriter _captured = new();
    private readonly int _exitCode = Environment.ExitCode;

    public EnigmaConsoleTests() => Console.SetError(_captured);

    public void Dispose()
    {
        Console.SetError(_stderr);

        // The app reports failure by setting it, and this process is a test run
        // rather than the app.
        Environment.ExitCode = _exitCode;

        foreach (var file in _files.Where(File.Exists))
        {
            File.Delete(file);
        }

        GC.SuppressFinalize(this);
    }

    // -- the machine ------------------------------------------------------------

    [Fact]
    public void AMessageIsEncipheredAndTheSameSettingsBringItBack()
    {
        var enciphered = Run(Sheet("barbarossa"), "ATTACKATDAWN");

        Assert.NotEqual("ATTACKATDAWN", enciphered);
        Assert.Equal("ATTACKATDAWN", Run(Sheet("barbarossa"), enciphered));
    }

    [Fact]
    public void TheMachineKeepsSteppingAcrossLines()
    {
        // One machine for the whole run, as the real one was: the same line twice
        // does not encipher to the same thing.
        var lines = Run(Sheet("barbarossa"), "AAAAA\nAAAAA").Split('\n');

        Assert.NotEqual(lines[0], lines[1]);
    }

    [Fact]
    public void GroupsAreWrittenAsASignallerWouldWriteThem()
    {
        Assert.All(
            Run(Sheet("barbarossa"), "ATTACKATDAWNX", groups: 5).Split(' '),
            group => Assert.True(group.Length <= 5));
    }

    // -- the army indicator -----------------------------------------------------

    [Fact]
    public void AMessageKeyIsReportedAndUsed()
    {
        var enciphered = Run(Sheet("barbarossa"), "ATTACKATDAWN", messageKey: "RTZ");

        Assert.Contains("Ground setting BLA", Stderr());

        // The rotors really moved to the message key: the same text at the ground
        // setting comes out differently.
        Assert.NotEqual(Run(Sheet("barbarossa"), "ATTACKATDAWN"), enciphered);
    }

    [Fact]
    public void AnIndicatorRecoversTheKeyTheSenderChose()
    {
        var sheet = Sheet("barbarossa");
        var enciphered = Run(sheet, "ATTACKATDAWN", messageKey: "RTZ");
        var indicator = Indicator(Stderr());

        Assert.Equal("ATTACKATDAWN", Run(Sheet("barbarossa"), enciphered, indicator: indicator));
    }

    [Fact]
    public void AnIndicatorThatCannotBeReadIsReportedAndStops()
    {
        Run(Sheet("barbarossa"), "AAAAA", indicator: "NOTANINDICATOR");

        Assert.NotEqual(0, Environment.ExitCode);
    }

    // -- the naval indicator ----------------------------------------------------

    [Fact]
    public void U534sIndicatorKeysTheMachineWhereItsOperatorDid()
    {
        // P1030690, read the naval way, has to leave the rotors at ODFF. Rather
        // than trusting the report, the same plaintext is run through a machine
        // started at ODFF outright and the two must agree.
        var read = Run(
            Sheet("u534"), "AAAAAAAAAA", table: BigramTables.QuelleA, indicator: "FNHC GVET");

        var atOdff = Sheet("u534");
        atOdff.Positions = "ODFF";

        var straight = Run(atOdff, "AAAAAAAAAA");

        Assert.Equal(straight, read);
    }

    [Fact]
    public void TheNavalWorkingIsReported()
    {
        Run(Sheet("u534"), "AAAAA", table: BigramTables.QuelleA, indicator: "FNHCGVET");

        var reported = Stderr();

        Assert.Contains("Schlüsselkenngruppe DUZ", reported);
        Assert.Contains("Verfahrenkenngruppe YMU", reported);
        Assert.Contains("rotors ODFF", reported);
    }

    [Fact]
    public void SendingTheNavalWayProducesTheIndicatorThatWentOut()
    {
        // The real trigrams and the real fillers, so this is U-534's own eight
        // letters rather than any eight that would round-trip.
        Run(
            Sheet("u534"),
            "AAAAA",
            table: BigramTables.QuelleA,
            keyGroup: "DUZ",
            messageGroup: "YMU",
            firstFiller: 'K',
            lastFiller: 'Z');

        Assert.Contains("indicator FNHCGVET", Stderr());
    }

    [Fact]
    public void WhatIsSentTheNavalWayIsReadBackTheNavalWay()
    {
        var enciphered = Run(
            Sheet("u534"),
            "ATTACKATDAWN",
            table: BigramTables.QuelleA,
            keyGroup: "DUZ",
            messageGroup: "YMU",
            firstFiller: 'K',
            lastFiller: 'Z');

        Assert.Equal(
            "ATTACKATDAWN",
            Run(Sheet("u534"), enciphered, table: BigramTables.QuelleA, indicator: "FNHCGVET"));
    }

    [Fact]
    public void TheWrongTableIsNotTheRightRotors()
    {
        var right = Run(
            Sheet("u534"), "AAAAA", table: BigramTables.QuelleA, indicator: "FNHCGVET");

        var wrong = Run(
            Sheet("u534"), "AAAAA", table: BigramTables.QuelleB, indicator: "FNHCGVET");

        Assert.NotEqual(right, wrong);
    }

    [Fact]
    public void ANavalIndicatorThatIsNotEightLettersIsReportedAndStops()
    {
        Run(Sheet("u534"), "AAAAA", table: BigramTables.QuelleA, indicator: "FNHC");

        Assert.NotEqual(0, Environment.ExitCode);
        Assert.Contains("eight letters", Stderr());
    }

    // -- the plumbing -----------------------------------------------------------

    private static KeySheet Sheet(string name)
    {
        Assert.True(KeySheets.All.TryGetValue(name, out var sheet));

        return sheet.Copy();
    }

    private string Stderr() => _captured.ToString();

    private static string Indicator(string reported) =>
        reported.Split("indicator ")[1].Split(',', '\n', '\r')[0].Trim();

    private string Run(
        KeySheet sheet,
        string message,
        string? messageKey = null,
        string? indicator = null,
        int? groups = null,
        BigramTable? table = null,
        string? keyGroup = null,
        string? messageGroup = null,
        char firstFiller = 'X',
        char lastFiller = 'X')
    {
        var input = Temp();
        var output = Temp();

        File.WriteAllText(input, message);

        var options = new ConsoleOptions(
            new FileInfo(input),
            new FileInfo(output),
            messageKey,
            indicator,
            Doubled: false,
            Prepare: false,
            groups,
            table,
            keyGroup,
            messageGroup,
            firstFiller,
            lastFiller);

        var services = new ServiceCollection().AddEnigmaServices().BuildServiceProvider();

        var console = new EnigmaConsole(
            services.GetRequiredService<IEnigmaMachineFactory>(),
            services.GetRequiredService<IPartsCatalogue>(),
            services.GetRequiredService<IIndicatorProcedure>(),
            services.GetRequiredService<INavalIndicatorProcedure>(),
            new Lifetime(),
            NullLogger<EnigmaConsole>.Instance,
            options,
            Options.Create(sheet));

        console.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        console.ExecuteTask?.GetAwaiter().GetResult();

        return File.Exists(output) ? File.ReadAllText(output).TrimEnd('\r', '\n') : string.Empty;
    }

    private string Temp()
    {
        var path = Path.Combine(Path.GetTempPath(), $"enigma-{Guid.NewGuid():N}");

        _files.Add(path);

        return path;
    }

    // The app tells the host to stop when it has finished reading; nothing here
    // is hosting it, so there is nothing to stop.
    private sealed class Lifetime : IHostApplicationLifetime
    {
        public CancellationToken ApplicationStarted => CancellationToken.None;

        public CancellationToken ApplicationStopping => CancellationToken.None;

        public CancellationToken ApplicationStopped => CancellationToken.None;

        public void StopApplication()
        {
        }
    }
}
