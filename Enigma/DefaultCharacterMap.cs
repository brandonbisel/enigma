namespace Enigma;

public class DefaultCharacterMap : ICharacterMap
{
    private static readonly char[] Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public string Name { get; } = "Default";

    public int Count => Characters.Length;

    public char GetCharacter(int index)
    {
        return Characters[index];
    }

    public int GetIndex(char character)
    {
        return Array.IndexOf(Characters, character);
    }
}