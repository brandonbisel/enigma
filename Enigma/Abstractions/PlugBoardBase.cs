namespace Enigma;

public abstract class PlugBoardBase : IPlugBoard
{
    protected abstract IDictionary<int, int> Wiring { get; }
    
    public int Translate(int input)
    {
        return Wiring[input];
    }

    public void Connect(int input, int output)
    {
        Wiring[input] = output;
        Wiring[output] = input;
    }

    public void Disconnect(int input, int output)
    {
        Wiring[input] = input;
        Wiring[output] = output;
    }

    public bool IsConnected(int input, int output)
    {
        return Wiring[input] == output;
    }

    public IEnumerable<Tuple<int, int>> GetConnections()
    {
        return Wiring.Where(x => x.Key != x.Value).Select(x => Tuple.Create(x.Key, x.Value));
    }
}