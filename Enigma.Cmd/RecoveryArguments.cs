namespace Enigma.Cmd;

/// <summary>
/// What a recovery run was asked to search, read off the command line and checked
/// before a host or a machine exists. Kept apart from the console for the same
/// reason <see cref="NavalArguments"/> is: an argument that does not make sense
/// should be refused in one line, and be testable without starting anything.
/// </summary>
/// <param name="Wanted">Whether a search was asked for at all.</param>
/// <param name="Box">The wheels that could have been in the machine.</param>
/// <param name="Fitted">How many of them it carries.</param>
/// <param name="Reflectors">The reflectors to try.</param>
/// <param name="Candidates">How many settings to report.</param>
public sealed record RecoveryArguments(
    bool Wanted,
    IReadOnlyList<string> Box,
    int Fitted,
    IReadOnlyList<string> Reflectors,
    int Candidates,
    string? Error = null)
{
    private static readonly string[] Nothing = [];

    public static RecoveryArguments Idle { get; } = new(false, Nothing, 0, Nothing, 0);

    /// <summary>
    /// Reads the search arguments. What is not given is taken from the key sheet,
    /// which is where everything already known about the machine is written: its
    /// model, its alphabet, its reflector, and how many wheels it carries. Only the
    /// box of wheels is a thing a key sheet has no place for, and it defaults to the
    /// wheels the sheet happens to name.
    /// </summary>
    public static RecoveryArguments Read(
        bool recover,
        string? wheels,
        int? fitted,
        string? reflectors,
        int? candidates,
        IReadOnlyList<string> sheetWheels,
        string sheetReflector)
    {
        if (!recover)
        {
            return wheels is null && fitted is null && reflectors is null && candidates is null
                ? Idle
                : Refused("--wheels, --fitted, --reflectors and --candidates only mean something with --recover.");
        }

        var box = Split(wheels) is { Count: > 0 } given ? given : sheetWheels;

        if (box.Count == 0)
        {
            return Refused(
                "Nothing to search. Give --wheels with the wheels that could have been in the machine, " +
                "such as --wheels \"I II III IV V\".");
        }

        var carried = fitted ?? (sheetWheels.Count > 0 ? sheetWheels.Count : 3);

        if (carried < 1)
        {
            return Refused("--fitted must be at least one.");
        }

        if (carried > box.Count)
        {
            return Refused(
                $"--fitted is {carried} but the box holds {box.Count} wheels; a wheel cannot be in two places at once.");
        }

        var behind = Split(reflectors) is { Count: > 0 } named ? named : [sheetReflector];

        if (behind.Count == 0 || behind.Any(string.IsNullOrWhiteSpace))
        {
            return Refused("A search needs at least one reflector to try. Give --reflectors, such as --reflectors \"B C\".");
        }

        var wanted = candidates ?? 5;

        return wanted < 1
            ? Refused("--candidates must be at least one.")
            : new RecoveryArguments(true, box, carried, behind, wanted);
    }

    private static RecoveryArguments Refused(string error) =>
        new(false, Nothing, 0, Nothing, 0, error);

    // Wheel names carry hyphens -- "K-I", "D-III" -- so they split on whitespace and
    // commas only, exactly as a key sheet's rotor order does.
    private static List<string> Split(string? value) =>
        (value ?? string.Empty)
        .Split([' ', '\t', ','], StringSplitOptions.RemoveEmptyEntries)
        .Select(name => name.ToUpperInvariant())
        .ToList();
}
