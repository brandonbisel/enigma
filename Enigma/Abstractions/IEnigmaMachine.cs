namespace Enigma;

public interface IEnigmaMachine
{
    IPlugBoard PlugBoard { get; }
    IEnumerable<IRotor> Rotors { get; }
    IReflector Reflector { get; }

    /// <summary>The alphabet this machine works in, which decides what its numbers mean.</summary>
    ICharacterMap CharacterMap { get; }
    
    int Translate(int input);
    IEnumerable<int> Translate(IEnumerable<int> input);
}