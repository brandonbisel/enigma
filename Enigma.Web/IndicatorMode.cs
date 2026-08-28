namespace Enigma.Web;

/// <summary>
/// Which side of which indicator procedure the operator is on, if either. Without
/// one the rotors simply start where the key sheet says.
///
/// The two services did this differently enough to be separate choices rather than
/// a setting on one. The Army sent its ground setting in clear; the Navy sent
/// nothing in clear at all.
/// </summary>
public enum IndicatorMode
{
    None,
    Sending,
    Receiving,
    NavalSending,
    NavalReceiving
}

/// <summary>Which of the two procedures a mode belongs to.</summary>
public static class IndicatorModes
{
    public static bool IsNaval(this IndicatorMode mode) =>
        mode is IndicatorMode.NavalSending or IndicatorMode.NavalReceiving;

    public static bool IsSending(this IndicatorMode mode) =>
        mode is IndicatorMode.Sending or IndicatorMode.NavalSending;

    public static bool IsReceiving(this IndicatorMode mode) =>
        mode is IndicatorMode.Receiving or IndicatorMode.NavalReceiving;
}
