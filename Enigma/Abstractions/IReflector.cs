namespace Enigma;

public interface IReflector
{
    string Name { get; }
    int Translate(int input);
}