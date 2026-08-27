namespace Enigma;

/// <summary>
/// The alphabet a machine works in: which characters exist and what order they sit
/// in. It is the single authority for how many contacts everything has, so a rotor,
/// a reflector and the plugboard of one machine all agree.
/// </summary>
public class CharacterMap : ICharacterMap
{
    private readonly char[] _characters;
    private readonly Dictionary<char, int> _indices;

    public CharacterMap(string name, string characters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(characters);

        _characters = characters.ToCharArray();
        _indices = new Dictionary<char, int>(_characters.Length);

        for (var i = 0; i < _characters.Length; i++)
        {
            if (!_indices.TryAdd(_characters[i], i))
            {
                throw new ArgumentException(
                    $"Character map '{name}' uses '{_characters[i]}' more than once.", nameof(characters));
            }
        }

        if (_characters.Length < 2)
        {
            throw new ArgumentException(
                $"Character map '{name}' needs at least two characters.", nameof(characters));
        }

        Name = name;
    }

    public string Name { get; }

    public int Count => _characters.Length;

    public char GetCharacter(int index) => _characters[index];

    /// <summary>The index of a character, or -1 if this alphabet does not contain it.</summary>
    public int GetIndex(char character) => _indices.TryGetValue(character, out var index) ? index : -1;

    /// <summary>The twenty six capital letters every service Enigma worked in.</summary>
    public static ICharacterMap Latin { get; } = new CharacterMap("Latin", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
}
