namespace Enigma;

public interface IPlugBoard
{
    int Translate(int input);

    /// <summary>
    /// The way back, from the rotors to the lamps. A cabled board is its own
    /// inverse so this is the same journey, but the Uhr is not, and the machine
    /// stays reciprocal only because it uses this on the return leg.
    /// </summary>
    int TranslateReverse(int input);

    
    void Connect(int input, int output);
    void Disconnect(int input, int output);
    bool IsConnected(int input, int output);

    IEnumerable<Tuple<int, int>> GetConnections();
}