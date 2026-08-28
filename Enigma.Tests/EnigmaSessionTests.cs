using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Enigma.Tests;

/// <summary>
/// The session is the seam both front ends work through, so what it owes them is
/// that a keystroke at a time and a whole message come to the same thing.
/// </summary>
public class EnigmaSessionTests
{
    [Fact]
    public void PressingKeysOneAtATimeMatchesThePublishedMessage()
    {
        // Barbarossa, driven the way a panel drives it rather than the way a pipe
        // does. The strongest vector in the suite, applied to the new seam.
        const string cipher =
            "EDPUDNRGYSZRCXNUYTPOMRMBOFKTBZREZKMLXLVEFGUEYSIOZVEQMIKUBPMMYLKLTTDEISMDICAGYKUACTCDOMOHWX" +
            "MUUIAUBSTSLRNBZSZWNRFXWFYSSXJZVIJHIDISHPRKLKAYUPADTXQSPINQMATLPIFSVKDASCTACDPBOPVHJK";

        const string plain =
            "AUFKLXABTEILUNGXVONXKURTINOWAXKURTINOWAXNORDWESTLXSEBEZXSEBEZXUAFFLIEGERSTRASZERIQTUNGXDUB" +
            "ROWKIXDUBROWKIXOPOTSCHKAXOPOTSCHKAXUMXEINSAQTDREINULLXUHRANGETRETENXANGRIFFXINFXRGTX";

        var session = Open("barbarossa");

        Assert.Equal(plain, string.Concat(cipher.Select(session.Press)));
    }

    [Fact]
    public void TypingAMessageMatchesPressingItsKeys()
    {
        const string message = "ATTACKATDAWNXXTHEWEATHERISFINE";

        var typed = Open("barbarossa").Type(message);
        var pressed = string.Concat(message.Select(Open("barbarossa").Press));

        Assert.Equal(typed, pressed);
    }

    [Fact]
    public void AKeyTheMachineDoesNotHaveLightsNoLampAndTurnsNoWheel()
    {
        // There is no such key to press, so nothing at all happens.
        var session = Open("default");
        var before = session.WindowText;

        Assert.Null(session.Press('7'));
        Assert.Null(session.Press(' '));
        Assert.Equal(before, session.WindowText);
    }

    [Fact]
    public void LowerCaseIsAccepted()
    {
        Assert.Equal(Open("default").Press('A'), Open("default").Press('a'));
    }

    [Fact]
    public void TheWindowFollowsTheWheels()
    {
        var session = Open("default");

        Assert.Equal("AAA", session.WindowText);

        session.Press('A');

        Assert.Equal("AAB", session.WindowText);
    }

    [Fact]
    public void ResettingPutsTheMachineBackToTheKeySheet()
    {
        var session = Open("barbarossa");
        var start = session.WindowText;
        var first = session.Press('A');

        session.Type("SOMEMORETEXT");
        session.Reset();

        Assert.Equal(start, session.WindowText);
        Assert.Equal(first, session.Press('A'));
    }

    [Fact]
    public void ResettingRestoresABoardPatchedByHand()
    {
        // Rebuilt from the key sheet rather than wound back, so the cables the
        // operator moved come back too.
        var session = Open("default");

        session.Patch('A', 'B');
        Assert.True(session.IsPatched('A', 'B'));

        session.Reset();

        Assert.False(session.IsPatched('A', 'B'));
    }

    [Fact]
    public void PatchingTheBoardChangesTheCipher()
    {
        var plain = Open("default");
        var patched = Open("default");

        patched.Patch('A', 'B');

        Assert.NotEqual(plain.Type("AAAAA"), patched.Type("AAAAA"));
    }

    [Fact]
    public void ACableToAnOccupiedJackIsRefused()
    {
        var session = Open("default");

        session.Patch('A', 'B');

        Assert.Throws<ArgumentException>(() => session.Patch('A', 'C'));
    }

    [Fact]
    public void CablesAreReportedAsLetters()
    {
        var session = Open("barbarossa");

        Assert.Contains(('A', 'V'), session.Cables());
    }

    [Fact]
    public void ALetterOutsideTheAlphabetCannotBeCabled()
    {
        Assert.Throws<ArgumentException>(() => Open("default").Patch('A', '7'));
    }

    [Fact]
    public void AKeySheetTheMachineCannotBeBuiltFromIsReportedNotThrown()
    {
        var result = EnigmaSession.Open(BuildFactory(), new KeySheet
        {
            Reflector = "B",
            Rotors = "I II NOSUCHWHEEL",
            RingSettings = "AAA",
            Positions = "AAA"
        });

        Assert.False(result.Succeeded);
        Assert.Null(result.Session);
        Assert.NotNull(result.Error);
        Assert.Contains("NOSUCHWHEEL", result.Error);
    }

    [Fact]
    public void APlugboardOnAMachineWithNoBoardIsReportedNotThrown()
    {
        var result = EnigmaSession.Open(BuildFactory(), new KeySheet
        {
            Model = "G-31",
            Reflector = "G",
            Rotors = "G-I G-II G-III",
            RingSettings = "AAA",
            Positions = "AAA",
            Plugboard = "AB CD"
        });

        Assert.False(result.Succeeded);
        Assert.Contains("no plugboard", result.Error);
    }

    [Fact]
    public void ATurningReflectorReportsItsPosition()
    {
        var session = Open("g31");

        Assert.NotNull(session.ReflectorPosition);

        var start = session.ReflectorPosition;

        session.Type(new string('A', 500));

        Assert.NotEqual(start, session.ReflectorPosition);
    }

    [Fact]
    public void AFixedReflectorHasNoPosition()
    {
        Assert.Null(Open("default").ReflectorPosition);
    }

    [Fact]
    public void WatchingReportsThePathOfEveryKeypress()
    {
        var session = Open("default");
        var traces = new List<TranslationTrace>();

        session.Traced += traces.Add;

        session.Type("ABC");

        Assert.Equal(3, traces.Count);
        Assert.Equal(traces[^1], session.LastTrace);
    }

    [Fact]
    public void WatchingSurvivesAReset()
    {
        // Reset builds a new machine, and a watcher that quietly stopped reporting
        // would be worse than one that threw.
        var session = Open("default");
        var traces = new List<TranslationTrace>();

        session.Traced += traces.Add;

        session.Press('A');
        session.Reset();
        session.Press('A');

        Assert.Equal(2, traces.Count);
    }

    [Fact]
    public void NotWatchingCostsNothing()
    {
        var session = Open("default");

        session.Press('A');

        Assert.Null(session.LastTrace);
    }

    [Fact]
    public void WatchingDoesNotChangeTheCipher()
    {
        var watched = Open("barbarossa");

        watched.Traced += _ => { };

        Assert.Equal(Open("barbarossa").Type("ATTACKATDAWN"), watched.Type("ATTACKATDAWN"));
    }

    private static EnigmaSession Open(string keySheet)
    {
        var catalogue = new KeySheetCatalogue(Extra);

        Assert.True(catalogue.TryGet(keySheet, out var sheet));

        var result = EnigmaSession.Open(BuildFactory(), sheet);

        Assert.True(result.Succeeded, result.Error);

        return result.Session!;
    }

    // The packaged sheets are all service machines, so a Zählwerk one is supplied
    // here rather than added to what ships.
    private static readonly KeyValuePair<string, KeySheet>[] Extra =
    [
        new("g31", new KeySheet
        {
            Name = "Zählwerk G-31",
            Model = "G-31",
            Reflector = "G",
            Rotors = "G-I G-II G-III",
            RingSettings = "AAA",
            Positions = "AAA",
            ReflectorPosition = "A",
            ReflectorRingSetting = "A"
        })
    ];

    private static IEnigmaMachineFactory BuildFactory() =>
        new ServiceCollection()
            .AddEnigmaServices()
            .BuildServiceProvider()
            .GetRequiredService<IEnigmaMachineFactory>();
}
