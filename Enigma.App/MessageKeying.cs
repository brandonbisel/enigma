using Enigma.Models;

namespace Enigma.App;

/// <summary>
/// Working out where the rotors actually start.
///
/// A key sheet fixed the wheel order, the rings and the board for the day, but not
/// where the rotors began. The sender chose that himself: he set them to the
/// <em>ground setting</em> the sheet gave (Grundstellung), enciphered his own
/// <em>message key</em> (Spruchschlüssel) at it, and sent the result in clear as
/// the indicator. The receiver did the same in reverse.
///
/// So with a procedure in use the sheet's positions are the ground setting, and
/// the machine is keyed at the message key instead. Faults come back as messages
/// rather than exceptions, because mistyping an indicator is an ordinary thing for
/// an operator to do.
/// </summary>
public static class MessageKeying
{
    /// <summary>
    /// Sending: enciphers a chosen message key at the sheet's ground setting and
    /// keys the machine at that message key.
    /// </summary>
    public static IndicatorResult Send(
        IIndicatorProcedure procedure, KeySheet daily, string messageKey, bool doubled = false)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(daily);

        return Attempt(() =>
        {
            var indicator = procedure.EncipherMessageKey(daily, daily.Positions, messageKey, doubled);

            return new IndicatorResult(
                daily.WithPositions(messageKey), daily.Positions, messageKey, indicator, null);
        });
    }

    /// <summary>
    /// Receiving: recovers the message key from a transmitted indicator and keys
    /// the machine at it.
    /// </summary>
    public static IndicatorResult Receive(
        IIndicatorProcedure procedure, KeySheet daily, string indicator)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(daily);

        return Attempt(() =>
        {
            var messageKey = procedure.RecoverMessageKey(daily, daily.Positions, indicator);

            return new IndicatorResult(
                daily.WithPositions(messageKey), daily.Positions, messageKey, indicator, null);
        });
    }

    private static IndicatorResult Attempt(Func<IndicatorResult> keying)
    {
        try
        {
            return keying();
        }
        catch (Exception exception)
            when (exception is ArgumentException or FormatException or InvalidOperationException)
        {
            return new IndicatorResult(null, null, null, null, exception.Message);
        }
    }
}

/// <summary>
/// What the indicator procedure worked out: the key sheet with the rotors moved to
/// the message key, and the pieces an operator writes down alongside the message.
/// </summary>
/// <param name="Sheet">The sheet to key the machine from, or null if it failed.</param>
/// <param name="GroundSetting">Where the rotors stood while the indicator was worked.</param>
/// <param name="MessageKey">Where the rotors start for the message itself.</param>
/// <param name="Indicator">What travels with the message, in clear.</param>
/// <param name="Error">Why it failed, in words fit to show an operator.</param>
public sealed record IndicatorResult(
    KeySheet? Sheet,
    string? GroundSetting,
    string? MessageKey,
    string? Indicator,
    string? Error)
{
    public bool Succeeded => Sheet is not null;
}
