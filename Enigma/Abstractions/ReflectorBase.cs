namespace Enigma;

public abstract class ReflectorBase : IReflector
{
    public abstract string Name { get; }
    public virtual bool IsThin => false;
    public int Contacts => Wiring.Count;
    protected abstract IDictionary<int, int> Wiring { get; }
    /// <summary>
    /// Virtual for one reason: the Zählwerk machines' reflector turns, so
    /// <see cref="RotatingReflectorBase"/> must read the wiring at an offset.
    /// </summary>
    public virtual int Translate(int input)
    {
        return Wiring[input];
    }
}