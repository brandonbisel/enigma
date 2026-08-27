using Enigma.Models;

namespace Enigma;

public interface IEnigmaMachineFactory
{
    IEnigmaMachine Create(EnigmaSettings settings);
}
