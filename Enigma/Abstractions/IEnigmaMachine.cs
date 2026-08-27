namespace Enigma;

public interface IEnigmaMachine
{
    IPlugBoard PlugBoard { get; }
    IEnumerable<IRotor> Rotors { get; }
    IReflector Reflector { get; }
    
    int Translate(int input);
    IEnumerable<int> Translate(IEnumerable<int> input);
}