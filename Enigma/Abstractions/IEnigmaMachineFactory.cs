using Enigma.Models;

namespace Enigma;

public interface IEnigmaMachineFactory
{
    IEnigmaMachine Create(KeySheet keySheet);
}
