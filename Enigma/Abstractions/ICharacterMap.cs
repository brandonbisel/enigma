namespace Enigma;

public interface ICharacterMap
{
    string Name { get; }
    int Count { get; }
    char GetCharacter(int index);

    /// <summary>The index of a character, or -1 if this alphabet does not contain it.</summary>
    int GetIndex(char character);
}
