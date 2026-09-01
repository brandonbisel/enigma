using Enigma.Models;

namespace Enigma.Analysis.Tests;

/// <summary>
/// The attack, judged the only way an attack can honestly be judged: give it the
/// ciphertext and nothing else, and see whether what comes back reads the message.
///
/// Note what is asserted. A break does not recover a string, it recovers a machine,
/// and the traffic pins that machine only as far as the traffic runs — so the test
/// is <see cref="SettingsEquivalence.ReadTheSame"/> rather than an equality of key
/// sheets. Asserting the letters would fail on answers that are right.
///
/// These searches take seconds rather than milliseconds. That is the feature: a
/// sweep of a hundred thousand settings over four hundred letters is a hundred
/// million passes through a real Enigma, and it is not worth faking.
/// </summary>
public class RotorSearchTests
{
    private static readonly SettingsEquivalence Equivalence = new(Machinery.Factory);

    private const string GrafSpeeCiphertext =
        "QQMWTQJWJTMNSXYLNSACMHXZZRXWLNQZZZDZVLVUTKXDXWSLSKNWEZNFCFRGLIUHXPVKINZAJVECMOTVPVNZIMRBUI" +
        "EUBZGFMZYPRMMEWTFZFGVLQSYQGWNDMQDNRZOVYMGAVLFMRAFZRYRMICPEZSMKBMHJUTDUQSBABROQLEFPRBZFJQSR" +
        "PMRYXWOXIJLDVIFVXJLMRQFGYHIMELRSDOTAIVVXMXMDPISBHLIMMBFMKJSWTJMCCPEGKPJAVBWKJUZQBDEWDJYBMT" +
        "YM";

    private static KeySheet Service(string rotors, string rings, string positions) => new()
    {
        Model = "Service", Reflector = "B", Rotors = rotors,
        RingSettings = rings, Positions = positions
    };

    [Fact]
    public void RecoversAnUnsteckeredMachineFromCiphertextAlone()
    {
        // An Enigma I with the board empty. The Ringstellung is a real one, so the
        // middle wheel turns over inside the message and the second phase has to
        // place it; the answer comes back written another way and reads the signal.
        var truth = Service("III I II", "CFH", "TVQ");
        var cipher = Machinery.Encipher(truth, Machinery.German[..400]);

        var space = SearchSpace.Of(
            Machinery.Parts,
            new KeySheet { Model = "Service", Reflector = "B" },
            ["I", "II", "III"],
            3);

        var found = new RotorSearch(Machinery.Factory).Run(space, space.Read(cipher), keep: 1);

        Assert.Equal("III I II", found[0].Settings.Rotors);
        Assert.True(
            Equivalence.ReadTheSame(found[0].Settings, truth, cipher),
            $"Recovered {found[0]}, which does not read the message.");
    }

    [Fact]
    public void WindingTheWheelsBackIsTheSameAsBuildingTheMachineAgain()
    {
        // The search reuses one machine across a wheel order, winding it back between
        // settings, because building a machine costs twice what running one over a few
        // hundred letters does. That is only safe if a wound-back machine cannot be
        // told from a fresh one — so every answer it reports has to score the same
        // when rebuilt from the key sheet it wrote.
        //
        // On a commercial machine, which is what this uses, there is a reflector to
        // put back as well as the wheels. It also has no plugboard at all, which is
        // the kind of machine this measure can actually break.
        var truth = new KeySheet
        {
            Model = "Commercial", Reflector = "G", Rotors = "K-II K-III K-I",
            RingSettings = "QRS", Positions = "QMT", ReflectorPosition = "A"
        };

        var cipher = Machinery.Encipher(truth, Machinery.German[..400]);
        var space = SearchSpace.Of(
            Machinery.Parts,
            new KeySheet { Model = "Commercial", Reflector = "G", ReflectorPosition = "A" },
            ["K-I", "K-II", "K-III"],
            3);

        var text = space.Read(cipher);
        var found = new RotorSearch(Machinery.Factory).Run(space, text, keep: 3);
        var measure = new IndexOfCoincidence(space.Alphabet);

        Assert.All(found, candidate =>
        {
            var machine = Machinery.Factory.Create(candidate.Settings);

            Assert.Equal(candidate.Score, measure.Of(text.Select(machine.Translate).ToArray()), 12);
        });

        // The wheel order is what this measure recovers dependably.
        Assert.Equal("K-II K-III K-I", found[0].Settings.Rotors);

        // And the setting is a break rather than an equivalence: it lands a letter
        // from the true one, so it garbles the opening and then reads the rest. A
        // test that demanded the whole message would call this a failure, and anyone
        // holding the decipherment would disagree.
        var (agreed, of) = Equivalence.AgreeOn(found[0].Settings, truth, cipher);

        Assert.True(
            agreed > of - 20,
            $"Recovered {found[0]}, which reads {agreed} of {of} letters.");
    }

    [Fact]
    public void TheAnswerIsTheSameHoweverManyCoresRunIt()
    {
        // The sweep is parallel, so ties have to be broken by the setting rather than
        // by whichever core reached it first. A search that answered differently on a
        // different machine could not be tested at all.
        var cipher = Machinery.Encipher(Service("I III II", "AAA", "KLM"), Machinery.German[..250]);
        var space = SearchSpace.Of(
            Machinery.Parts,
            new KeySheet { Model = "Service", Reflector = "B" },
            ["I", "II", "III"],
            3);

        var search = new RotorSearch(Machinery.Factory);
        var once = search.Wheels(space, space.Read(cipher), keep: 3);
        var twice = search.Wheels(space, space.Read(cipher), keep: 3);

        Assert.Equal(
            once.Select(candidate => candidate.ToString()),
            twice.Select(candidate => candidate.ToString()));
    }

    [Fact]
    public void ARingSettingFarFromAHidesTheTrueSettingFromTheFirstPhase()
    {
        // The blind spot in the first phase, stated as a number rather than as a
        // caveat. The phase leaves every ring at A and sweeps the positions, which
        // covers every alignment of the wiring — but a fast wheel whose ring really
        // sits at Q has its turnovers sixteen keypresses out of phase, so the middle
        // wheel spends much of the message one step from where it belongs.
        //
        // The result is that the true alignment scores like text with no structure in
        // it at all. It is not merely ranked low, it is invisible, and no shortlist
        // taken on score can contain it. That is why the second phase is given the
        // best of each wheel order rather than the best few settings overall: the
        // wiring is still right when the stepping is wrong, so the order survives
        // this even though the setting does not.
        var truth = Service("III I II", "QQQ", "HGF");
        var cipher = Machinery.Encipher(truth, Machinery.German[..400]);

        var space = SearchSpace.Of(
            Machinery.Parts,
            new KeySheet { Model = "Service", Reflector = "B" },
            ["I", "II", "III"],
            3);

        var text = space.Read(cipher);
        var measure = new IndexOfCoincidence(space.Alphabet);

        // Ringstellung QQQ at HGF is the alignment (17, 16, 15), which is what the
        // first phase would have to find at rings AAA.
        var aligned = space.Sheet(new Arrangement(["III", "I", "II"], "B"), "AAA", "RQP");
        var atTheTruth = measure.Of(
            text.Select(Machinery.Factory.Create(aligned).Translate).ToArray());

        Assert.InRange(atTheTruth, 0.036, 0.041);

        // And the setting itself, once the rings are right, reads as German.
        Assert.True(
            measure.Of(text.Select(Machinery.Factory.Create(truth).Translate).ToArray()) > 0.05,
            "The true setting no longer reads this message.");
    }

    [Fact]
    public void TheIndexOfCoincidenceCannotBreakASteckeredServiceMachine()
    {
        // The limit that matters, recorded rather than hidden. The Graf Spee signal is
        // the longest three wheel message here and the one on the fewest cables --
        // eight rather than ten -- so if this measure were going to break a service
        // Enigma it would break this one.
        //
        // It does not, and not by a little. Even told which three wheels were in the
        // machine, the best setting the sweep finds scores well above the true one.
        // The reason is that the board sits inside the rotor sandwich: pulling it out
        // does not relabel the plaintext, it shreds it, so the true setting sits
        // barely above a random one while the best of a hundred thousand wrong
        // settings sits well above that.
        //
        // This test is here so that a later change cannot quietly claim the break.
        Assert.True(KeySheets.TryGet("graf-spee", out var truth));

        var unsteckered = truth.Copy();
        unsteckered.Plugboard = string.Empty;

        var space = SearchSpace.Of(
            Machinery.Parts,
            new KeySheet { Model = "Service", Reflector = "B" },
            ["I", "V", "VI"],
            3);

        var text = space.Read(GrafSpeeCiphertext);
        var measure = new IndexOfCoincidence(space.Alphabet);

        var atTheTruth = measure.Of(
            text.Select(Machinery.Factory.Create(unsteckered).Translate).ToArray());

        var best = new RotorSearch(Machinery.Factory).Wheels(space, text, keep: 1)[0];

        Assert.InRange(atTheTruth, 0.038, 0.043);
        Assert.True(
            best.Score > atTheTruth,
            $"The true setting scored {atTheTruth:F5} and the best wrong one {best.Score:F5}; " +
            "if the true setting now wins, this measure has become able to do something it could not.");

        // And with the cables in, the very same wheels read the signal as German.
        Assert.True(
            measure.Of(text.Select(Machinery.Factory.Create(truth).Translate).ToArray()) > 0.055,
            "The pinned setting no longer reads the Graf Spee signal.");
    }
}
