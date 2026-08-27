namespace Enigma;

/// <summary>
/// A half-width rotor (Zusatzwalze) of the naval M4, which fits only in the space
/// a thin reflector frees.
///
/// It has no ratchet, so a pawl pushes against it and nothing turns, and no notch,
/// so it never drives the wheel beside it. Both are properties of the wheel rather
/// than of where it sits, so they are enforced here: however it is placed and
/// whoever calls it, it will not advance. Setting it by hand still works, which is
/// exactly what an operator did before closing the lid.
/// </summary>
public abstract class ThinRotorBase : RotorBase
{
    public sealed override bool IsThin => true;

    protected sealed override IEnumerable<int> TurnoverPositions => [];

    /// <summary>Does nothing, and returns the position the wheel is already at.</summary>
    public sealed override int Step() => Position;
}
