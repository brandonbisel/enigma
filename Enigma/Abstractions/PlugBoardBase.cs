namespace Enigma;

public abstract class PlugBoardBase : IPlugBoard
{
    protected abstract IDictionary<int, int> Wiring { get; }
    
    public virtual int Translate(int input)
    {
        return Wiring[input];
    }

    /// <summary>A board of cables joins letters in pairs, so it is its own inverse.</summary>
    public virtual int TranslateReverse(int input)
    {
        return Translate(input);
    }

    public void Connect(int input, int output)
    {
        if (input == output)
        {
            throw new ArgumentException(
                $"Contact {input} cannot be cabled to itself.", nameof(input));
        }

        // A jack takes one plug. Overwriting one end of an existing cable would
        // strand the letter at the other end, leaving two contacts pointing at the
        // same letter and a board that is no longer its own inverse.
        Occupied(input);
        Occupied(output);

        Wiring[input] = output;
        Wiring[output] = input;

        void Occupied(int contact)
        {
            if (Wiring[contact] != contact && Wiring[contact] != Other(contact))
            {
                throw new ArgumentException(
                    $"Contact {contact} is already cabled to {Wiring[contact]}.", nameof(input));
            }
        }

        int Other(int contact) => contact == input ? output : input;
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