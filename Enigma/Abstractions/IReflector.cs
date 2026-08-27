namespace Enigma;

public interface IReflector
{
    string Name { get; }

    /// <summary>True for the narrow reflectors that leave room for a fourth rotor.</summary>
    bool IsThin { get; }

    /// <summary>How many contacts the reflector has.</summary>
    int Contacts { get; }

    int Translate(int input);
}