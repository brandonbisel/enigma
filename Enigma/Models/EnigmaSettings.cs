namespace Enigma.Models;

public class EnigmaSettings
{
    public string Name { get; set; } = string.Empty;

    // Ordered as the rotors sit in the machine, left to right; the last one is the fast rotor.
    public IList<RotorSettings> Rotors { get; set; } = [];
    public string Reflector { get; set; } = string.Empty;
    public IDictionary<int, int> Plugboard { get; set; } = new Dictionary<int, int>();
}
