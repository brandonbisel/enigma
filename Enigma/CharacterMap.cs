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

    /// <summary>
    /// The ten digits, which is the whole alphabet of the numbers-only Enigma Z.
    /// Its wheels carry ten contacts rather than twenty six, and the keyboard is a
    /// single row of figures — the machine was built for weather reports and other
    /// traffic that was numeric to begin with.
    ///
    /// Ordered 0 to 9, which is how the wheel wirings are indexed here. The
    /// keyboard itself reads 1 to 0; that is an arrangement of keys rather than a
    /// property of the alphabet.
    /// </summary>
    public static ICharacterMap Digits { get; } = new CharacterMap("Digits", "0123456789");
}
