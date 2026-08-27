namespace Enigma;

/// <summary>
/// The arithmetic shared by anything that turns: a rotor, and on the Zählwerk
/// machines the reflector too. Kept in one place because a second hand-written
/// modulo is exactly where this project's first real bug lived — C#'s % is a
/// remainder and keeps the sign of its left operand.
/// </summary>
internal static class Wheel
{
    public static int Mod(int value, int contacts) => ((value % contacts) + contacts) % contacts;

    /// <summary>
    /// Reads a wiring at a given offset: the current enters at the contact the
    /// wheel's rotation brings under the input, and leaves shifted back again.
    /// </summary>
    public static int Translate(IDictionary<int, int> wiring, int input, int offset)
    {
        var contacts = wiring.Count;

        return Mod(wiring[Mod(input + offset, contacts)] - offset, contacts);
    }
}
