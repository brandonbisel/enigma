namespace Enigma;

/// <summary>The alphabet of every service Enigma: A to Z.</summary>
public class DefaultCharacterMap : CharacterMap
{
    public DefaultCharacterMap()
        : base("Default", "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
    {
    }
}
