namespace Enigma;

public interface ICharacterMap
{
    string Name { get; }
    int Count { get; }
    char GetCharacter(int index);
    int GetIndex(char character);
}
