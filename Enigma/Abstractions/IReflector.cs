namespace Enigma;

public interface IReflector
{
    string Name { get; }

    /// <summary>True for the narrow reflectors that leave room for a fourth rotor.</summary>
    bool IsThin { get; }

    int Translate(int input);
}