namespace Enigma;

/// <summary>
/// The wheels and reflectors the library ships with, under the names a key sheet
/// uses. A test asserts this matches what <c>AddEnigmaServices</c> registers, so
/// the two cannot drift apart.
/// </summary>
public static class MachineParts
{
    public static IReadOnlyList<string> RotorNames { get; } =
        ["I", "II", "III", "IV", "V", "VI", "VII", "VIII", "BETA", "GAMMA"];

    public static IReadOnlyList<string> ReflectorNames { get; } =
        ["A", "B", "C", "B-THIN", "C-THIN"];

    public static IReadOnlyList<string> EntryWheelNames { get; } =
        ["STANDARD", "QWERTZ"];

    public static IReadOnlyList<string> CharacterMapNames { get; } = ["LATIN"];
}
