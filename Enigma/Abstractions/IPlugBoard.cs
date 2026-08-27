namespace Enigma;

public interface IPlugBoard
{
    int Translate(int input);
    
    void Connect(int input, int output);
    void Disconnect(int input, int output);
    bool IsConnected(int input, int output);

    IEnumerable<Tuple<int, int>> GetConnections();
}