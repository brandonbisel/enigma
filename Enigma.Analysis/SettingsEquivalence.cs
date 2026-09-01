using Enigma.Models;

namespace Enigma.Analysis;

/// <summary>
/// Whether two key sheets are the same machine.
///
/// A break does not recover a string; it recovers a machine, and more than one
/// sheet can describe the same one. Shift a ring and its wheel's position together
/// and the wiring is where it was; do it on a wheel whose notch drives nothing and
/// the stepping is unchanged too, so nothing the machine ever does can tell the two
/// settings apart. Both of the alternate settings published beside the Dönitz and
/// Graf Spee signals are of that kind. The one published beside the Rasch message
/// is not: it reads that message and then parts from it a few letters later, which
/// is the traffic pinning the wheels only as far as it runs.
///
/// So there are two questions and they have different answers, and an attack that
/// asked only the first would call itself right too easily.
/// </summary>
public sealed class SettingsEquivalence
{
    private readonly IEnigmaMachineFactory _factory;

    public SettingsEquivalence(IEnigmaMachineFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <summary>
    /// Whether the two settings read one particular message the same way. This is
    /// what a break is actually judged by: the message is the only evidence there is.
    /// </summary>
    public bool ReadTheSame(KeySheet a, KeySheet b, string message) =>
        AgreeOn(a, b, message) is var (agreed, of) && agreed == of;

    /// <summary>
    /// How many of a message's characters the two settings read alike, and how many
    /// there were.
    ///
    /// A break is not all or nothing. A setting a letter away from the true one
    /// deciphers the message perfectly from the first turnover onwards and garbles
    /// only what comes before it — which is a broken message by any practical
    /// measure, and is not the same setting. Reporting the count says which of those
    /// has happened instead of collapsing both into "no".
    /// </summary>
    public (int Agreed, int Of) AgreeOn(KeySheet a, KeySheet b, string message)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var first = _factory.Create(a);
        var second = _factory.Create(b);
        var agreed = 0;
        var of = 0;

        foreach (var character in message ?? string.Empty)
        {
            var contact = Contact(first, character);

            if (contact < 0)
            {
                continue;
            }

            of++;

            if (first.Translate(contact) == second.Translate(contact))
            {
                agreed++;
            }
        }

        return (agreed, of);
    }

    /// <summary>
    /// Whether no message of any length, in any letters, could tell the two apart.
    /// </summary>
    public bool AreOneMachine(KeySheet a, KeySheet b) => PartsAt(a, b) is null;

    /// <summary>
    /// The first keypress at which some letter is read differently, or null if there
    /// is none — in which case the two really are one machine.
    ///
    /// This is a proof rather than a sample. The wheels return to where they started
    /// after a fixed number of keypresses, so a run at least that long visits every
    /// state the machine has; running one for each letter of the alphabet therefore
    /// compares the two machines at every position of every wheel for every key that
    /// could be pressed. Agreeing on all of that, they agree on everything. The
    /// alternative — a long run of one letter, which is how this began — samples one
    /// point of each of those permutations and can only ever fail to find a
    /// difference.
    /// </summary>
    public int? PartsAt(KeySheet a, KeySheet b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var contacts = _factory.Create(a).CharacterMap.Count;

        if (_factory.Create(b).CharacterMap.Count != contacts)
        {
            return 0;
        }

        var run = Math.Max(States(_factory.Create(a)), States(_factory.Create(b)));
        int? parted = null;

        for (var letter = 0; letter < contacts; letter++)
        {
            var first = _factory.Create(a);
            var second = _factory.Create(b);

            for (var i = 0; i < run; i++)
            {
                if (first.Translate(letter) == second.Translate(letter))
                {
                    continue;
                }

                parted = parted is { } already ? Math.Min(already, i) : i;
                run = parted.Value;

                break;
            }
        }

        return parted;
    }

    // An upper bound on how many keypresses it takes the wheels to come back to
    // where they started: every wheel that can turn, at every position it can hold.
    // The pawl drive's double step makes the true period shorter than this, which is
    // why it is a bound and not a count.
    private static int States(IEnigmaMachine machine)
    {
        var moving = machine.Rotors.Count(rotor => !rotor.IsThin) +
                     (machine.Reflector is IRotatingReflector ? 1 : 0);

        var states = 1;

        for (var i = 0; i < moving; i++)
        {
            states *= machine.CharacterMap.Count;
        }

        return states;
    }

    private static int Contact(IEnigmaMachine machine, char character)
    {
        var index = machine.CharacterMap.GetIndex(character);

        return index >= 0 ? index : machine.CharacterMap.GetIndex(char.ToUpperInvariant(character));
    }
}
