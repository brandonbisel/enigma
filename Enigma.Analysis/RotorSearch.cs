using Enigma.Models;

namespace Enigma.Analysis;

/// <summary>
/// Recovers the wheels from ciphertext alone, after Gillogly's method: sweep every
/// arrangement at every starting position with the board empty, score each
/// decipherment, and let the right setting rise.
///
/// It runs the real machine, built by the real factory from a real key sheet. There
/// is no faster private copy of the cipher in here, and there must not be: the only
/// duplicate implementation this repository sanctions is the test oracle, and a
/// second one inside an attack would be a second place for the cipher to be wrong —
/// in the one component whose whole job is to say when something is right.
///
/// <para><b>What it will and will not break.</b> The measure is the index of
/// coincidence, which needs no language data and therefore no source. From a few
/// hundred letters it recovers the wheels of a machine with no plugboard — about two
/// times in three over a full sweep, the measured rate being in ROADMAP.md — and it
/// has been seen to manage a board of three cables. It does <em>not</em> recover a
/// service Enigma cabled the way the Wehrmacht cabled one, on any length of message.
/// The reason is that the board sits inside the
/// rotor sandwich rather than outside it, so pulling it out does not relabel the
/// plaintext, it shreds it: at the true setting of the Graf Spee signal the
/// unsteckered decipherment scores 0.040 against a random 0.038, while the best
/// wrong setting in the same sweep reaches 0.049. The signal is real and it is
/// smaller than the noise. Closing that gap needs a measure that knows what German
/// looks like, which is a table of statistics, which is data, which needs a source.
/// Until there is one this stops here and says so — there is a test that pins the
/// failure, so that nobody later mistakes it for a success.</para>
///
/// A machine whose reflector turns is searched with that reflector left where the
/// key sheet puts it. Sweeping it too is another factor of the alphabet, and is not
/// done here.
/// </summary>
public sealed class RotorSearch
{
    private readonly IEnigmaMachineFactory _factory;
    private readonly IScore? _score;

    public RotorSearch(IEnigmaMachineFactory factory, IScore? score = null)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
        _score = score;
    }

    /// <summary>
    /// How many wheel orders are carried into the second phase, and how many settings
    /// from each.
    ///
    /// Taking the best few settings overall does not work, and the reason is worth
    /// stating because it is not obvious. The first phase scores against the wrong
    /// turnovers, and how wrong depends on the Ringstellung it is trying to find: a
    /// fast wheel whose ring sits far from A puts the true alignment sixteen or
    /// twenty keypresses out of phase, and the true setting then scores 0.0385 — the
    /// rate of text with no structure at all. It is invisible. What survives is the
    /// wheel <em>order</em>, which still ranks at or near the top because the wiring
    /// is right even when the stepping is not, so the second phase is given the best
    /// of each order rather than the best overall, and climbs from a mediocre
    /// starting point to the setting itself.
    /// </summary>
    private const int Orders = 5;

    private const int PerOrder = 1;

    /// <summary>Find the wheels, then settle the setting they are turned to.</summary>
    public IReadOnlyList<Candidate> Run(
        SearchSpace space,
        IReadOnlyList<int> ciphertext,
        int keep = 5,
        int rounds = 3,
        IProgress<SearchProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var shortlist = Wheels(
            space, ciphertext, Math.Max(keep, Orders * PerOrder), progress, cancellationToken);

        return Best(Settle(space, ciphertext, Promising(shortlist), rounds, cancellationToken), keep);
    }

    // The best few of each wheel order, from the most promising few orders.
    private static IEnumerable<Candidate> Promising(IEnumerable<Candidate> shortlist) =>
        shortlist
            .GroupBy(candidate => (candidate.Settings.Rotors, candidate.Settings.Reflector))
            .OrderByDescending(order => order.Max(candidate => candidate.Score))
            .Take(Orders)
            .SelectMany(order => order.OrderByDescending(candidate => candidate.Score).Take(PerOrder));

    /// <summary>
    /// The first phase. Every ring is left at the first letter of the alphabet and
    /// every starting position is tried, which covers every alignment of the wiring.
    /// Where the wheels carry each other over is wrong except by luck, and that is
    /// what the second phase is for.
    ///
    /// <paramref name="keep"/> is how many settings are kept <em>for each
    /// arrangement</em>, and every arrangement's own best are returned rather than a
    /// single ranking across all of them. That is deliberate, and <see cref="Orders"/>
    /// says why: the score here is a good guide to which wheels are in the machine and
    /// a poor one to where they are turned to.
    /// </summary>
    public IReadOnlyList<Candidate> Wheels(
        SearchSpace space,
        IReadOnlyList<int> ciphertext,
        int keep = 5,
        IProgress<SearchProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(space);

        var text = Text(ciphertext);
        var score = _score ?? new IndexOfCoincidence(space.Alphabet);
        var contacts = space.Alphabet.Count;
        var starts = Starts(contacts, space.Fitted);
        var home = space.Setting(new int[space.Fitted]);

        var found = new List<Candidate>();
        var gate = new Lock();
        var done = 0L;

        Parallel.ForEach(
            space.Arrangements,
            new ParallelOptions { CancellationToken = cancellationToken },
            arrangement =>
            {
                var trial = new Trial(_factory.Create(space.Sheet(arrangement, home, home)), text, score);
                var rings = new int[space.Fitted];
                var wheels = new int[space.Fitted];
                var best = new Shortlist(keep);

                for (var start = 0L; start < starts; start++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Place(start, contacts, wheels);
                    best.Offer(trial.Of(rings, wheels), start);
                }

                lock (gate)
                {
                    foreach (var (points, start) in best.Items)
                    {
                        Place(start, contacts, wheels);
                        found.Add(new Candidate(
                            space.Sheet(arrangement, home, space.Setting(wheels)), points));
                    }

                    done += starts;
                    progress?.Report(new SearchProgress(
                        done, space.Settings, found.MaxBy(candidate => candidate.Score)));
                }
            });

        // Every arrangement keeps its own best, and they are all returned: which of
        // them is worth settling is a decision for whoever asked, and one that cannot
        // be made by score alone. See Orders.
        return Ordered(found);
    }

    /// <summary>
    /// The second phase, which improves one thing at a time and then the other until
    /// neither improves.
    ///
    /// A ring setting shifted together with its wheel's position leaves the wiring
    /// where it was and moves only the letter showing in the window, which on these
    /// machines is where the notch is read. So sweeping the rings that way changes
    /// nothing except when the wheels carry each other over — and having changed
    /// that, the alignment the first phase chose against the wrong turnovers is
    /// worth choosing again. Hence the alternation: rings, then positions, then
    /// rings, until it settles.
    ///
    /// Only the two rightmost rings are swept. A notch further left drives nothing,
    /// so the traffic does not pin that ring at all and it is left at the first
    /// letter with the position carrying the difference. That is not a shortcut but
    /// the same fact the Dönitz signal's two published settings illustrate: shifted
    /// on the wheels whose notches drive nothing, and one machine for it.
    /// </summary>
    public IReadOnlyList<Candidate> Settle(
        SearchSpace space,
        IReadOnlyList<int> ciphertext,
        IEnumerable<Candidate> candidates,
        int rounds = 3,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(space);
        ArgumentNullException.ThrowIfNull(candidates);

        var text = Text(ciphertext);
        var score = _score ?? new IndexOfCoincidence(space.Alphabet);
        var contacts = space.Alphabet.Count;
        var settled = new List<Candidate>();

        foreach (var candidate in candidates)
        {
            var placed = candidate.Settings.Wheels(space.Alphabet);
            var arrangement = new Arrangement(
                placed.Select(wheel => wheel.Name).ToList(), candidate.Settings.ReflectorName());

            var settings = candidate.Settings;
            var trials = () => new Trial(_factory.Create(settings), text, score);
            var rings = placed.Select(wheel => wheel.RingSetting).ToArray();
            var wheels = placed.Select(wheel => wheel.Position).ToArray();
            var points = trials().Of(rings, wheels);

            for (var round = 0; round < Math.Max(rounds, 1); round++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var before = points;

                points = SweepRings(trials, contacts, rings, wheels, points, cancellationToken);
                points = SweepPositions(trials, contacts, rings, wheels, points, cancellationToken);

                if (points <= before)
                {
                    break;
                }
            }

            settled.Add(new Candidate(
                space.Sheet(arrangement, space.Setting(rings), space.Setting(wheels)), points));
        }

        return Best(settled, settled.Count);
    }

    /// <summary>
    /// How far either side of the alignment the first phase chose the ring sweep also
    /// looks. It has to look at all: the first phase chose that alignment against the
    /// wrong turnovers, and a wheel whose neighbour steps at the wrong moment fits
    /// best a letter or so away from where it really stands. Holding it still while
    /// the rings move leaves the search on a hill it cannot get off, which is what it
    /// did until this was widened.
    /// </summary>
    private const int Nearby = 2;

    private static int Span => 2 * Nearby + 1;

    /// <summary>
    /// Sweeps the rings of the two rightmost wheels, carrying each position with its
    /// ring so that the alignment of the wiring is untouched and only the turnovers
    /// move — and, with each pair of rings, reconsiders the alignment of the wheels
    /// to the left within <see cref="Nearby"/> letters, because moving a turnover is
    /// exactly what changes which alignment fits.
    ///
    /// The fast wheel's own alignment is left alone: the opening letters of a message
    /// pin it before anything has turned over, so it is the one the first phase gets
    /// right.
    /// </summary>
    private static double SweepRings(
        Func<Trial> trials,
        int contacts,
        int[] rings,
        int[] wheels,
        double points,
        CancellationToken cancellationToken)
    {
        var fitted = rings.Length;
        var offsets = new int[fitted];

        for (var i = 0; i < fitted; i++)
        {
            offsets[i] = Wrap(wheels[i] - rings[i], contacts);
        }

        var drifts = 1L;

        for (var i = 0; i < fitted - 1; i++)
        {
            drifts *= Span;
        }

        var middles = fitted > 1 ? contacts : 1;

        return Sweep(
            trials, drifts * contacts * middles, points, rings, wheels, cancellationToken,
            (index, candidateRings, candidateWheels) =>
            {
                var middle = (int)(index % middles);
                index /= middles;
                var fast = (int)(index % contacts);
                var drift = index / contacts;

                Array.Clear(candidateRings);

                for (var i = 0; i < fitted; i++)
                {
                    var by = i < fitted - 1 ? (int)(drift / Power(Span, i) % Span) - Nearby : 0;

                    candidateWheels[i] = Wrap(offsets[i] + by, contacts);
                }

                candidateRings[fitted - 1] = fast;
                candidateWheels[fitted - 1] = Wrap(candidateWheels[fitted - 1] + fast, contacts);

                if (fitted > 1)
                {
                    candidateRings[fitted - 2] = middle;
                    candidateWheels[fitted - 2] = Wrap(candidateWheels[fitted - 2] + middle, contacts);
                }
            });
    }

    /// <summary>
    /// Sweeps every starting position again with the rings as they now stand, which
    /// is the first phase asked a second time and given the right turnovers to ask it
    /// against.
    /// </summary>
    private static double SweepPositions(
        Func<Trial> trials,
        int contacts,
        int[] rings,
        int[] wheels,
        double points,
        CancellationToken cancellationToken)
    {
        var settled = (int[])rings.Clone();

        return Sweep(
            trials, Starts(contacts, wheels.Length), points, rings, wheels, cancellationToken,
            (index, candidateRings, candidateWheels) =>
            {
                settled.CopyTo(candidateRings, 0);
                Place(index, contacts, candidateWheels);
            });
    }

    /// <summary>
    /// Runs one sweep across every core, keeping the best setting it finds and
    /// improving <paramref name="rings"/> and <paramref name="wheels"/> in place if
    /// anything beat what was already there.
    ///
    /// Every worker gets its own machine, because a machine carries the positions of
    /// its wheels and two threads sharing one would quietly corrupt each other's —
    /// which is the same reason rotors are registered transient.
    /// </summary>
    private static double Sweep(
        Func<Trial> trials,
        long count,
        double points,
        int[] rings,
        int[] wheels,
        CancellationToken cancellationToken,
        Action<long, int[], int[]> place)
    {
        var fitted = rings.Length;
        var gate = new Lock();
        var bestRings = (int[])rings.Clone();
        var bestWheels = (int[])wheels.Clone();
        var best = points;
        var at = long.MinValue;

        Parallel.For(
            0L,
            count,
            new ParallelOptions { CancellationToken = cancellationToken },
            () => (Trial: trials(), Points: double.NegativeInfinity, At: long.MaxValue,
                   Rings: new int[fitted], Wheels: new int[fitted],
                   TryRings: new int[fitted], TryWheels: new int[fitted]),
            (index, _, local) =>
            {
                place(index, local.TryRings, local.TryWheels);

                var scored = local.Trial.Of(local.TryRings, local.TryWheels);

                if (scored <= local.Points)
                {
                    return local;
                }

                local.TryRings.CopyTo(local.Rings, 0);
                local.TryWheels.CopyTo(local.Wheels, 0);

                return local with { Points = scored, At = index };
            },
            local =>
            {
                lock (gate)
                {
                    // Ties go to the earlier setting, so that which core happened to
                    // reach it first cannot change the answer. A search that gave a
                    // different result on a different machine would be untestable.
                    if (local.Points < best || (local.Points == best && local.At >= at))
                    {
                        return;
                    }

                    best = local.Points;
                    at = local.At;
                    local.Rings.CopyTo(bestRings, 0);
                    local.Wheels.CopyTo(bestWheels, 0);
                }
            });

        bestRings.CopyTo(rings, 0);
        bestWheels.CopyTo(wheels, 0);

        return best;
    }

    private static long Power(long value, int exponent)
    {
        var raised = 1L;

        for (var i = 0; i < exponent; i++)
        {
            raised *= value;
        }

        return raised;
    }

    private static int Wrap(int value, int contacts) => ((value % contacts) + contacts) % contacts;

    private static int[] Text(IReadOnlyList<int> ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);

        // Two characters are the fewest that can agree with each other, and how often
        // two characters agree says nothing at all about one.
        if (ciphertext.Count < 2)
        {
            throw new ArgumentException(
                "A search needs at least two characters of ciphertext.", nameof(ciphertext));
        }

        return ciphertext.ToArray();
    }

    private static long Starts(int contacts, int fitted)
    {
        var starts = 1L;

        for (var i = 0; i < fitted; i++)
        {
            starts *= contacts;
        }

        return starts;
    }

    // The fast wheel counts fastest, so consecutive settings differ in the wheel
    // nearest the entry wheel, exactly as turning the machine by hand would.
    private static void Place(long index, int contacts, int[] wheels)
    {
        for (var i = wheels.Length - 1; i >= 0; i--)
        {
            wheels[i] = (int)(index % contacts);
            index /= contacts;
        }
    }

    private static IReadOnlyList<Candidate> Best(IEnumerable<Candidate> candidates, int keep) =>
        Ordered(candidates).Take(Math.Max(keep, 1)).ToList();

    // Ordered by score, then by what the setting says, because the workers report in
    // whatever order they finish and two settings can score exactly alike.
    private static IReadOnlyList<Candidate> Ordered(IEnumerable<Candidate> candidates) =>
        candidates
            .OrderByDescending(candidate => candidate.Score)
            .ThenBy(candidate => candidate.Settings.Rotors, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.Settings.Reflector, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.Settings.RingSettings, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.Settings.Positions, StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// One machine, wound back and read again for every setting tried on it. Building
    /// a machine costs more than running one over a few hundred letters, so a sweep
    /// of a million settings cannot afford to build a million machines — but nothing
    /// here may drift from what building one would have given, so what a keypress
    /// moves is what this puts back: the wheels, and on the machines that have one the
    /// reflector, which the drive turns as well. A test pins the two as the same.
    /// </summary>
    private sealed class Trial
    {
        private readonly IEnigmaMachine _machine;
        private readonly IRotor[] _rotors;
        private readonly IRotatingReflector? _turning;
        private readonly int _reflectorHome;
        private readonly int[] _text;
        private readonly int[] _output;
        private readonly IScore _score;

        public Trial(IEnigmaMachine machine, int[] text, IScore score)
        {
            _machine = machine;
            _rotors = machine.Rotors.ToArray();
            _turning = machine.Reflector as IRotatingReflector;
            _reflectorHome = _turning?.Position ?? 0;
            _text = text;
            _output = new int[text.Length];
            _score = score;
        }

        public double Of(int[] rings, int[] wheels)
        {
            for (var i = 0; i < _rotors.Length; i++)
            {
                _rotors[i].SetRingSetting(rings[i]);
                _rotors[i].SetPosition(wheels[i]);
            }

            _turning?.SetPosition(_reflectorHome);

            for (var i = 0; i < _text.Length; i++)
            {
                _output[i] = _machine.Translate(_text[i]);
            }

            return _score.Of(_output);
        }
    }

    /// <summary>
    /// The best few settings one worker has seen, held as scores and indices so that
    /// a sweep of a million settings does not write a million key sheets it is about
    /// to throw away.
    /// </summary>
    private sealed class Shortlist(int keep)
    {
        private readonly List<(double Points, long Start)> _items = [];
        private readonly int _keep = Math.Max(keep, 1);

        public IEnumerable<(double Points, long Start)> Items => _items;

        public void Offer(double points, long start)
        {
            if (_items.Count == _keep && points <= _items[^1].Points)
            {
                return;
            }

            // Written out rather than through FindIndex, which would take a closure
            // over the score and allocate one for every setting swept.
            var at = _items.Count;

            for (var i = 0; i < _items.Count; i++)
            {
                if (points > _items[i].Points)
                {
                    at = i;
                    break;
                }
            }

            _items.Insert(at, (points, start));

            if (_items.Count > _keep)
            {
                _items.RemoveAt(_items.Count - 1);
            }
        }
    }
}
