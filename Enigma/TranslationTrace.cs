namespace Enigma;

/// <summary>
/// One component's contribution to a keypress: the contact the current entered on
/// and the contact it left on. Contacts are numbers in the machine's own alphabet,
/// so a caller turns them into characters through <see cref="ICharacterMap"/>.
///
/// A component appears twice in a keypress, once each side of the reflector, and
/// the return leg is named with a trailing apostrophe.
/// </summary>
public readonly record struct TranslationStep(string Component, int Input, int Output);

/// <summary>
/// The whole path of one keypress, from the key to the lamp. The steps are in the
/// order the current took them, so the reflector sits in the middle with the way
/// in before it and the way out after.
/// </summary>
/// <param name="Input">The key that was pressed.</param>
/// <param name="Output">The lamp that lit.</param>
/// <param name="Window">
/// The rotor positions as they stood when the current flowed, left to right. The
/// wheels turn before the current flows, so this is the window the operator saw
/// as the lamp lit, not the one before the key went down.
/// </param>
public sealed record TranslationTrace(
    int Input,
    int Output,
    IReadOnlyList<int> Window,
    IReadOnlyList<TranslationStep> Steps);
