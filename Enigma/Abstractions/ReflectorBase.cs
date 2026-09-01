namespace Enigma;

public abstract class ReflectorBase : IReflector
{
    private int[]? _table;

    public abstract string Name { get; }
    public virtual bool IsThin => false;
    public int Contacts => Table.Length;
    protected abstract IDictionary<int, int> Wiring { get; }

    /// <summary>The wiring as the return leg reads it, once per character.</summary>
    protected int[] Table => _table ??= WiringTable.Flatten(Wiring);

    /// <summary>
    /// Virtual for one reason: the Zählwerk machines' reflector turns, so
    /// <see cref="RotatingReflectorBase"/> must read the wiring at an offset.
    /// </summary>
    public virtual int Translate(int input)
    {
        return Table[input];
    }
}
