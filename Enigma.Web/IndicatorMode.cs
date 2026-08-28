namespace Enigma.Web;

/// <summary>
/// Which side of the indicator procedure the operator is on, if either. Without
/// one the rotors simply start where the key sheet says.
/// </summary>
public enum IndicatorMode
{
    None,
    Sending,
    Receiving
}
