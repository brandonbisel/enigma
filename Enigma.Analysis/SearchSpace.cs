using Enigma.Models;

namespace Enigma.Analysis;

/// <summary>
/// What a search is searching: which wheels were in the box, how many of them the
/// machine takes, and which reflectors it might have been behind.
///
/// It is built from a key sheet holding what is <em>known</em> — the model, the
/// alphabet, the entry wheel, the reflector — and a box of wheels holding what is
/// not. The rotor order, the Ringstellung and the Grundstellung on that sheet are
/// ignored, because those are the things being looked for. Which wheels were issued
/// is a legitimate thing to know: a codebreaker knew what was in the box.
///
/// Arrangements that could not have been assembled are dropped here, by asking the
/// machine's own <see cref="IMachineLayout.Validate"/> rather than by restating its
/// rules. That is what keeps the M4's fitment — a thin wheel leftmost, and only
/// beside a thin reflector — true of a search without it being written down twice.
/// </summary>
public sealed class SearchSpace
{
    private readonly KeySheet _known;

    private SearchSpace(
        KeySheet known,
        ICharacterMap alphabet,
        int fitted,
        IReadOnlyList<Arrangement> arrangements)
    {
        _known = known;
        Alphabet = alphabet;
        Fitted = fitted;
        Arrangements = arrangements;
    }

    public ICharacterMap Alphabet { get; }

    /// <summary>How many wheels the machine carries.</summary>
    public int Fitted { get; }

    public IReadOnlyList<Arrangement> Arrangements { get; }

    /// <summary>
    /// How many settings a sweep of every wheel order at every starting position
    /// comes to. The thin wheel of an M4 is counted, because it does not turn but it
    /// is still set by hand and its position still matters.
    /// </summary>
    public long Settings => Arrangements.Count * (long)Math.Pow(Alphabet.Count, Fitted);

    public static SearchSpace Of(
        IPartsCatalogue parts,
        KeySheet known,
        IEnumerable<string> box,
        int fitted,
        IEnumerable<string>? reflectors = null)
    {
        ArgumentNullException.ThrowIfNull(parts);
        ArgumentNullException.ThrowIfNull(known);
        ArgumentNullException.ThrowIfNull(box);

        var alphabet = parts.GetCharacterMap(known.CharacterMapName());
        var layout = parts.GetLayout(known.ModelName());
        var entryWheel = parts.GetEntryWheel(known.EntryWheelName(layout.DefaultEntryWheel), alphabet);

        var wheels = box
            .Select(name => name.Trim().ToUpperInvariant())
            .Where(name => name.Length > 0)
            .Distinct()
            .ToList();

        if (fitted < 1 || fitted > wheels.Count)
        {
            throw new ArgumentException(
                $"A machine fitted with {fitted} wheels cannot be searched from a box of {wheels.Count}.",
                nameof(fitted));
        }

        var behind = (reflectors ?? [known.ReflectorName()])
            .Select(name => name.Trim().ToUpperInvariant())
            .Where(name => name.Length > 0)
            .Distinct()
            .ToList();

        if (behind.Count == 0)
        {
            throw new ArgumentException("A search needs at least one reflector to try.", nameof(reflectors));
        }

        var arrangements = new List<Arrangement>();

        foreach (var order in Orders(wheels, fitted))
        {
            foreach (var reflector in behind)
            {
                if (CouldBeBuilt(parts, layout, entryWheel, alphabet, order, reflector))
                {
                    arrangements.Add(new Arrangement(order, reflector));
                }
            }
        }

        if (arrangements.Count == 0)
        {
            throw new ArgumentException(
                $"No arrangement of {fitted} wheels from this box could be built as a " +
                $"{layout.Name} machine.",
                nameof(box));
        }

        return new SearchSpace(known.Copy(), alphabet, fitted, arrangements);
    }

    /// <summary>A key sheet for one arrangement at one setting, ready for the factory.</summary>
    public KeySheet Sheet(Arrangement arrangement, string ringSettings, string positions)
    {
        ArgumentNullException.ThrowIfNull(arrangement);

        var sheet = _known.Copy();

        sheet.Rotors = string.Join(" ", arrangement.Wheels);
        sheet.Reflector = arrangement.Reflector;
        sheet.RingSettings = ringSettings;
        sheet.Positions = positions;

        // The board is what a search leaves alone. The wheels are recovered through
        // the cables rather than after them, so the machine it runs has none in.
        sheet.Plugboard = string.Empty;
        sheet.Uhr = string.Empty;

        return sheet;
    }

    /// <summary>Ciphertext as contacts, dropping anything this machine has no key for.</summary>
    public int[] Read(string ciphertext) =>
        (ciphertext ?? string.Empty)
        .Select(character => Alphabet.GetIndex(character) is var index && index >= 0
            ? index
            : Alphabet.GetIndex(char.ToUpperInvariant(character)))
        .Where(contact => contact >= 0)
        .ToArray();

    /// <summary>A setting written the way a key sheet writes it.</summary>
    public string Setting(IReadOnlyList<int> wheels) =>
        string.Concat(wheels.Select(Alphabet.GetCharacter));

    private static IEnumerable<IReadOnlyList<string>> Orders(IReadOnlyList<string> wheels, int fitted)
    {
        var chosen = new string[fitted];
        var taken = new bool[wheels.Count];

        return Place(0);

        IEnumerable<IReadOnlyList<string>> Place(int slot)
        {
            if (slot == fitted)
            {
                yield return chosen.ToArray();
                yield break;
            }

            for (var i = 0; i < wheels.Count; i++)
            {
                // A wheel is a physical thing: it cannot be in two places at once.
                if (taken[i])
                {
                    continue;
                }

                taken[i] = true;
                chosen[slot] = wheels[i];

                foreach (var order in Place(slot + 1))
                {
                    yield return order;
                }

                taken[i] = false;
            }
        }
    }

    // Asked rather than reasoned about: the layout is the authority on what could be
    // assembled, and the factory on whether the parts fit the alphabet. Both report a
    // refusal by throwing, so a refusal is caught rather than predicted.
    private static bool CouldBeBuilt(
        IPartsCatalogue parts,
        IMachineLayout layout,
        IEntryWheel entryWheel,
        ICharacterMap alphabet,
        IReadOnlyList<string> order,
        string reflector)
    {
        try
        {
            var rotors = order.Select(name => parts.CreateRotor(name, alphabet)).ToList();
            var behind = parts.GetReflector(reflector, alphabet);

            if (rotors.Any(rotor => rotor.Contacts != alphabet.Count) ||
                behind.Contacts != alphabet.Count)
            {
                return false;
            }

            layout.Validate(rotors, behind, entryWheel);

            return true;
        }
        catch (Exception exception)
            when (exception is ArgumentException or FormatException or InvalidOperationException
                      or KeyNotFoundException)
        {
            return false;
        }
    }
}
