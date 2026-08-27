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
        // Resetting a pair that was never cabled would strand the letters they are
        // actually joined to, leaving two contacts pointing at the same letter.
        if (!IsConnected(input, output))
        {
            throw new ArgumentException(
                $"Contacts {input} and {output} are not connected to each other.", nameof(input));
        }

        Wiring[input] = input;
        Wiring[output] = output;
    }

    public bool IsConnected(int input, int output)
    {
        return Wiring[input] == output;
    }

    /// <summary>
    /// The cables on the board. A cable is one physical thing joining two letters,
    /// so it is reported once, from the lower contact to the higher.
    /// </summary>
    public IEnumerable<Tuple<int, int>> GetConnections()
    {
        return Wiring
            .Where(cable => cable.Key < cable.Value)
            .OrderBy(cable => cable.Key)
            .Select(cable => Tuple.Create(cable.Key, cable.Value));
    }
}