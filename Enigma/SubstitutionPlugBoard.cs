namespace Enigma;

/// <summary>
/// A plugboard whose substitution is any permutation, rather than a set of paired
/// cables.
///
/// An ordinary board joins letters two at a time: A to V means V to A, and the
/// board is its own inverse. The Enigma Uhr replaced the cables with a switch box
/// that broke that symmetry — under the Uhr, A may go to V while V goes somewhere
/// else. The machine stays reciprocal regardless, because the return leg runs the
/// substitution backwards rather than forwards, which is why
/// <see cref="IPlugBoard.TranslateReverse"/> exists.
///
/// This models the substitution an Uhr produces, not the dial that selects it.
/// See the note on the Uhr in docs/plugboard.md for what is missing.
///
/// The reason a non-paired board leaves the machine reciprocal is the same reason
/// the rotors may be non-paired: the current passes through each of them twice,
/// once each way, so only the reflector has to be its own inverse.
/// </summary>
public class SubstitutionPlugBoard : IPlugBoard
{
    private readonly int[] _forward;
    private readonly int[] _reverse;

    /// <param name="substitution">
    /// Where each letter goes on the way in, as a permutation: entry i is the
    /// letter that i becomes.
    /// </param>
    public SubstitutionPlugBoard(IReadOnlyList<int> substitution)
    {
        ArgumentNullException.ThrowIfNull(substitution);

        if (substitution.Distinct().Count() != substitution.Count ||
            substitution.Any(letter => letter < 0 || letter >= substitution.Count))
        {
            throw new ArgumentException(
                "A plugboard substitution must send each letter to a different letter, " +
                "so that nothing is lost on the way back.",
                nameof(substitution));
        }

        _forward = substitution.ToArray();
        _reverse = new int[_forward.Length];

        for (var letter = 0; letter < _forward.Length; letter++)
        {
            _reverse[_forward[letter]] = letter;
        }
    }

    /// <summary>Reads the substitution off a board of cables, for comparison.</summary>
    public static SubstitutionPlugBoard FromCables(
        ICharacterMap characterMap,
        IEnumerable<Tuple<int, int>> cables)
    {
        var substitution = Enumerable.Range(0, characterMap.Count).ToArray();

        foreach (var (a, b) in cables.Select(cable => (cable.Item1, cable.Item2)))
        {
            substitution[a] = b;
            substitution[b] = a;
        }

        return new SubstitutionPlugBoard(substitution);
    }

    public int Translate(int input) => _forward[input];

    public int TranslateReverse(int input) => _reverse[input];

    public void Connect(int input, int output) =>
        throw new NotSupportedException(
            "This board is wired as a whole substitution, not by patching single cables.");

    public void Disconnect(int input, int output) =>
        throw new NotSupportedException(
            "This board is wired as a whole substitution, not by patching single cables.");

    public bool IsConnected(int input, int output) => _forward[input] == output;

    public IEnumerable<Tuple<int, int>> GetConnections() =>
        _forward
            .Select((output, input) => Tuple.Create(input, output))
            .Where(cable => cable.Item1 != cable.Item2);
}
