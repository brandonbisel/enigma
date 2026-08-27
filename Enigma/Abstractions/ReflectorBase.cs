namespace Enigma;

public abstract class ReflectorBase : IReflector
{
    public abstract string Name { get; }
    protected abstract IDictionary<int, int> Wiring { get; }
    public int Translate(int input)
    {
        return Wiring[input];
    }
}