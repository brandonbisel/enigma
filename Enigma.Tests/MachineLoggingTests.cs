using Enigma.Reflectors;
using Enigma.Rotors;
using Microsoft.Extensions.Logging;

namespace Enigma.Tests;

public class MachineLoggingTests
{
    private static readonly ICharacterMap CharacterMap = new DefaultCharacterMap();

    [Fact]
    public void EnablingTheTraceDoesNotChangeTheCipher()
    {
        // The trace is assembled inside Translate, so it has to be provably inert.
        var quiet = BuildMachine(null);
        var traced = BuildMachine(new CapturingLogger());

        Assert.Equal(Encrypt(quiet), Encrypt(traced));
    }

    [Fact]
    public void TheTraceRecordsThePathThroughEveryComponent()
    {
        var logger = new CapturingLogger();
        var machine = BuildMachine(logger);

        machine.Translate(0);

        var entry = Assert.Single(logger.Messages, message => message.Contains("->"));

        Assert.Contains("plug", entry);
        Assert.Contains("III", entry);
        Assert.Contains("II", entry);
        Assert.Contains("I ", entry);
        Assert.Contains("B", entry);
        Assert.Contains("window", entry);
    }

    [Fact]
    public void TheTraceIsNotBuiltWhenDebugIsDisabled()
    {
        var logger = new CapturingLogger { Enabled = false };
        var machine = BuildMachine(logger);

        machine.Translate(0);

        Assert.Empty(logger.Messages);
    }

    private static EnigmaMachine BuildMachine(ILogger<EnigmaMachine>? logger) =>
        new(new PlugBoard(CharacterMap),
            [new RotorI(), new RotorII(), new RotorIII()],
            new ReflectorB(),
            logger,
            CharacterMap);

    private static string Encrypt(IEnigmaMachine machine)
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
