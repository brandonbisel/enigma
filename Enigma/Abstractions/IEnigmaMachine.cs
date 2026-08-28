namespace Enigma;

public interface IEnigmaMachine
{
    IPlugBoard PlugBoard { get; }
    IEnumerable<IRotor> Rotors { get; }
    IReflector Reflector { get; }

    /// <summary>The alphabet this machine works in, which decides what its numbers mean.</summary>
    ICharacterMap CharacterMap { get; }

    /// <summary>
    /// Which machine this is: how its wheels are driven, and what could be fitted to
    /// it. A caller asks this to know whether the machine has a plugboard at all.
    /// </summary>
    IMachineLayout Layout { get; }
    
    int Translate(int input);
    IEnumerable<int> Translate(IEnumerable<int> input);

    /// <summary>
    /// Raised once per keypress with the path the current took. Subscribing is what
    /// makes the machine assemble a trace at all, so a machine nobody is watching
    /// pays nothing for this.
    /// </summary>
    event Action<TranslationTrace>? Translated;
}