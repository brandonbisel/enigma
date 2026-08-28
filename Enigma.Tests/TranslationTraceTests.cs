using Enigma.Reflectors;
using Enigma.Rotors;
using Microsoft.Extensions.Logging;

namespace Enigma.Tests;

/// <summary>
/// The trace is what lets a caller watch the machine work. It has to describe the
/// path faithfully, and above all it must not disturb the cipher it is describing.
/// </summary>
public class TranslationTraceTests
{
    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    [Fact]
    public void WatchingDoesNotChangeTheCipher()
    {
        var quiet = BuildMachine();
        var watched = BuildMachine();

        watched.Translated += _ => { };

        Assert.Equal(Encipher(quiet), Encipher(watched));
    }

    [Fact]
    public void EveryKeypressIsReported()
    {
        var machine = BuildMachine();
        var traces = Watch(machine);

        machine.Translate([0, 1, 2, 3]);

        Assert.Equal(4, traces.Count);
    }

    [Fact]
    public void TheTraceReportsTheKeyPressedAndTheLampLit()
    {
        var machine = BuildMachine();
        var traces = Watch(machine);

        var output = machine.Translate(0);

        var trace = Assert.Single(traces);

        Assert.Equal(0, trace.Input);
        Assert.Equal(output, trace.Output);
    }

    [Fact]
    public void TheStepsFollowTheCurrentFromKeyToLamp()
    {
        var machine = BuildMachine();
        var traces = Watch(machine);

        machine.Translate(0);

        // In through the board and the stator, right to left across the wheels,
        // back off the reflector, then out the way it came.
        Assert.Equal(
            ["plug", "Standard", "III", "II", "I", "B", "I'", "II'", "III'", "Standard'", "plug"],
            Assert.Single(traces).Steps.Select(step => step.Component));
    }

    [Fact]
    public void EachStepBeginsWhereTheLastOneEnded()
    {
        // Derived rather than asserted by hand: a path with a gap in it would be
        // describing a machine that is not the one doing the enciphering.
        var machine = BuildMachine();
        var traces = Watch(machine);

        machine.Translate([.. Enumerable.Range(0, 26)]);

        foreach (var steps in traces.Select(trace => trace.Steps))
        {
            foreach (var (before, after) in steps.Zip(steps.Skip(1)))
            {
                Assert.Equal(before.Output, after.Input);
            }
        }
    }

    [Fact]
    public void TheTraceBeginsAtTheKeyAndEndsAtTheLamp()
    {
        var machine = BuildMachine();
        var traces = Watch(machine);

        machine.Translate(7);

        var trace = Assert.Single(traces);

        Assert.Equal(trace.Input, trace.Steps[0].Input);
        Assert.Equal(trace.Output, trace.Steps[^1].Output);
    }

    [Fact]
    public void TheWindowIsWhereTheWheelsStandAsTheLampLights()
    {
        // The wheels turn before the current flows, so the trace must report the
        // window the operator saw lit, not the one before the key went down.
        var machine = BuildMachine();
        var traces = Watch(machine);

        machine.Translate(0);

        Assert.Equal(machine.Rotors.Select(rotor => rotor.Position), Assert.Single(traces).Window);
    }

    [Fact]
    public void UnsubscribingStopsTheTrace()
    {
        var machine = BuildMachine();
        var traces = new List<TranslationTrace>();

        void Watcher(TranslationTrace trace) => traces.Add(trace);

        machine.Translated += Watcher;
        machine.Translate(0);
        machine.Translated -= Watcher;
        machine.Translate(0);

        Assert.Single(traces);
    }

    [Fact]
    public void TheLogLineSaysWhatTheTraceSays()
    {
        // The two used to be assembled separately. Formatting the log from the trace
        // is what stops a diagnostic and a display ever disagreeing.
        var logger = new CapturingLogger();
        var machine = BuildMachine(logger);
        var traces = Watch(machine);

        machine.Translate(0);

        var trace = Assert.Single(traces);
        var line = Assert.Single(logger.Messages);

        var path = trace.Steps.Select(step =>
            $"{step.Component} {Letter(step.Input)}>{Letter(step.Output)}");

        Assert.Contains(string.Join(" | ", path), line);
        Assert.Contains($"window {string.Concat(trace.Window.Select(Letter))}", line);
        Assert.Contains($"{Letter(trace.Input)} -> {Letter(trace.Output)}", line);
    }

    [Fact]
    public void ATraceIsStillProducedForALoggerWithNoWatcher()
    {
        // Debug logging and a watcher are independent reasons to build a trace.
        var logger = new CapturingLogger();
        var machine = BuildMachine(logger);

        machine.Translate(0);

        Assert.Single(logger.Messages);
    }

    [Fact]
    public void NothingIsLoggedForAWatcherWhenDebugIsOff()
    {
        var logger = new CapturingLogger { Enabled = false };
        var machine = BuildMachine(logger);
        var traces = Watch(machine);

        machine.Translate(0);

        Assert.Single(traces);
        Assert.Empty(logger.Messages);
    }

    private static List<TranslationTrace> Watch(IEnigmaMachine machine)
    {
        var traces = new List<TranslationTrace>();

        machine.Translated += traces.Add;

        return traces;
    }

    private static string Letter(int contact) => CharacterMap.GetCharacter(contact).ToString();

    private static EnigmaMachine BuildMachine(ILogger<EnigmaMachine>? logger = null) =>
        new(new PlugBoard(CharacterMap),
            [new RotorI(), new RotorII(), new RotorIII()],
            new ReflectorB(),
            logger,
            CharacterMap);

    private static string Encipher(IEnigmaMachine machine)
    {
        var input = new string('A', 60).Select(CharacterMap.GetIndex);

        return string.Concat(machine.Translate(input).Select(CharacterMap.GetCharacter));
    }

    private sealed class CapturingLogger : ILogger<EnigmaMachine>
    {
        public bool Enabled { get; init; } = true;

        public List<string> Messages { get; } = [];

        public bool IsEnabled(LogLevel logLevel) => Enabled;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
