namespace Enigma;

public abstract class RotorBase : IRotor
{
    private int[]? _forward;
    private int[]? _reverse;

    public abstract string Name { get; }
    public virtual bool IsThin => false;
    public int Contacts => Forward.Length;
    public int Position { get; protected set; }
    public int RingSetting { get; protected set; }
    protected abstract IEnumerable<int> TurnoverPositions { get; }
    protected abstract IDictionary<int, int> Wiring { get; }

    // The ring setting shifts the wiring relative to the letter ring, so every
    // translation works from the position net of the ring rather than the raw position.
    private int Offset => Mod(Position - RingSetting);

    // Both directions are flattened from the one wiring rather than written beside
    // it, so a wheel cannot be wired one way going and another coming back.
    private int[] Forward => _forward ??= WiringTable.Flatten(Wiring);

    private int[] Reverse => _reverse ??= WiringTable.Flatten(WiringTable.Invert(Wiring));

    protected RotorBase()
    {
    }

    public virtual int Step()
    {
        Position = Mod(Position + 1);
        return Position;
    }

    public int GetPosition()
    {
        return Position;
    }

    public void SetPosition(int position)
    {
        Position = Mod(position);
    }

    public void SetRingSetting(int ringSetting)
    {
        RingSetting = Mod(ringSetting);
    }

    public int Translate(int input)
    {
        return Wheel.Translate(Forward, input, Offset);
    }

    public int TranslateReverse(int input)
    {
        return Wheel.Translate(Reverse, input, Offset);
    }

    /// <summary>
    /// Where the turnover notch is cut. On the Enigma I and the Enigma K it is on
    /// the index ring, so the wheel carries its neighbour at a fixed letter in the
    /// window whatever the Ringstellung is — rotor I always at Q. On the older
    /// Enigma D and the Enigma Z it is cut into the rotor body instead, so it keeps
    /// its place against the wiring and setting the ring carries the turnover with
    /// it. Every wheel here is of the first kind unless it says otherwise.
    /// </summary>
    protected virtual bool NotchOnTheIndexRing => true;

    public bool IsTurnoverPosition()
    {
        return TurnoverPositions.Contains(
            NotchOnTheIndexRing ? Position : Mod(Position - RingSetting));
    }

    public IEnumerable<int> GetTurnoverPositions()
    {
        return TurnoverPositions;
    }

    private int Mod(int value)
    {
        return Wheel.Mod(value, Forward.Length);
    }
}
